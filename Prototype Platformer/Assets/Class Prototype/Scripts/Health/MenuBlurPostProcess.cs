using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBlurPostProcess : MonoBehaviour
{
    public Material BlendMat;
    public Material BlurMat;

    void Start()
    {
    }

    //void FixedUpdate()
    //{
    //    Rend(Source, Dest);
    //}

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //draws the pixels from the source texture to the destination texture
        var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, temporaryTexture, BlurMat, 0);
        Graphics.Blit(temporaryTexture, destination, BlurMat, 1);
        RenderTexture.ReleaseTemporary(temporaryTexture);
        
    }

    
    

    void Clear(RenderTexture destination)
    {
        RenderTexture.active = destination;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
    }
}
