using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlurTextures : MonoBehaviour
{
    public RenderTexture Source;
    public RenderTexture Dest;
    public Material BlendMat;
    public Material BlurMat;
    
    void Start()
    {
        BlendMat = Instantiate(BlendMat);
        BlendMat.SetTexture("_Texture1", Dest);
        BlendMat.SetTexture("_Texture2", Source);
        BlendMat.SetFloat("_Blend", .1f);
    }

    void FixedUpdate()
    {
        Rend(Source, Dest);
    }


    void Rend(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, BlendMat, -1);
    }

    void OnEnable()
    {
        Clear(Dest);
    }

    void Clear(RenderTexture destination)
    {
        RenderTexture.active = destination;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
    }
}
