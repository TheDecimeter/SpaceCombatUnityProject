using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keep track of various goals in the game
///  eg. close to a desired weapon
///      player you were hunting is killed
///      etc
/// </summary>
public partial class AI : MonoBehaviour
{

    private class CheckerKillNearestPlayer :IChecker
    {
        AI outer;
        PlayerHealth TargetPlayer;
        int priority;
        float timer = 0;
        float updateTime = 1;

        public CheckerKillNearestPlayer(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;
        }

        public int Do(int priority, bool failed=false)
        {
            //if you are currently performing a higher level task,
            //don't override it!
            if (priority > this.priority)
                return priority;

            if (timer == 0||failed)
                Reset();

            timer += Time.deltaTime;
            if (timer > updateTime)
                timer = 0;

            return this.priority;

        }

        private void Reset()
        {
            float minDist = float.MaxValue;
            PlayerHealth closestPlayer=null;
            foreach (Transform c in outer.levelStats.PlayerArray.transform)
            {
                if (c != outer.transform) {
                    PlayerHealth p = c.GetComponent<PlayerHealth>();
                    if (p)
                    {
                        if (!p.isDead)
                        {
                            float distToPlayer = Vector3.Distance(outer.transform.position, c.position);
                            if(distToPlayer<minDist)
                            {
                                minDist = distToPlayer;
                                closestPlayer = p;
                                //TODO XXX remove this break which seaks first player
                                break;
                            }
                        }
                    }
                }
            }
            
            TargetPlayer = closestPlayer;
            outer.MakePathToTarget(TargetPlayer.gameObject, () => outer.TaskAttackPlayerInRoom(TargetPlayer));
        }
    }


    private void MakePathToTarget(GameObject Target, Task EndTask)
    {
        int[][] levelPath;

        levelPath = new int[levelStats.MapDemensionsY][];
        for (int i = 0; i < levelStats.MapDemensionsY; ++i)
        {
            levelPath[i] = new int[levelStats.MapDemensionsX];
        }

        //find map pos for target and outer
        Queue<Node> q = new Queue<Node>();
        int x,  y;
        GetMapGridPos(transform.position, out x, out y);
        q.Enqueue(new Node(x, y, 1));

        int Tx,  Ty;
        GetMapGridPos(Target.transform.position, out Tx, out Ty);

        //get asteroid colission position and avoid if applicaple
        int Ax, Ay;
        WarningLightManager warningLight = levelStats.GetComponent<WarningLightManager>();
        if(warningLight.WarningLight.activeInHierarchy)
            GetMapGridPos(warningLight.WarningLight.transform.position, out Ax, out Ay);
        else
        {
            Ax = -1;
            Ay = -1;
        }

        int count = 1000;
        //bfs around to target
        while (q.Count > 0)
        {
            count -= 1;
            if (count < 0)
            {
                Debug.LogError("Had to use loop counter bfs player loc");
                break;
            }
            Node c = q.Dequeue();

            if (!validTile(c.x, c.y))
                continue;
            if (levelPath[c.y][c.x] != 0)
                continue;

            levelPath[c.y][c.x] = c.cost;

            if (c.x == Tx && c.y == Ty)
                break; //stop when you've reached the target

            if (c.y + 1 < levelPath.Length && !(c.x==Ax&&c.y + 1==Ay))
                q.Enqueue(new Node(c.x, c.y + 1, c.cost + 1));
            if (c.y - 1 >= 0 && !(c.x == Ax && c.y - 1 == Ay))
                q.Enqueue(new Node(c.x, c.y - 1, c.cost + 1));
            if (c.x + 1 < levelPath[0].Length && !(c.x + 1 == Ax && c.y == Ay))
                q.Enqueue(new Node(c.x + 1, c.y, c.cost + 1));
            if (c.x - 1 >= 0 && !(c.x - 1== Ax && c.y == Ay))
                q.Enqueue(new Node(c.x - 1, c.y, c.cost + 1));
        }

        //print("grid to player \n" + AI.GridToString(levelPath));

        count = 100;
        //work from target pos to your pos to convert mapped route to tasks
        TaskList.Clear();
        TaskList.Push(EndTask);
        int cx = Tx,  cy = Ty,  cc = levelPath[Ty][Tx];
        //print("startingCost " + cc);
        while (cx != x || cy != y)
        {
            count -= 1;
            if (count < 0)
            {
                Debug.LogError("Had to use loop counter task loading");
                break;
            }
            //check left
            if (nextStep(cx - 1, cy, cc, levelPath))
            {
                //print("go East " + cx + "," + cy);
                TaskList.Push(TaskAssignGoThroughEastDoor);
                cx -= 1;
            }
            //check right
            if (nextStep(cx + 1, cy, cc, levelPath))
            {
                //print("go West " + cx + "," + cy);
                TaskList.Push(TaskAssignGoThroughWestDoor);
                cx += 1;
            }
            //check down
            if (nextStep(cx, cy - 1, cc, levelPath))
            {
                //print("go North " + cx + "," + cy);
                TaskList.Push(TaskAssignGoThroughNorthDoor);
                cy -= 1;
            }
            //check up
            if (nextStep(cx, cy + 1, cc, levelPath))
            {
                //print("go South "+cx+","+cy);
                TaskList.Push(TaskAssignGoThroughSouthDoor);
                cy += 1;
            }
            cc--;
        }

        print("\n");

        currentTask = TaskList.Pop();
    }

    private bool nextStep(int x, int y, int cost, int[][] levelPath)
    {
        if (x < 0 || x > levelStats.MapDemensionsX - 1)
            return false;
        if (y < 0 || y > levelStats.MapDemensionsY - 1)
            return false;
        return levelPath[y][x] == cost - 1;
    }

    private bool validTile(int x, int y)
    {
        //if (x < 0 || x > outer.levelStats.MapDemensionsX - 1)
        //    return false;
        //if (y < 0 || y > outer.levelStats.MapDemensionsY - 1)
        //    return false;
        if (levelStats.Map[y][x] != null && !levelStats.Map[y][x].isClosed)
            return true;
        return false;
    }
    private class Node
    {
        public int x, y;
        public int cost;
        public Node(int x, int y, int cost)
        {
            this.x = x;
            this.y = y;
            this.cost = cost;
        }
    }

    public static string GridToString(int[][] p)
    {
        string s = "";
        for(int i=p.Length-1; i>=0; i--)
        {
            for (int j = 0; j < p[i].Length; ++j)
                s += "" + p[i][j];
            s += "\n";
        }
        return s;
    }



    private interface IChecker
    {
        int Do(int priority, bool failed=false);
    }
}
