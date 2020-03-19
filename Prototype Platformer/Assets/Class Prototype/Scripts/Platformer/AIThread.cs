using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public partial class AI : MonoBehaviour
{


    public class Doer
    {
        public delegate bool Runner(float deltaTime);
        public List<Runner> Runners;
        private bool KeepRunning = true;
        private Thread t;

        public Doer()
        {
            Runners = new List<Runner>();
        }

        public void AddRunner(Runner runner)
        {
            Runners.Add(runner);
        }

        public void Start()
        {
            KeepRunning = true;
            t = new Thread(AsyncRun);
            t.Start();
        }
        public void Stop()
        {
            KeepRunning = false;
        }
        public void StopAndWait()
        {
            KeepRunning = false;
            t.Join();
        }

        private void AsyncRun()
        {
            float deltaTime = 0.1f;
            while (KeepRunning && Runners.Count > 0)
            {
                for (int i = Runners.Count - 1; i >= 0; --i)
                {
                    if (Runners[i](deltaTime))
                        Runners.RemoveAt(i);
                }
                Thread.Sleep(100);
            }
        }

        ~Doer()
        {
            Stop();
        }
    }

}
