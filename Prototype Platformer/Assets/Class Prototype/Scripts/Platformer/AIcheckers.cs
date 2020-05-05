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

    private class CheckerHarm : IChecker
    {
        AI outer;
        float timer=0;
        float updateTime;
        PlayerHealth health;

        public CheckerHarm(AI outer, int updateTime, int priority)
        {
            this.outer = outer;
            this.updateTime = updateTime;
            health = outer.GetComponent<PlayerHealth>();
        }
        public int Do(int priority, float deltaTime, bool failed = false)
        {
            timer += deltaTime;
            if (timer > updateTime)
            {
                timer = 0;
                health.AsyncDamage(1);
            }
            return priority;
        }
    }

    private class CheckerIdle : IChecker
    {
        AI outer;
        int priority;

        public CheckerIdle(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;
        }
        public int Do(int priority, float deltaTime, bool failed = false)
        {
            if (priority > this.priority)
                return priority;

            if (outer.currentTask == null)
                outer.currentTask = outer.TaskComplete;

            return this.priority;
        }
    }

    private class CheckerGrabItem : IChecker
    {
        AI outer;
        CharacterMovement_Physics player;
        int topRank;
        

        public CheckerGrabItem(AI outer, int topRank)
        {
            this.topRank = topRank;
            this.outer = outer;
            player = outer.GetComponent<CharacterMovement_Physics>();
        }
        public int Do(int priority, float deltaTie, bool failed = false)
        {
            if (player._currentItem!=null&&player._currentItem.Rank() < topRank)
            {
                Item i = player.offeredItem;
                if (i != null && i.Rank() > player._currentItem.Rank())
                    outer.TriggerPickup(true);
                else
                    outer.TriggerPickup(false);
            }
            return priority;
        }
    }


    private class CheckerKillPlayers : IChecker
    {
        AI outer;
        int priority;
        float timer = 0;
        const float updateTime = 1;
        List<PlayerHealth> players;
        PlayerHealth targetPlayer;

        public CheckerKillPlayers(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;
            players = new List<PlayerHealth>();
            
            foreach (Transform child in outer.levelStats.PlayerArray.transform)
                if(child!=outer.transform)
                    players.Add(child.GetComponent<PlayerHealth>());
        }

        public int Do(int priority, float deltaTime, bool failed = false)
        {
            if (priority > this.priority)
                return priority;

            timer += deltaTime;
            if (timer > updateTime)
            {
                timer = 0;
                return Reset(priority);
            }

            return this.priority;
        }

        private int Reset(int inCommingPriority)
        {
            print("resetting attack for  " +outer.asyncname+" remaining players: "+players.Count);
            for (int i=players.Count-1;i>=0; --i)
            {

                if (players[i].isDead)
                {
                    print(outer.asyncname + " removed player " + players[i].asyncname);
                    players.RemoveAt(i);
                    continue;
                }
                if (players[i].framesDamage > 0 && i != 0)
                {
                    print(outer.asyncname + " skipping poisoned player " + players[i].asyncname);
                    continue;
                }

                PlayerHealth p = players[i];

                int px, py,mx,my;
                outer.GetMapGridPos(p.AsyncPosition, out px, out py);
                outer.GetMapGridPos(outer.AsyncPosition, out mx, out my);

                if (mx != px || my != py)
                {
                    print(outer.asyncname+" skipping out of room player" + players[i].asyncname);
                    continue;
                }

                float distP = Vector2.Distance(outer.AsyncPosition, p.AsyncPosition);
                foreach(PlayerHealth pp in players)
                {
                    if (pp.framesDamage > 0)
                        continue;

                    float dist = Vector2.Distance(outer.AsyncPosition, pp.AsyncPosition);
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
            print("     DID NOT TARGET PLAYER "+outer.asyncname);
            if(inCommingPriority==priority)
                return 0;
            return inCommingPriority;
        }
    }

    private class CheckerEscapePod : IChecker
    {
        AI outer;
        int priority;
        EscapePodLauncher EscapePod;
        float timer = 0;
        const float updateTime = 1;

        public CheckerEscapePod(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;

            EscapePod = outer.levelStats.GetComponent<EscapePodLauncher>();
        }

        public int Do(int priority, float deltaTime, bool failed = false)
        {
            if (priority > this.priority)
                return priority;

            if (!EscapePod.AsyncIncomming)
                return priority;

            timer += deltaTime;
            if (timer > updateTime)
            {
                Reset();
                timer = 0;
            }

            return this.priority;
        }

        private void Reset()
        {
            outer.MakePathToTarget(EscapePod.AsyncPosition, outer.TaskTraverseRoom,null);
        }
    }

    private class CheckerAvoidAsteroids : IChecker
    {
        WarningLightManager asteroidImpactPoint;
        AI outer;
        int priority, overwrittenPriority;

        public CheckerAvoidAsteroids(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;
            
            asteroidImpactPoint = outer.levelStats.GetComponent<WarningLightManager>();
            overwrittenPriority = 0;
        }

        public int Do(int priority, float deltaTime, bool failed = false)
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
            if (!asteroidImpactPoint.AsyncIncomming)
                return AppropriatePriority(priority,this.priority,overwrittenPriority);
            if(!InRoomWithColission())
                return AppropriatePriority(priority, this.priority, overwrittenPriority);

            FleeRoom();

            overwrittenPriority = priority;
            return this.priority;
        }

        private void FleeRoom()
        {
            print("     flee room "+outer.asyncname);
            
            DoorBehavior door;
            float MinDist = float.MaxValue;
            Task BestPath=null; //the game rules dictate that there will be a door available, so this will be set

            if (outer.TryFindEastDoor(out door))
            {
                float dist = (door.AsyncPosition - outer.AsyncPosition).magnitude;
                if (dist < MinDist)
                {
                    MinDist = dist;
                    BestPath = outer.TaskAssignGoThroughEastDoor;
                }
            }

            if (outer.TryFindWestDoor(out door))
            {
                float dist = (door.AsyncPosition - outer.AsyncPosition).magnitude;
                if (dist < MinDist)
                {
                    MinDist = dist;
                    BestPath = outer.TaskAssignGoThroughWestDoor;
                }
            }

            if (outer.TryFindNorthDoor(out door))
            {
                float dist = (door.AsyncPosition - outer.AsyncPosition).magnitude;
                if (dist < MinDist)
                {
                    MinDist = dist;
                    BestPath = outer.TaskAssignGoThroughNorthDoor;
                }
            }


            if (outer.TryFindSouthDoor(out door))
            {
                float dist = (door.AsyncPosition - outer.AsyncPosition).magnitude;
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
            outer.GetMapGridPos(asteroidImpactPoint.AsyncPosition, out Ax, out Ay);
            int x, y;
            outer.GetMapGridPos(outer.AsyncPosition, out x, out y);
            return (x == Ax && y == Ay);
        }
    }

    private class CheckerGoToNearestPlayer :IChecker
    {
        AI outer;
        PlayerHealth TargetPlayer;
        int priority;
        float timer = 0;
        float updateTime = 1;
        WarningLightManager warningLight;
        List<PlayerHealth> players;

        public CheckerGoToNearestPlayer(AI outer, int priority)
        {
            this.outer = outer;
            this.priority = priority;
            warningLight = outer.levelStats.GetComponent<WarningLightManager>();

            players = new List<PlayerHealth>();
            foreach (Transform child in outer.levelStats.PlayerArray.transform)
                if (child != outer.transform)
                    players.Add(child.GetComponent<PlayerHealth>());
        }

        public int Do(int priority, float deltaTime, bool failed=false)
        {
            //if you are currently performing a higher level task,
            //don't override it!
            if (priority > this.priority)
                return priority;

            if (timer == 0||failed)
                Reset();

            timer += deltaTime;
            if (timer > updateTime)
                timer = 0;

            return this.priority;

        }

        private void Reset()
        {
            //print("" + outer.gameObject.name + "RESETING route");
            float minDist = float.MaxValue;
            PlayerHealth closestPlayer=null;

            int Ax, Ay;
            if (warningLight.AsyncIncomming)
                outer.GetMapGridPos(warningLight.AsyncPosition, out Ax, out Ay);
            else
            {
                Ax = -1;
                Ay = -1;
            }

            for (int i = players.Count - 1; i >= 0; --i)
            {
                if (players[i].isDead)
                {
                    players.RemoveAt(i);
                    continue;
                }
                if (players[i].framesDamage > 0)
                    continue;

                float distToPlayer = Vector3.Distance(outer.AsyncPosition, players[i].AsyncPosition);
                if (distToPlayer < minDist)
                {
                    //Don't target a player who is being asteroided
                    int Px, Py;
                    outer.GetMapGridPos(players[i].AsyncPosition, out Px, out Py);
                    if (Px != Ax || Py != Ay)
                    {
                        minDist = distToPlayer;
                        closestPlayer = players[i];
                        //TODO XXX remove this break which seaks first player
                        //break;
                    }
                }
            }

                

            
            TargetPlayer = closestPlayer;
            if (TargetPlayer)
                outer.MakePathToTarget(TargetPlayer.AsyncPosition, () => outer.TaskAttackPlayerInRoom(TargetPlayer),warningLight);
            else
            {
                outer.TaskList.Clear();
                outer.currentTask = outer.TaskComplete;
            }
        }
    }


    private void MakePathToTarget(Vector3 Target, Task EndTask,WarningLightManager warningLight)
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
        GetMapGridPos(AsyncPosition, out x, out y);
        q.Enqueue(new Node(x, y, 1));

        int Tx,  Ty;
        GetMapGridPos(Target, out Tx, out Ty);

        //get asteroid colission position and avoid if applicaple
        int Ax=-1, Ay=-1;
        if (warningLight != null)
        {

            if (warningLight.AsyncIncomming)
                GetMapGridPos(warningLight.AsyncPosition, out Ax, out Ay);
        }

        //print("\n");
        //print("\n");
        //print(""+gameObject.name);
        //print("at " + transform.position+" "+AsyncPosition);

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
            //print("checking " + c.x + " " + c.y);

            if (!validTile(c.x, c.y))
                continue;
            if (levelPath[c.y][c.x] != 0)
                continue;

            levelPath[c.y][c.x] = c.cost;

            if (c.x == Tx && c.y == Ty)
            {
                //print("found target");
                break; //stop when you've reached the target
            }

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
        {
            //if (levelStats.Map[y][x] == null)
            //    print("tile at " + x + " " + y + " is null");
            //else if (levelStats.Map[y][x].isClosed)
            //    print("tile at " + x + " " + y + " is closed");
            return true;
        }
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
        int Do(int priority, float deltaTime, bool failed=false);
    }
}
