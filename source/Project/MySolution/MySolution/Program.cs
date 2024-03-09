/*using System;
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
            List<char> charList = ReadFileAndConvertToCharList( filePath);

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

        /*double BinaryCrossEntropy(List<double> predictedValues, List<double> actualValues)
        {
            double sumLoss = 0.0;

            // Ensure both lists have the same length
            if (predictedValues.Count != actualValues.Count)
                throw new ArgumentException("Predicted values and actual values must have the same length.");

            // Calculate binary cross-entropy loss for each predicted-actual pair
            for (int i = 0; i < predictedValues.Count; i++)
            {
                double predicted = predictedValues[i];
                double actual = actualValues[i];

                // Ensure the predicted value is within a valid range (e.g., [0, 1])
                predicted = Math.Max(1e-15, Math.Min(1 - 1e-15, predicted));

                // Calculate binary cross-entropy loss for this pair
                double loss = -((actual * Math.Log(predicted)) + ((1 - actual) * Math.Log(1 - predicted)));

                sumLoss += loss;
            }

            // Average the loss over all predicted-actual pairs
            double averageLoss = sumLoss / predictedValues.Count;

            return averageLoss;
        }}}*/



/*using System;
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

           // List<char> charList = new List<char>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Remove special characters from each line before processing
                        string cleanedLine = line.Replace("\r", "").Replace("\n", "")
                                                    .Replace("\t", "").Replace(" ", "");

                        // Convert the cleaned line to char array and add to charList
                        charList.AddRange(cleanedLine.ToCharArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading the file: " + ex.Message);
            }



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
}*/



using System;
using NeoCortexApi;
using NeoCortexApiSample;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
//using static NeoCortexApi.Utility.GroupBy2<R>;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Network;


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
            foreach (var item in x)
            {
                Console.Write(item + " ");
            }





            //Prototype for building the prediction engine.}
            MultiSequenceLearning experiment = new MultiSequenceLearning();

            {






                for (int i = 0; i < x.Count; i += block_size)
                {
                    List<double> chunk = x.GetRange(i, block_size);


                    Console.WriteLine("Batch:");
                    foreach (var item in chunk)
                    {
                        Console.Write(item + " ");
                    }


                    var predictor = experiment.Run(chunk);
                    int n = x.Count;
                    if (n < 32)
                    {
                        continue;
                    }



                    Console.Write("Ask Question: ");

                    // Read the user's input from the console
                    string inputText = Console.ReadLine();

                    List<double> asciiVal = new List<double>();

                    foreach (char c in inputText)
                    {
                        asciiVal.Add((double)c);
                    }



                    //Generate chatGPT model
                    PredictNextElement(predictor, asciiVal);


                }



            }




        }


        public static void PredictNextElement(Predictor predictor, List<double> myList)
        {
            Debug.WriteLine("------------------------------");

            foreach (var item in myList)
            {
                //list <double> res = predictor.Predict(asciiValues);
                var res = predictor.Predict(item);
                if (item == myList.First())
                {
                    if (res.Count > 0)
                    {
                        //foreach (var pred in res)
                        //    {
                        //    Debug.WriteLine($"{pred.PredictedInput} - {pred.Similarity}");
                        //    }

                        var tokens = res.First().PredictedInput.Split('_');

                        var tokens2 = res.First().PredictedInput.Split('-');

                        Console.WriteLine($"token 2 ={tokens2.Last()}");

                        var tokens3 = tokens2.Last();

                        Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens3.Last()}");

                        //List<double> responseSequence = new List<double>(asciiValues);

                        //responseSequence.Add(tokens3.Last());

                        string[] parts = tokens[0].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                        // Create a list to store the double values
                        List<double> doubleList = new List<double>();

                        // Parse each part into a double and add it to the list
                        foreach (string part in parts)
                        {
                            double value = double.Parse(part);
                            doubleList.Add(value);
                        }
                        //List<double> responseSequence = new List<double>(Array.ConvertAll(tokens[0].Split(' - '), double.Parse));


                        string generatedResponse = DecodeNumericalSequence(doubleList);

                        Console.WriteLine("Generated Response: " + generatedResponse); ;
                    }
                }
                else
                    Debug.WriteLine("Nothing predicted :( ");
                //}


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







































































































