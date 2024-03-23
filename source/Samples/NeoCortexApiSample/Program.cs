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
        private static List<double> inputValues1;

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


            List<double> inputValues = new List<double>();
            List<double> testingData = new List<double>();

            //  Path to the input text file.
            string filePath = "C:/Users/lenovo/Desktop/neocortexapi_SE/source/Project_SE/MySolution/MySolution/bin/Debug/net8.0filename.txt";

            string filePath1 = "C:/Users/lenovo/Desktop/testData.txt";
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

            //  List<double> asciiVal = new List<double>() { 'e','f'};
            MultiSequenceLearning experiment = new MultiSequenceLearning();

            //  Get the flattened list of batches with overlapping starting from the 4th index
            List<double> batch1 = SplitIntoBatches(inputValues, 8, 4);

            Console.WriteLine($" The size list is {batch1.Count}");

            Console.WriteLine($" The modified list is");
            foreach (var item in batch1)
            {
                Console.Write(item + " ");
            }



            for (int i = 0; i < batch1.Count; i += 8)
            {
                List<double> batch = batch1.GetRange(i, 8);





                var predictor = experiment.Run(inputValues1);







                asciiVareturnedPredicor = predictor.AddPredictor(predictor);
                predictor.Reset();



                PredictNextElement(predictor, testingData);



                Console.Write("Ask Question: ");

                // Read the user's input from the console
                string inputText = Console.ReadLine();

                List<double> asciiVal = new List<double>();

                foreach (char c in inputText)
                {

                    asciiVal.Add(c);

                }



            }

        }


        private static void PredictNextElement(Predictor predictor, List<double> list)
        {

            Debug.WriteLine("------------------------------");
            int countOfMatches = 0;
            int totalPredictions = 0;
            string predictedSequence = "";
            string predictedNextElement = "";
            string predictedNextElementsList = "";

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
                    predictedSequence = tokens[0];
                    predictedNextElement = tokens2.Last();
                    predictedNextElementsList = string.Join("-", tokens3.Skip(1));
                    Debug.WriteLine($"Predicted Sequence: {predictedSequence}, predicted next element {predictedNextElement}");

                    if (nextItem == double.Parse(predictedNextElement))
                    {
                        countOfMatches++;
                    }
                }
                else
                {
                    Debug.WriteLine("Nothing predicted :(");
                }

                totalPredictions++;

                // Accuracy logic added which is based on count of matches and total predictions.
                double accuracy = AccuracyCalculation(list, countOfMatches, totalPredictions, predictedSequence, predictedNextElement, predictedNextElementsList);
                Debug.WriteLine($"Final Accuracy for elements found in predictedNextElementsList = {accuracy}%");

            }

            Debug.WriteLine("------------------------------");
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
        //    {

        //    Debug.WriteLine("------------------------------");





        //    foreach (var item in myList)
        //        {

        //        var res = predictor.Predict(item);
        //       //   if (item == myList.First())
        //            {
        //            if (res.Count > 0)
        //                {


        //                var tokens = res.First().PredictedInput.Split('_');

        //                var tokens2 = res.First().PredictedInput.Split('-');

        //                Console.WriteLine($"token2 ={tokens2.Last()}");


        //                Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens2.Last()}");
        //                //  Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens4.Last()}");

        //                //Split a string into an array of substrings
        //                string[] parts = tokens[0].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

        //                // Create a list to store the double values
        //                List<double> doubleList = new List<double>();

        //                // Parse each part into a double and add it to the list
        //                foreach (string part in parts)
        //                    {
        //                    double value = double.Parse(part);
        //                    doubleList.Add(value);
        //                    }
        //                string generatedResponse = DecodeNumericalSequence(doubleList);

        //                Console.WriteLine($"\"Generated Response: +{ generatedResponse }");

        //                Debug.WriteLine("Generated Response: " + generatedResponse); 
        //                }
        //            else
        //                Debug.WriteLine("Nothing predicted :( ");


        //            Debug.WriteLine("------------------------------");
        //            }
        //        }

        //    static string DecodeNumericalSequence(List<double> generatedTokens)
        //        {
        //        // Decode generated tokens to characters

        //        string decodedString = "";
        //        foreach (int token in generatedTokens)
        //            {
        //            decodedString += (char)token;
        //            }
        //        return decodedString;
        //        }

        //    }





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

        public static object PredictNextElement(List<double> inputList)
        {
            throw new NotImplementedException();
        }
    }
}














































