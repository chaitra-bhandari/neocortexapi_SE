﻿using NeoCortexApi;
using NeoCortexApi.Encoders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static NeoCortexApiSample.MultiSequenceLearning;

namespace NeoCortexApiSample
{
    class Program
    {
        /// <summary>
        /// This sample shows a typical experiment code for SP and TM.
        /// You must start this code in debugger to follow the trace.
        /// and TM.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //
            // Starts experiment that demonstrates how to learn spatial patterns.
            SpatialPatternLearning experiment = new SpatialPatternLearning();
            experiment.Run();

            //
            // Starts experiment that demonstrates how to learn spatial patterns.
            //SequenceLearning experiment = new SequenceLearning();
            //experiment.Run();

            //GridCellSamples gridCells = new GridCellSamples();
            //gridCells.Run();

            // RunMultiSimpleSequenceLearningExperiment();


            //RunMultiSequenceLearningExperiment();
        }

        private static void RunMultiSimpleSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            sequences.Add("S1", new List<double>(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, }));
            sequences.Add("S2", new List<double>(new double[] { 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0 }));

            //
            // Prototype for building the prediction engine.
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);
        }


        /// <summary>
        /// This example demonstrates how to learn two sequences and how to use the prediction mechanism.
        /// First, two sequences are learned.
        /// Second, three short sequences with three elements each are created und used for prediction. The predictor used by experiment privides to the HTM every element of every predicting sequence.
        /// The predictor tries to predict the next element.
        /// </summary>
        private static void RunMultiSequenceLearningExperiment()
        {
            List<double> asciiSequence = new List<double>();
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();
            // The path to the text file
            string filePath = "Input Data / filename.txt";
           
            // Check if the file exists
            if (System.IO.File.Exists(filePath))
            {
                // Read all text from the file
                string fileContent = System.IO.File.ReadAllText(filePath);
                // Count the total number of characters
                int characterCount = fileContent.Length;

                // Display the length
                Console.WriteLine($"Total number of characters in the file: {characterCount}");
                // Convert the string to an array of characters
                char[] charArray = fileContent.ToCharArray();

                // Sort the array of characters
                //Array.Sort(charArray);

                // Reconstruct the sorted string
                string sortedString = new string(charArray);

                // Display the sorted string
                //Console.WriteLine($"Sorted characters in the file: {sortedString}");

                foreach (char character in charArray)
                {
                    double asciiValue = (double)character;
                    asciiSequence.Add(asciiValue);
                    //Console.WriteLine($"Character: {character}, ASCII Value: {asciiValue}");

                }
                Console.WriteLine("ASCII Sequence:");
                foreach (int asciiCode in asciiSequence)
                {
                    Console.Write(asciiCode + " ");
                }
                sequences.Add("S1", asciiSequence);
            }
            else
            {
                Console.WriteLine("File not found.");
            }

            //sequences.Add("S1", new List<double>(new double[] { 0.0, 1.0, 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 5.0, 4.0, 3.0, 7.0, 1.0, 9.0, 12.0, 11.0, 12.0, 13.0, 14.0, 11.0, 12.0, 14.0, 5.0, 7.0, 6.0, 9.0, 3.0, 4.0, 3.0, 4.0, 3.0, 4.0 }));
            //sequences.Add("S2", new List<double>(new double[] { 0.8, 2.0, 0.0, 3.0, 3.0, 4.0, 5.0, 6.0, 5.0, 7.0, 2.0, 7.0, 1.0, 9.0, 11.0, 11.0, 10.0, 13.0, 14.0, 11.0, 7.0, 6.0, 5.0, 7.0, 6.0, 5.0, 3.0, 2.0, 3.0, 4.0, 3.0, 4.0 }));

            //sequences.Add("S1", new List<double>(new double[] { 0.0, 1.0, 2.0, 3.0, 4.0, 2.0, 5.0, }));
            //sequences.Add("S2", new List<double>(new double[] { 8.0, 1.0, 2.0, 9.0, 10.0, 7.0, 11.00 }));

            //sequences.Add("S1", new List<double>(new double[] { 'a', 'b', 'c', 'd' }));
            //sequences.Add("S2", new List<double>(new double[] { 'e', 'f', 'g', 'h' }));


            //
            // Prototype for building the prediction engine.
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

            //
            // These list are used to see how the prediction works.
            // Predictor is traversing the list element by element. 
            // By providing more elements to the prediction, the predictor delivers more precise result.
            var list1 = new double[] { 'F', 'i', 'r', 's', 't' };
            var list2 = new double[] { 'F', 'I', 'R', 'S', 'T' };
            var list3 = new double[] { 'c', 'i' };
            // var list1 = new double[] { 1.0, 2.0, 3.0, 4.0, 2.0, 5.0 };
            //var list2 = new double[] { 2.0, 3.0, 4.0 };
            //var list3 = new double[] { 8.0, 1.0, 2.0 };

            predictor.Reset();
            PredictNextElement(predictor, list1);

            predictor.Reset();
            PredictNextElement(predictor, list2);

            predictor.Reset();
            PredictNextElement(predictor, list3);
        }

        private static void PredictNextElement(Predictor predictor, double[] list)
        {
            Debug.WriteLine("------------------------------");

            foreach (var item in list)
            {
                var res = predictor.Predict(item);

                if (res.Count > 0)
                {
                    foreach (var pred in res)
                    {
                        Debug.WriteLine($"{pred.PredictedInput} - {pred.Similarity}");
                    }

                    var tokens = res.First().PredictedInput.Split('_');

                    var tokens2 = res.First().PredictedInput.Split('-');
                    Console.WriteLine(tokens2.Last());
                    var tokens3 = tokens2.Last();
                    //char tokens4 = Convert.ToChar(tokens3);
                    Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens3.Last()}");
                    //Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens4}");
                }
                else
                    Debug.WriteLine("Nothing predicted :( ");
            }

            Debug.WriteLine("------------------------------");
        }
    }
}










