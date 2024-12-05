using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;
using System.Linq;

class unweightedGraph
{
    public unweightedGraph(Dictionary<string, List<string>> nodes)
    {
        this.nodes = nodes;
    }
    public Dictionary<string,string> findRoutes(string startNode)
    {
        Queue<string> toVisit = new Queue<string>();
        Dictionary<string,string> visited = new Dictionary<string,string>();
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
                distanceToNode.Add(findRoute(node.Key, routes).Count() - 1); // -1 as the route will contain the score node as well and we want to find the distance to the node

            }
        }

        return (nodes.Count() - 1) / distanceToNode.Sum();
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
    public Dictionary<string, KeyValuePair<string, int>> findRoutes(string startNode)
    {
        Queue<string> toVisit = new Queue<string>();
        Dictionary<string, KeyValuePair<string, int>> visited = new Dictionary<string, KeyValuePair<string, int>>();
        toVisit.Enqueue(startNode);
        visited.Add(startNode, new KeyValuePair<string, int>(string.Empty, 0));
        string currentNode;

        while (toVisit.Count() > 0)
        {
            currentNode = toVisit.Dequeue();
            foreach (KeyValuePair<string, int> adjacentNode in this.nodes[currentNode])
            {
                if (!visited.Keys.Contains(adjacentNode.Key))
                {
                    toVisit.Enqueue(adjacentNode.Key);
                    visited.Add(adjacentNode.Key, new KeyValuePair<string, int>(currentNode, nodes[currentNode][adjacentNode.Key]));
                }
            }

        }
        return visited;
    }
    public Dictionary<string, int> findRoute(string start, Dictionary<string, KeyValuePair<string, int>> routes)
    {
        Dictionary<string, int> route = new Dictionary<string, int>();
        string nextNode = start;
        route.Add(start, 0);
        while (routes[nextNode].Key != ""  )
        {
            route.Add(routes[nextNode].Key, routes[nextNode].Value);
            nextNode = routes[nextNode].Key;
        }
        return route;
    }
    public double calcScore(string scoreNode)
    {

        List<double> distanceToNode = new List<double>();
        Dictionary<string, KeyValuePair<string, int>> routes = findRoutes(scoreNode);

        foreach (KeyValuePair<string, Dictionary<string, int>> node in nodes)
        {
            if (node.Key != scoreNode)
            {
                distanceToNode.Add(findRoute(node.Key, routes).Values.Sum() - 1); 

            }
        }

        return (nodes.Count() - 1) / distanceToNode.Sum();
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
                {"B" , new  Dictionary<string,int> { { "A", 1 } , { "C", 4 }, { "E", 1 }, { "H", 1 } } },
                {"C" , new  Dictionary<string,int> { { "A", 1 } , { "B", 4 }, { "E", 1 }, { "D", 3 } } },
                {"D" , new  Dictionary<string,int> { { "C", 3 } , { "E", 2 }, { "G", 5 }, { "F", 1 } } },
                {"E" , new  Dictionary<string,int> { { "A", 5 } , { "B", 1 }, { "C", 1 } , { "D", 2 }, { "G", 2 }, } },
                {"F" , new  Dictionary<string,int> { { "D", 1 } , { "G", 1 }, } },
                {"G" , new  Dictionary<string,int> { { "B", 1 } , { "D", 5 } , { "E", 2 }, { "F", 1 }, } },
                {"H" , new  Dictionary<string,int> { { "B", 1 } , { "G", 2 }, { "I", 3 }, } },
                {"I" , new  Dictionary<string,int> { { "H", 3 } , { "J", 3 }, } },
                {"J" , new  Dictionary<string,int> { { "I", 3 }, } },

            };


            /*unweightedGraph testGraph = new unweightedGraph(adjacenies_unweighted);*/
            weightedGraph testGraph = new weightedGraph(adjacenies_weighted);
            KeyValuePair<string, double> result = findBestScore(testGraph);
            Console.WriteLine("the best node is {0} with a score of: {1}", result.Key, result.Value);

        }
        public static KeyValuePair<string, double> findBestScore(unweightedGraph graph)
        {
            double bestScore = 0;
            string? bestNode = null;
            
            foreach (KeyValuePair<string, List<string>> node in graph.getNodes())
            {
                double newScore = graph.calcScore(node.Key);
                if (newScore > bestScore)
                {
                    bestScore = newScore;
                    bestNode = node.Key;
                }
            }
            return new KeyValuePair<string, double>  ( bestNode, bestScore ); 
        }
        public static KeyValuePair<string, double> findBestScore(weightedGraph graph)
        {
            double bestScore = 0;
            string? bestNode = null;

            foreach (KeyValuePair<string, Dictionary<string,int>> node in graph.getNodes())
            {
                double newScore = graph.calcScore(node.Key);
                if (newScore > bestScore)
                {
                    bestScore = newScore;
                    bestNode = node.Key;
                }
            }
            return new KeyValuePair<string, double>(bestNode, bestScore);
        }


    }
}



