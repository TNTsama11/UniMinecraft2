using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;

public class Terrain : MonoBehaviour
{
    
    public static byte GetTerrainBlock(Vector3i worldPosition)
    {
        Perlin noise = new LibNoise.Generator.Perlin(1f, 1f, 1f, 8, GameManager.randomSeed, QualityMode.High);
        Random.InitState(GameManager.randomSeed);
        //因为柏林噪音在(0,0)点是上下左右对称的，所以我们设置一个很远很远的地方作为新的(0,0)点
        Vector3 offset = new Vector3(Random.value * 100000, Random.value * 100000, Random.value * 100000);
        float noiseX = Mathf.Abs((worldPosition.x + offset.x) / 20);
        float noiseY = Mathf.Abs((worldPosition.y + offset.y) / 20);
        float noiseZ = Mathf.Abs((worldPosition.z + offset.z) / 20);
        double noiseValue = noise.GetValue(noiseX, noiseY, noiseZ);

        noiseValue += (20 - worldPosition.y) / 15f;
        noiseValue /= worldPosition.y / 5f;

        if (noiseValue > 0.5f)
        {
            return 1;
        }

        return 0;
    }

}
