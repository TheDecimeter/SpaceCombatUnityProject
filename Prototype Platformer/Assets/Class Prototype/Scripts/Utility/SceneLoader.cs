using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour {
    public UndestroyableData saveData;
    public float sceneLoadDelay = 3f;
    public CanvasGroup sceneFadeOverlay;
    public float sceneFadeDuration = 0f;
    public static bool Loading { get; protected set; }

    public void Start()
    {
        Loading = false;

        //if (sceneFadeOverlay != null) StartCoroutine(FadeGameIn());
    }

    public void LoadScene (string scene)
    {
        StartCoroutine(LoadSceneDelay(scene));
        // loading in hud 
        //CanvasHud.SetActive(true);
    }

    public void RestartScene ()
    {
        saveData.EndRound();
        StartCoroutine(LoadSceneDelay(SceneManager.GetActiveScene().name));
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
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
        if (!Loading)
        {
            //Debug.LogWarning("Loading scene");
            Loading = true;
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            asyncLoad.allowSceneActivation = false;

            yield return new WaitForSeconds(sceneLoadDelay - sceneFadeDuration);
            if (sceneFadeOverlay != null)
            {
                sceneFadeOverlay.gameObject.SetActive(true);
                sceneFadeOverlay.alpha = 0;
            }

            float timer = 0f;

            while (timer < 1f)
            {
                if (sceneFadeOverlay != null) sceneFadeOverlay.alpha = Mathf.Lerp(0f, 1f, timer);
                timer += Time.deltaTime / sceneFadeDuration;
                yield return null;
            }

            if (sceneFadeOverlay != null) sceneFadeOverlay.alpha = 1f;
            //Debug.LogWarning("awaiting load");

            asyncLoad.allowSceneActivation = true;
            //SceneManager.LoadScene(sceneName);
        }
        else
        {
            //Debug.LogError("Duplicate loading not allowed");
        }
    }
}
