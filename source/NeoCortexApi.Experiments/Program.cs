using System;
using NeoCortexApi;
using NeoCortexApiSample;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections;
using System.Numerics;
using System.Drawing;
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
        private static void RunMultiSequenceLearningExperiment()
            {

            List<double> inputValues = new List<double>();
            List<double> testingData = new List<double>();

            //Path to the input text file(Training data).
            string filePathTrainData = @"filename.txt";

            //Call the function to read the file and convert to char array.
            List<char> charListOfTrainData = ReadFileAndConvertToCharList(filePathTrainData);

            //Path to the input text file(Testin Data).
            string filePathTestData = @"Testdata.txt";
            List<char> charListOfTeatData = ReadFileAndConvertToCharList(filePathTestData);

            //Add asciiValue to a List 
            foreach (char character in charListOfTrainData)
                {
                double asciiValue = (double)character;

                inputValues.Add(asciiValue);

                }

            //Add asciiValue to a List 
            foreach (char character in charListOfTeatData)
                {
                double asciiValue = (double)character;

                testingData.Add(asciiValue);

                }



            MultiSequenceLearning experiment = new MultiSequenceLearning();

            // Define block size
            int block_size = 8;

            //Define overlaping charcters size.
            int ovrlap = 4;


            // Get the list of batches with overlapping starting from the 4th index
            List<double> oberlappingSequence = SplitIntoBatches(inputValues, block_size, ovrlap);

            for (int i = 0; i < oberlappingSequence.Count; i += 8)
                {
                List<double> batch = oberlappingSequence.GetRange(i, 8);

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
                PredictNextElement(predictor, asciiVal);



                }
            }

        public static void PredictNextElement(Predictor predictor, List<double> myList)
            {
            Debug.WriteLine("------------------------------");

            foreach (var item in myList)
                {

                var res = predictor.Predict(item);
                if (item == myList.First())
                    {
                    if (res.Count > 0)
                        {
                        var tokens = res.First().PredictedInput.Split('_');

                        var tokens2 = res.First().PredictedInput.Split('-');

                        Console.WriteLine($"token 2 ={tokens2.Last()}");

                        var tokens3 = tokens2.Last();

                        Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens3.Last()}");

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
                        }
                    }
                else
                    Debug.WriteLine("Nothing predicted :( ");

                Debug.WriteLine("------------------------------");

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
                }
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

            for (int i = 0; i < numbers.Count - 8; i += 4) // Increment by 4
                {
                List<double> sequence = numbers.GetRange(i, 8);
                overlappingSequence.AddRange(sequence);
                }

            return overlappingSequence;

            }
        }
    }



















































