
using NeoCortexApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NeoCortexApiSample
{
    class Program
    {
        static void Main(string[] args)
        {
            RunExperiment();
        }

        private static void RunExperiment()
        {
            List<double> asciiSequence = GetAsciiSequenceFromFile("filename.txt");

            Console.WriteLine("ASCII Sequence:");
            foreach (int asciiCode in asciiSequence)
            {
                Console.Write(asciiCode + " ");
            }

            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>> { { "S1", asciiSequence } };

            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

            TestPredictions(predictor, new double[] { 'F', 'i', 'r', 's', 't' });
            TestPredictions(predictor, new double[] { 'S', 'E', 'C', 'O', 'N', 'D' });
            //TestPredictions(predictor, new double[] { 'c', 'i' });
            TestPredictions(predictor, new double[] { 'f', 'i', 'r', 's','t'});
            TestPredictions(predictor, new double[] { 's', 'e','c','o','n','d' });

        }

        private static List<double> GetAsciiSequenceFromFile(string filePath)
        {
            try
            {
                // Read the content of a file
                string fileContent = File.ReadAllText(filePath);
                // Remove spaces, tabs, carriage returns, and line feeds
                string cleanedContent = fileContent.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
                // Convert the cleaned content to a single string
                string joinedString = string.Join("", cleanedContent.ToCharArray());
                // Write the cleaned content to a new file
                File.WriteAllText(@"outputFilePath", joinedString);
                // Display a success message
                Console.WriteLine("Spaces removed successfully.");


                string fileContent1 = File.ReadAllText(@"outputFilePath");
                return fileContent1.Select(character => (double)character).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading the file: " + ex.Message);
                return new List<double>();
            }
        }

        private static void TestPredictions(Predictor predictor, double[] list)
        {
            Debug.WriteLine("------------------------------");

            foreach (var item in list)
            {
                var res = predictor.Predict(item);

                if (res.Count > 0)
                {
                    var pred = res.First();
                    Debug.WriteLine($"{pred.PredictedInput} - {pred.Similarity}");

                    var tokens = pred.PredictedInput.Split('_');
                    var tokens2 = pred.PredictedInput.Split('-');

                    Console.WriteLine(tokens2.Last());

                    var tokens3 = tokens2.Last();

                    Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens3.Last()}");
                }
                else
                {
                    Debug.WriteLine("Nothing predicted :( ");
                }
            }

            Debug.WriteLine("------------------------------");
        }
    }
}









































