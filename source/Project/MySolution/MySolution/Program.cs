using System;
using NeoCortexApi;
using NeoCortexApiSample;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using static NeoCortexApi.Utility.GroupBy2<R>;

namespace NeoCortexApiSample
{
    class Program
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


            //Path to the input text file.
            string filePath = @"filename.txt";

            //Call the function to read the file and convert to char array.
            List<char> charList = ReadFileAndConvertToCharList(filePath);

            //Add asciiValue to a List 
            foreach (char character in charList)
            {
                double asciiValue = (double)character;

                inputValues.Add(asciiValue);

            }

            //Print the ascii value
            Console.WriteLine("ASCII Sequence:");
            foreach (var item in inputValues)
            {
                Console.Write(item + " ");
            }

            //Call a method to devide the inpit values as batches

            //Define block size.
            int block_size = 8;

            //Define batch size.
            int batch_size = 4;

            List<double> x = GetBatch(inputValues, block_size, batch_size);

            for (int i = 0; i < x.Count; i += block_size)
            {
                List<double> chunk = x.GetRange(i, block_size);



                Console.WriteLine("Batch:");
                foreach (var item in chunk)
                {
                    Console.Write(item + " ");
                }

                //Prototype for building the prediction engine.}
                MultiSequenceLearning experiment = new MultiSequenceLearning();
                var predictor = experiment.Run(chunk);




                // These list are used to see how the prediction works.
                // Predictor is traversing the list element by element. 
                // By providing more elements to the prediction, the predictor delivers more precise result.
                var list1 = new double[] { 'T', 'r', 'a', 'i', 'n' };
                //var list2 = new double[] { 'F', 'I', 'R', 'S', 'T' };
                // var list3 = new double[] { 'y', 'o', 'u' };


                //predictor.Reset();
                //PredictNextElement(predictor, list1);

                //predictor.Reset();
                //PredictNextElement(predictor, list2);

                predictor.Reset();
                PredictNextElement(predictor, list1);
            }
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

                    Console.WriteLine($"token 2 ={tokens2.Last()}");

                    var tokens3 = tokens2.Last();

                    Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens3.Last()}");

                }
                else
                    Debug.WriteLine("Nothing predicted :( ");
            }

            Debug.WriteLine("------------------------------");
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

        //Function to divide the input values to a set of batches
        public static List<double> GetBatch(List<double> data, int block_size, int batch_size)
        {
            Random random = new Random();
            int totalDataSize = data.Count;
            if (totalDataSize < block_size)
            {

                {
                    int paddingLength = 8 - totalDataSize;
                    if (paddingLength > 0)
                    {
                        for (int i = 0; i < paddingLength; i++)
                        {
                            data.Add(0); // Add zeros at the end
                        }
                    }
                }

            }
            Console.WriteLine($"totalDataSize: {totalDataSize}");
            int dataSize = data.Count;
            List<double> x = new List<double>();

            for (int i = 0; i < batch_size; i++)
            {
                int startIndex = random.Next(dataSize - block_size);
                for (int j = 0; j < block_size; j++)
                {
                    x.Add(data[startIndex + j]);
                }
            }

            return x;
        }
        

    }

}






















