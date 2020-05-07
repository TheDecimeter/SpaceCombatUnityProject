using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoadAttribute]
public static class PauseStateChangedExample
{
    public static bool paused = false;
    // register an event handler when the class is initialized
    static PauseStateChangedExample()
    {
        EditorApplication.pauseStateChanged += LogPauseState;
    }

    private static void LogPauseState(PauseState state)
    {
        Debug.Log(state);
        paused = state==PauseState.Paused;
    }
}
#endif



public partial class AI : MonoBehaviour
{
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if(doer!=null)
                doer.StopAndWait();
        }
        else
        {
            if(doer!=null)
                doer.Start();
        }
    }

    public class Doer
    {
        public delegate bool Runner(float deltaTime);
        public List<Runner> Runners;
        private bool KeepRunning = false;
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
            if (KeepRunning)
                return;
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
            if (!KeepRunning)
                return;
            KeepRunning = false;
            t.Join();
        }

        private void AsyncRun()
        {
            float deltaTime = 0.1f;
            while (KeepRunning && Runners.Count > 0)
            {
#if UNITY_EDITOR
                if(!PauseStateChangedExample.paused)
#endif
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
