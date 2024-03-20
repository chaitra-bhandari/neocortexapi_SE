﻿using System;
using NeoCortexApi;

using NeoCortexApiSample;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections;
using System.Numerics;
using System.Drawing;


namespace NeoCortexApiSample
    {
    public class Program

        {
        static void Main(string[] args)
            {
            RunLanguageSemanticExperiment();
            }

        /// <summary>
        /// This example demonstrates how to learn sequences and how to use the prediction mechanism.
        /// First,string is converted into an array of characters, and asciii value of each character is stored in a list.
        /// Second,sequences are learned from the text file.
        /// Third,testing data is used used for prediction. The predictor used by experiment privides to the HTM every element of every predicting sequence.
        /// The predictor tries to predict the next element.
        /// </summary>
        private static void RunLanguageSemanticExperiment()
            {

            //List<double> inputValues1= new List<double>() {'a','e','i','o','u','m' };
            List<double> inputValues = new List<double>();
            List<double> testingData = new List<double>();

            //Path to the input text file(Training data).
            string filePathTrainData = @"filename.txt";

            //Call the function to read the file and convert to char array.
            List<char> charListOfTrainData = ReadFileAndConvertToCharList(filePathTrainData);

            //Call a function to Convert CharList To AsciiList
            inputValues = ConvertCharListToAsciiList(charListOfTrainData);

            //Path to the input text file(Testin Data).
            string filePathTestData = @"Testdata.txt";
            List<char> charListOfTeatData = ReadFileAndConvertToCharList(filePathTestData);

            testingData = ConvertCharListToAsciiList(charListOfTeatData);
            ////Add asciiValue to a List 
            //foreach (char character in charListOfTrainData)
            //    {
            //    double asciiValue = (double)character;

            //    inputValues.Add(asciiValue);

            //    }

            ////Add asciiValue to a List 
            //foreach (char character in charListOfTeatData)
            //    {
            //    double asciiValue = (double)character;

            //    testingData.Add(asciiValue);

            //    }




            RunLanguageSemanticExperiment experiment = new RunLanguageSemanticExperiment();

            // Define block size
            int block_size = 8;

            //Define overlaping charcters size.
            int ovrlap = 4;


            // Get the list of batches with overlapping starting from the 4th index
            List<double> overlappingSequence = SplitIntoBatches(inputValues, block_size, ovrlap);

            for (int i = 0; i < overlappingSequence.Count; i += block_size)
                {
                List<double> batch = overlappingSequence.GetRange(i, block_size);

                var predictor = experiment.Run(batch);

                predictor.Reset();
                PredictNextElement(predictor, testingData);

                //In the commomd promt print as ask Question
                Console.Write("Ask Question: ");

                // Read the user's input from the console
                string inputText = Console.ReadLine();

                List<double> asciiVal = new List<double>();

                foreach (char c in inputText)
                    {

                    asciiVal.Add(c);
                    }

               
                predictor.Reset();
                PredictNextElement(predictor, asciiVal);



                }
            }

        //public static void PredictNextElement(Predictor predictor, List<double> list)
        //    {
        //    Debug.WriteLine("------------------------------");
        //    int countOfMatches = 0;
        //    int totalPredictions = 0;
        //    string predictedSequence = "";
        //    string predictedNextElement = "";
        //    string predictedNextElementsList = "";


        //    for (int i = 0; i < list.Count-1 ; i++)
        //        {
        //        var item = list[i];
        //        var nextItem = list[i + 1];
        //        var res = predictor.Predict(item);

        //        if (res.Count > 0)
        //            {
        //            var tokens = res.First().PredictedInput.Split('_');
        //            var tokens2 = res.First().PredictedInput.Split('-');
        //            var tokens3 = res.Last().PredictedInput.Split('_');
        //            predictedSequence = tokens[0];
        //            predictedNextElement = tokens2.Last();
        //            predictedNextElementsList = string.Join("-", tokens3.Skip(1));
        //            Debug.WriteLine($"Predicted Sequence: {predictedSequence}, predicted next element {predictedNextElement}");

        //            //Split a string into an array of substrings
        //            string[] parts = tokens[0].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

        //            // Create a list to store the double values
        //            List<double> doubleList = new List<double>();

        //            // Parse each part into a double and add it to the list
        //            foreach (string part in parts)
        //                {
        //                double value = double.Parse(part);
        //                doubleList.Add(value);
        //                }
        //            string generatedResponse = DecodeNumericalSequence(doubleList);

        //            Console.WriteLine("Generated Response: " + generatedResponse); ;

        //            if (nextItem == double.Parse(predictedNextElement))
        //                {
        //                countOfMatches++;
        //                }
        //            }
        //        else
        //            {
        //            Debug.WriteLine("Nothing predicted :(");
        //            }

        //        totalPredictions++;

        //        // Accuracy logic added which is based on count of matches and total predictions.
        //        double accuracy = AccuracyCalculation(list, countOfMatches, totalPredictions, predictedSequence, predictedNextElement, predictedNextElementsList);
        //        Debug.WriteLine($"Final Accuracy for elements found in predictedNextElementsList = {accuracy}%");

        //        }

        //    Debug.WriteLine("------------------------------");
        //    }

        //public static string DecodeNumericalSequence(List<double> generatedTokens)
        //    {
        //    // Decode generated tokens to characters
        //    string decodedString = "";
        //    foreach (int token in generatedTokens)
        //        {
        //        decodedString += (char)token;
        //        }

        //    return decodedString;
        //    }


        //// Accuracy logic added which is based on count of matches and total predictions.
        //// Accuracy is calculated in the context of predicting the next element in a sequence.
        //// The accuracy is calculated as the percentage of correctly predicted next elements (countOfMatches)
        //// out of the total number of predictions (totalPredictions).
        //private static double AccuracyCalculation(List<double> list, int countOfMatches, int totalPredictions, string predictedSequence, string predictedNextElement, string predictedNextElementsList)
        //    {
        //    double accuracy = (double)countOfMatches / totalPredictions * 100;
        //    Debug.WriteLine(string.Format("The test data list: ({0}).", string.Join(", ", list)));

        //    // Append to file in each iteration
        //    if (predictedNextElementsList != "")
        //        {
        //        string line = $"Predicted Sequence Number is: {predictedSequence}, Predicted Sequence: {predictedNextElementsList}, Predicted Next Element: {predictedNextElement}, with Accuracy =: {accuracy}%";

        //        Debug.WriteLine(line);

        //        }
        //    else
        //        {
        //        string line = $"Nothing is predicted, Accuracy is: {accuracy}%";
        //        Debug.WriteLine(line);
        //        }
        //    return accuracy;
        //    }
        ////
        ////}}


        public static void PredictNextElement(Predictor predictor, List<double> list)
            {
            Debug.WriteLine("------------------------------");

            double highestAccuracy = 0;
            string sequenceWithHighestAccuracy = "";
            List<double> tokensWithHighestAccuracy = new List<double>();

            for (int i = 0; i < list.Count - 1; i++)
                {
                var item = list[i];
                var nextItem = list[i + 1];
                var res = predictor.Predict(item);

                if (res.Count > 0)
                    {
                    var tokens = res.First().PredictedInput.Split('_');
                    var tokens2 = res.First().PredictedInput.Split('-');
                    var tokens3 = res.Last().PredictedInput.Split('_');
                    string predictedSequence = tokens[0];
                    string predictedNextElement = tokens2.Last();
                    string predictedNextElementsList = string.Join("-", tokens3.Skip(1));
                    Debug.WriteLine($"Predicted Sequence: {predictedSequence}, predicted next element {predictedNextElement}");

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
                    string generatedResponse = DecodeNumericalSequence(doubleList);

                    Console.WriteLine("Generated Response: " + generatedResponse); ;

                    int countOfMatches = (nextItem == double.Parse(predictedNextElement)) ? 1 : 0;
                    double accuracy = AccuracyCalculation(list, countOfMatches, 1, predictedSequence, predictedNextElement, predictedNextElementsList);

                    if (accuracy > highestAccuracy)
                        {
                        highestAccuracy = accuracy;
                        sequenceWithHighestAccuracy = predictedSequence;
                        tokensWithHighestAccuracy = doubleList;
                        }
                    }
                else
                    {
                    Debug.WriteLine("Nothing predicted :(");
                    }

                }

            Debug.WriteLine("------------------------------");
            Console.WriteLine($"Sequence with Highest Accuracy: {sequenceWithHighestAccuracy}");
            Console.WriteLine($"Highest Accuracy: {highestAccuracy}%");
            Console.WriteLine($"Generated Response: {DecodeNumericalSequence(tokensWithHighestAccuracy)}");
            }

        public static string DecodeNumericalSequence(List<double> generatedTokens)
            {
            // Decode generated tokens to characters
            string decodedString = "";
            foreach (int token in generatedTokens)
                {
                decodedString += (char)token;
                }

            return decodedString;
            }


        // Accuracy logic added which is based on count of matches and total predictions.
        // Accuracy is calculated in the context of predicting the next element in a sequence.
        // The accuracy is calculated as the percentage of correctly predicted next elements (countOfMatches)
        // out of the total number of predictions (totalPredictions).
        private static double AccuracyCalculation(List<double> list, int countOfMatches, int totalPredictions, string predictedSequence, string predictedNextElement, string predictedNextElementsList)
            {
            double accuracy = (double)countOfMatches / totalPredictions * 100;
            Debug.WriteLine(string.Format("The test data list: ({0}).", string.Join(", ", list)));

            // Append to file in each iteration
            if (predictedNextElementsList != "")
                {
                string line = $"Predicted Sequence Number is: {predictedSequence}, Predicted Sequence: {predictedNextElementsList}, Predicted Next Element: {predictedNextElement}, with Accuracy =: {accuracy}%";

                Debug.WriteLine(line);

                }
            else
                {
                string line = $"Nothing is predicted, Accuracy is: {accuracy}%";
                Debug.WriteLine(line);
                }
            return accuracy;
            }






        //public static void PredictNextElement(Predictor predictor, List<double> myList)
        //        {
        //        Debug.WriteLine("------------------------------");

        //        foreach (var item in myList)
        //            {

        //            var res = predictor.Predict(item);
        //            if (item == myList.First())
        //                {
        //                if (res.Count > 0)
        //                    {
        //                    var tokens = res.First().PredictedInput.Split('_');

        //                    var tokens2 = res.First().PredictedInput.Split('-');

        //                    Console.WriteLine($"token 2 ={tokens2.Last()}");

        //                    var tokens3 = tokens2.Last();

        //                    Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens3.Last()}");

        //                    //Split a string into an array of substrings
        //                    string[] parts = tokens[0].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

        //                    // Create a list to store the double values
        //                    List<double> doubleList = new List<double>();

        //                    // Parse each part into a double and add it to the list
        //                    foreach (string part in parts)
        //                        {
        //                        double value = double.Parse(part);
        //                        doubleList.Add(value);
        //                        }
        //                    string generatedResponse = DecodeNumericalSequence(doubleList);

        //                    Console.WriteLine("Generated Response: " + generatedResponse); ;
        //                    }
        //                }
        //            else
        //                Debug.WriteLine("Nothing predicted :( ");

        //            Debug.WriteLine("------------------------------");

        //            static string DecodeNumericalSequence(List<double> generatedTokens)
        //                {

        //                // Decode generated tokens to characters
        //                string decodedString = "";
        //                foreach (int token in generatedTokens)
        //                    {
        //                    decodedString += (char)token;
        //                    }
        //                return decodedString;
        //                }
        //            }
        //        }


        //Function to convert CharList To AsciiList
        public static List<double> ConvertCharListToAsciiList(List<char> charListOfTrainData)
            {
            List<double> inputValues = new List<double>();

            foreach (char character in charListOfTrainData)
                {
                double asciiValue = (double)character;
                inputValues.Add(asciiValue);
                }

            return inputValues;
            }

        //Function to read the file and return  char array.
        public static List<char> ReadFileAndConvertToCharList(string filePath)

            {
            List<char> charList = new List<char>();

            try
                {
                // Read all text from the file
                string fileContent = File.ReadAllText(filePath);

                //Remove \r, \n, \t, and regular spaces
                string cleanedContent = fileContent.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");

                //Join all characters into a single string
                string joinedString = string.Join("", cleanedContent.ToCharArray());

                //Write the joined string to the output file
                File.WriteAllText(@"outputFilePath", joinedString, Encoding.UTF8);

                Console.WriteLine("The spaces is removed successfully.");

                //Read the modified file
                string fileContent1 = File.ReadAllText(@"outputFilePath");

                //Convert the string to a char array
                char[] charArray = fileContent1.ToCharArray();

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

        //Method divide the whole sequence into batch of 8 characters with overlapping of 4 charcters.
        public static List<double> SplitIntoBatches(List<double> numbers, int batchSize, int overlap)
            {

            List<double> overlappingSequence = new List<double>();

            for (int i = 0; i < numbers.Count - batchSize; i += overlap) // Increment by 4
                {
                List<double> sequence = numbers.GetRange(i, batchSize);
                overlappingSequence.AddRange(sequence);
                }

            return overlappingSequence;

            }
        }
   }



















































