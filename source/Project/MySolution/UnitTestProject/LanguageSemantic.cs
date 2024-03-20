
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NeoCortexApiSample;


[TestClass]
public class LanguageSemantic
    {
    // Test case for checking a valid file path
    [TestMethod]
    public void ReadFileAndConvertToCharList_ValidFilePath_ReturnsListOfChars()
        {
        // Given a valid test file
        string filePath = @"filename.txt"; 

        //Call a method to convert strings into charlist
        List<char> charList = Program.ReadFileAndConvertToCharList(filePath);

        // Assert to check the file path
        Assert.IsNotNull(filePath);
        // Assert to check file not empt
        // y
        Assert.IsTrue(charList.Count > 0);
         
        }



    [TestMethod]
    public void ConvertCharListToAsciiList_Test()
        {

        string testingFilePath = "@filename.txt";
        // Create a list of characters representing test data

        List<char> result = Program.ReadFileAndConvertToCharList(testingFilePath);

        // Call the method under test to convert the character list to ASCII values
        List<double> actualAsciiValues = Program.ConvertCharListToAsciiList(result);


        // Create a list of expected ASCII values corresponding to the characters in the test data
        List<double> expectedAsciiValues = new List<double> { 84,104,101,32,116,114,97,105,110,32,102,114,111,109,32,73,110,100,105,97,73,32,97,109,32,102,114,111,109,32,73,110,100,105,97,105,32,108,105,118,101,32,105,110,32,71,101,114,109,97,110,121

};

        // Assert
        // Compare the expected ASCII values with the actual ASCII values obtained from the method call
        CollectionAssert.AreEqual(expectedAsciiValues, actualAsciiValues);
        }


    [TestMethod]
    public void SplitIntoBatches_Test()
        {
        // Arrange
        List<double> inputValues = new List<double> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
        int block_size = 8;
        int overlap = 4;

        // Expected result after splitting into batches with the specified block size and overlap
        List<double> expectedBatch = new List<double> { 1, 2, 3, 4, 5, 6, 7, 8, 5, 6, 7, 8, 9, 10, 11, 12 };

        // Act
        List<double> result = Program.SplitIntoBatches(inputValues, block_size, overlap);

        // Assert
        CollectionAssert.AreEqual(expectedBatch, result);
        }

    [TestMethod]
    public void Predictor_PredictNextElement_ReturnsCorrectPrediction()

        {

        List<double> inputValues = new List<double>();

        string filePath = "/Users/chaitrabhandari/Desktop/filename.txt"; 

        List<char> charList = Program.ReadFileAndConvertToCharList(filePath);

        //Add asciiValue to a List 
        foreach (char character in charList)
            {
            double asciiValue = (double)character;

            inputValues.Add(asciiValue);
            }

        string testingData = "input";

        List<double> asciiVal = new List<double>();

        foreach (char c in testingData)
            {

            asciiVal.Add(c);

            }

            RunLanguageSemanticExperiment experiment = new RunLanguageSemanticExperiment();

            var predictor = experiment.Run(inputValues);

            Program.PredictNextElement(predictor, asciiVal);

          
            //Assert to check 
            Assert.IsNotNull(predictor); 
            }


 }

