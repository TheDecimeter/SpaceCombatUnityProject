using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseTile
{
    private const int North = 0, East = 1, South = 2, West = 3, Any=4;
    //the way the tiles are laid out
    TileInformation[][] map;
    //the next to close tile's location in the map
    Node activeCloser;
    Node[] closableList;
    int closableListEnd;
    int openTiles, stopAt;

    LinkedList<Node> CloseOrder = new LinkedList<Node>();


    HashSet<Node> visited;
    int visitedTiles;


    public CloseTile(TileInformation [][] map, int startFromEdge, int stopAt)
    {
        this.map = map;
        this.stopAt = stopAt;


        int count = 0;
        switch (startFromEdge) {
            case Any:
                //count all tiles with an empty space beside them
                for (int i = 0; i < map.Length; ++i)
                    for (int j = 0; j < map[i].Length; ++j)
                        if (map[i][j] != null &&
                            (i - 1 < 0 || j - 1 < 0 || i + 1 == map.Length || j + 1 == map[0].Length ||
                             map[i - 1][j] == null || map[i + 1][j] == null ||
                             map[i][j - 1] == null || map[i][j + 1] == null ))
                            count++;
                
                //and put those tile's coordinates into an array.
                closableList = new Node[count];
                count = 0;
                for (int i = 0; i < map.Length; ++i)
                    for (int j = 0; j < map[i].Length; ++j)
                        if (map[i][j] != null &&
                            (i - 1 < 0 || j - 1 < 0 || i + 1 == map.Length || j + 1 == map[0].Length ||
                             map[i - 1][j] == null || map[i + 1][j] == null ||
                             map[i][j - 1] == null || map[i][j + 1] == null))
                            closableList[count++]=new Node(i,j);
                //MonoBehaviour.print("closable List size " + closableList.Length);
                break;

            case South:
                for (int i = 0; i < map.Length; ++i)
                {
                    for (int j = 0; j < map[i].Length; ++j)
                        if (map[i][j] != null)
                            count++;
                    if (count > 0) break;
                }

                closableList = new Node[count];
                count = 0;
                for (int i = 0; i < map.Length; ++i)
                {
                    for (int j = 0; j < map[i].Length; ++j)
                        if (map[i][j] != null)
                            closableList[count++] = new Node(i, j);
                    if (count > 0) break;
                }
                break;

            case North:
                for (int i = map.Length-1; i >=0; ++i)
                {
                    for (int j = 0; j < map[i].Length; ++j)
                        if (map[i][j] != null)
                            count++;
                    if (count > 0) break;
                }

                closableList = new Node[count];
                count = 0;
                for (int i = map.Length - 1; i >= 0; ++i)
                {
                    for (int j = 0; j < map[i].Length; ++j)
                        if (map[i][j] != null)
                            closableList[count++] = new Node(i, j);
                    if (count > 0) break;
                }
                break;

            case East:
                for (int j = map.Length - 1; j >= 0; ++j)
                {
                    for (int i = 0; i < map[i].Length; ++i)
                        if (map[i][j] != null)
                            count++;
                    if (count > 0) break;
                }

                closableList = new Node[count];
                count = 0;
                for (int j = map.Length - 1; j >= 0; ++j)
                {
                    for (int i = 0; i < map[i].Length; ++i)
                        if (map[i][j] != null)
                            closableList[count++] = new Node(i, j);
                    if (count > 0) break;
                }
                break;

            case West:
                for (int j = 0; j <map.Length; ++j)
                {
                    for (int i = 0; i < map[i].Length; ++i)
                        if (map[i][j] != null)
                            count++;
                    if (count > 0) break;
                }

                closableList = new Node[count];
                count = 0;
                for (int j = 0; j < map.Length; ++j)
                {
                    for (int i = 0; i < map[i].Length; ++i)
                        if (map[i][j] != null)
                            closableList[count++] = new Node(i, j);
                    if (count > 0) break;
                }
                break;
        }

        //see how many open tiles the level has
        openTiles = 0;
        for (int i = 0; i < map.Length; ++i)
            for (int j = 0; j < map[i].Length; ++j)
                if (map[i][j] != null)
                    openTiles++;

        closableListEnd = closableList.Length;

        setNextToClose();
    }

    public bool getNextToClose(out int x, out int y)
    {
        if (openTiles == stopAt||activeCloser == null)
        {
            x = -1;
            y = -1;
            //MonoBehaviour.print("in getNextToClose "+x+" "+y);
            return false;
        }
        x = activeCloser.j;
        y = activeCloser.i;
        //MonoBehaviour.print("in getNextToClose " + x + " " + y);
        return true;
    }
    public void close()
    {
        if (openTiles == stopAt)
            return;
        map[activeCloser.i][activeCloser.j].isClosed = true;
        openTiles--;
        setNextToClose();
    }

    private void setNextToClose()
    {
        if (activeCloser == null)
        {
            List<Node> candidates = new List<Node>();
            for (int i = 0; i < map.Length; ++i)
                for (int j = 0; j < map[i].Length; ++j)
                {
                    if (map[i][j] != null && !map[i][j].isClosed)
                    {
                        candidates.Add(new Node(i, j));
                    }
                }
            //candidates.Sort((a, b) => Random.Range(-1, 1));
            Shuffle(candidates);
            CloseOrder = new LinkedList<Node>(candidates);

            activeCloser = CloseOrder.First.Value;
            CloseOrder.RemoveFirst();
        }

        if (map[activeCloser.i][activeCloser.j].isClosed)
        {
            activeCloser = CloseOrder.First.Value;
            CloseOrder.RemoveFirst();
        }

        int count = 111;

        while (ClosingSplitsMap(activeCloser))
        {
            count--;
            if (count == 0)
            {
                Debug.LogError("used loop count ");
                break;
            }
            CloseOrder.AddLast(activeCloser);
            activeCloser = CloseOrder.First.Value;
            CloseOrder.RemoveFirst();
        }

        return;

        
    }
    
    private static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private bool positionContainsOpenTile(int x, int y)
    {
        //make sure coordinates are within boundaries
        if (y == -1) return false;
        if (y == map.Length) return false;
        if (x == -1) return false;
        if (x == map[0].Length) return false;

        //make sure the map position isn't empty
        if (map[y][x] == null)
            return false;
        //make sure the tile isn't already closed
        if (map[y][x].isClosed)
            return false;

        return true;
    }

    private bool ClosingSplitsMap(Node atTile)
    {
        visited = new HashSet<Node>();
        map[atTile.i][atTile.j].isClosed = true;

        //move to only one neighbor and recursively
        //check tiles from there
        if (positionContainsOpenTile(atTile.j-1, atTile.i))
            recursiveSplitCheck(atTile.j-1, atTile.i);
        else if (positionContainsOpenTile(atTile.j+1, atTile.i))
            recursiveSplitCheck(atTile.j+1, atTile.i);
        else if (positionContainsOpenTile(atTile.j, atTile.i-1))
            recursiveSplitCheck(atTile.j, atTile.i-1);
        else if (positionContainsOpenTile(atTile.j, atTile.i+1))
            recursiveSplitCheck(atTile.j, atTile.i+1);

        map[atTile.i][atTile.j].isClosed = false;
        //MonoBehaviour.print("in closingSplitsMap visited.Count: " +visited.Count+ " openTiles: " + openTiles);
        return visited.Count != openTiles-1;
    }
    private void recursiveSplitCheck(int x, int y)
    {
        //MonoBehaviour.print("in recursiveSplitCheck " + x + " " + y);
        //check to make sure the tile is open
        if (!positionContainsOpenTile(x, y))
        {
            //MonoBehaviour.print("invalid Position");
            return;
        }
        //check to make sure the tile hasn't already been counted
        if (visited.Contains(new Node(y, x)))
        {
            //MonoBehaviour.print("node already visited");
            return;
        }

        visited.Add(new Node(y, x));

        recursiveSplitCheck(x-1, y);
        recursiveSplitCheck(x+1, y);
        recursiveSplitCheck(x, y-1);
        recursiveSplitCheck(x, y+1);
    }

    private void activeCloserIsWorthless()
    {
        for(int i=0; i<closableListEnd; ++i)
        {
            if (closableList[i].Equals(activeCloser))
            {
                closableList[i] = closableList[--closableListEnd];
                break;
            }
        }
        if (closableListEnd != 0)
            activeCloser = closableList[Random.Range(0, closableListEnd)];
        else activeCloser = null;
    }

    private class Node //: System.IComparable<Node>
    {
        public int i, j;

        public Node() { }

        public Node(int i, int j)
        {
            set(i, j);
        }

        public void set(int i, int j)
        {
            this.i = i;
            this.j = j;
        }

        //i can has hash set
        public override int GetHashCode()
        {
            return i*31+j;
        }
        public override bool Equals(object o)
        {
            if (o == null)
                return false;
            if (ReferenceEquals(this, o))
                return true;
            if (o.GetType() != typeof(Node))
                return false;

            Node that = (Node)o;
            return (this.i == that.i && this.j == that.j);
        }
        

        //public int CompareTo(Node other)
        //{
        //    return Random.Range(-1, 1);
        //}
    }
}
