using NeoCortexApi;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NeoCortexApiSample
{
    /// <summary>
    /// Implements an experiment that demonstrates how to learn sequences.
    /// </summary>
    public class MultiSequenceLearning
    {
        /// <summary>
        /// Runs the learning of sequences.
        /// </summary>
        /// <param name="sequences">Dictionary of sequences. KEY is the sequence name, the VALUE is the list of elements of the sequence.</param>
        public Predictor Run(List<double> inputValues)
        {
            Console.WriteLine($"Hello NeocortexApi! Experiment {nameof(MultiSequenceLearning)}");

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

        /// <summary>
        ///
        /// </summary>
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

            // For more information see the following paper: https://www.scitepress.org/Papers/2021/103142/103142.pdf
            HomeostaticPlasticityController hpc = new HomeostaticPlasticityController(mem, numUniqueInputs * 150, (isStable, numPatterns, actColAvg, seenInputs) =>
            {
                if (isStable)
                    // Event should be fired when entering the stable state.
                    Debug.WriteLine($"STABLE: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");
                else
                    // Ideal SP should never enter unstable state after the stable state.
                    Debug.WriteLine($"INSTABLE: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");

                // We are not learning in the unstable state.
                isInStableState = isStable;

                // Clear active and predictive cells.
                // tm.Reset(mem);
            }, numOfCyclesToWaitOnChange: 50);

            SpatialPoolerMT sp = new SpatialPoolerMT(hpc);
            sp.Init(mem);
            tm.Init(mem);

            layer1.HtmModules.Add("encoder", encoder);
            layer1.HtmModules.Add("sp", sp);

            double[] inputs = inputValues.ToArray();
            int cycle = 0;
            int matches = 0;
            var lastPredictedValues = new List<string>(new string[] { "0" });
            int maxCycles = 3500;

            // Training SP to stabilize. Newborn stage.
            for (int i = 0; i < maxCycles && !isInStableState; i++)
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

            cls.ClearState();
            layer1.HtmModules.Add("tm", tm);

            int maxPrevInputs = inputValues.Count - 1;
            List<string> previousInputs = new List<string> { "-1.0" };
            bool isLearningCompleted = false;

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

                        var lyrOut = layer1.Compute(input, true) as ComputeCycle;

                        var activeColumns = layer1.GetResult("sp") as int[];

                        previousInputs.Add(input.ToString());
                        if (previousInputs.Count > (maxPrevInputs + 1))
                            previousInputs.RemoveAt(0);

                        if (previousInputs.Count < maxPrevInputs)
                            continue;

                        string key = GetKey(previousInputs, input);

                        List<Cell> actCells = lyrOut.ActiveCells.Count == lyrOut.WinnerCells.Count
                            ? lyrOut.ActiveCells
                            : lyrOut.WinnerCells;

                        cls.Learn(key, actCells.ToArray());

                        Debug.WriteLine($"Col  SDR: {Helpers.StringifyVector(lyrOut.ActivColumnIndicies)}");
                        Debug.WriteLine($"Cell SDR: {Helpers.StringifyVector(actCells.Select(c => c.Index).ToArray())}");

                        if (lastPredictedValues.Contains(key))
                        {
                            matches++;
                            Debug.WriteLine($"Match. Actual value: {key} - Predicted value: {lastPredictedValues.FirstOrDefault(key)}.");
                        }
                        else
                            Debug.WriteLine($"Mismatch! Actual value: {key} - Predicted values: {String.Join(',', lastPredictedValues)}");

                        if (lyrOut.PredictiveCells.Count > 0)
                        {
                            var predictedInputValues = cls.GetPredictedInputValues(lyrOut.PredictiveCells.ToArray(), 3);
                            Console.WriteLine($"predictedInputValues : {predictedInputValues}");
                            foreach (var item in predictedInputValues)
                            {
                                Debug.WriteLine($"Current Input: {input} \t| Predicted Input: {item.PredictedInput} - {item.Similarity}");
                            }

                            lastPredictedValues = predictedInputValues.Select(v => v.PredictedInput).ToList();
                            Console.WriteLine($"The lastpredicted values : {lastPredictedValues}");
                        }
                        else
                        {
                            Debug.WriteLine($"NO CELLS PREDICTED for next cycle.");
                            lastPredictedValues = new List<string>();
                        }
                    }

                    double maxPossibleAccuracy = ((double)inputValues.Count - 1) / inputValues.Count * 100.0;
                    double accuracy = matches / (double)inputValues.Count * 100.0;
                    Debug.WriteLine($"Cycle: {cycle}\tMatches={matches} of {inputValues.Count()}\t {accuracy}%");

                    if (accuracy >= maxPossibleAccuracy)
                    {
                        maxMatchCnt++;
                        Debug.WriteLine($"100% accuracy reached {maxMatchCnt} times.");

                        if (maxMatchCnt >= 30)
                        {
                            sw.Stop();
                            Debug.WriteLine($"Sequence learned. The algorithm is in the stable state after 30 repeats with with accuracy {accuracy} of maximum possible {maxMatchCnt}. Elapsed sequence {inputValues} learning time: {sw.Elapsed}.");
                            isLearningCompleted = true;
                            break;
                        }
                    }
                    else if (maxMatchCnt > 0)
                    {
                        Debug.WriteLine($"At 100% accuracy after {maxMatchCnt} repeats we get a drop of accuracy with accuracy {accuracy}. This indicates unstable state. Learning will be continued.");
                        maxMatchCnt = 0;
                    }

                    tm.Reset(mem);
                }

                if (!isLearningCompleted)
                    throw new Exception($"The system didn't learn with expected accuracy!");
            }

            Debug.WriteLine("------------ END ------------");

            return new Predictor(layer1, mem, cls);
        }

        /// <summary>
        /// Constructs the unique key of the element of a sequence. This key is used as input for HtmClassifier.
        /// It makes sure that all elements that belong to the same sequence are prefixed with the sequence.
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

                key += prevInputs[i];
            }

            return key;
        }

        private double CalculateBinaryCrossEntropyLoss(HtmClassifier<string, ComputeCycle> classifier, string target, double input)
        {
            double loss = 0.0;

            // Retrieve the predicted probabilities from the classifier
            double[] probabilities = classifier.GetPredictedProbabilities(target);

            // Assuming target is a binary value (0 or 1)
            int targetValue = Convert.ToInt32(target);

            // Calculate binary cross-entropy loss
            loss = -((targetValue * Math.Log(probabilities[1] + double.Epsilon)) + ((1 - targetValue) * Math.Log(probabilities[0] + double.Epsilon)));

            return loss;
        }
    }
}
