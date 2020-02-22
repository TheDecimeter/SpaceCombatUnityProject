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

        public CheckerKillNearestPlayer(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;
        }

        public void Do(int priority)
        {
            //if you are currently performing a higher level task,
            //don't override it!
            if (priority > this.priority)
                return;


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
            
        }


        private void MakePathToTarget()
        {
            byte[][] levelPath;

            levelPath = new byte[outer.levelStats.MapDemensionsY][];
            for (int i = 0; i < outer.levelStats.MapDemensionsY; ++i)
            {
                levelPath[i] = new byte[outer.levelStats.MapDemensionsX];
            }

            //find map pos for target and outer

            //bfs around to target
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
            public byte cost;
            public Node(int x, int y, int cost)
            {
                this.x = x;
                this.y = y;
                this.cost = (byte)cost;
            }
        }
    }





    private interface IChecker
    {
        void Do(int priority);
    }
}
