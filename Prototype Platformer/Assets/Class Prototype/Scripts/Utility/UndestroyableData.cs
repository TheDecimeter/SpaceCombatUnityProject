using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// TODO
/// Add whether or not camera rotation is a thing and only return a non zero cam rotation if that is the case
/// use player count in file settings
/// </summary>

public class UndestroyableData : MonoBehaviour
{
    private string fileName = "/SCCdata.dat";

    [Header("Map Size in Menu")]
    public int MenuMapDemensionX = 8;
    public int MenuMapDemensionY = 8;
    public bool MenuFillLevel = false;
    public int MenuMaxTileQuantity = 32;
    [Header("LevelClosing in menu")]
    public int MenuCloseTileWarnFrames = 120;
    public int MenuCloseTileXManyFrames = 1;
    public int MenuChuckAsteroidSpeed = 70;
    public int MenuChuckAsteroidEvery = 20;
    public int MenuChuckAsteroidPlusOrMinus = 5;

    [Header("GUI Stuff")]
    public TextMeshProUGUI WinText;
    public bool VerticalSplit = true;

    static public UnityEvent DataReadyToBeRead=new UnityEvent();


    //Game Rounds   Getter Setter
    public void SetRounds(int rounds)
    {
        Debug.LogWarning("   Data - Set rounds " + rounds);
        sSave.RoundCounter = -1;
        sSave.Rounds = rounds;

        sSave.ScoreKeeper.player = new int[4];
    }
    public int GetRounds()
    {
        
        return sSave.Rounds;
    }
    public void GetRoundsE(IntUpdater.Get get)
    {
        Debug.Log("   Data - Get rounds " + sSave.Rounds);
        get(sSave.Rounds);
    }


    //Player Count   Getter Setter
    public void SetPlayers(int players)
    {
        sSave.PlayerCount = players;
    }
    public void GetPlayers(IntUpdater.Get get)
    {
        get(sSave.PlayerCount);
    }


    //Map Dimensions  Getter Setter
    public void SetMapWidth(int width)
    {
        sSave.Play.MapDemensionX = width;
        if (sSave.Play.FillLevel)
            sSave.Play.MaxTiles = sSave.Play.MapDemensionX * sSave.Play.MapDemensionY;
    }
    public void GetMapWidth(IntUpdater.Get get)
    {
        get(sSave.Play.MapDemensionX);
    }

    public void SetMapHeight(int height)
    {
        sSave.Play.MapDemensionY = height;
        if(sSave.Play.FillLevel)
            sSave.Play.MaxTiles = sSave.Play.MapDemensionX * sSave.Play.MapDemensionY;
    }
    public void GetMapHeight(IntUpdater.Get get)
    {
        get(sSave.Play.MapDemensionY);
    }

    public void SetMapTileCount(int count)
    {
        if (count >= sSave.Play.MapDemensionX * sSave.Play.MapDemensionY)
        {
            count = sSave.Play.MapDemensionX * sSave.Play.MapDemensionY;
            sSave.Play.FillLevel = true;
        }
        else
            sSave.Play.FillLevel = false;
        sSave.Play.MaxTiles = count;
    }
    public void GetMapTileCount(IntUpdater.Get get)
    {
        if (sSave.Play.FillLevel||sSave.Play.MaxTiles >= sSave.Play.MapDemensionX * sSave.Play.MapDemensionY)
        {
            sSave.Play.MaxTiles = sSave.Play.MapDemensionX * sSave.Play.MapDemensionY;
            sSave.Play.FillLevel = true;
        }
        get(sSave.Play.MaxTiles);
    }


    //camera settings
    public void SetVerticalScreenSplit(bool vertical)
    {
        if(sSave.PlayerCount==2)
            sSave.CamRot.verticalSplit = vertical;
    }
    public void GetVerticalScreenSplit(BoolUpdater.Get get)
    {
        get(sSave.CamRot.verticalSplit);
    }



    //camera settings
    public void SetTouchScreenControls(bool active)
    {
        sSave.TouchScreenControls = active;
    }
    public void GetTouchScreenControls(BoolUpdater.Get get)
    {
        get(sSave.TouchScreenControls);
    }
    
    public void SetLayerBlur(bool active)
    {
        sSave.LayerBlur = active;
    }
    public void GetLayerBlur(BoolUpdater.Get get)
    {
        get(sSave.LayerBlur);
    }
    public static bool GetLayerBlur()
    {
        return sSave.LayerBlur;
    }





    //Fires when data is meaningful (this might be just after Start)
    public void AddDataReadEvent(UnityAction listener)
    {
        DataReadyToBeRead.AddListener(listener);
    }












    // Start is called before the first frame update
    void Awake()
    {

        WinText.text = "";
        if (sSave.ScoreKeeper.player == null)
        {

            sSave.ScoreKeeper.player = new int[4];
            sSave.CamRot.player = new int[5];
            LoadFile(fileName);
            if (sSave.CamRot.player == null)
            {
                Debug.LogWarning("camrot was truned to null");
                sSave.CamRot.player = new int[5];
            }

            //TODO make this dynamic
            //sSave.PlayerCount = 4;
        }
    }
    


    private int[][] SetCamRotation()
    {
        int[][] r;
        r = new int[4][];
        for (int i = 0; i < 4; ++i)
            r[i] = new int[4];
        return r;
    }

    private void OnDestroy()
    {
        DataReadyToBeRead = new UnityEvent();
        SaveFile(fileName);
    }


    

    public void CloseStartMenu()
    {
        sSave.StartMenu.StartMenuOpened = false;
    }
    public void OpenStartMenu()
    {
        sSave.StartMenu.StartMenuOpened = true;
    }
    public void SetupStartMenu(ref bool OpenStartMenu)
    {
        //print("menuNotSaved" + save.StartMenu.MenuNotSaved);
        if (sSave.StartMenu.MenuNotSaved)
        {
            sSave.StartMenu.MenuNotSaved = false;
            isMenuOpened();
        }
        else
        {
            OpenStartMenu = isMenuOpened();
        }
        SendDataIfReady();
    }




    public void IncreaseScore(int PlayerNumber, int HowMuch)
    {
        if (WinText.text == "")
        {
            WinText.text = "Player " + (PlayerNumber + 1) + " Wins";
            sSave.ScoreKeeper.player[PlayerNumber] += HowMuch;
        }
        else if (WinText.text.Contains("" + (PlayerNumber + 1)))
        {
            sSave.ScoreKeeper.player[PlayerNumber] += HowMuch;
            WinText.text += "\ntwice?";
        }

        //print("score increased for " + PlayerNumber + " score: " + save.ScoreKeeper.player[PlayerNumber]);
    }
    public IEnumerable<int> GetScore()
    {
        if (sSave.ScoreKeeper.player == null)
            sSave.ScoreKeeper.player = new int[4];
        for(int i=0; i<4;++i)
            yield return sSave.ScoreKeeper.player[i];
    }





    public void EndRound()
    {
        sSave.RoundCounter++;
        if (sSave.Rounds == sSave.RoundCounter)
            sSave.StartMenu.StartMenuOpened = true;
        Debug.LogWarning("   Data - end round " + sSave.Rounds+" "+sSave.RoundCounter);
    }

    public bool isMenuOpened()
    {
        if (sSave.StartMenu.MenuOpenedNotSaved)
            sSave.StartMenu.StartMenuOpened = FindObjectOfType<StartMenu>().OpenMenuOnStart;
        sSave.StartMenu.MenuOpenedNotSaved = false;
        return sSave.StartMenu.StartMenuOpened;
    }

    private void SendDataIfReady()
    {
        if (!sSave.StartMenu.MenuNotSaved &&
            !sSave.StartMenu.LevelNotSaved)
            DataReadyToBeRead.Invoke();
    }

    public void SetUpLevel(ref int xDim, ref int yDim, ref bool FillLevel, ref int maxTiles,
        ref int WarnFrames, ref int closeFrames,
        ref int asteroidSpeed, ref int chuckAsteroidFrames, ref int PlusOrMinus)
    {
        if (sSave.StartMenu.LevelNotSaved)
        {
            //save.StartMenu.StartMenuOpened=FindObjectOfType<StartMenu>().OpenMenuOnStart;
            isMenuOpened();
            sSave.StartMenu.LevelNotSaved = false;

            //pass back saved values
            xDim = sSave.StartMenu.MapDemensionX;
            yDim = sSave.StartMenu.MapDemensionY;

            maxTiles = sSave.StartMenu.MaxTiles;
            FillLevel = sSave.StartMenu.FillLevel;

            //save play settings

            sSave.Play.WarnRoomFrames = WarnFrames;
            sSave.Play.CloseRoomFrames = closeFrames;

            sSave.Play.AsteroidSpeed = asteroidSpeed;
            sSave.Play.ChuckAsteroidEvery = chuckAsteroidFrames;
            sSave.Play.ChuckAsteroidPlusOrMinus = PlusOrMinus;
            

            //save menu settings
            sSave.StartMenu.MapDemensionX = MenuMapDemensionX;
            sSave.StartMenu.MapDemensionY = MenuMapDemensionY;

            sSave.StartMenu.MaxTiles = MenuMaxTileQuantity;
            sSave.StartMenu.FillLevel = MenuFillLevel;

            sSave.StartMenu.WarnRoomFrames = MenuCloseTileWarnFrames;
            sSave.StartMenu.CloseRoomFrames = MenuCloseTileXManyFrames;

            sSave.StartMenu.AsteroidSpeed = MenuChuckAsteroidSpeed;
            sSave.StartMenu.ChuckAsteroidEvery = MenuChuckAsteroidEvery;
            sSave.StartMenu.ChuckAsteroidPlusOrMinus = MenuChuckAsteroidPlusOrMinus;
        }

        //then load the appropriate level settings
        if (sSave.StartMenu.StartMenuOpened)
        {
            xDim = sSave.StartMenu.MapDemensionX;
            yDim = sSave.StartMenu.MapDemensionY;

            maxTiles = sSave.StartMenu.MaxTiles;
            FillLevel = sSave.StartMenu.FillLevel;

            WarnFrames = sSave.StartMenu.WarnRoomFrames;
            closeFrames = sSave.StartMenu.CloseRoomFrames;

            asteroidSpeed = sSave.StartMenu.AsteroidSpeed;
            chuckAsteroidFrames = sSave.StartMenu.ChuckAsteroidEvery;
            PlusOrMinus = sSave.StartMenu.ChuckAsteroidPlusOrMinus;
        }
        else
        {
            //print("start menu closed in LevelSetup");
            xDim = sSave.Play.MapDemensionX;
            yDim = sSave.Play.MapDemensionY;

            maxTiles = sSave.Play.MaxTiles;
            FillLevel = sSave.Play.FillLevel;

            WarnFrames = sSave.Play.WarnRoomFrames;
            closeFrames = sSave.Play.CloseRoomFrames;

            asteroidSpeed = sSave.Play.AsteroidSpeed;
            chuckAsteroidFrames = sSave.Play.ChuckAsteroidEvery;
            PlusOrMinus = sSave.Play.ChuckAsteroidPlusOrMinus;
        }
        SendDataIfReady();
    }

    public int GetRotation()
    {
        return sSave.CamRot.player[sSave.PlayerCount];
    }
    public void SetRotation(int val)
    {
        sSave.CamRot.player[sSave.PlayerCount] = val;
    }

    private static class sSave
    {
        public static class StartMenu
        {
            public static int MapDemensionX, MapDemensionY, MaxTiles, 
                ChuckEvery, CloseRoomFrames, WarnRoomFrames,
                AsteroidSpeed,ChuckAsteroidEvery,ChuckAsteroidPlusOrMinus;
            public static bool FillLevel;
            public static bool StartMenuOpened, LevelNotSaved = true, 
                MenuNotSaved = true, MenuOpenedNotSaved=true;
        }

        public static class ScoreKeeper
        {
            public static int[] player;
        }

        public static class Play
        {
            public static int MapDemensionX, MapDemensionY, MaxTiles,
                ChuckEvery, CloseRoomFrames, WarnRoomFrames,
                AsteroidSpeed, ChuckAsteroidEvery, ChuckAsteroidPlusOrMinus;
            public static bool FillLevel;
        }
        public static int RoundCounter, Rounds, PlayerCount;
        public static bool NotYetHappened = true, TouchScreenControls, LayerBlur = true;

        public static class CamRot
        {
            public static bool verticalSplit;
            public static int [] player;
        }
    }

    [System.Serializable]
    private class fSave
    {
        public int[] PlayerRot;
        public int PlayerCount;
        public bool ScreenSplitVertical = true,TouchScreenControls,LayerBlur;
        public int rounds,mapx,mapy,tileQuant;

        public override string ToString()
        {
            return " players: " + PlayerCount + ", TouchControls: " + TouchScreenControls + ", " + LayerBlur + ", " +
                "rounds: " + rounds + ", mapx: " + mapx + ", mapy: " + mapy + ", tiles: " + tileQuant; 
        }
    }

    public void SaveFile(string fileName)
    {
        string destination = Application.persistentDataPath + fileName;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        fSave data = new fSave();
        data.PlayerCount = sSave.PlayerCount;
        data.PlayerRot = sSave.CamRot.player;
        data.ScreenSplitVertical = sSave.CamRot.verticalSplit;

        data.rounds = sSave.Rounds;

        data.mapx=sSave.Play.MapDemensionX;
        data.mapy=sSave.Play.MapDemensionY;
        data.tileQuant=sSave.Play.MaxTiles;

        data.TouchScreenControls = sSave.TouchScreenControls;
        data.LayerBlur = sSave.LayerBlur;


        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadFile(string fileName)
    {
        try
        {
            string destination = Application.persistentDataPath + fileName;
            FileStream file;

            SetDefaults();
            if (File.Exists(destination)) file = File.OpenRead(destination);
            else
            {
                Debug.LogError("File not found");
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();
            fSave data = (fSave)bf.Deserialize(file);
            file.Close();

            sSave.PlayerCount = data.PlayerCount;
            sSave.CamRot.player = data.PlayerRot;
            sSave.CamRot.verticalSplit = data.ScreenSplitVertical;

            sSave.Rounds = data.rounds;

            sSave.Play.MapDemensionX = data.mapx;
            sSave.Play.MapDemensionY = data.mapy;
            sSave.Play.MaxTiles = data.tileQuant;

            sSave.Play.FillLevel = (sSave.Play.MaxTiles == sSave.Play.MapDemensionY * sSave.Play.MapDemensionX);

            sSave.TouchScreenControls = data.TouchScreenControls;
            sSave.LayerBlur = data.LayerBlur;

            print("read saved data "+data);
        }
        catch(System.Exception e)
        {
            Debug.LogError("Crashed while trying to load file");
            SetDefaults();
        }
    }


    private void SetDefaults()
    {
        sSave.PlayerCount = 4;
        sSave.CamRot.player = new int[5];

        sSave.CamRot.verticalSplit = true;

        sSave.Rounds = 10;

        sSave.Play.FillLevel = true;
        sSave.Play.MapDemensionX = 3;
        sSave.Play.MapDemensionY = 4;
        sSave.Play.MaxTiles = 12;

#if UNITY_ANDROID
        sSave.TouchScreenControls = true;
#else
        sSave.TouchScreenControls = false;
#endif
#if UNITY_STANDALONE_LINUX
        sSave.LayerBlur = false;
#else
        sSave.LayerBlur = true;
#endif
    }
}
