using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NeoCortexApi;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;

namespace NeoCortexApiSample
{
    public class Class2
    {
        static void Main(string[] args)
        {
            RunLanguageSemanticExperiment();
        }

        private static void RunLanguageSemanticExperiment()
        {
            // Read learning sequences from a file
            string learningSequenceFilePath = "learning_sequences.txt";
            List<int[]> learningSequences = ReadSequencesFromFile(learningSequenceFilePath);

            // Split data into training and testing sets (90% - training, 10% - testing)
            int splitIndex = (int)(learningSequences.Count * 0.9);
            List<int[]> trainingData = learningSequences.GetRange(0, splitIndex);
            List<int[]> testingData = learningSequences.GetRange(splitIndex, learningSequences.Count - splitIndex);

            // Train the model
            Predictor predictor = TrainModel(trainingData);

            // Calculate prediction accuracy using binary cross-entropy
            double accuracy = CalculateAccuracy(predictor, testingData);
            Console.WriteLine($"Prediction accuracy: {accuracy}");

            // Predict (infer) continuation of text based on user input
            Console.WriteLine("Enter some text:");
            string userInput = Console.ReadLine();
            string predictedText = PredictText(predictor, userInput);
            Console.WriteLine($"Predicted text: {predictedText}");
        }

        private static List<int[]> ReadSequencesFromFile(string filePath)
        {
            List<int[]> sequences = new List<int[]>();

            // Read text from file
            string fileContent = File.ReadAllText(filePath);

            // Remove control characters
            string cleanedContent = RemoveControlCharacters(fileContent);

            // Split into sequences
            string[] sequenceStrings = cleanedContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Convert sequences to int arrays
            foreach (string sequenceString in sequenceStrings)
            {
                int[] sequence = new int[sequenceString.Length];
                for (int i = 0; i < sequenceString.Length; i++)
                {
                    sequence[i] = sequenceString[i];
                }
                sequences.Add(sequence);
            }

            return sequences;
        }

        private static string RemoveControlCharacters(string input)
        {
            return string.Concat(input.Split(new char[] { '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static Predictor TrainModel(List<int[]> trainingData)
        {
            int inputBits = 100;
            int numColumns = 1024;

            // Setup HTM configuration
            HtmConfig config = new HtmConfig(new int[] { inputBits }, new int[] { numColumns });

            // Initialize connections, spatial pooler, temporal memory, etc.
            Connections connections = new Connections(config);
            SpatialPooler spatialPooler = new SpatialPooler(config);
            TemporalMemory temporalMemory = new TemporalMemory(config);

            // Train the model using training data
            foreach (int[] sequence in trainingData)
            {
                spatialPooler.Compute(sequence);

                temporalMemory.Compute(spatialPooler);
                temporalMemory.Learn();
            }

            // Return the trained Predictor
            return new Predictor(spatialPooler, connections, temporalMemory);
        }

        private static double CalculateAccuracy(Predictor predictor, List<int[]> testingData)
        {
            double totalError = 0.0;
            int totalBits = 0;

            foreach (int[] sequence in testingData)
            {
                // Compute prediction for each sequence
                predictor.Predict(sequence);

                // Compute binary cross-entropy
                totalError += predictor.ComputeAverageError();
                totalBits += sequence.Length * 8; // Assuming each character is 8 bits
            }

            // Return binary cross-entropy
            return totalError / totalBits;
        }

        private static string PredictText(Predictor predictor, string userInput)
        {
            // Convert user input to integer array
            int[] inputSequence = new int[userInput.Length];
            for (int i = 0; i < userInput.Length; i++)
            {
                inputSequence[i] = userInput[i];
            }

            // Predict continuation of text based on user input
            StringBuilder predictedText = new StringBuilder(userInput);

            for (int i = 0; i < 100; i++) // Limiting to 100 characters
            {
                // Predict the next character
                int predictedCharCode = predictor.PredictNext(inputSequence);

                // Append the predicted character to the text
                predictedText.Append((char)predictedCharCode);

                // Update input sequence for next prediction
                for (int j = 0; j < inputSequence.Length - 1; j++)
                {
                    inputSequence[j] = inputSequence[j + 1];
                }
                inputSequence[inputSequence.Length - 1] = predictedCharCode;
            }

            return predictedText.ToString();
        }
    }
}