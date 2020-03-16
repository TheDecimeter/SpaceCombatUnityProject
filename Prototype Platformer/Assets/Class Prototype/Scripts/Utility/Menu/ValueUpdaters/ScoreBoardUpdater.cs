using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoardUpdater : MonoBehaviour
{
    public TextMeshProUGUI[] HumanScore;
    public TextMeshProUGUI[] AIScore;


    private string human = "  Player {0}<pos=42%>:<pos=53.5%>{1}",bot= "*Player_{0}*<pos=50%>*{1}*";

    void Start()
    {
        
    }

    private void SetString(int index, int score, bool human)
    {
        if(human)
        {

            string scoreStr = "" + score;
            if (score < 10)
                scoreStr = "  " + score;
            else if (score < 100)
                scoreStr += " " + score;

            HumanScore[index].text = string.Format(this.human, index + 1, scoreStr);
            AIScore[index].text = "";
        }
        else
        {
            string scoreStr = ""+score;
            if (score < 10)
                scoreStr = "_"+score+"_";
            else if (score < 100)
                scoreStr += "_" + score;

            AIScore[index].text = string.Format(bot, index + 1, scoreStr);
            HumanScore[index].text = "";
        }
    }

    public void UpdateScoreboard(GameObject playerArray, IEnumerable<int> scores)
    {
        int []score = new int[4];
        int index = 0;
        int totalScore = 0;
        foreach (int s in scores)
        {
            score[index++] = s;
            totalScore += s;
        }

        if (totalScore == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        index = 0;
        foreach (Transform child in playerArray.transform)
        {
            AI ai = child.GetComponent<AI>();
            SetString(index, score[index], !(ai && ai.enabled));
            index++;
        }

    }
    
}
