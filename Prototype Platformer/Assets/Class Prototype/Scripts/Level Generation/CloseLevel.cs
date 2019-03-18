using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseLevel
{
    const int waitToWarn = 0, warn = 1, close = 2;
    int closingStage, frameTicker, timeToWarn, timeToWait;
    public bool stop = false;
    CloseTile closer;
    public CloseLevel(TileInformation[][] map, int CloseFromWhatDirection, int framesToWait, int framesToWarn, int stopAt)
    {
        frameTicker=0;
        closingStage = waitToWarn;
        timeToWarn = framesToWarn;
        timeToWait = framesToWait;
        closer=new CloseTile(map, CloseFromWhatDirection, stopAt);
    }

    public bool warnCheck(out int x, out int y)
    {
        if (stop)
        {
            x = -1;
            y = -1;
            return false;
        }

        frameTicker++;
        switch (closingStage)
        {
            case waitToWarn:
                if (frameTicker > timeToWait)
                {
                    closingStage = warn;
                    frameTicker = 0;
                    return closer.getNextToClose(out x, out y);
                }
                closer.getNextToClose(out x, out y);
                return false;

            case warn:
                if (frameTicker > timeToWarn)
                {
                    closingStage = waitToWarn;
                    frameTicker = 0;
                    closer.getNextToClose(out x, out y);
                    closer.close();
                    //x = -1;
                    //y = -1;
                    return false;
                }
                return closer.getNextToClose(out x, out y);
                
        }

        //x = -1;
        //y = -1;
        closer.getNextToClose(out x, out y);
        return false;
    }

}
