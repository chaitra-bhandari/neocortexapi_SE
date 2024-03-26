
global using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NeoCortexApiSample;

[TestClass]
public class LanguageSemantic
    {
    // Test case for checking a valid file path
    [TestMethod]
    public void ReadFileAndConvertToCharList_ValidFilePath_ReturnsListOfChars()
        {
        // Arrange
        string filePath = "/Users/chaitrabhandari/Desktop/filename.txt"; // Assuming you have a valid test file


        // Act
        List<char> charList = Program.ReadFileAndConvertToCharList(filePath);

        // Assert
        Assert.IsNotNull(charList);
        Assert.IsTrue(charList.Count > 0);
        
        }


    [TestMethod]
    public void ReadFileAndConvertToCharList_Test()
        {
        // Arrange
        string filePath = "/Users/chaitrabhandari/Desktop/filename.txt"; // Path to your sample text file


        // Act
        List<char> result = Program.ReadFileAndConvertToCharList(filePath);

        // Assert
        // Define expected content based on the sample text file
        List<char> expected = new List<char>
            {
               'T', 'h', 'e','t','o','t','a','l'
            };

        CollectionAssert.AreEqual(expected, result);
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

    //[TestMethod]
    //public void Predictor_PredictNextElement_ReturnsCorrectPrediction()

    //    {

    //    List<double> inputValues = new List<double>();

    //    string filePath = "/Users/chaitrabhandari/Desktop/filename.txt"; // Assuming you have a valid test file

    //    List<char> charList = Program.ReadFileAndConvertToCharList(filePath);

    //    //Add asciiValue to a List 
    //    foreach (char character in charList)
    //        {
    //        double asciiValue = (double)character;

    //        inputValues.Add(asciiValue);
    //        }

    //    string testingData = "input";

    //    List<double> asciiVal = new List<double>();

    //    foreach (char c in testingData)
    //        {

    //        asciiVal.Add(c);

    //        }
    //    MultiSequenceLearning experiment = new MultiSequenceLearning();

    //    var predictor = experiment.Run(inputValues);

    //    Program.PredictNextElement(predictor, asciiVal);


    //    // Assert
    //    // Assert your expected prediction result
    //    Assert.IsNotNull(predictor); // Example assertion, modify based on your requirements
    //                                 // Assert.AreEqual();
    //    }



    //[TestMethod]
    //public void TestBinaryCrossEntropyCalculation()
    //    {
    //    // Define the actual outputs and predicted sets
    //    List<int> actualOutputs = new List<int> { 9427, 9483, 9633, 9768, 9997, 10009, 10062, 10089, 10142, 10689, 10753, 10944, 11202, 11357, 11384, 11570, 11585, 11670, 12002, 12290 };
    //    List<List<int>> predictedSets = new List<List<int>>
    //    {
    //        new List<int> {10101, 10247, 10718, 10755, 10932, 11215, 11367, 11558, 11773, 12111, 12288, 12325, 12423, 12608, 12649, 13071, 13341, 13478, 13821},
    //        new List<int> {625, 1903, 2028, 3494, 3543, 3572, 3597, 3610, 3638, 3768, 3849, 4037, 4061, 4129, 4348, 4354, 4628, 4693, 4772, 4974},
    //        new List<int> {1639, 1645, 3531, 3544, 3604, 3622, 3630, 3635, 3758, 3773, 3830, 3838, 4029, 4044, 4095, 4130, 4131, 4157, 4336, 4346, 4361, 4366, 4500, 4638, 4642, 4678, 4688, 4763, 4770, 4905, 4951, 4954, 5048, 5181},
    //        new List<int> {1632, 1908, 2029, 2988, 3025, 3061, 3129, 3202, 3246, 3301, 3491, 3548, 3561, 3575, 3614, 3760, 3831, 4031, 4064, 4369},
    //        new List<int> {1625, 1903, 2028, 3494, 3543, 3572, 3587, 3591, 3605, 3610, 3638, 3768, 3849, 4037, 4061, 4129, 4146, 4348, 4354, 4628, 4693, 4772, 4974},
    //        new List<int> {1646, 1901, 2049, 3493, 3529, 3566, 3591, 3610, 3649, 3763, 3845, 4035, 4056, 4129, 4331, 4350, 4632, 4680, 4755, 4964},
    //        new List<int> {1639, 1645, 1646, 1901, 1904, 2027, 2049, 3492, 3493, 3529, 3531, 3544, 3566, 3567, 3597, 3604, 3622, 3630, 3635, 3649, 3758, 3763, 3773, 3830, 3838, 3845, 4029, 4035, 4044, 4052, 4056, 4130, 4131, 4331, 4336, 4346, 4350, 4361, 4366, 4632, 4638, 4642, 4678, 4680, 4688, 4755, 4763, 4770, 4951, 4954, 4964 }
    //    };

    //    double leastBCE = 1.0;
    //    double totalBCE = 0.0;
    //    double bce;
    //    // Define the threshold for correctness (adjust as needed)
    //    int threshold = 10;

    //    // Calculate binary cross-entropy for each predicted set and actual outputs
    //    foreach (var predictedSet in predictedSets)
    //        {
    //        bce = RunLanguageSemantic.CalculateBinaryCrossEntropy(actualOutputs, predictedSet, threshold);

    //        if (bce > 0 && bce < leastBCE)
    //            {
    //            leastBCE = bce;
    //            }
    //        }

    //    totalBCE += leastBCE;

    //    // Output BCE for the current set
    //    Console.WriteLine($"Binary Cross-Entropy for Set: {totalBCE}");

    //    // Assert the binary cross-entropy value (add your assertions here)
    //    Assert.IsTrue(totalBCE >= 0);  // Basic assertion, ensuring BCE is non-negative


    //    }




    }
