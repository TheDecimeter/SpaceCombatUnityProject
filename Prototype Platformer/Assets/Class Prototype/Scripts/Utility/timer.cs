using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    public float time = 1;
    private float ymove = float.NaN;
    private float xmove = float.NaN;
    public int framesToMove=100;
    // Start is called before the first frame update
    void Start()
    {
        if (float.IsNaN(ymove))
        {
            if(Random.Range(0,2)==0)
                ymove = Random.Range(50, 80);
            else
                ymove = Random.Range(-80, -50);
        }
        if (float.IsNaN(xmove))
        {
            if (Random.Range(0, 2) == 0)
                xmove = Random.Range(50, 80);
            else
                xmove = Random.Range(-80, -50);

        }
        StartCoroutine(TimeDelay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator TimeDelay ()
    {
        yield return new WaitForSeconds(time);

        Color zm = GetComponent<Text>().color;
        while (framesToMove-- > 0)
        {
            zm.a-=.01f;
            transform.position = new Vector3(
                transform.position.x + xmove, transform.position.y + ymove, transform.position.z);
            yield return null;
        }

        //float timer = 0f;

        //float timer = 1f;
        //Color zm = GetComponent<Text>().color;

        //while (timer < 1f)
        //{
        //    zm.a = Mathf.Lerp(1f, 0f, timer);
        //    timer += Time.deltaTime/5;
        //    yield return null;
        //}
        gameObject.SetActive(false);

        //if (sceneFadeOverlay != null) sceneFadeOverlay.alpha = 1f;

        //SceneManager.LoadScene(sceneName);
    }
}
