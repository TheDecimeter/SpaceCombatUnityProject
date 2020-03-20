using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageStacker : MonoBehaviour
{
    // Grab the camera's view when this variable is true.
    bool grab=true;
    public RenderTexture outputCamRenderTexture;
    public Camera outputCam;

    // The "m_Display" is the GameObject whose Texture will be set to the captured image.
    public Renderer m_Display;

    //private void Update()
    //{
    //    //Press space to start the screen grab
    //    if (Input.GetKeyDown(KeyCode.Space))
    //        grab = true;
    //}

    private void OnPostRender()
    {
        //if (grab)
        //{
        //    //Create a new texture with the width and height of the screen
        //    outputCam = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //    //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
        //    texture.ReadPixels( new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        //    texture.Apply();
        //    //Check that the display field has been assigned in the Inspector
        //    if (m_Display != null)
        //        //Give your GameObject with the renderer this texture
        //        m_Display.material.mainTexture = texture;
        //    //Reset the grab state
        //}
        
        //outputCam.targetTexture = outputCamRenderTexture;
        RenderTexture.active = outputCamRenderTexture;
        outputCam.Render();
        Texture2D tempResidualTex=new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tempResidualTex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tempResidualTex.Apply();

        m_Display.material.mainTexture = tempResidualTex;

        RenderTexture.active = null;
        outputCam.targetTexture = null;

    }
}
