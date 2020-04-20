using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Jh_Lib;


public class TileMap : MonoBehaviour
{
    Dictionary<int, Tile> mTileMap = new Dictionary<int, Tile>();
    GameObject mSysytemFactory;

    public void SetTileMap(Dictionary<int, Tile> map_data)
    {
        mTileMap = map_data;
        mSysytemFactory = GameObject.Find("SystemManager");
    }

    public static Vector3 IndexToPos(int index)
    {
        Vector3 pos;
        int index_x = index % Const.TileCntX;
        int index_y = (index / Const.TileCntX) * -1;
        pos.x = index_x + 0.5f + Const.StartPosX;
        pos.y = index_y - 0.5f + Const.StartPosY;
        pos.z = 0;

        return pos;
    }

    public static int PosToIndex(Vector3 pos)
    {
        Vector2 zero_pos;
        zero_pos.x = pos.x - Const.StartPosX;
        zero_pos.y = pos.y - Const.StartPosY;

        int index_x = Mathf.FloorToInt(zero_pos.x);
        int index_y = Mathf.FloorToInt(zero_pos.y * -1);

        int index;
        index = index_x + index_y * Const.TileCntX;
        index = index >= Const.TileCntX * Const.TileCntY ? Const.TileCntX * Const.TileCntY : index;

        return index;
    }

    static public Vector3 getTileCenterPos(Vector3 pos)
    {
        int index = PosToIndex(pos);
        Vector3 res_pos = IndexToPos(index);

        return res_pos;
    }

    public bool IsWalkable(Vector3 pos)
    {
        int index = PosToIndex(pos);
        return IsWalkable(index);
    }

    public bool IsWalkableFree(Vector3 pos)
    {
        int index = PosToIndex(pos);
        // 맵 범위 밖
        if (!mTileMap.ContainsKey(index)) return false;

        return mTileMap[index].Walkable;
    }

    public bool IsWalkable(int index)
    {
        // 맵 범위 밖
        if (!mTileMap.ContainsKey(index)) return false;

        // 물폭탄 타일
        if (mSysytemFactory.GetComponent<BombFactory>().isBombTile(index)) return false; 

        return mTileMap[index].Walkable;
    }

    public bool IsBlocked(int index)
    {
        // 맵 범위 밖
        if (!mTileMap.ContainsKey(index)) return false;

        // 물폭탄 타일
        if (mSysytemFactory.GetComponent<BombFactory>().isBombTile(index)) return false;

        return mTileMap[index].IsBlocked();
    }

    public void HitTile(int index)
    {
        if (!mTileMap.ContainsKey(index)) return;

        mTileMap[index].HitTile();
    }

    public void SetTile(int index, Tile tile)
    {
        mTileMap[index] = tile;
    }

    public Dictionary<int, Tile> GetTileMap()
    {
        return mTileMap;
    }
}
