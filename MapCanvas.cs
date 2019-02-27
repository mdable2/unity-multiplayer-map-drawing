using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCanvas : MonoBehaviour
{
    public static Texture2D map { get; private set; }

    public static byte[] GetAllTextureData()
    {
        return map.GetRawTextureData();
    }

    // Start is called before the first frame update
    void Start()
    {
        PrepareTemporaryTexture();
    }

    private void PrepareTemporaryTexture()
    {
        map = (Texture2D)GameObject.Instantiate(GetComponent<Renderer>().material.mainTexture);
        GetComponent<Renderer>().material.mainTexture = map;
    }

    internal static void SetAllTextureData(byte[] textureData)
    {
        map.LoadRawTextureData(textureData);
        map.Apply();
    }
}
