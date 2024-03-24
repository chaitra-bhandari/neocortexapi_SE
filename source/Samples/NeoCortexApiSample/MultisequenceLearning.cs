﻿
using NeoCortexApi;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;


namespace NeoCortexApiSample
{
    /// <summary>
    /// Implements an experiment that demonstrates how to learn sequences.
    /// </summary>
    public class MultisequenceLearning
    {
        /// <summary>
        /// Runs the learning of sequences.
        /// </summary>
        /// <param name="inputValues">List of sequences</param>
        public Predictor Run(List<double> inputValues)
        {
            Console.WriteLine($"Hello NeocortexApi! Experiment {nameof(MultisequenceLearning)}");

            int inputBits = 100;
            int numColumns = 1024;

            HtmConfig cfg = new HtmConfig(new int[] { inputBits }, new int[] { numColumns })
            {
                Random = new ThreadSafeRandom(42),

                CellsPerColumn = 25,
                GlobalInhibition = true,
                LocalAreaDensity = -1,
                NumActiveColumnsPerInhArea = 0.02 * numColumns,
                PotentialRadius = (int)(0.15 * inputBits),
                //InhibitionRadius = 15,

                MaxBoost = 10.0,
                DutyCyclePeriod = 25,
                MinPctOverlapDutyCycles = 0.75,
                MaxSynapsesPerSegment = (int)(0.02 * numColumns),

                ActivationThreshold = 15,
                ConnectedPermanence = 0.5,

                // Learning is slower than forgetting in this case.
                PermanenceDecrement = 0.25,
                PermanenceIncrement = 0.15,

                // Used by punishing of segments.
                PredictedSegmentDecrement = 0.1
            };

            //double max = 20;
            double max = 255;
            Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "W", 15},
                { "N", inputBits},
                { "Radius", -1.0},
                { "MinVal", 0.0},
                { "Periodic", false},
                { "Name", "scalar"},
                { "ClipInput", false},
                { "MaxVal", max}
            };

            EncoderBase encoder = new ScalarEncoder(settings);

            return RunExperiment(inputBits, cfg, encoder, inputValues);
        }


        private Predictor RunExperiment(int inputBits, HtmConfig cfg, EncoderBase encoder, List<double> inputValues)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int maxMatchCnt = 0;

            var mem = new Connections(cfg);

            bool isInStableState = false;

            HtmClassifier<string, ComputeCycle> cls = new HtmClassifier<string, ComputeCycle>();

            var numUniqueInputs = inputValues.Distinct<double>().ToList().Count;

            CortexLayer<object, object> layer1 = new CortexLayer<object, object>("L1");

            TemporalMemory tm = new TemporalMemory();

            // For more information see following paper: https://www.scitepress.org/Papers/2021/103142/103142.pdf
            HomeostaticPlasticityController hpc = new HomeostaticPlasticityController(mem, numUniqueInputs * 150, (isStable, numPatterns, actColAvg, seenInputs) =>
            {
                if (isStable)
                    // Event should be fired when entering the stable state.
                    Debug.WriteLine($"STABLE: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");
                else
                    // Ideal SP should never enter unstable state after stable state.
                    Debug.WriteLine($"INSTABLE: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");

                // We are not learning in instable state.
                isInStableState = isStable;

                // Clear active and predictive cells.
                //tm.Reset(mem);
            }, numOfCyclesToWaitOnChange: 50);


            SpatialPoolerMT sp = new SpatialPoolerMT(hpc);
            sp.Init(mem);
            tm.Init(mem);

            // Please note that we do not add here TM in the layer.
            // This is omitted for practical reasons, because we first eneter the newborn-stage of the algorithm
            // In this stage we want that SP get boosted and see all elements before we start learning with TM.
            // All would also work fine with TM in layer, but it would work much slower.
            // So, to improve the speed of experiment, we first ommit the TM and then after the newborn-stage we add it to the layer.
            layer1.HtmModules.Add("encoder", encoder);
            layer1.HtmModules.Add("sp", sp);

            double[] inputs = inputValues.ToArray();
            int[] prevActiveCols = new int[0];

            int cycle = 0;
            int matches = 0;

            //string lastPredictedValue = "0";
            var lastPredictedValues = new List<string>(new string[] { "0" });

            // var lastPredictedValues = "0";
            int maxCycles = 3500;

            //
            // Training SP to get stable. New-born stage.
            //

            for (int i = 0; i < maxCycles && isInStableState == false; i++)
            {
                matches = 0;

                cycle++;

                Debug.WriteLine($"-------------- Newborn Cycle {cycle} ---------------");


                foreach (var input in inputs)
                {
                    Debug.WriteLine($" -- {input} --");

                    var lyrOut = layer1.Compute(input, true);

                    if (isInStableState)
                        break;
                }

                if (isInStableState)
                    break;

            }

            // Clear all learned patterns in the classifier.
            cls.ClearState();

            // We activate here the Temporal Memory algorithm.
            layer1.HtmModules.Add("tm", tm);


            int maxPrevInputs = inputValues.Count - 1;

            List<string> previousInputs = new List<string>();

            previousInputs.Add("-1.0");

            // Set on true if the system has learned the sequence with a maximum acurracy.
            bool isLearningCompleted = false;

            double leastBCE = 1.0;
            double totalBCE = 0.0;
            double bce = 0.0;
            // Now training with SP+TM. SP is pretrained on the given input pattern set.
            foreach (var input1 in inputs)
            {
                for (int i = 0; i < maxCycles; i++)
                {
                    matches = 0;

                    cycle++;
                    Debug.WriteLine($"-------------- Cycle {cycle} ---------------");

                    foreach (var input in inputs)
                    {
                        Debug.WriteLine($"-------------- {input} ---------------");

                        // lyrOut is null when the TM is added to the layer inside of HPC callback by entering of the stable state.

                        var lyrOut = layer1.Compute(input, true) as ComputeCycle;
                        // if (isInStableState && lyrOut != null)

                        var activeColumns = layer1.GetResult("sp") as int[];

                        previousInputs.Add(input.ToString());
                        if (previousInputs.Count > (maxPrevInputs + 1))
                            previousInputs.RemoveAt(0);

                        // In the pretrained SP with HPC, the TM will quickly learn cells for patterns
                        // In that case the starting sequence 4-5-6 might have the sam SDR as 1-2-3-4-5-6,
                        // Which will result in returning of 4-5-6 instead of 1-2-3-4-5-6.
                        // HtmClassifier allways return the first matching sequence. Because 4-5-6 will be as first
                        // memorized, it will match as the first one.
                        if (previousInputs.Count < maxPrevInputs)

                            continue;


                        string key = GetKey(previousInputs, input);

                        List<Cell> actCells;

                        if (lyrOut.ActiveCells.Count == lyrOut.WinnerCells.Count)
                        {
                            actCells = lyrOut.ActiveCells;
                        }
                        else
                        {
                            actCells = lyrOut.WinnerCells;
                        }

                        cls.Learn(key, actCells.ToArray());

                        Debug.WriteLine($"Col  SDR: {Helpers.StringifyVector(lyrOut.ActivColumnIndicies)}");
                        Debug.WriteLine($"Cell SDR: {Helpers.StringifyVector(actCells.Select(c => c.Index).ToArray())}");
                        String SdrValues = Helpers.StringifyVector(actCells.Select(c => c.Index).ToArray());

                        // Split the input string by commas and remove any leading or trailing whitespace from each substring
                        string[] substrings_SDR = SdrValues.Split(',').Select(s => s.Trim()).ToArray();

                        // Convert each substring to an integer and add it to a list
                        List<int> intList_SDR = new List<int>();
                        foreach (var substring in substrings_SDR)
                        {
                            if (int.TryParse(substring, out int intValue))
                            {
                                intList_SDR.Add(intValue);
                            }

                        }

                        // If the list of predicted values from the previous step contains the currently presenting value,
                        // we have a match.

                        if (lastPredictedValues.Contains(key))
                        {
                            matches++;
                            Debug.WriteLine($"Match. Actual value: {key} - Predicted value: {lastPredictedValues.FirstOrDefault(key)}.");
                        }
                        else
                            Debug.WriteLine($"Missmatch! Actual value: {key} - Predicted values: {String.Join(',', lastPredictedValues)}");

                        if (lyrOut.PredictiveCells.Count > 0)
                        {
                            //var predictedInputValue = cls.GetPredictedInputValue(lyrOut.PredictiveCells.ToArray());
                            var predictedInputValues = cls.GetPredictedInputValues(lyrOut.PredictiveCells.ToArray(), 7);
                            Debug.WriteLine($"predictedInputValues : {predictedInputValues.Count}");

                            foreach (var item in predictedInputValues)
                            {
                                Debug.WriteLine($"Current Input: {input}  t| Predicted Input: {item.PredictedInput} - {item.Similarity}  t| BestMatching:{item.BestMatchString} ");


                                //Extracting predictive_cells values 
                                String predictive_cells = item.BestMatchString;

                                string[] substrings_predictivecells = predictive_cells.Split(',').Select(s => s.Trim()).ToArray();

                                // Convert each substring to an integer and add it to a list
                                List<int> intList_predictivecells = new List<int>();
                                foreach (var var in substrings_predictivecells)
                                {
                                    if (int.TryParse(var, out int intValue_predictiveCells))
                                    {
                                        intList_predictivecells.Add(intValue_predictiveCells);
                                    }

                                }

                                //lists of predictedSets
                                List<List<int>> predictedSets = new List<List<int>> { intList_predictivecells };
                                int threshold = 15;

                                for (int j = 0; j < predictedSets.Count; j++)
                                {
                                    //Method to calculate CalculateCorrectness id called
                                    bce = CalculateCorrectness(intList_SDR, predictedSets[j], threshold);

                                    if (bce > 0 && bce < leastBCE)
                                    {
                                        //least bce is stored among all the losses
                                        leastBCE = bce;
                                    }

                                    Debug.WriteLine($"Binary Cross-Entropy: {bce}");

                                }
                            }

                            //total least binary cross entropy loss value is added
                            totalBCE += leastBCE;
                            Debug.WriteLine($"The leastBCE is {leastBCE}");

                            lastPredictedValues = predictedInputValues.Select(v => v.PredictedInput).ToList();

                        }
                        else
                        {
                            Debug.WriteLine($"NO CELLS PREDICTED for next cycle.");
                            lastPredictedValues = new List<string>();
                        }
                    }

                    /// <summary>
                    /// Calculate Correctness for Binary Cross Entropy
                    /// Active SDRs are extracted for each cycle are compared with predicted values
                    /// threshold is set based on minimun difference between Active SDRs and predictedValues
                    /// For each comparison, the method assigns a binary value (1 or 0) indicating whether the prediction falls within the acceptable range defined by the threshold.
                    /// If the absolute difference is within the threshold, it assigns a value of 1, indicating correctness. Otherwise, it assigns a value of 0, indicating incorrectness.
                    /// </summary>
                    /// <param name="actualOutputs">Active SDR values</param>
                    /// <param name="predictedValues">predictedValues for each cycle</param>
                    /// <param name="threshold">threshold value</param>
                    /// <returns>Binary cross entropy loss of type double</returns>
                    static double CalculateCorrectness(List<int> actualOutputs, List<int> predictedValues, int threshold)
                    {

                        // Determine correctness based on threshold
                        var correctness = actualOutputs.Zip(predictedValues, (actual, pred) => Math.Abs(actual - pred) <= threshold ? 1 : 0).ToList();

                        // Compute binary cross-entropy
                        double bce = CalculateBinaryCrossEntropy(correctness);
                        return bce;
                    }

                    /// <summary>
                    /// Calculate Binary Cross Entropy loss
                    /// Sum up the binary cross-entropy values for each prediction (0 for correct predictions, 1 for incorrect predictions)
                    /// Then divide total bce by the total number of predictions.
                    /// </summary>
                    /// <param name="correctness"> difference between active SDR values and predcted values </param>
                    /// <returns>val of type double </returns>
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

                    //Accuracy from binary cross entropy
                    double accuracyFromBinaryCrossEntropy = (1 - aveBCE) * 100.0;

                    Debug.WriteLine($"accuracyFromBinaryCrossEntropy  {accuracyFromBinaryCrossEntropy}%");

                    // The first element (a single element) in the sequence cannot be predicted
                    double maxPossibleAccuraccy = (double)((double)inputValues.Count - 1) / (double)inputValues.Count * 100.0;

                    //double accuracy = (double)matches / (double)inputValues.Count * 100.0;


                    Debug.WriteLine($"Cycle: {cycle} tMatches={matches} of {inputValues.Count()} t {maxPossibleAccuraccy}%");

                    if (accuracyFromBinaryCrossEntropy >= maxPossibleAccuraccy)
                    {
                        maxMatchCnt++;
                        Debug.WriteLine($"100% accuracy reached {maxMatchCnt} times.");


                        // Experiment is completed if we are 30 cycles long at the 100% accuracy.
                        if (maxMatchCnt >= 30)
                        {
                            sw.Stop();
                            Debug.WriteLine($"Sequence learned. The algorithm is in the stable state after 30 repeats with with accuracy {accuracyFromBinaryCrossEntropy} of maximum possible {maxMatchCnt}. Elapsed sequence {inputValues} learning time: {sw.Elapsed}.");
                            isLearningCompleted = true;
                            break;
                        }
                    }
                    else if (maxMatchCnt > 0)
                    {
                        Debug.WriteLine($"At 100% accuracy after {maxMatchCnt} repeats we get a drop of accuracy with accuracy {accuracyFromBinaryCrossEntropy}. This indicates instable state. Learning will be continued.");
                        maxMatchCnt = 0;
                    }

                    // This resets the learned state, so the first element starts allways from the beginning.
                    tm.Reset(mem);
                }

                if (isLearningCompleted == false)
                    throw new Exception($"The system didn't learn with expected acurracy!");

            }

            Debug.WriteLine("------------ END ------------");

            return new Predictor(layer1, mem, cls);
        }

        /// <summary>
        /// Constracts the unique key of the element of an sequece. This key is used as input for HtmClassifier.
        /// It makes sure that alle elements that belong to the same sequence are prefixed with the sequence.
        /// The prediction code can then extract the sequence prefix to the predicted element.
        /// </summary>
        /// <param name="prevInputs"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GetKey(List<string> prevInputs, double input)
        {
            string key = String.Empty;

            for (int i = 0; i < prevInputs.Count; i++)
            {
                if (i > 0)
                    key += "-";

                key += (prevInputs[i]);
            }

            return key;
        }
    }
}






