using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //视距
    public int viewRange = 30;

    private void Start()
    {

    }

    void Update()
    {
        for(float x = transform.position.z - Chunk.width * 3; x < transform.position.x + Chunk.width * 3; x += Chunk.width)
        {
            for(float y = transform.position.y - Chunk.height * 3; y < transform.position.y + Chunk.height * 1; y += Chunk.height)
            {
                if (y <= Chunk.height * 16 && y > 0)//Y轴允许最大16个Chunk 最高256
                {
                    for (float z = transform.position.z - Chunk.width * 3; z < transform.position.z + Chunk.width * 3; z += Chunk.width)
                    {
                        int xx = Chunk.width * Mathf.FloorToInt(x / Chunk.width);
                        int yy = Chunk.height * Mathf.FloorToInt(y / Chunk.height);
                        int zz = Chunk.width * Mathf.FloorToInt(z / Chunk.width);
                        if (!Map.instance.ChunkExisits(xx, yy, zz))
                        {
                            Map.instance.CreateChunk(new Vector3i(xx, yy, zz));
                        }
                    }
                }
            }
        }
    }
}
