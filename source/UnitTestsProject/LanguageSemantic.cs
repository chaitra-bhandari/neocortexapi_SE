using System;
using NeoCortexApiSample;
using System.Collections.Generic;
//namespace NeoCortexApiSample;


namespace UnitTestsProject
    {
   


        [TestClass]
        public class LanguageSemantic
            {
            /// <summary>
            /// Test case for checking a valid file path
            /// </summary>
            [TestMethod]
            [DataRow(@"testData.txt")]
            public void ReadFileAndConvertToCharList_ValidFilePath_ReturnsListOfChars(string filePathToTrainingData)
                {

                List<char> charList = Program.ReadFileAndConvertToCharList(filePathToTrainingData);

                // Assert
                Assert.IsNotNull(charList);
                Assert.IsTrue(charList.Count > 0);

                }

            /// <summary>
            /// Test case for checking a valid file path
            /// </summary>
            [TestMethod]
            [DataRow(@"trainData.txt")]
            public void ReadFileAndConvertToCharList_Test(string filePathToTrainingData)
                {

                List<char> result = Program.ReadFileAndConvertToCharList(filePathToTrainingData);


                List<double> charList = Program.ConvertToAscii(result);

                List<double> expectedList = new List<double>() { 109, 121, 32, 100, 97, 121, 32, 97, 116, 46 };

                Assert.IsNotNull(result);
                Assert.IsNotNull(charList);
                Assert.AreEqual(result.Count, charList.Count);
                CollectionAssert.AreEqual(expectedList, charList);
                }

            // <summary>
            /// Test case for checking a valid file path
            /// </summary>
            [TestMethod]
            public void SplitIntoBatches_Test()
                {
                // Arrange
                List<double> inputValues = new List<double> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
                int block_size = 8;
                int overlap = 4;

                // Expected result after splitting into batches with the specified block size and overlap
                List<double> expectedBatch = new List<double> { 1, 2, 3, 4, 5, 6, 7, 8, 5, 6, 7, 8, 9, 10, 11, 12 };


                List<double> result = Program.SplitIntoBatches(inputValues, block_size, overlap);


                CollectionAssert.AreEqual(expectedBatch, result);
                }


            //<summary>
            //Test case to check predicted output and accuracy, by using "trainData1.txt" and user input as string. wihout testdata text file
            /// </summary>
            [TestMethod]
            [DataRow(@"trainData1.txt")]
            public void Predictor_PredictNextElement_ForInput(string filePathToTrainingData)

                {

                List<double> inputValues = new List<double>();
                List<double> testingData = new List<double>();


                //Call the method to read the file and convert to char array.
                List<char> charListOfTrainData = Program.ReadFileAndConvertToCharList(filePathToTrainingData);


                //Call the method to convert character to ASCII.
                inputValues = Program.ConvertToAscii(charListOfTrainData);


                int batch_size = 8;
                int overlap = 4;


                string testText = "Night divides";

                List<double> asciiVal = new List<double>();

                foreach (char c in testText)
                    {

                    asciiVal.Add(c);

                    }


                //Get the flattened list of batches with overlapping starting from the 4th index.
                List<double> totalBatch = Program.SplitIntoBatches(inputValues, batch_size, overlap);

                for (int i = 0; i < totalBatch.Count; i += batch_size)
                    {
                    List<double> batch = totalBatch.GetRange(i, batch_size);

                    // Prototype for building the prediction engine.
                    RunLanguagrSemantic experiment = new RunLanguagrSemantic();

                    var predictor = experiment.Run(batch);

                    Program.PredictNextElement(predictor, asciiVal);

                    Assert.IsNotNull(predictor);
                    }

                }

            //<summary>
            //Test case to check predicted output and accuracy, by using "trainData1.txt" and ""rainData1.txt as string.
            ///</summary>
            [TestMethod]
            [DataRow(@"trainData1.txt", @"testData1.txt")]
            public void Predictor_PredictNextElement_ForUserInput(string filePathToTrainingData, string filePathToTestData)

                {

                List<double> inputValues = new List<double>();
                List<double> testingData = new List<double>();


                //Call the method to read the file and convert to char array.
                List<char> charListOfTrainData = Program.ReadFileAndConvertToCharList(filePathToTrainingData);

                //Call the method to read the file and convert to char array.
                List<char> charListTestData = Program.ReadFileAndConvertToCharList(filePathToTestData);

                //Call the method to convert character to ASCII.
                inputValues = Program.ConvertToAscii(charListOfTrainData);
                testingData = Program.ConvertToAscii(charListTestData);

                int batch_size = 8;
                int overlap = 4;


                string testText = "Night divides";

                List<double> asciiVal = new List<double>();

                foreach (char c in testText)
                    {

                    asciiVal.Add(c);

                    }


                // Get the flattened list of batches with overlapping starting from the 4th index.
                List<double> totalBatch = Program.SplitIntoBatches(inputValues, batch_size, overlap);

                for (int i = 0; i < totalBatch.Count; i += batch_size)
                    {
                    List<double> batch = totalBatch.GetRange(i, batch_size);

                    // Prototype for building the prediction engine.
                    RunLanguagrSemantic experiment = new RunLanguagrSemantic();

                    var predictor = experiment.Run(batch);

                    Program.PredictNextElement(predictor, testingData);


                    Program.PredictNextElement(predictor, asciiVal);

                    Assert.IsNotNull(predictor);
                    }
                }
            }
        }
    
        
    

