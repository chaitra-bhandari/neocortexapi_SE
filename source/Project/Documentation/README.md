ML 23/24-10 Multi-Sequence Learning with language semantic.
Introduction
This project aims to implement the application of multi-sequence learning techniques to generate a predicting code based on song or story text that represents the completion engine as used by GPT with prediction accuracy in C#/. NET Core. This project will first explore the existing RunMultiSequenceLearningExperiment(), which was implemented in NeoCortex API, a .NET Core library. NeoCortex API is the implementation of Hierarchical Temporal Memory Cortical Learning Algorithm based on Spatial Pooler, Temporal Pooler, various encoders and CorticalNetwork algorithms.

This project implements a new method RunLanguageSemanticExperiment() for learning data from the text file (long text). The data will be divided into two parts. 90% of the data will be used for training and 10% for testing. The model will be trained with the training data first. After learning is completed, the model is tested by testing data from another file to evaluate its prediction accuracy.

Getting Started
The input data and test data are stored in files named "trainData.txt" and "testData.txt," respectively. Both datasets consist of textual data.

e.g :

  You know the day destroys the night
  
  Night divides the day
  
  Tried to run, tried to hide.
1. Processing input data
Input data processing involves transforming a text file sequence into a list of ASCII characters, achieved by excluding control characters such as \r, \n, \t, etc.

      //Path to the learning text file.
      string filePathToTrainingData = @"trainData.txt";

      //Call the method to read the file and convert to char array.
      List<char> charListOfTrainData = ReadFileAndConvertToCharList(filePathToTrainingData);

     public static List<char> ReadFileAndConvertToCharList(string filePath)
       {
          List<char> charList = new List<char>();
          try
          {
          string fileContent = File.ReadAllText(filePath);
         //Remove \r, \n, \t
         string cleanedContent = fileContent.Replace("\r", "").Replace("\n", "").Replace("\t", "");

         //Join all characters into a single string
         string joinedString = string.Join("", cleanedContent.ToCharArray());

         //Write the joined string to the output file
         File.WriteAllText(@"outputFilePath", joinedString, Encoding.UTF8);

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
2. Convert the charList to ASCII representation.
The provided code converts the charList retrieved from the ReadFileAndConvertToCharList(string filePath) method into ASCII values and stores them in the inputValues list.

      //Call the method to convert character to ASCII.
      inputValues = ConvertToAscii(charListOfTraindata);
      testingData = ConvertToAscii(charListTestData);

      //Method to convert character to ASCII
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
3. Generate an overlapping sequence of input data
The provided code returns an overlapping sequence, where each 8-character segment overlaps by 4 characters with the adjacent segments.

List<double> totalBatch = SplitIntoBatches(inputValues, batch_size, overlap);
static List<double> SplitIntoBatches(List<double> inputValues, int batchSize, int overlap)
     {

          List<double> overlappingSequence = new List<double>();

           for (int i = 0; i < inputValues.Count - batchSize; i += overlap) 
               {
               List<double> sequence = inputValues.GetRange(i, batchSize);
               overlappingSequence.AddRange(sequence);
               }

           return overlappingSequence;

      }
4. Retrieve the batch of input data from the overlapping sequence
A list of batches, each containing 8 characters, is passed iteratively to the Run() method impelemented in MultiSequenceLearning class.

int batch_size = 8;
int overlap = 4;
List<double> totalBatch = SplitIntoBatches(inputValues, batch_size, overlap);
for (int i = 0; i < totalBatch.Count; i += batch_size)
     {
         List<double> batch = totalBatch.GetRange(i, batch_size);
         // Prototype for building the prediction engine.
         MultiSequenceLearning experiment = new MultiSequenceLearning();
         var predictor = experiment.Run(batch);
      }
5. Compute the binary cross-entropy.
During the learning phase, the binary cross-entropy loss is computed by comparing the active SDRs with predicted values for each cycle. To determine accuracy, the average of the minimum binary cross-entropy values is calculated and then divided by the total number of cycles.

// Calculate binary cross entropy                   
static double CalculateCorrectness(List<int> actualOutputs, List<int> predictedValues, int threshold)
     {
           //Determine correctness based on threshold
            var correctness = actualOutputs.Zip(predictedValues, (actual, pred) => Math.Abs(actual - pred) <= threshold ? 1                         : 0).ToList();
            // Compute binary cross-entropy
            double bce = CalculateBinaryCrossEntropy(correctness);                       
return bce;
     }                                                     
static double CalculateBinaryCrossEntropy(List<int> correctness)
     {                                                  
            // Compute binary cross-entropy
            double bce = 0;
            foreach (var c in correctness)
                     {   
                        bce += c == 1 ? 0 : 1;
                      }
          return bce / correctness.Count;
     }
//Calculate average binary cross entropy 
double aveBCE = totalBCE / cycle;

double accuracyFromBinaryCrossEntropy = (1 - aveBCE) * 100.0;
Debug.WriteLine($"accuracyFromBinaryCrossEntropy  {accuracyFromBinaryCrossEntropy}%");
double accuracy = ( 1 - leastBCE)* 100.0;
Console.WriteLine($"Accuracy: {accuracy * 100}%");
                         
For the complete code for calculating binary cross entropy, refer to the CalculateBinaryCrossEntropy() and CalculateCorrectness() methods implemented in the MultiSequenceLearning.

6. Implement the code for prediction (inference).
The user's input or test data from a text file is passed to the predictor, which utilizes the PredictNextElement method to make predictions.

   //Read the user's input from the console
    Console.Write("Ask Question: ");

    //Read the user's input from the console
    string inputText = Console.ReadLine();

    List<double> asciiVal = new List<double>();

    foreach (char c in inputText)
       {

         asciiVal.Add(c);

       }
PredictNextElement(predictor, asciiVal);
7. Calculate the accuracy of predictions and output the prediction with the highest accuracy.
The accuracy is calculated based on the number of successful matches between predicted and total predictions.The output is determined by selecting the string with the highest accuracy from all the predictions made.

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

        Console.WriteLine($"\"Generated Response: {generatedResponse}");

        Debug.WriteLine("Generated Response: " + generatedResponse);

         if (predictedVal == double.Parse(tokens2.Last()))
             {
               totalMatches++;
               Console.WriteLine($" totalMatches:{totalMatches}");
             }

           }
         else
             {
               Debug.WriteLine("Nothing predicted :( ");
              }

              totalPredictions++;

              double accuracy = Accuracycalculation(totalMatches, totalPredictions);
              highestaccuracy = GetHighestAccuracyPrediction(accuracy, maxAccuracy, generatedResponse);
For the complete code for calculating prediction accuracy, refer to the PredictNextElement() method implemented in the MultiSequenceLearning

Results


