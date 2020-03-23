using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    public GameObject PlayerArray;
    public GameObject Menu;
    //public Text ScoreBoard;
    public ScoreBoardUpdater ScoreBoard;
    public UndestroyableData savedData;
    public SceneLoader sceneLoader;

    public bool OpenMenuOnStart = true;
    //private ControlStruct _controllerStatus;

    // Start is called before the first frame update
    void Start()
    {
        savedData.SetupStartMenu( ref OpenMenuOnStart);
        Menu.SetActive(OpenMenuOnStart);
        if (OpenMenuOnStart)
            SetupMenu();
    }
    

    private void SetupMenu()
    {
        PlayerArray.transform.position = new Vector3(-111, -111, -111);
        foreach (Transform child in PlayerArray.transform)
            child.gameObject.SetActive(false);
        ScoreBoard.UpdateScoreboard(PlayerArray, savedData.GetScore());
        UpdateText();
    }
    private void UpdateScoreBoard()
    {
        //bool displayScore = false;
        //foreach (int i in savedData.GetScore())
        //    if (i > 0)
        //    {
        //        displayScore = true;
        //        break;
        //    }
        //int index = 0;
        //if (displayScore)
        //{
        //    ScoreBoard.text = "Score: \n";
        //    foreach (int i in savedData.GetScore())
        //        ScoreBoard.text += "Player " + (++index) + " Score: " + i + "\n";
        //}
        //else
        //    ScoreBoard.text = "";
    }

    private void UpdateText()
    {
    }

    
    private int GetRounds(UndestroyableData data)
    {
        int r = 0;
        data.GetRoundsE((x) => { r = x; });
        return r;
    }

    public void StartGame()
    {
        //savedData.SetRounds(GetRounds(savedData));

        savedData.SetRounds(ValidateVal(savedData.GetRoundsE, savedData.SetRounds, 1, 50));

        savedData.SetRounds(ValidateVal(savedData.GetPlayers, savedData.SetPlayers, 0, 4));

        savedData.SetRounds(ValidateVal(savedData.GetMapTileCount, savedData.SetMapTileCount, 6, 100));
        savedData.SetRounds(ValidateVal(savedData.GetMapHeight, savedData.SetMapHeight, 3, 10));
        savedData.SetRounds(ValidateVal(savedData.GetMapWidth, savedData.SetMapWidth, 3, 10));


        savedData.CloseStartMenu();
        sceneLoader.sceneLoadDelay = 0;
        sceneLoader.sceneFadeDuration = 0;
        sceneLoader.RestartScene();
    }
    public void QuitProgram()
    {
        print("application quit");
        Application.Quit();
    }

    delegate void Get(IntUpdater.Get get);
    delegate void Set(int val);
    private int ValidateVal(Get GetValue, Set SetValue, int min, int max)
    {
        int val = 0;
        GetValue((x) => { val = x; });

        val = Mathf.Clamp(val, min, max);

        SetValue(val);
        GetValue((x) => { val = x; });
        return val;
    }
}
