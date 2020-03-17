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


    private class CheckerIdle : IChecker
    {
        AI outer;
        int priority;

        public CheckerIdle(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;
        }
        public int Do(int priority, bool failed = false)
        {
            if (priority > this.priority)
                return priority;

            if (outer.currentTask == null)
                outer.currentTask = outer.TaskComplete;

            return this.priority;
        }
    }


    private class CheckerKillPlayers : IChecker
    {
        AI outer;
        int priority;
        float timer = 0;
        const float updateTime = 1;
        List<PlayerHealth> players;

        public CheckerKillPlayers(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;
            players = new List<PlayerHealth>();
            
            foreach (Transform child in outer.levelStats.PlayerArray.transform)
                if(child!=outer.transform)
                    players.Add(child.GetComponent<PlayerHealth>());
        }

        public int Do(int priority, bool failed = false)
        {
            if (priority > this.priority)
                return priority;

            timer += Time.deltaTime;
            if (timer > updateTime)
            {
                timer = 0;
                return Reset(priority);
            }

            return this.priority;
        }

        private int Reset(int inCommingPriority)
        {
            for(int i=players.Count-1;i>=0; --i)
            {

                if (players[i].isDead)
                {
                    players.RemoveAt(i);
                    continue;
                }

                PlayerHealth p = players[i];

                int px, py,mx,my;
                outer.GetMapGridPos(p.transform.position, out px, out py);
                outer.GetMapGridPos(outer.transform.position, out mx, out my);

                if (mx != px || my != py)
                    continue;

                float distP = Vector2.Distance(outer.transform.position, p.transform.position);
                foreach(PlayerHealth pp in players)
                {
                    float dist = Vector2.Distance(outer.transform.position, pp.transform.position);
                    if (dist < distP)
                    {
                        distP = dist;
                        p = pp;
                    }
                }

                outer.TaskList.Clear();
                outer.currentTask = () => outer.TaskAssignAttackPlayerInRoom(p);
                return priority;
            }
            if(inCommingPriority==priority)
                return 0;
            return inCommingPriority;
        }
    }

    private class CheckerEscapePod : IChecker
    {
        AI outer;
        int priority;
        GameObject EscapePod;
        float timer = 0;
        const float updateTime = 1;

        public CheckerEscapePod(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;

            EscapePod = outer.levelStats.GetComponent<EscapePodLauncher>().EscapePod;
        }

        public int Do(int priority, bool failed = false)
        {
            if (priority > this.priority)
                return priority;

            if (!EscapePod.activeInHierarchy)
                return priority;

            timer += Time.deltaTime;
            if (timer > updateTime)
            {
                Reset();
                timer = 0;
            }

            return this.priority;
        }

        private void Reset()
        {
            outer.MakePathToTarget(EscapePod.gameObject, outer.TaskTraverseRoom);
        }
    }

    private class CheckerAvoidAsteroids : IChecker
    {
        GameObject asteroidImpactPoint;
        AI outer;
        int priority, overwrittenPriority;

        public CheckerAvoidAsteroids(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;
            
            WarningLightManager warningLight = outer.levelStats.GetComponent<WarningLightManager>();
            asteroidImpactPoint = warningLight.WarningLight;
            overwrittenPriority = 0;
        }

        public int Do(int priority, bool failed = false)
        {
            //if a higher priority goal checker is in control, yield to it
            if (priority > this.priority)
                return priority;

            if (priority == this.priority && failed)
                FleeRoom();

            //if you are already heading out of the room, no need to flee
            if (HeadingOutOfRoom()) 
                return priority;

            //if this checker took control but is now finished, return priority to its previous value 
            // otherwise, if there is no asteroid threat, don't take control.
            if (!asteroidImpactPoint.activeInHierarchy)
                return AppropriatePriority(priority,this.priority,overwrittenPriority);
            if(!InRoomWithColission())
                return AppropriatePriority(priority, this.priority, overwrittenPriority);

            FleeRoom();

            overwrittenPriority = priority;
            return this.priority;
        }

        private void FleeRoom()
        {
            print("     flee room");

            List<DoorBehavior> doors = new List<DoorBehavior>();
            DoorBehavior door;
            float MinDist = float.MaxValue;
            Task BestPath=null; //the game rules dictate that there will be a door available, so this will be set

            if (outer.TryFindEastDoor(out door))
            {
                float dist = (door.transform.position - outer.transform.position).magnitude;
                if (dist < MinDist)
                {
                    MinDist = dist;
                    BestPath = outer.TaskAssignGoThroughEastDoor;
                }
            }

            if (outer.TryFindWestDoor(out door))
            {
                float dist = (door.transform.position - outer.transform.position).magnitude;
                if (dist < MinDist)
                {
                    MinDist = dist;
                    BestPath = outer.TaskAssignGoThroughWestDoor;
                }
            }

            if (outer.TryFindNorthDoor(out door))
            {
                float dist = (door.transform.position - outer.transform.position).magnitude;
                if (dist < MinDist)
                {
                    MinDist = dist;
                    BestPath = outer.TaskAssignGoThroughNorthDoor;
                }
            }


            if (outer.TryFindSouthDoor(out door))
            {
                float dist = (door.transform.position - outer.transform.position).magnitude;
                if (dist < MinDist)
                {
                    MinDist = dist;
                    BestPath = outer.TaskAssignGoThroughSouthDoor;
                }
            }

            outer.TaskList.Clear();
            outer.TaskList.Push(outer.TaskComplete);
            outer.currentTask = BestPath;

        }

        private bool HeadingOutOfRoom()
        {
            if (outer.currentTask == outer.TaskGoThroughEastDoor)
                return true;
            if (outer.currentTask == outer.TaskGoThroughWestDoor)
                return true;
            if (outer.currentTask == outer.TaskGoThroughSouthDoor)
                return true;
            if (outer.currentTask == outer.TaskGoThroughNorthDoor)
                return true;
            return false;
        }


        private bool InRoomWithColission()
        {
            int Ax, Ay;
            outer.GetMapGridPos(asteroidImpactPoint.transform.position, out Ax, out Ay);
            int x, y;
            outer.GetMapGridPos(outer.transform.position, out x, out y);
            return (x == Ax && y == Ay);
        }
    }

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
            print("" + outer.gameObject.name + "RESETING route");
            float minDist = float.MaxValue;
            PlayerHealth closestPlayer=null;

            int Ax, Ay;
            WarningLightManager warningLight = outer.levelStats.GetComponent<WarningLightManager>();
            if (warningLight.WarningLight.activeInHierarchy)
                outer.GetMapGridPos(warningLight.WarningLight.transform.position, out Ax, out Ay);
            else
            {
                Ax = -1;
                Ay = -1;
            }

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
                                //Don't target a player who is being asteroided
                                int Px, Py;
                                outer.GetMapGridPos(p.transform.position, out Px, out Py);
                                if (Px != Ax || Py != Ay)
                                {
                                    minDist = distToPlayer;
                                    closestPlayer = p;
                                    //TODO XXX remove this break which seaks first player
                                    //break;
                                }
                            }
                        }
                    }
                }
            }
            
            TargetPlayer = closestPlayer;
            if(TargetPlayer)
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
        if(cc!=0)// if an asteroid is going for the player their "target location" will be undreachable
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

        //print("\n");

        currentTask = TaskList.Pop();
    }

    /// <summary>
    /// Return the correct priority level, this returns a previous (overwritten) priortiy if
    /// this priority is the current priority, otherwise, it returns the current priority
    /// This is called if an overall task is completed
    /// </summary>
    /// <param name="currentPriority">The current prioirty (fed into the Do method)</param>
    /// <param name="thisPriority">The priority of the checker</param>
    /// <param name="overwrittenPriority">The previous priority before it was overwritten by the checker</param>
    /// <returns>currentPriority or overwritten priority</returns>
    private static int AppropriatePriority(int currentPriority, int thisPriority, int overwrittenPriority)
    {
        if (currentPriority == thisPriority)
            return overwrittenPriority;
        return currentPriority;
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
