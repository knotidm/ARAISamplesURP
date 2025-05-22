using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.InferenceEngine;
using static Unity.InferenceEngine.Functional;
using UnityEngine;

/*
 *              Whisper Inference Code
 *              ======================
 *  
 *  Put this script on the Main Camera
 *  
 *  In Assets/StreamingAssets put:
 *  
 *  AudioDecoder_Tiny.sentis
 *  AudioEncoder_Tiny.sentis
 *  LogMelSepctro.sentis
 *  vocab.json
 * 
 *  Drag a 30s 16khz mono uncompressed audioclip into the audioClip field. 
 * 
 *  Install package com.unity.nuget.newtonsoft-json from packagemanger
 *  Install package com.unity.sentis
 * 
 */


public class RunWhisper : MonoBehaviour
{
    [SerializeField] private ModelAsset AudioDecoder_Tiny;
    [SerializeField] private ModelAsset AudioEncoder_Tiny;
    [SerializeField] private ModelAsset LogMelSepctro;

    Worker decoderEngine, encoderEngine, spectroEngine;

    const BackendType backend = BackendType.GPUCompute;

    // Link your audioclip here. Format must be 16Hz mono non-compressed.
    public AudioClip audioClip;

    public SpeechRecognitionController speechRecognitionController;

    // This is how many tokens you want. It can be adjusted.
    const int maxTokens = 100;

    //Special tokens
    const int END_OF_TEXT = 50257;
    const int START_OF_TRANSCRIPT = 50258;
    const int ENGLISH = 50259;
    const int TRANSCRIBE = 50359;
    const int START_TIME = 50364;

    int numSamples;
    float[] data;
    string[] tokens;

    int currentToken = 0;
    int[] outputTokens = new int[maxTokens];

    // Used for special character decoding
    int[] whiteSpaceCharacters = new int[256];

    Tensor<float> encodedAudio;

    bool transcribe = false;
    string outputString = "";

    // Maximum size of audioClip (30s at 16kHz)
    const int maxSamples = 30 * 16000;

    void Start()
    {
        SetupWhiteSpaceShifts();

        GetTokens();

        Model decoder = ModelLoader.Load(AudioDecoder_Tiny);
        Model encoder = ModelLoader.Load(AudioEncoder_Tiny);
        Model spectro = ModelLoader.Load(LogMelSepctro);

        decoderEngine = new Worker(decoder, backend);
        encoderEngine = new Worker(encoder, backend);
        spectroEngine = new Worker(spectro, backend);


    }

    public void Transcribe()
    {
        // Reset output tokens
        outputTokens[0] = START_OF_TRANSCRIPT;
        outputTokens[1] = ENGLISH;
        outputTokens[2] = TRANSCRIBE;
        outputTokens[3] = START_TIME;
        currentToken = 3;

        // Reset output string (transcript)
        outputString = "";

        // Load audio and encode it
        LoadAudio();
        EncodeAudio();
        transcribe = true;
    }

    void LoadAudio()
    {
        if (audioClip.frequency != 16000)
        {
            Debug.Log($"The audio clip should have frequency 16kHz. It has frequency {audioClip.frequency / 1000f}kHz");
            return;
        }

        numSamples = audioClip.samples;

        if (numSamples > maxSamples)
        {
            Debug.Log($"The AudioClip is too long. It must be less than 30 seconds. This clip is {numSamples / audioClip.frequency} seconds.");
            return;
        }

        data = new float[numSamples];
        audioClip.GetData(data, 0);
    }


    void GetTokens()
    {
        var jsonText = File.ReadAllText(Application.streamingAssetsPath + "/vocab.json");
        var vocab = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonText);
        tokens = new string[vocab.Count];
        foreach (var item in vocab)
        {
            tokens[item.Value] = item.Key;
        }
    }

    void EncodeAudio()
    {
        using var input = new Tensor<float>(new TensorShape(1, numSamples), data);

        // Pad out to 30 seconds at 16khz if necessary
        float[] paddedData = new float[maxSamples];
        System.Array.Copy(data, 0, paddedData, 0, numSamples);
        using var input30seconds = new Tensor<float>(new TensorShape(1, maxSamples), paddedData);

        spectroEngine.Schedule(input30seconds);
        var spectroOutput = spectroEngine.PeekOutput() as Tensor<float>;

        encoderEngine.Schedule(spectroOutput);
        encodedAudio = encoderEngine.PeekOutput() as Tensor<float>;
    }


    // Update is called once per frame
    void Update()
    {
        if (transcribe && currentToken < outputTokens.Length - 1)
        {
            using var tokensSoFar = new Tensor<int>(new TensorShape(1, outputTokens.Length), outputTokens);

            var inputs = new Dictionary<string, Tensor>
            {
                {"encoded_audio",encodedAudio },
                {"encoded_audio",encodedAudio },
                {"tokens" , tokensSoFar }
            };

            decoderEngine.SetInput("encoded_audio", encodedAudio); // encodedAudio is Tensor<float>
            decoderEngine.SetInput("tokens", tokensSoFar);       // tokensSoFar is Tensor<int>
            decoderEngine.Schedule();
            var tokensOut = decoderEngine.PeekOutput() as Tensor<float>;

            // tokensOut is Tensor<float> from PeekOutput()
            // Expected shape: [batch_size, sequence_length, vocab_size]
            // We need ArgMax along dimension 2 (vocab_size).
            // The original code used tokensPredictions[currentToken] to get the ID.
            // This implies currentToken refers to the sequence position.

            float[] tokensOutData = tokensOut.DownloadToArray(); // Makes data readable on CPU
            int batchSize = tokensOut.shape[0];
            int sequenceLength = tokensOut.shape[1];
            int vocabSize = tokensOut.shape[2];

            int ID = -1; // Default to an invalid ID

            // Assuming batchSize is 1 for this specific processing logic
            if (batchSize == 1 && currentToken < sequenceLength)
            {
                float maxProbability = float.MinValue;
                int predictedVocabIndex = -1;
                int offsetForCurrentToken = currentToken * vocabSize;

                for (int i = 0; i < vocabSize; ++i)
                {
                    float currentProbability = tokensOutData[offsetForCurrentToken + i];
                    if (currentProbability > maxProbability)
                    {
                        maxProbability = currentProbability;
                        predictedVocabIndex = i;
                    }
                }
                ID = predictedVocabIndex;
            }
            else if (batchSize != 1)
            {
                Debug.LogWarning("Manual ArgMax in RunWhisper expected batch size of 1 but got " + batchSize);
            }
            // Note: The variable 'ID' will then be used as before:
            // outputTokens[++currentToken] = ID;
            // if (ID == END_OF_TEXT) ...

            outputTokens[++currentToken] = ID;

            if (ID == END_OF_TEXT)
            {
                transcribe = false;
            }
            else if (ID >= tokens.Length)
            {
                outputString += $"(time={(ID - START_TIME) * 0.02f})";
                speechRecognitionController.onResponse.Invoke(outputString);
            }
            else outputString += GetUnicodeText(tokens[ID]);

            Debug.Log(outputString);
        }
    }

    // Translates encoded special characters to Unicode
    string GetUnicodeText(string text)
    {
        var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(ShiftCharacterDown(text));
        return Encoding.UTF8.GetString(bytes);
    }

    string ShiftCharacterDown(string text)
    {
        string outText = "";
        foreach (char letter in text)
        {
            outText += ((int)letter <= 256) ? letter :
                (char)whiteSpaceCharacters[(int)(letter - 256)];
        }
        return outText;
    }

    void SetupWhiteSpaceShifts()
    {
        for (int i = 0, n = 0; i < 256; i++)
        {
            if (IsWhiteSpace((char)i)) whiteSpaceCharacters[n++] = i;
        }
    }

    bool IsWhiteSpace(char c)
    {
        return !(('!' <= c && c <= '~') || ('¡' <= c && c <= '¬') || ('®' <= c && c <= 'ÿ'));
    }

    private void OnDestroy()
    {
        decoderEngine?.Dispose();
        encoderEngine?.Dispose();
        spectroEngine?.Dispose();
    }
}
