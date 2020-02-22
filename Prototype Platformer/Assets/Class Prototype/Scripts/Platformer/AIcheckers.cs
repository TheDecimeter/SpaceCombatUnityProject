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

        public int Do(int priority)
        {
            //if you are currently performing a higher level task,
            //don't override it!
            if (priority > this.priority)
                return priority;

            if (timer == 0)
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
                            }
                        }
                    }
                }
            }

            TargetPlayer = closestPlayer;
            MakePathToTarget();
        }


        private void MakePathToTarget()
        {
            int[][] levelPath;

            levelPath = new int[outer.levelStats.MapDemensionsY][];
            for (int i = 0; i < outer.levelStats.MapDemensionsY; ++i)
            {
                levelPath[i] = new int[outer.levelStats.MapDemensionsX];
            }

            //find map pos for target and outer
            Queue<Node> q = new Queue<Node>();
            int x;int y;
            outer.GetMapGridPos(outer.transform.position, out x, out y);
            q.Enqueue(new Node(x, y, 1));

            int Tx; int Ty;
            outer.GetMapGridPos(TargetPlayer.transform.position, out Tx, out Ty);


            //bfs around to target
            while (q.Count > 0)
            {
                Node c = q.Dequeue();
                if (!validTile(c.x, c.y))
                    continue;
                
                levelPath[c.y][c.x] = c.cost;

                if (c.x == Tx && c.y == Ty)
                    break; //stop when you've reached the target

                q.Enqueue(new Node(c.x, c.y + 1, c.cost + 1));
                q.Enqueue(new Node(c.x, c.y - 1, c.cost + 1));
                q.Enqueue(new Node(c.x + 1, c.y, c.cost + 1));
                q.Enqueue(new Node(c.x - 1, c.y, c.cost + 1));
            }

            //work from target pos to your pos to convert mapped route to tasks
            outer.TaskList.Clear();
            outer.TaskList.Push( ()=>outer.TaskAttackPlayerInRoom(TargetPlayer) );
            int cx = Tx; int cy = Ty; int cc = levelPath[Ty][Tx];
            while (cx != x && cy != y)
            {
                //check left
                if (nextStep(cx - 1, cy, cc, levelPath))
                {
                    outer.TaskList.Push(outer.TaskAssignGoThroughWestDoor);
                    cx -= 1;
                }
                //check right
                if (nextStep(cx + 1, cy, cc, levelPath))
                {
                    outer.TaskList.Push(outer.TaskAssignGoThroughEastDoor);
                    cx += 1;
                }
                //check down
                if (nextStep(cx, cy - 1, cc, levelPath))
                {
                    outer.TaskList.Push(outer.TaskAssignGoThroughNorthDoor);
                    cx -= 1;
                }
                //check up
                if (nextStep(cx, cy + 1, cc, levelPath))
                {
                    outer.TaskList.Push(outer.TaskAssignGoThroughSouthDoor);
                    cx -= 1;
                }
            }

            outer.currentTask = outer.TaskList.Pop();
        }

        private bool nextStep(int x, int y,int cost, int[][] levelPath)
        {
            if (x < 0 || x > outer.levelStats.MapDemensionsX - 1)
                return false;
            if (y < 0 || y > outer.levelStats.MapDemensionsY - 1)
                return false;
            return levelPath[y][x]==cost-1;
        }

        private bool validTile(int x, int y)
        {
            if (x < 0 || x > outer.levelStats.MapDemensionsX - 1)
                return false;
            if (y < 0 || y > outer.levelStats.MapDemensionsY - 1)
                return false;
            if (outer.levelStats.Map[y][x] != null && !outer.levelStats.Map[y][x].isClosed)
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
    }





    private interface IChecker
    {
        int Do(int priority);
    }
}
