
using UnityEngine;


public enum BlockDirection : byte
{
    Front=0,
    Back,
    Left,
    Right,
    Top,
    Bottom
}

public class Block 
{
    //方块ID
    public byte id;
    //方块名字
    public string name;
    //图标
    public Texture icon;

    public BlockDirection direction = BlockDirection.Front;
    //前面贴图坐标
    public byte textureFrontX;
    public byte textureFrontY;

    //后面贴图的坐标
    public byte textureBackX;
    public byte textureBackY;

    //右面贴图的坐标
    public byte textureRightX;
    public byte textureRightY;

    //左面贴图的坐标
    public byte textureLeftX;
    public byte textureLeftY;

    //上面贴图的坐标
    public byte textureTopX;
    public byte textureTopY;

    //下面贴图的坐标
    public byte textureBottomX;
    public byte textureBottomY;

    //上下左右前后面贴图都不一样的方块
    public Block(byte id, string name, 
        byte textureFrontX, byte textureFrontY,
        byte textureBackX, byte textureBackY,
        byte textureRightX, byte textureRightY,
            byte textureLeftX, byte textureLeftY, 
            byte textureTopX, byte textureTopY, 
            byte textureBottomX, byte textureBottomY)
    {
        this.id = id;
        this.name = name;

        this.textureFrontX = textureFrontX;
        this.textureFrontY = textureFrontY;

        this.textureBackX = textureBackX;
        this.textureBackY = textureBackY;

        this.textureRightX = textureRightX;
        this.textureRightY = textureRightY;

        this.textureLeftX = textureLeftX;
        this.textureLeftY = textureLeftY;

        this.textureTopX = textureTopX;
        this.textureTopY = textureTopY;

        this.textureBottomX = textureBottomX;
        this.textureBottomY = textureBottomY;
    }

    //贴图都是A的方块
    public Block(byte id,string name,byte textureX,byte textureY)
        : this(id,name,textureX,textureY,textureX,textureY,textureX,textureY,textureX,textureY,textureX,textureY,textureX,textureY)
    {

    }

    //上面是贴图A，其他面是贴图B的方块
    public Block(byte id,string name,byte textureX,byte textureY,byte textureTopX,byte textureTopY)
        : this(id, name, textureX, textureY, textureX, textureY, textureX, textureY, textureX, textureY, textureTopX, textureTopY, textureX, textureY)
    {

    }

    //上面是贴图A，下面是贴图B，其他面都是贴图C
    public Block(byte id, string name, byte textureX, byte textureY, byte textureTopX, byte textureTopY, byte textureBottomX, byte textureBottomY)
       : this(id, name, textureX, textureY, textureX, textureY, textureX, textureY, textureX, textureY, textureTopX, textureTopY, textureBottomX, textureBottomY)
    {

    }

    //上面是贴图A，下面是贴图B，前面是贴图C，其他面都是D
    public Block(byte id, string name, byte textureFrontX, byte textureFrontY, byte textureX, byte textureY, byte textureTopX, byte textureTopY, byte textureBottomX, byte textureBottomY)
        : this(id, name, textureFrontX, textureFrontY, textureX, textureY, textureX, textureY, textureX, textureY, textureTopX, textureTopY, textureBottomX, textureBottomY)
    {

    }
}
