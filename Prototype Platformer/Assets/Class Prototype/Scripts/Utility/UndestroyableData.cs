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

    static public UnityEvent DataReadyToBeRead=new UnityEvent();

    // Start is called before the first frame update
    void Awake()
    {

        WinText.text = "";
        if (sSave.ScoreKeeper.player == null)
        {

            sSave.ScoreKeeper.player = new int[4];
            sSave.CamRot.player = new int[4];
            LoadFile(fileName);
            if (sSave.CamRot.player == null)
            {
                Debug.LogWarning("camrot was truned to null");
                sSave.CamRot.player = new int[4];
            }

            //TODO make this dynamic
            sSave.PlayerCount = 4;
        }
    }
    

    public void AddDataReadEvent(UnityAction listener)
    {
        DataReadyToBeRead.AddListener(listener);
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


    public void SetRounds(int rounds)
    {
        sSave.RoundCounter = -1;
        sSave.Rounds = rounds;

        sSave.ScoreKeeper.player = new int[4];
    }

    public int GetRounds()
    {
        return sSave.Rounds;
    }

    public void CloseStartMenu()
    {
        sSave.StartMenu.StartMenuOpened = false;
    }
    public void OpenStartMenu()
    {
        sSave.StartMenu.StartMenuOpened = true;
    }
    public void SetupStartMenu(ref int RecommendedRounds, ref bool OpenStartMenu)
    {
        //print("menuNotSaved" + save.StartMenu.MenuNotSaved);
        if (sSave.StartMenu.MenuNotSaved)
        {
            sSave.StartMenu.MenuNotSaved = false;
            sSave.Rounds = RecommendedRounds;
            isMenuOpened();
        }
        else
        {
            //print("menuSaved, should it be opened?" + save.StartMenu.MenuNotSaved);
            RecommendedRounds = sSave.Rounds;
            OpenStartMenu = isMenuOpened();
        }
        SendDataIfReady();
    }




    public void IncreaseScore(int PlayerNumber, int HowMuch)
    {
        if (WinText.text == "")
            WinText.text = "Player " + (PlayerNumber + 1) + " Wins";
        else
            WinText.text += "\ntwice?";

        sSave.ScoreKeeper.player[PlayerNumber] += HowMuch;
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

            //save play settings
            sSave.Play.MapDemensionX = xDim;
            sSave.Play.MapDemensionY = yDim;

            sSave.Play.MaxTiles = maxTiles;
            sSave.Play.FillLevel = FillLevel;

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
        return sSave.CamRot.player[sSave.PlayerCount-1];
    }
    public void SetRotation(int val)
    {
        sSave.CamRot.player[sSave.PlayerCount-1] = val;
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
        public static bool NotYetHappened=true;

        public static class CamRot
        {
            public static int [] player;
        }
    }

    [System.Serializable]
    private class fSave
    {
        public int[] PlayerRot;
        public int PlayerCount;

        public fSave(int [] playerRot, int playerCount)
        {
            this.PlayerCount = playerCount;
            this.PlayerRot = playerRot;
        }
    }

    public void SaveFile(string fileName)
    {
        string destination = Application.persistentDataPath + fileName;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        fSave data = new fSave(sSave.CamRot.player,sSave.PlayerCount);
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

            if (File.Exists(destination)) file = File.OpenRead(destination);
            else
            {
                Debug.LogError("File not found");
                SetDefaults();
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();
            fSave data = (fSave)bf.Deserialize(file);
            file.Close();

            sSave.PlayerCount = data.PlayerCount;
            sSave.CamRot.player = data.PlayerRot;
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
        sSave.CamRot.player = new int[4];
    }
}
