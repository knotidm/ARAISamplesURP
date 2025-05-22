using SentenceSimilarityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.InferenceEngine;
using static Unity.InferenceEngine.Functional;
using UnityEngine;


public class SentenceSimilarity : MonoBehaviour
{
    public ModelAsset modelAsset;
    public Model runtimeModel;
    public Worker worker;


    /// <summary>
    /// Load the model on awake
    /// </summary>
    void Awake()
    {
        // Load the ONNX model
        runtimeModel = ModelLoader.Load(modelAsset);

        // Create an engine and set the backend as GPU //GPUCompute
        worker = new Worker(runtimeModel, BackendType.CPU);
    }


    void OnDisable()
    {
        // Tell the GPU we're finished with the memory the engine used
        worker.Dispose();
    }


    /// <summary>
    /// Encode the input
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Tensor<float> Encode(List<string> input)
    {
        // Step 1: Tokenize the sentences
        Dictionary<string, Tensor> inputSentencesTokensTensor = SentenceSimilarityUtils_.TokenizeInput(input);

        // Step 2: Compute embedding and get the output
        foreach (var entry in inputSentencesTokensTensor)
        {
            worker.SetInput(entry.Key, entry.Value);
        }
        worker.Schedule();

        // Step 3: Get the output from the neural network
        Tensor<float> outputTensor = worker.PeekOutput("last_hidden_state") as Tensor<float>;

        // Step 4: Perform pooling
        Tensor<float> MeanPooledTensor = SentenceSimilarityUtils_.MeanPooling(inputSentencesTokensTensor["attention_mask"], outputTensor);

        // Step 5: Normalize the results
        Tensor<float> NormedTensor = SentenceSimilarityUtils_.L2Norm(MeanPooledTensor);

        return NormedTensor;
    }


    /// <summary>
    /// We calculate the similarity scores between the input sequence (what the user typed) and the comparison
    /// sequences (the robot action list)
    /// This similarity score is simply the cosine similarity. It is calculated as the cosine of the angle between two vectors. 
    /// It is particularly useful when your texts are not the same length
    /// </summary>
    /// <param name="InputSequence"></param>
    /// <param name="ComparisonSequences"></param>
    /// <returns></returns>
    public Tensor<float> SentenceSimilarityScores(Tensor<float> InputSequence, Tensor<float> ComparisonSequences)
    {
        // InputSequence (A) shape: [N, K]
        // ComparisonSequences (B) shape: [M, K]
        // Result (A * B^T) shape: [N, M]

        float[] aData = InputSequence.DownloadToArray();
        int n = InputSequence.shape[0]; // num_input_sentences
        int kA = InputSequence.shape[1]; // embedding_dim

        float[] bData = ComparisonSequences.DownloadToArray();
        int m = ComparisonSequences.shape[0]; // num_comparison_sentences
        int kB = ComparisonSequences.shape[1]; // embedding_dim

        if (kA != kB)
        {
            Debug.LogError("Embedding dimensions do not match for MatMul!");
            // Return an empty or error tensor? For now, let it potentially fail later or return null.
            // Or create an empty tensor: return new Tensor<float>(new TensorShape(n,m)); 
             return new Tensor<float>(new TensorShape(n, m), new float[n * m]); // Return zeroed tensor
        }

        float[] resultData = new float[n * m];

        for (int i = 0; i < n; ++i) // Over rows of A (InputSequence)
        {
            for (int j = 0; j < m; ++j) // Over rows of B (ComparisonSequences, which become columns of B^T)
            {
                float sum = 0;
                for (int l = 0; l < kA; ++l) // Over columns of A / columns of B (embedding_dim)
                {
                    // A[i, l] * B[j, l] (since B is effectively transposed)
                    sum += aData[i * kA + l] * bData[j * kB + l];
                }
                resultData[i * m + j] = sum;
            }
        }
        return new Tensor<float>(new TensorShape(n, m), resultData);
    }



    /// <summary>
    /// Get the most similar action and its index given the player input
    /// </summary>
    /// <param name="inputSentence"></param>
    /// <param name="comparisonSentences"></param>
    /// <returns></returns>
    public Tuple<int, float> RankSimilarityScores(string inputSentence, string[] comparisonSentences)
    {
        // Step 1: Transform string and string[] to lists
        List<string> InputSentences = new List<string>();
        List<string> ComparisonSentences = new List<string>();

        InputSentences.Add(inputSentence);
        ComparisonSentences = comparisonSentences.ToList();

        // Step 2: Encode the input sentences and comparison sentences
        Tensor<float> NormEmbedSentences = Encode(InputSentences);
        Tensor<float> NormEmbedComparisonSentences = Encode(ComparisonSentences);

        // Calculate the similarity score of the player input with each action
        Tensor<float> scores = SentenceSimilarityScores(NormEmbedSentences, NormEmbedComparisonSentences);
        
        // scores is Tensor<float>, result of our new SentenceSimilarityScores
        // Shape: [num_input_sentences, num_comparison_sentences]
        // We need ArgMax for axis 1. The original keepDims=true is implicitly handled 
        // by how we structure the loop if num_input_sentences > 1.
        // The script seems to assume num_input_sentences is 1.

        float[] scoresData = scores.DownloadToArray();
        int numInputSentences = scores.shape[0];
        int numComparisonSentences = scores.shape[1];
        
        int topScoreIndex = -1;
        float topScoreValue = float.MinValue;

        if (numInputSentences == 1) // Script's current logic implies this
        {
            for (int j = 0; j < numComparisonSentences; ++j)
            {
                if (scoresData[j] > topScoreValue)
                {
                    topScoreValue = scoresData[j];
                    topScoreIndex = j;
                }
            }
        }
        else
        {
            // Handle case for multiple input sentences if necessary, though current script doesn't.
            // For now, just operate on the first sentence's scores or log a warning.
            Debug.LogWarning("RankSimilarityScores processing multiple input sentences, but taking best from first.");
             for (int j = 0; j < numComparisonSentences; ++j) // Default to first sentence
            {
                if (scoresData[j] > topScoreValue)
                {
                    topScoreValue = scoresData[j];
                    topScoreIndex = j;
                }
            }
        }
        
        // Original return was Tuple.Create(scoreIndexInt, score)
        // scoreIndexInt was the index, score was the value.
        // The original `float score = scores[scoreIndexInt];` was problematic.
        // Our new topScoreIndex and topScoreValue are what's needed.
        // So, the variables `scoreIndexInt` and `score` should be assigned these values.
        int scoreIndexInt = topScoreIndex;
        float score = topScoreValue; 
        // The rest of the method `return Tuple.Create(scoreIndexInt, score);` will use these.

        // Return the similarity score and the action index
        return Tuple.Create(scoreIndexInt, score);
    }
}