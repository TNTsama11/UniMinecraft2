using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{
    public static int width = 16;
    public static int height = 16;

    public byte[,,] blocks;
    public Vector3i position;

    private Mesh mesh;
    //四角面的顶点
    private List<Vector3> vertices = new List<Vector3>();
    //三角面顶点的索引
    private List<int> triangles = new List<int>();
    //所有的UV信息
    private List<Vector2> uv = new List<Vector2>();
    //UV贴图每行每列的宽度（0-1
    public static float textureOffset = 1 / 32f;
    //让UV缩小一点，避免过界
    public static float shrinkSize = 0.001f;

    //当前的Chunk是否正在生成
    private bool isWorking = false;
    private bool isFinished = false;

    void Start()
    {
        position = new Vector3i(this.transform.position);
        if (Map.instance.ChunkExisits(position))
        {
            Debug.Log("次方块已存在" + position);
            Destroy(this.gameObject);
        }
        else
        {
            Map.instance.chunks.Add(position, this.gameObject);
            this.name= "Chunk(" + position.x + "," + position.y + "," + position.z + ")";
           // StartFunction();
        }

        //mesh = new Mesh();

        //AddFrontFace();
        //AddBackFace();
        //AddRightFace();
        //AddLeftFace();
        //AddTopFace();
        //AddBottomFace();

        //mesh.vertices = vertices.ToArray();
        //mesh.triangles = triangles.ToArray();
        ////重新计算边界和法线    
        //mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
        //GetComponent<MeshFilter>().mesh = mesh;

    }

    private void Update()
    {
        if (isWorking == false && isFinished == false)
        {
            isFinished = true;
            StartFunction();
        }
    }

    private void StartFunction()
    {
        isWorking = true;
        mesh = new Mesh();
        mesh.name = "Chunk";
        StartCoroutine(CreateMap());
    }

    IEnumerator CreateMap()
    {
        blocks = new byte[width, height, width];
        for(int x = 0; x < Chunk.width; x++)
        {
            for(int y = 0; y < Chunk.width; y++)
            {
                for(int z = 0; z < Chunk.width; z++)
                {
                    byte blockId = Terrain.GetTerrainBlock(new Vector3i(x, y, z) + position);
                    if(blockId==1&&Terrain.GetTerrainBlock(new Vector3i(x, y + 1, z) + position) == 0)
                    {
                        blocks[x, y, z] = 2;
                    }
                    else
                    {
                        blocks[x, y, z] = Terrain.GetTerrainBlock(new Vector3i(x, y, z) + position);
                    }
                }
            }
        }
        yield return null;
        StartCoroutine(CreateMesh());
    }

    //将所有面的顶点和三角面的索引添加进去
    IEnumerator CreateMesh()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < Chunk.width; x++)
        {
            for (int y = 0; y < Chunk.height; y++)
            {
                for (int z = 0; z < Chunk.width; z++)
                {
                    //获取当前坐标的block对象
                    Block block = BlockList.GetBlock(this.blocks[x, y, z]);
                    if (block == null)
                    {
                        continue;
                    }
                    if (IsBlockTransparent(x + 1, y, z))
                    {
                        AddFrontFace(x, y, z,block);
                    }
                    if (IsBlockTransparent(x - 1, y, z))
                    {
                        AddBackFace(x, y, z, block);
                    }
                    if (IsBlockTransparent(x, y, z + 1))
                    {
                        AddRightFace(x, y, z, block);
                    }
                    if (IsBlockTransparent(x, y, z - 1))
                    {
                        AddLeftFace(x, y, z, block);
                    }
                    if (IsBlockTransparent(x, y + 1, z))
                    {
                        AddTopFace(x, y, z, block);
                    }
                    if (IsBlockTransparent(x, y - 1, z))
                    {
                        AddBottomFace(x, y, z, block);
                    }
                }
            }
        }

        
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        //重新计算边界和法线    
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //赋值给组件渲染面
        this.GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;

        yield return null;
        isWorking = false;
    }
    //判断该面相邻的块是否为空气或透明 Chunk中的局部坐标
    public  bool IsBlockTransparent(int x,int y,int z)
    {
        if (x >= width || y >= height || z >= width || x < 0 || y < 0 || z < 0)
        {
            return true;
        }
        else
        {
            //如果当前方块id=0，是透明
            return this.blocks[x, y, z] == 0;
        }
        
    }


    private void AddFrontFace(int x, int y, int z,Block block)
    {
        //组成这个面的第一个三角面，创建顺序决定了法线朝向，unity中使用左手坐标系，所以顺时针创建
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);
        //第二个三角面
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);

        //添加四角面顶点
        vertices.Add(new Vector3(0+x, 0+y, 0+z));
        vertices.Add(new Vector3(0+x, 0+y, 1+z));
        vertices.Add(new Vector3(0+x, 1+y, 1+z));
        vertices.Add(new Vector3(0+x, 1+y, 0+z));

        //添加UV坐标点顺序与四角面顶点顺序一致
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    private void AddBackFace(int x, int y, int z, Block block)
    {

        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);

        vertices.Add(new Vector3(-1 + x, 0 + y, 1 + z));
        vertices.Add(new Vector3(-1 + x, 0 + y, 0 + z));
        vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z));
        vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z));

        uv.Add(new Vector2(block.textureBackX * textureOffset, block.textureBackY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureBackX * textureOffset, block.textureBackY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    private void AddRightFace(int x, int y, int z, Block block)
    {
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);

        vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
        vertices.Add(new Vector3(-1 + x, 0 + y, 1 + z));
        vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z));
        vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));

        uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    private void AddLeftFace(int x, int y, int z, Block block)
    {
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);

        vertices.Add(new Vector3(-1 + x, 0 + y, 0 + z));
        vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));
        vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));
        vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z));

        uv.Add(new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    private void AddTopFace(int x, int y, int z, Block block)
    {
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);

        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);

        vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));
        vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));
        vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z));
        vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z));

        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    private void AddBottomFace(int x, int y, int z, Block block)
    {
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);

        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);

        vertices.Add(new Vector3(-1 + x, 0 + y, 0 + z));
        vertices.Add(new Vector3(-1 + x, 0 + y, 1 + z));
        vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
        vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));

        uv.Add(new Vector2(block.textureBottomX * textureOffset, block.textureBottomY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureBottomX * textureOffset, block.textureBottomY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

}
