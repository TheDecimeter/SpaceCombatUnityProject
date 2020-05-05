using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBlurPostProcess : MonoBehaviour
{
    public Material HardBlendMat;
    public Material Blend2Mat;
    public Material BlendMat;
    public Material BlurMat;

    public RenderTexture cache;

    void Start()
    {
        HardBlendMat = Instantiate(HardBlendMat);
        BlendMat = Instantiate(BlendMat);
        Blend2Mat = Instantiate(Blend2Mat);


        HardBlendMat.SetFloat("_Intensity", .1f);

        Blend2Mat.SetFloat("_Blend", .1f);
        BlendMat.SetFloat("_Blend", .1f);
        cache = null;
    }

    //void FixedUpdate()
    //{
    //    Rend(Source, Dest);
    //}

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //draws the pixels from the source texture to the destination texture

        //RenderTexture temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        //Graphics.Blit(source, temporaryTexture, BlurMat, 0);
        //Graphics.Blit(temporaryTexture, destination, BlurMat, 1);
        //RenderTexture.ReleaseTemporary(temporaryTexture);

        //if (cache == null)
        //{
        //    print("resetting cache");
        //    cache = new RenderTexture(source);
        //}

        //Stack(source, cache);
        //Rend(source, cache);
        //Stack(cache, source);
        //Rend(source, destination);

        RenderTexture temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        if (cache == null)
        {
            print("resetting cache");
            cache = new RenderTexture(source.width, source.height, 16, RenderTextureFormat.RGB111110Float);
            cache.Create();
            Graphics.Blit(source, temporaryTexture, BlurMat, 0);
            Graphics.Blit(temporaryTexture, cache, BlurMat, 1);
            //Graphics.Blit(source, cache);
            HardBlendMat.SetTexture("_BTex", cache);
        }
        //Combine(cache);
        //Rend2(source, cache);
        //Graphics.Blit(cache, destination);
        //Stack(cache, source);
        //Rend(source, cache);
        //Graphics.Blit(cache, destination);

        Graphics.Blit(source, temporaryTexture, HardBlendMat);
        Graphics.Blit(temporaryTexture, cache);
        Graphics.Blit(temporaryTexture, destination);
        RenderTexture.ReleaseTemporary(temporaryTexture);
    }

    private void OnDisable()
    {
        cache = null;
    }

    void Stack(RenderTexture Dest, RenderTexture Source)
    {
        BlendMat.SetTexture("_Texture1", Dest);
        BlendMat.SetTexture("_Texture2", Source);
    }
    void Rend(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, BlendMat, -1);
    }

    void Combine(RenderTexture c2)
    {
        Blend2Mat.SetTexture("_Texture2", c2);
    }
    void Rend2(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, Blend2Mat, -1);
    }


    void OnDestroy()
    {
        //RenderTexture.ReleaseTemporary(cache);
    }

    void Clear(RenderTexture destination)
    {
        RenderTexture.active = destination;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
    }
}
