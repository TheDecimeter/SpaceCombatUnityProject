using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour {
    public TextMeshProUGUI WinText;
    public UndestroyableData saveData;
    public float sceneLoadDelay = 3f;
    public CanvasGroup sceneFadeOverlay;
    public float sceneFadeDuration = 0f;

    private int [] scores=new int[4];

    public void Start()
    {
        WinText.text = "";
        int id = 0;
        foreach(int i in saveData.GetScore())
        {
            scores[id++] = i;
        }


        if (sceneFadeOverlay != null) StartCoroutine(FadeGameIn());
    }

    public void LoadScene (string scene)
    {
        StartCoroutine(LoadSceneDelay(scene));
        // loading in hud 
        //CanvasHud.SetActive(true);
    }

    public void RestartScene ()
    {
        int id = 0;
        foreach (int i in saveData.GetScore())
        {
            if (scores[id++] != i)
            {
                WinText.text = "Player " + id + "\nWins";
                break;
            }
        }

        saveData.EndRound();
        StartCoroutine(LoadSceneDelay(SceneManager.GetActiveScene().name));
    }

    IEnumerator FadeGameIn ()
    {
        sceneFadeOverlay.alpha = 1f;

        float timer = 0f;

        while (timer < 1f)
        {
            if (sceneFadeOverlay != null) sceneFadeOverlay.alpha = Mathf.Lerp(1f, 0f, timer);
            timer += Time.deltaTime / sceneFadeDuration;
            yield return null;
        }

        if (sceneFadeOverlay != null) sceneFadeOverlay.alpha = 0f;
    }

    IEnumerator LoadSceneDelay (string sceneName)
    {
        yield return new WaitForSeconds(sceneLoadDelay - sceneFadeDuration);

        float timer = 0f;

        while (timer < 1f)
        {
            if (sceneFadeOverlay != null) sceneFadeOverlay.alpha = Mathf.Lerp(0f, 1f, timer);
            timer += Time.deltaTime / sceneFadeDuration;
            yield return null;
        }

        if (sceneFadeOverlay != null) sceneFadeOverlay.alpha = 1f;

        SceneManager.LoadScene(sceneName);
    }
}
