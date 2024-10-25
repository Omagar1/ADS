
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace maze
{
    /*class EndlessStack
    {
        public void add(int[] valToAdd)
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

        public int[] pop()
        {
            int[] temp = values[-1];
            values.RemoveAt(-1);
            return temp;
        }

        public List<int[]> convertToList()
        {
            return values;
        }


        private List<int[]> values = new List<int[]>();
    }*/




    class Program
    {
        static bool inRange(int[,] maze, int[] pos) // checks if a position is in the maze's range 
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
     
        static List<int[]>? findRoute(int[] currentPos, List<int[]> previous, List<int[]> visited, int[,] maze)
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
                        List<int[]> route = findRoute(lookPos, previous, visited, maze);
                        Console.WriteLine("current Location: ({0},{1})", currentPos[0], currentPos[1]);// test 
                        if (route != null)
                        {
                            /*Console.WriteLine("(y,x): ({0},{1})", y, x); //test*/
                            return route;
                        }

                    } // checks if lookPos is not in range as would signify that the End has been found and previous is > 1  as to check we are not at the start, 
                      // && maze[y, x] != 1 && !currentPos.SequenceEqual(lookPos) && !visited.Any(array => array.SequenceEqual(lookPos))
                    else if ((!inRange(maze, lookPos) && previous.Count > 1 ) )
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

        static void Main(string[] args)
        {
            List<int[]> previous = new List<int[]>();
            List<int[]> visited = new List<int[]>();






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
            List<int[]> result = findRoute(STARTPos, previous, visited, maze2);

            // display result
            if (result != null)
            {
                Console.WriteLine("route:");
                /*List<int[]> resultList = result.convertToList();*/
                foreach (int[] node in result){
                    Console.Write("({0},{1}) then: ",node[0], node[1]);
                }
                Console.Write("End");
            }
            else
            {
                Console.WriteLine("No Exit Found");
            }
            // test 

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
