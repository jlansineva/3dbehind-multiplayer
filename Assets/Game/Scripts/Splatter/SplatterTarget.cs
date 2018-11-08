using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatterTarget : MonoBehaviour
{
    public Color color = new Color(1, 1, 1);

    [Range(2, 12)]
    public int detail = 4;
    public float size = 1;

    private Texture2D texture;
    private float radiusMultiplier;
    private bool dirty = false;

    public int pow2Size
    {
        get
        {
            return Mathf.ClosestPowerOfTwo((int)Mathf.Pow(2, detail));
        }
    }

    void Start ()
    {
        texture = new Texture2D(pow2Size, pow2Size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Trilinear;

        var fillColor = color;
        var fillColorArray = texture.GetPixels();

        for (var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = fillColor;
        }

        texture.SetPixels(fillColorArray);

        texture.Apply();

        // Assign texture to renderer's material.
        GetComponent<Renderer>().material.mainTexture = texture;

        radiusMultiplier = (pow2Size / size) * 0.5f;
    }

    private void Update()
    {
        if (dirty)
        {
            texture.Apply();
            dirty = false;
        }
    }

    public void DrawCircle(Vector3 point, Vector3 normal, float baseRadius, Color color)
    {
        var collider = GetComponent<MeshCollider>();
        var ray = new Ray(point + normal * 2, -normal);

        RaycastHit hit;
        if (!collider.Raycast(ray, out hit, 5f))
            return;

        DrawCircle(hit.textureCoord, baseRadius, color);
    }

    public void DrawCircle(Vector2 uv, float radius, Color color)
    {
        radius *= radiusMultiplier;

        //Debug.Log(radius + " " + uv.x + " " + uv.y + " " + gameObject.name);

        int halfRadius = Mathf.RoundToInt(radius + 0.5f);
        var texSpacePos = new Vector2(texture.width * uv.x, texture.height * uv.y);

        int pointX = Mathf.RoundToInt(texSpacePos.x);
        int sX = Mathf.Max(pointX - halfRadius, 0);
        int eX = Mathf.Min(pointX + halfRadius, texture.width);

        int pointY = Mathf.RoundToInt(texSpacePos.y);
        int sY = Mathf.Max(pointY - halfRadius, 0);
        int eY = Mathf.Min(pointY + halfRadius, texture.height);

        float radiusSquared = radius * radius;

        for (int y = sY; y < eY; y++)
        {
            for (int x = sX; x < eX; x++)
            {
                var diffSqr = (texSpacePos - new Vector2(x, y)).sqrMagnitude - radiusSquared;

                if (diffSqr <= 0)
                    texture.SetPixel(x, y, color);
                else if (diffSqr <= 2)
                {
                    var originalColor = texture.GetPixel(x, y);
                    var newColor = Color.Lerp(originalColor, color, (2 - diffSqr) * 0.5f);
                    newColor.a = 1;
                    texture.SetPixel(x, y, newColor);
                }
            }
        }

        dirty = true;
    }
}
