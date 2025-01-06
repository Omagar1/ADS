using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

class unweightedGraph
{
    public unweightedGraph(Dictionary<string, List<string>> nodes)
    {
        this.nodes = nodes;
    }
    public Dictionary<string,string> findRoutes(string startNode)
    {
        Queue<string> toVisit = new Queue<string>();
        Dictionary<string,string> visited = new Dictionary<string,string>();// in format node: node from
        toVisit.Enqueue(startNode); 
        visited.Add(startNode, string.Empty);
        string currentNode; 

        while (toVisit.Count() > 0) 
        {
            currentNode = toVisit.Dequeue();
            foreach (string adjacentNode in this.nodes[currentNode])
            {
                if (!visited.Keys.Contains(adjacentNode))
                {
                    toVisit.Enqueue(adjacentNode);
                    visited.Add(adjacentNode, currentNode);
                }
            }

        }
        return visited;
    }
    public List<string> findRoute(string start, Dictionary<string, string> routes)
    {
        List<string> route = new List<string>();
        string nextNode = start; 
        route.Add(start);
        while (routes[nextNode] != "")
        {
            route.Add(routes[nextNode]);
            nextNode = routes[nextNode];
        }
        return route;
    }
    public double calcScore(string scoreNode)
    {

        List<double> distanceToNode = new List<double>();
        Dictionary<string, string> routes = findRoutes(scoreNode);

        foreach (KeyValuePair<string, List<string>> node in nodes)
        {
            if (node.Key != scoreNode)
            {
                distanceToNode.Add(findRoute(node.Key, routes).Count()-1); // -1 as the route will contain the score/starting node as well

            }
        }

        return (nodes.Count() - 1) / distanceToNode.Sum(); // nodes.Count() - 1 = n - 1 part of equation :  distanceToNode.Sum() = d(u,v) part of equation 
    }
    public KeyValuePair<string, double> findBestScore()
    {
        double bestScore = 0;
        string? bestNode = null;

        foreach (KeyValuePair<string, List<string>> node in nodes)
        {
            double newScore = this.calcScore(node.Key);
            if (newScore > bestScore)
            {
                bestScore = newScore;
                bestNode = node.Key;
            }
        }
        return new KeyValuePair<string, double>(bestNode, bestScore);
    }
    public KeyValuePair<string, double> findWorstScore()
    {
        double worstScore = 1;
        string? worstNode = null;

        foreach (KeyValuePair<string, List<string>> node in nodes)
        {
            double newScore = this.calcScore(node.Key);
            if (newScore < worstScore)
            {
                worstScore = newScore;
                worstNode = node.Key;
            }
        }
        return new KeyValuePair<string, double>(worstNode, worstScore);
    }
    public Dictionary<string, List<string>> getNodes()
    {
        return nodes;   
    }

    private Dictionary<string, List<string>> nodes;
}

class weightedGraph
{
    public weightedGraph(Dictionary<string, Dictionary<string, int>> nodes)
    {
        this.nodes = nodes;
    }
    public Dictionary<string, int> findRoutesLengths(string startNode) // using Dijkstra's algorithm
    {
        // set up
        List<string> visited = new List<string>();  
        Queue<string> toVisit = new Queue<string>();
        Dictionary<string, int> shortestRouteTo = new Dictionary<string, int>();
        foreach (string node in nodes.Keys)
        {
            shortestRouteTo[node] = (node != startNode)? Int32.MaxValue : 0;
        }
        // traversal
        toVisit.Enqueue(startNode);
        while(toVisit.Count() != 0)
        {

            string currentNode = toVisit.Dequeue();
            visited.Add(currentNode);
            foreach (KeyValuePair<string, int> node in nodes[currentNode])
            {
                
                // if new route better than current route
                shortestRouteTo[node.Key] = (node.Value + shortestRouteTo[currentNode] < shortestRouteTo[node.Key]) ? node.Value + shortestRouteTo[currentNode] : shortestRouteTo[node.Key] ;
                if (!visited.Contains(node.Key))
                {
                    toVisit.Enqueue(node.Key);
                }
            } 
        }
        return shortestRouteTo;
        
        
    }
    public double calcScore(string scoreNode)
    {
        Dictionary<string, int> distanceToNodes = findRoutesLengths(scoreNode);
        return (nodes.Count() - 1) / (double)distanceToNodes.Values.Sum(); // nodes.Count() - 1 = n - 1 part of equation :  distanceToNodes.Sum() = d(u,v) part of equation
    }

    public KeyValuePair<string, double> findBestScore()
    {
        double bestScore = 0;
        string? bestNode = null;

        foreach (KeyValuePair<string, Dictionary<string, int>> node in nodes)
        {
            double newScore = this.calcScore(node.Key);
            if (newScore > bestScore)
            {
                bestScore = newScore;
                bestNode = node.Key;
            }
        }
        return new KeyValuePair<string, double>(bestNode, bestScore);
    }
    
    
    public KeyValuePair<string, double> findWorstScore()
    {
        double worstScore = 1;
        string? worstNode = null;

        foreach (KeyValuePair<string, Dictionary<string, int>> node in nodes)
        {
            double newScore = this.calcScore(node.Key);
            if (newScore < worstScore)
            {
                worstScore = newScore;
                worstNode = node.Key;
            }
        }
        return new KeyValuePair<string, double>(worstNode, worstScore);
    }
    public Dictionary<string, Dictionary<string, int>> getNodes()
    {
        return nodes;
    }

    private Dictionary<string, Dictionary<string, int>> nodes;
}



namespace Project3
{
    class Project3
    {
        public static void Main()
        {

            Dictionary<string, List<string>> adjacenies_unweighted = new Dictionary<string, List<string>>()
            {
                {"Alicia" , new List<string> {"Britney"} },
                {"Britney", new List<string>{ "Alicia" , "Claire" } },
                {"Claire" ,new List<string> {"Alicia" , "Diana" } },
                {"Diana", new List<string> { "Claire", "Harry", "Edward" } },
                {"Edward" , new List<string> { "Fred", "Diana", "Harry", "Gloria" } },
                {"Fred" , new List<string> { "Edward", "Gloria" } },
                {"Harry" , new List<string> { "Diana", "Gloria","Edward" } },
                {"Gloria", new List<string> { "Fred", "Harry", "Edward" } },
            };
            Dictionary<string, Dictionary<string, int>> adjacenies_weighted = new Dictionary<string, Dictionary<string, int>>()
            {
                {"A" , new  Dictionary<string,int> { { "B", 1 } , { "C", 1 }, { "E", 5 } } },
                {"B" , new  Dictionary<string,int> { { "A", 1 } , { "C", 4 }, { "E", 1 }, { "G", 1 }, { "H", 1 } } },
                {"C" , new  Dictionary<string,int> { { "A", 1 } , { "B", 4 }, { "E", 1 }, { "D", 3 } } },
                {"D" , new  Dictionary<string,int> { { "C", 3 } , { "E", 2 }, { "G", 5 }, { "F", 1 } } },
                {"E" , new  Dictionary<string,int> { { "A", 5 } , { "B", 1 }, { "C", 1 } , { "D", 2 }, { "G", 2 }, } },
                {"F" , new  Dictionary<string,int> { { "D", 1 } , { "G", 1 }, } },
                {"G" , new  Dictionary<string,int> { { "B", 1 } , { "D", 5 } , { "E", 2 }, { "F", 1 }, } },
                {"H" , new  Dictionary<string,int> { { "B", 1 } , { "G", 2 }, { "I", 3 }, } },
                {"I" , new  Dictionary<string,int> { { "H", 3 } , { "J", 3 }, } },
                {"J" , new  Dictionary<string,int> { { "I", 3 }, } },

            };


            unweightedGraph testGraph1 = new unweightedGraph(adjacenies_unweighted);
            weightedGraph testGraph2 = new weightedGraph(adjacenies_weighted);

            KeyValuePair<string, double> result1 = testGraph1.findBestScore();
            Console.WriteLine("the best node in testGraph1 is {0} with a score of: {1}", result1.Key, result1.Value);
            KeyValuePair<string, double> result2 = testGraph1.findWorstScore();
            Console.WriteLine("the worst node in testGraph1 is {0} with a score of: {1}", result2.Key, result2.Value);

            KeyValuePair<string, double> result3 = testGraph2.findBestScore();
            Console.WriteLine("the best node in testGraph2 is {0} with a score of: {1}", result3.Key, result3.Value);
            KeyValuePair<string, double> result4 = testGraph2.findWorstScore();
            Console.WriteLine("the worst node in testGraph2 is {0} with a score of: {1}", result4.Key, result4.Value);

            // --- tests ---
            /*Dictionary<string, int> bestRoutes = testGraph2.findRoutesLengths("B");
            foreach (KeyValuePair<string, int> node in bestRoutes)
            {
                Console.WriteLine(node.Key + ": " + node.Value); // test
            }*/


        }

    }
}



