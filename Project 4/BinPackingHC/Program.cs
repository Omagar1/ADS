// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public struct BinPackingSolution
{
    public int[] solution;
    public double fitness;
    public int iterations;
    public Dictionary<int, double> bins; 
    
}

public class HillClimbing
{
    
    public Dictionary<int,double[]> getbins(int[] soulution, double[] dataSet)
    {
        Dictionary<int, double[]> bins = new Dictionary<int, double[]>();
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

    public BinPackingSolution start(double[] dataSet, double binCapacity)
    {
        int numBins = (int)Math.Round(dataSet.Sum()/binCapacity, MidpointRounding.AwayFromZero);
        Random random = new Random();
        int noChangeCount = 0;
        int iterations = 0;
        int[] currentSolution = new int[dataSet.Length];

        for (int i = 0; i < currentSolution.Length; i++)
        {
            currentSolution[i] = random.Next(1,numBins);
        }
        double currentFitness = calcFitness(currentSolution, dataSet, binCapacity);

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
            }
            else if (currentFitness <= 0 && numBins < dataSet.Length  && noChangeCount >= tolerance - 1)
            {
                //in case of where a solution without overflow is found
                //we know the worst case is one bin per item so check teh program dosent go past this hence seccond condition
                //we want to exhuast every posibilteies at that number of bins before we add a bin hennce the third condition
                numBins++;
                noChangeCount = 0;
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
        result.bins = getbins(currentSolution, dataSet); 
        return result;


    }
    private int numChanges;
    private int tolerance; 
}
