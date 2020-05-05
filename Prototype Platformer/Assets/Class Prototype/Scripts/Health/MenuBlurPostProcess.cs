using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBlurPostProcess : MonoBehaviour
{
    public Material BlendMat;
    public Material BlurMat;

    private RenderTexture cache;

    void Start()
    {
        BlendMat = Instantiate(BlendMat);
        BlendMat.SetFloat("_Blend", .1f);
    }

    //void FixedUpdate()
    //{
    //    Rend(Source, Dest);
    //}

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //draws the pixels from the source texture to the destination texture

        RenderTexture temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, temporaryTexture, BlurMat, 0);
        Graphics.Blit(temporaryTexture, destination, BlurMat, 1);
        RenderTexture.ReleaseTemporary(temporaryTexture);

        //if (cache == null)
        //{
        //    print("resetting cache");
        //    cache = new RenderTexture(source);
        //}

        //Stack(source,cache);
        //Rend(source, cache);
        //Stack(cache,source);
        //Rend(source, destination);
    }
    void Stack(RenderTexture one, RenderTexture other)
    {
        BlendMat = Instantiate(BlendMat);
        BlendMat.SetTexture("_Texture1", one);
        BlendMat.SetTexture("_Texture2", other);
    }
    void Rend(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, BlendMat, -1);
    }




    void Clear(RenderTexture destination)
    {
        RenderTexture.active = destination;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
    }
}
