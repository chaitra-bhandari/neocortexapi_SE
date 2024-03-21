#  ML 23/24-10 Multi-Sequence Learning with language semantic.


# Introduction

This project aims to implement the application of multi-sequence learning techniques to generate a predicting code based on song or story text that represents the completion engine as used by GPT with prediction accuracy in C#/. NET Core. This project will first explore the existing __RunMultiSequenceLearningExperiment()__, which was implemented in __NeoCortex__ API, a .NET Core library. NeoCortex API is the implementation of Hierarchical Temporal Memory Cortical Learning Algorithm based on Spatial Pooler, Temporal Pooler, various encoders and CorticalNetwork algorithms.

This project implements a new method __RunLanguageSemanticExperiment()__ for learning data from the text file (long text). The data will be divided into two parts. 90% of the data will be used for training and 10% for testing. The model will be trained with the training data first. After learning is completed, the model is tested by testing data from another file to evaluate its prediction accuracy.


# Getting Started

## Processing input data
Input data processing involves transforming a lengthy text file sequence into a list of ASCII characters, achieved by excluding control characters such as \r, \n, \t, etc.

 ```csharp

      
      List<double> inputValues = new List<double>();
      //input text file path
      string filePath = @"filename.txt";
      List<char> charList = ReadFileAndConvertToCharList(filePath);

      //Add asciiValue to a List 
     foreach (char character in charList)
         {
         double asciiValue = (double)character;
         inputValues.Add(asciiValue);
        }

     public static List<char> ReadFileAndConvertToCharList(string filePath)
       {
     List<char> charList = new List<char>();
          try
          {
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
```

## Generate an overlapping sequence of input data
 The provided code returns an overlapping sequence, where each 8-character segment overlaps by 4 characters with the adjacent segments.
 ```csharp

  static List<double> SplitIntoBatches(List<double> numbers, int batchSize, int overlap)
            {

            List<double> overlappingSequence = new List<double>();

            for (int i = 0; i < numbers.Count - 8; i += 4) // Increment by 4
                {
                List<double> sequence = numbers.GetRange(i, 8);
                overlappingSequence.AddRange(sequence);
                }

            return overlappingSequence;

            }
```
           
       

