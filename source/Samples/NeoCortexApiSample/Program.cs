﻿using System;
using NeoCortexApi;
using NeoCortexApiSample;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
//using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections;
using System.Numerics;
using System.Drawing;
using System.Linq;
using System.IO;
using ScottPlot.Drawing.Colormaps;
using System.Runtime.Intrinsics.X86;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
//using static NeoCortexApi.Utility.GroupBy2<R>;
namespace NeoCortexApiSample
{
    public class Program

    {
        static void Main(string[] args)
        {
            RunMultiSequenceLearningExperiment();
        }
        /// <summary>
        /// This example demonstrates how to learn sequences and how to use the prediction mechanism.
        /// First,string is converted into an array of characters, and asciii value of each character is stored in a list.
        /// Second,sequences are learned from the text file.
        /// Third,testing data is used used for prediction. The predictor used by experiment privides to the HTM every element of every predicting sequence.
        /// The predictor tries to predict the next element.
        /// </summary>
        ///


        private static void RunMultiSequenceLearningExperiment()
        {


            List<double> inputValues = new List<double>() { 'T', 'h', 'e', 't', 'o', 't', 'e' };
            List<double> testingData = new List<double>();

            //  Path to the input text file.
            string filePath = "C:/Users/DELL/Desktop/sample.txt";

            string filePath1 = "C:/Users/DELL/Desktop/training.txt";
            List<char> charList1 = ReadFileAndConvertToCharList(filePath1);

            //Call the function to read the file and convert to char array.
            List<char> charList = ReadFileAndConvertToCharList(filePath);
            //Add asciiValue to a List 
            foreach (char character in charList)
            {
                double asciiValue = (double)character;

                inputValues.Add(asciiValue);

            }


            foreach (char character in charList1)
            {
                double asciiValue1 = (double)character;

                testingData.Add(asciiValue1);

            }



            Console.WriteLine("ASCII Sequence:");

            foreach (var item in inputValues)
            {
                Console.Write(item + " ");
            }


            List<Predictor> asciiVareturnedPredicor = new List<Predictor>();
            // Print the ascii value






            int batch_size = 8;
            int overlap = 4;

            //  Get the flattened list of batches with overlapping starting from the 4th index.
            List<double> totalBatch = SplitIntoBatches(inputValues, batch_size, overlap);

            for (int i = 0; i < totalBatch.Count; i += batch_size)
            {
                List<double> batch = totalBatch.GetRange(i, batch_size);

                // Prototype for building the prediction engine.
                MultiSequenceLearning experiment = new MultiSequenceLearning();
                var predictor = experiment.Run(batch);

                ////Predictions for the next elements in the input/test sequence.
                //PredictNextElement(predictor, testingData);


                // Read the user's input from the console
                Console.Write("\n Ask Question: ");

                // Read the user's input from the console
                string inputText = Console.ReadLine();

                List<double> asciiVal = new List<double>();

                foreach (char c in inputText)
                {

                    asciiVal.Add(c);

                }
                //Predictions for the next elements in the input/test sequence.
                PredictNextElement(predictor, asciiVal);
            }

        }

        /// <summary>
        /// Reads a set of list from an text file/user input.
        /// Calculates prediction accuracy for all the predictions.
        /// Output is determined by selecting the string with the highest accuracy.
        /// </summary>
        /// <param name="predictor">obj of class Predictor</param>
        /// <param name="list">List of sequences</param>
        private static void PredictNextElement(Predictor predictor, List<double> list)
        {
            Debug.WriteLine("------------------------------");

            int totalMatches = 0;
            int totalPredictions = 0;
            double maxAccuracy = 0.0;
            string bestResponse = "";
            string generatedResponse = "";
            double highestaccuracy = 0.0;
            double accuracy = 0.0;

            for (int i = 0; i < list.Count - 1; i++)
            {
                var val = list[i];
                var predictedVal = list[i + 1];
                var res = predictor.Predict(val);

                if (res.Count > 0)
                {
                    var tokens = res.First().PredictedInput.Split('_');
                    var tokens2 = res.First().PredictedInput.Split('-');

                    var tokens3 = tokens2.Last();
                    var token4 = res.First().PredictedInput.Split('_').Last();

                    Debug.WriteLine($"token4: {token4}, predicted next element {tokens2.Last()}");

                    Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens2.Last()}");

                    //Split a string into an array of substrings
                    string[] parts = tokens[0].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    // Create a list to store the double values
                    List<double> doubleList = new List<double>();

                    // Parse each part into a double and add it to the list
                    foreach (string part in parts)
                    {
                        double value = double.Parse(part);
                        doubleList.Add(value);
                    }
                    generatedResponse = DecodeNumericalSequence(doubleList);

                    Console.WriteLine($" \n Generated Response: {generatedResponse}");

                    Debug.WriteLine("Generated Response: " + generatedResponse);

                    if (predictedVal == double.Parse(tokens2.Last()))
                    {
                        totalMatches++;
                        Console.WriteLine($" totalMatches:{totalMatches}");
                    }
                    else
                    {
                        Console.WriteLine("Nothing predicted :( ");
                    }

                }
                else
                {
                    Debug.WriteLine("Nothing predicted :( ");
                }

                totalPredictions++;

                //accuracy = Accuracycalculation(totalMatches, totalPredictions);
                //Console.WriteLine($" sequence {generatedResponse} with  Accuracy: {accuracy}%");
                //Debug.WriteLine($" sequence {generatedResponse} with  Accuracy: {accuracy}%");

                (double maxAccuracyCalculated, string bestGeneratedResponse) = Accuracycalculation(totalMatches, totalPredictions, ref maxAccuracy, generatedResponse);
                highestaccuracy = maxAccuracyCalculated;
                bestResponse = bestGeneratedResponse;


                // Debug.WriteLine($"Predicted sequence : {generatedResponse},sequence with accuracy: {accuracy}%");
            }

            Console.WriteLine($" sequence {bestResponse} with highest Accuracy: {highestaccuracy}%");
            Debug.WriteLine($" sequence {bestResponse} with highest Accuracy: {highestaccuracy}%");
        }

        /// <summary>
        /// Calculates highest accuracy among all the prediction.
        /// </summary>
        /// <param name="accuracy">calculated accuracy </param>
        /// <param name="maxAcuuracy">maximum accuracy </param>
        /// <param name="generatedResponse">generated response for each cycle </param>
        ///<returns> an object of bestGeneratedResponse  </returns>
        //public static (double maxAccuracy, string bestGeneratedResponse) GetHighestAccuracyPrediction(double accuracy, double maxAccuracy, string generatedResponse)
        //    {
        //    string bestGeneratedResponse = "";
        //     if (accuracy > maxAccuracy)
        //        {
        //        maxAccuracy = accuracy;
        //        bestGeneratedResponse = generatedResponse;
        //        Console.WriteLine($" \n sequence {generatedResponse} with macAccuracy:  {maxAccuracy} %");
        //        }
        //    return (maxAccuracy, bestGeneratedResponse);
        //    }

        ///<summary>
        ///Method to calculate accuracy.
        ///</summary>
        ///<param name="totalMatches">totalMatches among all the predictions</param>
        ///<param name="totalPredictions">total number of predictions</param>
        ///<returns> accuracy as double val </returns>
        public static (double maxAccuracy, string bestGeneratedResponse) Accuracycalculation(int totalMatches, int totalPredictions, ref double maxAccuracy, string generatedResponse)
        {
            double accuracy = (double)totalMatches / totalPredictions * 100;
            Console.WriteLine($" sequence {generatedResponse} with  Accuracy: {accuracy}%");
            // return accuracy;
            string bestGeneratedResponse = "";
            if (accuracy > maxAccuracy)
            {
                maxAccuracy = accuracy;
                bestGeneratedResponse = generatedResponse;

            }
            Console.WriteLine($" sequence with maxAccuracy:  {maxAccuracy} %");
            return (maxAccuracy, bestGeneratedResponse);


        }

        ///<summary>
        ///Method to decode list values into characters.
        ///</summary>
        ///<param name="generatedTokens">generated prediction for each iteration</param>
        ///<returns>Object of string</returns>
        static string DecodeNumericalSequence(List<double> generatedTokens)
        {
            // Decode generated tokens to characters

            string decodedString = "";
            foreach (int token in generatedTokens)
            {
                decodedString += (char)token;
            }
            return decodedString;
        }

        ///<summary>
        ///Method to convert characters to ASCII values.
        ///</summary>
        ///<param name="charList">character list </param>
        ///<returns>Object of ascii values list</returns>
        public static List<double> ConvertToAscii(List<char> charList)
        {
            List<double> asciiValues = new List<double>();

            foreach (char character in charList)
            {
                double asciiValue = (double)character;
                asciiValues.Add(asciiValue);
            }

            return asciiValues;
        }

        ///<summary>
        ///Method to read the file and return char array.
        ///</summary>
        ///<param name="filePath">file path of text file</param>
        ///<returns>Object of character list</returns>
        public static List<char> ReadFileAndConvertToCharList(string filePath)

        {
            List<char> charList = new List<char>();

            try
            {
                // Read all text from the file
                string fileContent = File.ReadAllText(filePath);

                //Remove \r, \n, \t, and regular spaces
                string cleanedContent = fileContent.Replace("\r", "").Replace("\n", "").Replace("\t", "");

                //Join all characters into a single string
                string joinedString = string.Join("", cleanedContent.ToCharArray());

                //Write the joined string to the output file
                File.WriteAllText(@"outputFilePath", joinedString, Encoding.UTF8);

                Console.WriteLine("The spaces is removed successfully.");

                //Read the modified file
                string modified_fileContent = File.ReadAllText(@"outputFilePath");

                //Convert the string to a char array
                char[] charArray = modified_fileContent.ToCharArray();

                //Convert the char array to a list
                charList.AddRange(charArray);
            }
            catch (Exception ex)
            {
                //if not file found- Handle exceptions (e.g., file not found, access denied, etc.)
                Console.WriteLine("Error reading the file: " + ex.Message);
            }

            return charList;
        }

        ///<summary>
        ///Method to get an overlapping sequence, where each 8-character segment overlaps by 4 characters with the adjacent segments.
        ///</summary>
        ///<param name="inputValues">list of input values</param>
        ///<param name="batchSize">batchSize = 8 </param>
        ///<param name="overlap">overlap= 4 </param>
        ///<returns>Object of list of overlappingSequence</returns>
        public static List<double> SplitIntoBatches(List<double> inputValues, int batchSize, int overlap)
        {

            List<double> overlappingSequence = new List<double>();

            for (int i = 0; i < inputValues.Count - batchSize; i += overlap)
            {
                List<double> sequence = inputValues.GetRange(i, batchSize);
                overlappingSequence.AddRange(sequence);
            }
            return overlappingSequence;

        }


    }
}

