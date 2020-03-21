using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlurTextures : MonoBehaviour
{
    public RenderTexture Source;
    public RenderTexture Dest;
    public Material BlendMat;

    private Material mat;
    void Start()
    {
        //BlendMat = GetComponent<RawImage>().material;
        //BlendMat.SetFloat("_Blend", .5f);
        //BlendMat.SetTexture("_MainTex", Source);


        mat = BlendMat;//new Material(Shader.Find("Blur"));
    }

    //void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    //draws the pixels from the source texture to the destination texture
    //    var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
    //    Graphics.Blit(source, temporaryTexture, mat, 0);
    //    Graphics.Blit(temporaryTexture, destination, mat, 1);
    //    RenderTexture.ReleaseTemporary(temporaryTexture);
    //}

    void LateUpdate()
    {
        //RenderTexture buffer = RenderTexture.GetTemporary(Source.width, Source.height, 24);
        //Graphics.Blit(Source, buffer, BlendMat);
        //Graphics.Blit(buffer, Dest);
        //RenderTexture.ReleaseTemporary(buffer);
        ////Dest = (RenderTexture)BlendMat.GetTexture("_MainTex");

        //Dest = Blur(Source, 10);
        Rend(Source, Dest);
    }

    //RenderTexture Blur(RenderTexture source, int iterations)
    //{
    //    RenderTexture rt = source;
    //    RenderTexture blit = RenderTexture.GetTemporary(Source.width, Source.height);
    //    for (int i = 0; i < iterations; i++)
    //    {
    //        Graphics.SetRenderTarget(blit);
    //        GL.Clear(true, true, Color.black);
    //        Graphics.Blit(rt, blit, mat);
    //        Graphics.SetRenderTarget(rt);
    //        GL.Clear(true, true, Color.black);
    //        Graphics.Blit(blit, rt, mat);
    //    }
    //    RenderTexture.ReleaseTemporary(blit);
    //    return rt;
    //}


    void Rend(RenderTexture source, RenderTexture destination)
    {
        //draws the pixels from the source texture to the destination texture
        //var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        mat.SetTexture("_Texture1", source);
        mat.SetTexture("_Texture2", destination);
        mat.SetFloat("_Blend", .9f);
        Graphics.Blit(source, destination, mat, -1);
        //Graphics.Blit(temporaryTexture, destination, mat, -1);
        //Graphics.Blit(temporaryTexture, destination);
        //RenderTexture.ReleaseTemporary(temporaryTexture);
    }

    //big blur
    //void Rend(RenderTexture source, RenderTexture destination)
    //{
    //    //draws the pixels from the source texture to the destination texture
    //    var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
    //    Graphics.Blit(source, temporaryTexture, mat, -1);
    //    Graphics.Blit(temporaryTexture, destination, mat, 1);
    //    //Graphics.Blit(temporaryTexture, destination);
    //    RenderTexture.ReleaseTemporary(temporaryTexture);
    //}
    //void Rend(RenderTexture source, RenderTexture destination)
    //{
    //    var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
    //    Graphics.Blit(source, temporaryTexture, mat, -1);
    //    //Graphics.Blit(temporaryTexture, destination, mat, 1);
    //    Graphics.Blit(temporaryTexture, destination);
    //    RenderTexture.ReleaseTemporary(temporaryTexture);
    //}
}
