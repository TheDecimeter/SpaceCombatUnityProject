using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndestroyableData : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        if (save.ScoreKeeper.player == null)
            save.ScoreKeeper.player = new int[4];
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void HowManyRounds(int rounds)
    {
        save.RoundCounter = -1;
        save.Rounds = rounds;

        save.ScoreKeeper.player = new int[4];
    }
    public void CloseStartMenu()
    {
        save.StartMenu.StartMenuOpened = false;
    }
    public void OpenStartMenu()
    {
        save.StartMenu.StartMenuOpened = true;
    }
    public void SetupStartMenu(ref int RecommendedRounds, ref bool OpenStartMenu)
    {
        print("menuNotSaved" + save.StartMenu.MenuNotSaved);
        if (save.StartMenu.MenuNotSaved)
        {
            save.StartMenu.MenuNotSaved = false;
            save.Rounds = RecommendedRounds;
            save.StartMenu.StartMenuOpened = OpenStartMenu;
        }
        else
        {
            //print("menuSaved, should it be opened?" + save.StartMenu.MenuNotSaved);
            RecommendedRounds = save.Rounds;
            OpenStartMenu = save.StartMenu.StartMenuOpened;
        }
    }





    public void IncreaseScore(int PlayerNumber, int HowMuch)
    {
        save.ScoreKeeper.player[PlayerNumber] += HowMuch;
        print("score increased for " + PlayerNumber + " score: " + save.ScoreKeeper.player[PlayerNumber]);
    }
    public IEnumerable<int> GetScore()
    {
        if (save.ScoreKeeper.player == null)
            save.ScoreKeeper.player = new int[4];
        for(int i=0; i<4;++i)
            yield return save.ScoreKeeper.player[i];
    }





    public void EndRound()
    {
        save.RoundCounter++;
        if (save.Rounds == save.RoundCounter)
            save.StartMenu.StartMenuOpened = true;
    }

    public void SetUpLevel(ref int xDim, ref int yDim, ref bool FillLevel, ref int maxTiles,
        ref int WarnFrames, ref int closeFrames,
        ref int asteroidSpeed, ref int chuckAsteroidFrames, ref int PlusOrMinus)
    {
        if (save.StartMenu.LevelNotSaved)
        {
            save.StartMenu.LevelNotSaved = false;

            //save play settings
            save.Play.MapDemensionX = xDim;
            save.Play.MapDemensionY = yDim;

            save.Play.MaxTiles = maxTiles;
            save.Play.FillLevel = FillLevel;

            save.Play.WarnRoomFrames = WarnFrames;
            save.Play.CloseRoomFrames = closeFrames;

            save.Play.AsteroidSpeed = asteroidSpeed;
            save.Play.ChuckAsteroidEvery = chuckAsteroidFrames;
            save.Play.ChuckAsteroidPlusOrMinus = PlusOrMinus;
            

            //save menu settings
            save.StartMenu.MapDemensionX = MenuMapDemensionX;
            save.StartMenu.MapDemensionY = MenuMapDemensionY;

            save.StartMenu.MaxTiles = MenuMaxTileQuantity;
            save.StartMenu.FillLevel = MenuFillLevel;

            save.StartMenu.WarnRoomFrames = MenuCloseTileWarnFrames;
            save.StartMenu.CloseRoomFrames = MenuCloseTileXManyFrames;

            save.StartMenu.AsteroidSpeed = MenuChuckAsteroidSpeed;
            save.StartMenu.ChuckAsteroidEvery = MenuChuckAsteroidEvery;
            save.StartMenu.ChuckAsteroidPlusOrMinus = MenuChuckAsteroidPlusOrMinus;
        }

        //then load the appropriate level settings
        if (save.StartMenu.StartMenuOpened)
        {
            xDim = save.StartMenu.MapDemensionX;
            yDim = save.StartMenu.MapDemensionY;

            maxTiles = save.StartMenu.MaxTiles;
            FillLevel = save.StartMenu.FillLevel;

            WarnFrames = save.StartMenu.WarnRoomFrames;
            closeFrames = save.StartMenu.CloseRoomFrames;

            asteroidSpeed = save.StartMenu.AsteroidSpeed;
            chuckAsteroidFrames = save.StartMenu.ChuckAsteroidEvery;
            PlusOrMinus = save.StartMenu.ChuckAsteroidPlusOrMinus;
        }
        else
        {
            xDim = save.Play.MapDemensionX;
            yDim = save.Play.MapDemensionY;

            maxTiles = save.Play.MaxTiles;
            FillLevel = save.Play.FillLevel;

            WarnFrames = save.Play.WarnRoomFrames;
            closeFrames = save.Play.CloseRoomFrames;

            asteroidSpeed = save.Play.AsteroidSpeed;
            chuckAsteroidFrames = save.Play.ChuckAsteroidEvery;
            PlusOrMinus = save.Play.ChuckAsteroidPlusOrMinus;
        }
        
    }

    private static class save
    {
        public static class StartMenu
        {
            public static int MapDemensionX, MapDemensionY, MaxTiles, 
                ChuckEvery, CloseRoomFrames, WarnRoomFrames,
                AsteroidSpeed,ChuckAsteroidEvery,ChuckAsteroidPlusOrMinus;
            public static bool FillLevel;
            public static bool StartMenuOpened, LevelNotSaved = true, MenuNotSaved = true;
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
        public static int RoundCounter, Rounds;
        public static bool NotYetHappened=true;
    }
}
