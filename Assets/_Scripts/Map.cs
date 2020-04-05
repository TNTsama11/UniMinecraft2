using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    public static Map instance;
    public static GameObject chunkPrefab;
    public Dictionary<Vector3i, GameObject> chunks = new Dictionary<Vector3i, GameObject>();
    //是否正在生成
    private bool isSpawningChunk = false;

    void Awake()
    {
        instance = this;
        chunkPrefab = Resources.Load("Prefabs/Chunk") as GameObject;
    }


    void Update()
    {
        
    }
    //创建Chunk
    public void CreateChunk(Vector3i pos)
    {
        if (isSpawningChunk)
        {
            return;
        }
        StartCoroutine(SpawnChunk(pos));
    }

    public IEnumerator SpawnChunk(Vector3i pos)
    {
        isSpawningChunk = true;
        Instantiate(chunkPrefab, pos, Quaternion.identity);
        yield return null;
        isSpawningChunk = false;
    }
    //通过坐标来判断Chunk是否存在
    public bool ChunkExisits(Vector3i worldPosition)
    {
        return this.ChunkExisits(worldPosition.x, worldPosition.y, worldPosition.z);
    }
    public bool ChunkExisits(int x, int y,int z)
    {
        return chunks.ContainsKey(new Vector3i(x, y, z));
    }
}
