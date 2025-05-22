using SentenceSimilarityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.InferenceEngine;
using Unity.InferenceEngine.Functional;
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
        worker.Execute(inputSentencesTokensTensor);

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
        Tensor<float> SentenceSimilarityScores_ = Functional.MatMul(InputSequence, Functional.Transpose(ComparisonSequences, new int[] { 1, 0 }));
        return SentenceSimilarityScores_;
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
        scores.MakeReadable(); // Be able to read this tensor

        // Helper to return only best score and index
        Tensor<int> scoreIndex = Functional.ArgMax(scores, 1, true);
        scoreIndex.MakeReadable();

        int scoreIndexInt = scoreIndex[0];
        scores.MakeReadable();
        float score = scores[scoreIndexInt];

        // Return the similarity score and the action index
        return Tuple.Create(scoreIndexInt, score);
    }
}