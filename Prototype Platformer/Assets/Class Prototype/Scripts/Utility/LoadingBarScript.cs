using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBarScript : MonoBehaviour
{
    public string LoadingSceneName;
    public Slider sliderBar;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadNewScene());
    }
    
    IEnumerator LoadNewScene()
    {
        
        AsyncOperation async = SceneManager.LoadSceneAsync(LoadingSceneName);
        
        while (!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / 0.9f);
            sliderBar.value = progress;
            yield return null;

        }

    }

}