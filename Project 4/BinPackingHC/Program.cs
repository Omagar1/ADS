// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
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

    public Dictionary<int,double> getbins(int[] soulution, double[] dataSet)
    {
        Dictionary<int, double> bins = new Dictionary<int, double>();
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

        for (int i = 0; i < currentSolution.Length; i++)
        {
            currentSolution[i] = random.Next(1,numBins);
        }
        double currentFitness = calcFitness(currentSolution, dataSet, binCapacity);


        // writing to a file
        if (optputFileName != null)
        {
            string outStr = "\n initial solution: " + string.Join(", ", currentSolution) + " with fitness:" + currentFitness.ToString() + "\n";
            File.AppendAllText(optputFileName, outStr);  
        }
        //Console.WriteLine("initial solution: {0} with fitness{1}", currentSolution.ToString(), currentFitness); test


        while (noChangeCount < tolerance) 
        {
            int[] newSolution = smallChange(currentSolution, numBins);
            double newFitness = calcFitness(newSolution, dataSet, binCapacity);
            if (newFitness > currentFitness)
            {
                // better solution found
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
                //we know the worst case is one bin per item so check teh program dosent go past this hence seccond condition
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
    public void writeSolToFile(string filePath, BinPackingSolution solution, bool[] outVar)
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
    public static void getBins_test(int[][] test_soulutions, double[] test_dataSet, double test_binSize, string filePath)
    {
        BinPackingHC test1 = new BinPackingHC(test_dataSet, test_binSize);
        File.AppendAllText(filePath, "--- getbins() test ---\n");
        foreach (int[] test_sol in test_soulutions)
        {
            try
            {
                File.AppendAllText(filePath, string.Join(',', test1.getbins(test_sol, test_dataSet))+"\n");
            }
            catch
            {
                File.AppendAllText(filePath, "error\n");
            }
        }
    }
}

namespace binPacking
{
    class Program
    {
        static void Main(string[] args)
        {
            //double[] DATASET = [55.44, 15.12, 0.77, 3.1, 95.24, 89.59, 7.12, 58.77, 78.98, 55.76, 30.34, 17.5, 44.05, 74.76, 28.4, 96.55, 87.78, 65.56, 72.91, 39.74, 39.52, 38.72, 8.12, 32.31, 12.28, 52.15, 89.93, 15.9, 3.37, 2.18];
            string dataSetPath = "C:\\Users\\josia\\Documents\\UNI Stuff\\CS\\24 25\\Algorithms and Data Structures\\CourseWork\\ADS\\Project 4\\dataset.csv";
            double binSize = 130;
            BinPackingHC sol1 = new BinPackingHC(dataSetPath, binSize);
            // for unit testing
            string unitTests_filePath = "C:\\Users\\josia\\Documents\\UNI Stuff\\CS\\24 25\\Algorithms and Data Structures\\CourseWork\\ADS\\Project 4\\unitTests.txt";
            int[][] testSolutions = 
            {
                new int[] { 1, 2, 2, 1, 3 },
                new int[] {2,1,3,2,3},
                new int[] {1,2,3,4, 5 },
                new int[] {1,1,1,1, 1 },
                new int[] { 1 },
                new int[] {1, 2 },
                new int[] { -1, -2, -3, -4, -5,},
                new int[] {12,3,42,56,2,32, },
                new int[] {1,2,3,4,5,6,},
            };

            double[] test_dataSet = { 4, 2, 3, 1, 2, };
            double test_BinSize = 5;

            BinPackingHC_Tests.getBins_test(testSolutions, test_dataSet,test_BinSize, )
            
            
            
            
            // for System testing
            string outFilePath = "C:\\Users\\josia\\Documents\\UNI Stuff\\CS\\24 25\\Algorithms and Data Structures\\CourseWork\\ADS\\Project 4\\Results.txt";

            bool[] outVar = { true, true, true, true, true };
            for(int i = 0; i < 100; i++)
            {
                BinPackingSolution res = sol1.start();
                sol1.writeSolToFile(outFilePath,res,outVar);
            }

            //sol1.start();
            //Console.WriteLine("Solution Found with Fitness: {0} and {1} number of bins, after {2} iterations",solution.fitness, solution.bins.Count(), solution.iterations);
        }
    }
}
