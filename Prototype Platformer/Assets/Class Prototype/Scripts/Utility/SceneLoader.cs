﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class SceneLoader : MonoBehaviour {
    public GameObject CanvasHud;
    public float sceneLoadDelay = 3f;
    public CanvasGroup sceneFadeOverlay;
    public float sceneFadeDuration = 0f;

    public void Start()
    {
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
        FindObjectOfType<UndestroyableData>().EndRound();
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
