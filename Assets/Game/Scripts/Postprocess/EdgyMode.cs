using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgyMode : MonoBehaviour
{
    public Material shaderInstance;
    public Material mixWithDepth;
    private RenderTexture firstPassTexture; // Vertex lit render from child camera
    private Camera firstPassCamera;

    private void Awake()
    {
        firstPassCamera = transform.Find("FirstPassCamera").GetComponent<Camera>();
        firstPassTexture = new RenderTexture(Screen.width, Screen.height, 1);
        firstPassCamera.targetTexture = firstPassTexture;

        UpdateProperties();
    }

    private void Start()
    {
        UpdateProperties();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        shaderInstance.SetTexture("_VertexLit", firstPassTexture);
        Graphics.Blit(source, destination, shaderInstance);
    }

    private void UpdateProperties()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        shaderInstance.SetFloat("_ScreenWidth", Screen.width);
        shaderInstance.SetFloat("_ScreenHeight", Screen.height);
    }
}
    