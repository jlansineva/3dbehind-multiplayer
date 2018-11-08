using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgyMode : MonoBehaviour
{
    public Material shaderInstance;
    public Material mixWithDepth;
    public RenderTexture firstPassTexture; // Vertex lit render from child camera

    private void Awake()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        firstPassTexture.width = Screen.width;
        firstPassTexture.height = Screen.height;
        shaderInstance.SetFloat("_ScreenWidth", Screen.width);
        shaderInstance.SetFloat("_ScreenHeight", Screen.height);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        shaderInstance.SetTexture("_VertexLit", firstPassTexture);
        Graphics.Blit(source, destination, shaderInstance);
    }
}
    