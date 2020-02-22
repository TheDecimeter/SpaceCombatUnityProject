using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    public GameObject PlayerArray;
    public GameObject Menu;
    public Text Text;
    public Text ScoreBoard;
    public UndestroyableData savedData;
    public SceneLoader sceneLoader;

    public bool OpenMenuOnStart = true;
    public int SuggestedRounds = 10;
    public int MaxRounds = 50;
    //private ControlStruct _controllerStatus;

    // Start is called before the first frame update
    void Start()
    {
        savedData.SetupStartMenu(ref SuggestedRounds, ref OpenMenuOnStart);
        Menu.SetActive(OpenMenuOnStart);
        if (OpenMenuOnStart)
            SetupMenu();
    }
    

    private void SetupMenu()
    {
        PlayerArray.transform.position = new Vector3(-111, -111, -111);
        foreach (Transform child in PlayerArray.transform)
            child.gameObject.SetActive(false);
        UpdateScoreBoard();
        UpdateText();
    }
    private void UpdateScoreBoard()
    {
        bool displayScore = false;
        foreach (int i in savedData.GetScore())
            if (i > 0)
            {
                displayScore = true;
                break;
            }
        int index = 0;
        if (displayScore)
        {
            ScoreBoard.text = "Score: \n";
            foreach (int i in savedData.GetScore())
                ScoreBoard.text += "Player " + (++index) + " Score: " + i + "\n";
        }
        else
            ScoreBoard.text = "";
    }

    private void UpdateText()
    {
        Text.text = "Rounds: " + SuggestedRounds;
    }

    public void ControllerListener(ControlStruct controls)
    {
        if (!OpenMenuOnStart)
            return;
        if (controls.moveLeft < -.1)
        {
            SuggestedRounds--;
            if (SuggestedRounds < 1)
                SuggestedRounds = MaxRounds;
            UpdateText();
        }
        if (controls.moveLeft > .1)
        {
            SuggestedRounds++;
            if (SuggestedRounds >MaxRounds)
                SuggestedRounds = 1;
            UpdateText();
        }
        if (controls.jump)
        {
            //turn off menu
            //reload scene
            savedData.HowManyRounds(SuggestedRounds);
            savedData.CloseStartMenu();
            sceneLoader.sceneLoadDelay = 0;
            sceneLoader.sceneFadeDuration = 0;
            sceneLoader.RestartScene();
        }
        if (controls.door)
        {
            //exit
            Application.Quit();
        }
    }
}
