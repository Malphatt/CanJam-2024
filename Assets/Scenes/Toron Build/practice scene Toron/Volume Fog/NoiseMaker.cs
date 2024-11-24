using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    public Material fogMat;
    static Texture3D texture;
    public int size = 32, height = 16;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Noise.Seed = (int)Stopwatch.GetTimestamp();
            TextureFormat format = TextureFormat.RFloat;
            TextureWrapMode wrapMode = TextureWrapMode.Clamp;

            // Create the texture and apply the configuration
            if (texture == null)
                texture = new Texture3D(size, height, size, format, false);
            texture.wrapMode = wrapMode;

            fogMat.SetTexture("_Noise", texture);

            CreateTexture();
        }
    }

    public float noiseSize = 1;
    void CreateTexture()
    {
        // Create a 3-dimensional array to store color data
        Color[] colors = new Color[size * size * height];

        int[] scales = new int[] { 1, 2, 4, 8 };

        float[][,,] noise = new float[4][,,];
        Parallel.For(0, 4, (int i) =>
        {
            noise[i] = Noise.Calc3D(size, height, size, noiseSize * scales[i]);
        });

        for (int z = 0; z < size; z++)
        {
            int zOffset = z * size * height;
            for (int y = 0; y < height; y++)
            {
                int yOffset = y * size;
                for (int x = 0; x < size; x++)
                {
                    float r = noise[0][z, y, x] / 255;
                    float g = noise[1][z, y, x] / 255;
                    float b = noise[2][z, y, x] / 255;
                    float a = noise[3][z, y, x] / 255;
                    float all = Mathf.Clamp01((r + g * 0.5f + b * 0.25f + a * 0.125f) / 1.8f);
                    colors[x + yOffset + zOffset] = new Color(all, 0, 0, 0);
                }
            }
        }
        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();
    }


    [MenuItem("Create/3D Texture")]
    static void CreateTexture3D()
    {
        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(texture, "Assets/3DTexture.asset");
    }
}
