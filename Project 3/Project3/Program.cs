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
    public Dictionary<string, List<string>> getNodes()
    {
        return nodes;   
    }

    private Dictionary<string, List<string>> nodes;
}
namespace Project3
{
    class Project3
    {
        public static void Main()
        {
            
            Dictionary<string, List<string>> adjacenies = new Dictionary<string, List<string>>()
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
            unweightedGraph testGraph = new unweightedGraph(adjacenies);

            Dictionary<string, double> result = findBestScore(testGraph);
            Console.WriteLine("the best node is {0} with a score of: {1}", result.Keys.ToArray()[0], result.Values.ToArray()[0]);

        }
        public static Dictionary<string, double> findBestScore(unweightedGraph graph)
        {
            double bestScore = 0;
            string? bestNode = null;
            
            foreach (KeyValuePair<string, List<string>> node in graph.getNodes())
            {
                double newScore = calcScore(node.Key, graph);
                if (newScore > bestScore)
                {
                    bestScore = newScore;
                    bestNode = node.Key;
                }
            }
            return new Dictionary<string, double> { { bestNode, bestScore } }; 
        }

        public static double calcScore(string scoreNode, unweightedGraph graph)
        {
            
            List<double> distanceToNode = new List<double>();
            Dictionary<string, string> routes = graph.findRoutes(scoreNode);
            Dictionary<string, List<string>> nodes = graph.getNodes();

            foreach (KeyValuePair<string, List<string>> node in nodes)
            {
                if (node.Key != scoreNode)
                {
                    distanceToNode.Add(graph.findRoute(node.Key, routes).Count()-1); // -1 as the route will contain the score node as well and we want to find the distance to the node

                }
            }

            return (nodes.Count() - 1) / distanceToNode.Sum();
        }

    }
}



