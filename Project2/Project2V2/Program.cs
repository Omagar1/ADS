﻿

using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;

namespace maze
{
    class EndlessStack
    {
        public void push(Node valToAdd)
        {
            this.values.Add(valToAdd);
        }

        public bool isEmpty()
        {
            if (this.values.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Node pop()
        {
            Node temp = values[values.Count()-1];
            values.RemoveAt(values.Count() - 1);
            return temp;
        }

        public List<Node> convertToList()
        {
            return values;
        }

        public int count()
        {
            return this.values.Count;   
        }


        private List<Node> values = new List<Node>();
    }
    abstract class routingAlgorithms
    {
        public bool inRange(int[,] maze, int[] pos) // checks if a position is in the maze's range 
        {
            if (pos[0] < 0 || pos[0] >= maze.GetLength(0) || pos[1] < 0 || pos[1] >= maze.GetLength(1))
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        protected int[,] maze;
        
    }

    class depthFirst : routingAlgorithms
    {
        public depthFirst(int[,] maze)
        {
            this.maze = maze;
            this.visited = new List<int[]>();
        }
        public List<int[]>? findRoute(int[] currentPos) // for the fist call 
        {
            List<int[]> previous = new List<int[]>();
            return findRoute(currentPos, previous);
        }
        public List<int[]>? findRoute(int[] currentPos, List<int[]> previous)
        { // returns to exits if found if not returns null.
            Console.WriteLine("current Location: ({0},{1})", currentPos[0], currentPos[1]);// test 
            previous.Add(currentPos);
            visited.Add(currentPos);

            for (int y = currentPos[0] - 1; y <= currentPos[0] + 1; y++)
            {
                for (int x = currentPos[1] - 1; x <= currentPos[1] + 1; x++)
                {

                    int[] lookPos = { y, x };
                    /*Console.WriteLine("(y,x): ({0},{1})", y, x); //test
                    Console.WriteLine(y <= currentPos[1] + 1); */

                    // checks if lookPos is a valid position, if lookPos points to a zero, if lookPos is not current position, and if look pos has already been explored.
                    if (inRange(maze, lookPos) && maze[y, x] == 0 && !currentPos.SequenceEqual(lookPos) && !visited.Any(array => array.SequenceEqual(lookPos)))
                    {
                        // end not found explore more
                        List<int[]> route = findRoute(lookPos, previous);
                        Console.WriteLine("current Location: ({0},{1})", currentPos[0], currentPos[1]);// test 
                        if (route != null)
                        {
                            /*Console.WriteLine("(y,x): ({0},{1})", y, x); //test*/
                            return route;
                        }

                    } // checks if lookPos is not in range as would signify that the End has been found and previous is > 1  as to check we are not at the start, 
                      // && maze[y, x] != 1 && !currentPos.SequenceEqual(lookPos) && !visited.Any(array => array.SequenceEqual(lookPos))
                    else if ((!inRange(maze, lookPos) && previous.Count > 1))
                    {
                        // end found 
                        /*previous.Add(currentPos);*/
                        /*previous.Add(lookPos);*/
                        /*Console.WriteLine("(y,x): ({0},{1})", y, x); //test*/
                        return previous;

                    }

                }
            }
            // No end found and no more nodes to traverse to
            Console.WriteLine("going back no Route found"); //test
            return null;
        }
        private List<int[]> visited;
    }

    class Node
    {
        public Node(int[] Pos, Node previousNode = null, bool isEnd = false)
        {
            this.position = Pos;
            this.previousNode = previousNode;
            this.isEnd = isEnd; 
        }
        public bool samePos(int[] pos)
        {
            return pos.SequenceEqual(this.position);
        }
        // I know this breaks encapsulation but I was just going to write getters and setter for all of them anyway
        public int[] position;
        public Node? previousNode;
        public bool isEnd;
    }

    class breadthFirst : routingAlgorithms
    {
        public breadthFirst(int[,] maze)
        {
            this.maze = maze;
            this.toVisit = new Queue<Node>();
            this.visitedlist = new List<Node> ();
        }
        public EndlessStack traverseNode(Node currentNode, EndlessStack route)
        {
            if (currentNode.previousNode == null)
            {
                return route; 
            }
            else
            {
                route.push(currentNode);
                return traverseNode(currentNode.previousNode, route);
            }
        }
        
        public List<int[]>? findRoute(Node currentNode) 
        {
            bool isStart = true;
            // travese graph ( im treating the matrixies as a graph)
            toVisit.Enqueue(currentNode);
            do
            {
                currentNode = toVisit.Dequeue();
                Console.WriteLine("current Location: ({0},{1})", currentNode.position[0], currentNode.position[1]);// test 
                for (int y = currentNode.position[0] - 1; y <= currentNode.position[0] + 1; y++)
                {
                    for (int x = currentNode.position[1] - 1; x <= currentNode.position[1] + 1; x++)
                    {
                        

                        int[] lookPos = { y, x };
                        if (!currentNode.position.SequenceEqual(lookPos)) // to prevent current location from being consider 
                        {
                            bool onEdge = !inRange(this.maze, lookPos);
                            //Console.WriteLine("is ({0},{1}) in queue: {2} ", y, x, visitedlist.Any(node => node.samePos(lookPos))); //test
                            if (!onEdge && maze[y, x] == 0 && !currentNode.position.SequenceEqual(lookPos) && !visitedlist.Any(node => node.samePos(lookPos)) && !toVisit.Any(node => node.samePos(lookPos)) )
                            {
                                
                                Node lookNode = new Node(lookPos, currentNode, onEdge);
                                //Console.WriteLine("added Node ({0},{1}) to queue", lookNode.position[0],lookNode.position[1]);
                                toVisit.Enqueue(lookNode);
                            }
                            else if(!isStart && onEdge) // to prevent the start being counted
                            {
                                Console.WriteLine("End Found"); //test
                                currentNode.isEnd = true;
                            }
                        }
                        
                       
                    }
                }
                visitedlist.Add(currentNode);
                // test
                Console.Write("queue: {");
                foreach (Node node in toVisit)
                {
                    Console.Write("({0},{1}), ", node.position[0], node.position[1]); 
                }
                Console.Write("}\n"); 

                isStart = false;
            } while (toVisit.Count != 0);
           

            // find routes 
            List<EndlessStack> routes = new List<EndlessStack>();

            foreach(Node node in visitedlist)
            {
                EndlessStack route = new EndlessStack();  
                if (node.isEnd)
                {
                    route = traverseNode(node, route);
                    routes.Add(route); 
                }
            }
            // find shortest route
            int bestRouteLength = 0; 
            int bestRouteIndex = 0;


            if (routes.Count > 0)
            {
                for (int i = 0; i < routes.Count; i++)
                {
                    if (routes[i].count() > bestRouteLength)
                    {
                        bestRouteIndex = i;
                    }
                }
                // convet the best route to list of int[]
                List<int[]> bestRoute = new List<int[]>();
                while (routes[bestRouteIndex].count() > 0)
                {
                    bestRoute.Add(routes[bestRouteIndex].pop().position);
                }
                return bestRoute;


            }
            else 
            {
                return null; 
            }




        }

        private Queue<Node> toVisit;
        private List<Node> visitedlist;

    }



    class Program
    {
 
        static void Main(string[] args)
        {

            int[,] maze = {
                { 1,0,1,1,1,1,1,1},
                { 1,0,0,0,0,0,0,1},
                { 1,1,1,1,1,1,0,1},
                { 1,0,0,0,0,0,0,1},
                { 1,0,1,1,1,1,1,1},
                { 1,0,0,0,0,0,0,1},
                { 1,1,1,1,1,1,0,1},
                { 1,0,0,0,0,0,0,1},
                { 1,0,1,1,1,1,1,1},
                { 1,0,0,0,0,0,0,0},
                { 1,1,1,1,1,1,1,1}
            };

            int[,] maze2 = {
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                { 1,0,1,0,0,0,1,1,0,0,0,1,1,1,1,1,1},
                { 1,1,0,0,0,1,1,0,1,1,1,0,0,1,1,1,1},
                { 1,0,1,1,0,0,0,0,0,1,1,1,0,0,1,1,1},
                { 1,1,0,1,1,1,1,1,1,0,1,1,1,1,1,1,1},
                { 1,1,0,1,0,0,1,0,1,1,1,1,1,1,1,1,1},
                { 1,0,0,1,1,0,1,1,1,0,1,0,0,1,0,1,1},
                { 1,0,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1},
                { 1,0,0,1,1,0,1,1,0,1,1,1,1,1,1,0,1},
                { 1,1,0,0,0,1,1,0,1,1,0,0,0,0,0,0,1},
                { 1,0,0,1,1,1,1,1,0,0,0,1,1,1,1,0,1},
                { 1,0,1,0,0,1,1,1,1,1,0,1,1,1,1,0,0},
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            };



            int[] STARTPos = { 0, 1 };
            depthFirst Alg1 = new depthFirst(maze);
            breadthFirst Alg2 = new breadthFirst(maze2);
            Node STARTPosNode = new Node(STARTPos);
            List<int[]> result = Alg2.findRoute(STARTPosNode);

            // display result
            if (result != null)
            {
                Console.WriteLine("route:");
                /*List<int[]> resultList = result.convertToList();*/
                foreach (int[] node in result)
                {
                    Console.Write("({0},{1}) then: ", node[0], node[1]);
                }
                Console.Write("End");
            }
            else
            {
                Console.WriteLine("No Exit Found");
            }
            // test
            // 



            /*Node node1 = new Node(new int[] { 1, 1});
            Node node2 = new Node(new int[] { 1, 2 });
            Node node3 = new Node(new int[] { 1, 3 });
            Node node4 = new Node(new int[] { 1, 4 });
            List<Node> visitedlist = new List<Node>();
            visitedlist.Add(node1);
            visitedlist.Add(node2); 
            visitedlist.Add(node3); 
            visitedlist.Add(node4); 
*/



            //Console.WriteLine(node1.samePos(new int[] { 1, 2,})); // Should print true
           /* Console.WriteLine(visitedlist.Any(node => node.samePos(new int[] {1,0}))); // should print flase*/
            

            //Console.WriteLine(maze[1, 2]);
            /*int y = 1;
            int x = 1;

            int[] checkVar = { y, x };
            Console.WriteLine(maze[y, x] == 0 && (STARTPos != checkVar));
            Console.WriteLine(maze[y, x] == 0);
            Console.WriteLine((STARTPos != checkVar));
            Console.WriteLine(checkVar - STARTPos);


            visited.Add(STARTPos);

            int[] lookPos = { y, x };



            Console.WriteLine(visited.Any(array => array.SequenceEqual(lookPos))); */
        }



    }
}