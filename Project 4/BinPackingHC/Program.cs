// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ObjectiveC;
using static System.Runtime.InteropServices.JavaScript.JSType;

public struct BinPackingSolution
{
    public int[] solution;
    public double fitness;
    public int iterations;
    public SortedDictionary<int, double> bins; 
    
}

public class BinPackingHC
{
    public BinPackingHC(double[] dataSet, double binCapacity) 
    {
        this.dataSet = dataSet;
        this.binCapacity = binCapacity;
        this.numChanges = (int)Math.Round((dataSet.Length * 0.2), MidpointRounding.ToPositiveInfinity);
        this.tolerance = 100000;

    }
    public BinPackingHC(string dataSetFilePath, double binCapacity)
    {
        //.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        string[] temp = File.ReadAllLines(dataSetFilePath);
        this.dataSet = Array.ConvertAll<string, double>(temp, str => Convert.ToDouble(str));
        this.binCapacity = binCapacity;
        this.numChanges = (int)Math.Round((dataSet.Length * 0.2), MidpointRounding.ToPositiveInfinity);
        this.tolerance = 100000;
        

    }

    public Dictionary<int,double> getbins(int[] soulution, double[] dataSet) // totals size of all items assigned to each bin by a solution
    {
        Dictionary<int, double> bins = new Dictionary<int, double>(); // in format binNum:total size of intems in bin
        for (int i = 0; i < soulution.Length; i++)
        {
            if (bins.ContainsKey(soulution[i]))
            {    
                bins[soulution[i]] += dataSet[i];
            }
            else
            {
                bins.Add(soulution[i], dataSet[i]); 
            }
        }
        return bins;
    }
    public double calcFitness(int[] soulution, double[] dataSet, double binCapacity)
    {
        Dictionary<int, double> bins = getbins(soulution, dataSet);
        double overflowCount = 0; // has to be double because of return type

        foreach(KeyValuePair<int, double> bin in bins) {
            if(bin.Value > binCapacity)
            {
                overflowCount--;
            }
            else
            {
                bins[bin.Key] = bin.Value/binCapacity;
            }
        }
        return (overflowCount == 0 )? bins.Values.Average() : overflowCount;
    }

    public int[] smallChange(int[] curentSoulution, int numBins)
    {
        int[] newSolution = curentSoulution.Clone() as int[];
        Random random = new Random();
        for(int i = 0; i < numChanges; i++)
        {
            newSolution[random.Next(0, newSolution.Length - 1)] = random.Next(1, numBins); 
        }
        return newSolution;

    }

    public BinPackingSolution start(string? optputFileName = null) // only writes to a file if file name provided 
    {
        // initialising vars that change during running
        int numBins = (int)Math.Round(dataSet.Sum()/binCapacity, MidpointRounding.ToPositiveInfinity);
        Random random = new Random();
        int noChangeCount = 0;
        int iterations = 0;
        int[] currentSolution = new int[dataSet.Length];

        // generating initial solution
        for (int i = 0; i < currentSolution.Length; i++)
        {
            currentSolution[i] = random.Next(1,numBins);
        }
        double currentFitness = calcFitness(currentSolution, dataSet, binCapacity);


        // writing to file
        if (optputFileName != null)
        {
            string outStr = "\n initial solution: " + string.Join(", ", currentSolution) + " with fitness:" + currentFitness.ToString() + "\n";
            File.AppendAllText(optputFileName, outStr);  
        }
        //Console.WriteLine("initial solution: {0} with fitness{1}", currentSolution.ToString(), currentFitness); test


        while (noChangeCount < tolerance) 
        {   
            // gettining new solution
            int[] newSolution = smallChange(currentSolution, numBins);
            double newFitness = calcFitness(newSolution, dataSet, binCapacity);
            if (newFitness > currentFitness)
            {
                // case: better solution found
                currentSolution = newSolution;
                currentFitness = newFitness;
                noChangeCount = 0;
                // writing to a file
                if (optputFileName != null)
                {
                    string outStr = "better solution found: " + string.Join(", ", currentSolution)+ " with fitness:" + currentFitness.ToString() + "\n";
                    File.AppendAllText(optputFileName, outStr);
                }
            }
            else if (currentFitness <= 0 && numBins < dataSet.Length  && noChangeCount >= tolerance - 1)
            {
                //in case of where a solution without overflow is found
                //we know the worst case is one bin per item so check the program dosent go past this hence seccond condition
                //we want to exhuast every posibilteies at that number of bins before we add a bin hennce the third condition
                numBins++;
                noChangeCount = 0;
                //Console.WriteLine("num bins incresed to {0} ", numBins); //test
            }
            else
            {
                noChangeCount++;
            }
            iterations++;
        }
        BinPackingSolution result = new BinPackingSolution();
        result.solution = currentSolution;
        result.fitness = currentFitness;
        result.iterations = iterations;
        result.bins = new SortedDictionary<int,double>(getbins(currentSolution, dataSet));
        if (optputFileName != null)
        {
            string outStr1 = "final solution : " + string.Join(", ", currentSolution) + " with fitness:" + currentFitness.ToString() + "\n";
            string outStr2 = "Found After: " + iterations.ToString() + " iterations with " + result.bins.Count().ToString() + " bins used\n";
            string outStr3 = "bins: "+ string.Join(", ", result.bins) + "\n";
            File.AppendAllText(optputFileName, outStr1);
            File.AppendAllText(optputFileName, outStr2);
            File.AppendAllText(optputFileName, outStr3);

        }
        return result;


    }
    public void writeSolToFile(string filePath, BinPackingSolution solution, bool[] outVar) // used for system tests 
    {
        string[] outStringArr = new string[5];
        outStringArr[0] = outVar[0]? string.Join(", ", solution.solution) : "";
        outStringArr[1] = outVar[1] ? solution.fitness.ToString() : "";
        outStringArr[2] = outVar[2] ? solution.iterations.ToString() : "";
        outStringArr[3] = outVar[3] ? solution.bins.Count().ToString() : "";
        outStringArr[4] = outVar[4] ? string.Join(", ", solution.bins) : "";
        string outString = string.Join("; ", outStringArr) + "\n";
        File.AppendAllText(filePath,outString );

    }


    private double[] dataSet;
    private double binCapacity;
    private int numChanges;
    private int tolerance; 
}

public static class BinPackingHC_Tests
{
    public static void getBins_test(double[] dataSet, double binCapacity, string filePath)
    {
        // read file - will be in format solution;expexted result
        string[] temp = File.ReadAllLines(filePath + "\\getBins_testData.txt");

        BinPackingHC test = new BinPackingHC(dataSet, binCapacity);
        StringWriter outStr = new StringWriter(); // will be in format solution;expexted result;actual result;pass
        // splitting and formating into solutions and sexpected results
        foreach (string line in temp)
        {
            string[] lineArr = line.Split(';');
            // formating test solution;
            int[] testSolution = Array.ConvertAll<string, int>(lineArr[0].Split(','), str => Convert.ToInt32(str));
            outStr.Write(string.Join(',', testSolution) + ";");
            // formating expectedResult
           
            Dictionary<int, double> expectedResult = new Dictionary<int, double>();
            string[] expectedResultKVPArr = lineArr[1].Split(',');
            foreach(string kVPStr  in expectedResultKVPArr)
            {    
                string[] kVPArr = kVPStr.Split(':');
                expectedResult.Add(Convert.ToInt32(kVPArr[0]), Convert.ToInt32(kVPArr[1])); 
            }
            outStr.Write(string.Join(",", expectedResult.Select(kvp => kvp.Key + ":" + kvp.Value)) + ";");

            // tests - in a try catch statement as if any input is in wrong format it will error so that will class as the test passing as thats what was expected
            try
            {
                Dictionary<int, double> result = test.getbins(testSolution, dataSet); 
                outStr.Write(string.Join(",", result.Select(kvp => kvp.Key + ":" + kvp.Value)) + ";");
                bool pass = expectedResult.Count() == result.Count() && !expectedResult.Except(result).Any();
                outStr.Write(pass.ToString() + "; \n");
            }
            catch
            {
                outStr.Write("Error;");
                outStr.Write(true + "; \n");
            }


        }
        // writing to file
        File.WriteAllText(filePath + "\\getBins_testResults.txt", outStr.ToString());
        
    }


    public static void smallChange_test(double[] dataSet, double binCapacity, string filePath)
    {
        // read file - will be in format: solution - no expected solution as small change is random, pass result calculated diffrently
        string[] temp = File.ReadAllLines(filePath + "\\smallChange_testData.txt");
        // setiing up Varibles for test
        BinPackingHC test = new BinPackingHC(dataSet, binCapacity);
        StringWriter outStr = new StringWriter(); // will be in format solution;actual result;pass
        int numBins = (int)Math.Round(dataSet.Sum() / binCapacity, MidpointRounding.ToPositiveInfinity);
        int numChanges = (int)Math.Round((dataSet.Length * 0.2), MidpointRounding.ToPositiveInfinity);

        // splitting and formating into solutions and sexpected results
        foreach (string line in temp)
        {
            // formating test solution;
            int[] testSolution = Array.ConvertAll<string, int>(line.Split(','), str => Convert.ToInt32(str));
            outStr.Write(string.Join(',', testSolution) + ";");

            // tests - in a try catch statement as if any input is in wrong format it will error so that will class as the test passing as thats what was expected
            try
            {
                int[] result = test.smallChange(testSolution, numBins);
                outStr.Write(string.Join(',', result) + ";");
                // calculating pass value
                // pass will be defined as true if the sum off all elements of result is within a the range of the orininal soulutions sum +- numberchanegs * number of bins
                bool pass = (result.Sum() >= (testSolution.Sum() - numChanges * numBins) && result.Sum() <= (testSolution.Sum() + numChanges * numBins));
                outStr.Write(pass.ToString() + "; \n");
            }
            catch
            {
                outStr.Write("Error;");
                outStr.Write(true + "; \n");
            }


        }
        // writing to file
        File.WriteAllText(filePath + "\\smallChange_testResults.txt", outStr.ToString());

    }
    
    public static void calcFitness_test(double[] dataSet, double binCapacity, string filePath) 
    {
        // read file - will be in format solution;expexted result
        string[] temp = File.ReadAllLines(filePath + "\\calcFitness_testData.txt");

        BinPackingHC test = new BinPackingHC(dataSet, binCapacity);
        StringWriter outStr = new StringWriter(); // will be in format solution;expexted result;actual result;pass
        const int decimalPlacesToCompare = 3; 
        // splitting and formating into solutions and sexpected results
        foreach (string line in temp)
        {
            string[] lineArr = line.Split(';');
            // formating test solution;
            int[] testSolution = Array.ConvertAll<string, int>(lineArr[0].Split(','), str => Convert.ToInt32(str));
            outStr.Write(string.Join(',',testSolution) + ";");
            // formating expectedResult
            double expectedResult = Math.Round(Convert.ToDouble(lineArr[1]), decimalPlacesToCompare);
            outStr.Write(expectedResult.ToString() + ";");
            // tests - in a try catch statement as if any input is in wrong format it will error so that will class as the test passing as thats what was expected
            try
            {

                double result = Math.Round(test.calcFitness(testSolution, dataSet, binCapacity), decimalPlacesToCompare);
                outStr.Write(result.ToString() + ";");
                bool pass = result == expectedResult;
                outStr.Write(pass.ToString() + "; \n");
            }
            catch
            {
                outStr.Write("Error;");
                outStr.Write(true + "; \n");
            }
        }
        // writing to file
        File.WriteAllText(filePath + "\\calcFitness_testResults.txt", outStr.ToString());
    }
}

namespace binPacking
{
    class Program
    {
        static void Main(string[] args)
        {
            //double[] DATASET = [55.44, 15.12, 0.77, 3.1, 95.24, 89.59, 7.12, 58.77, 78.98, 55.76, 30.34, 17.5, 44.05, 74.76, 28.4, 96.55, 87.78, 65.56, 72.91, 39.74, 39.52, 38.72, 8.12, 32.31, 12.28, 52.15, 89.93, 15.9, 3.37, 2.18];
            //string dataSetPath = "C:\\Users\\FiercePC\\Documents\\GitHub\\ADS\\Project 4\\dataset.csv";
            //double binSize = 130;
            //BinPackingHC sol1 = new BinPackingHC(dataSetPath, binSize);


            // ---- for unit testing --- 
            string unitTests_filePath = "C:\\Users\\FiercePC\\Documents\\GitHub\\ADS\\Project 4\\UnitTests";
            double[] test_dataSet = { 4, 2, 3, 1, 2};
            double test_BinSize = 5;

            /*BinPackingHC_Tests.getBins_test(testSolutions, test_dataSet, test_BinSize, unitTests_filePath); */
            //BinPackingHC_Tests.getBins_test(test_dataSet, test_BinSize, unitTests_filePath);
            //BinPackingHC_Tests.smallChange_test(test_dataSet, test_BinSize, unitTests_filePath);
            BinPackingHC_Tests.calcFitness_test(test_dataSet, test_BinSize, unitTests_filePath);





            // for System testing
            /*string outFilePath = "C:\\Users\\FiercePC\\Documents\\GitHub\\ADS\\Project 4\\Results.txt";

            bool[] outVar = { true, true, true, true, true };
            for(int i = 0; i < 100; i++)
            {
                BinPackingSolution res = sol1.start();
                sol1.writeSolToFile(outFilePath,res,outVar);
            }*/

            //sol1.start();
            //Console.WriteLine("Solution Found with Fitness: {0} and {1} number of bins, after {2} iterations",solution.fitness, solution.bins.Count(), solution.iterations);
        }
    }
}
