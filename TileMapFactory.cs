using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Jh_Lib;


public class TileMapFactory : MonoBehaviour
{
    public GameObject TileObj;

    List<int> mLoadMapData = new List<int>();
    Dictionary<int, Tile> mTileMap = new Dictionary<int, Tile>();
    GameObject mTileMapFactoryObject;

    public Dictionary<int, Tile> MakeTileMap()
    {
        mTileMapFactoryObject = new GameObject("TileMapFactoryObject");
        LoadTileMapInfo();
        MakeTiles();

        return mTileMap;
    }

    // Json 파일에 저장된 맵 데이터 로드
    void LoadTileMapInfo()
    {
        List<int> data = LoadData();

        if (data != null)
            mLoadMapData = data;
        else
        {
            for (int i = 0; i < Const.TileCntX * Const.TileCntY; i++)
                mLoadMapData.Add(0);
        }
    }

    // 맵 구성 요소들 생성(돌, 꽃, 바구니 등등..)
    void MakeTiles()
    {
        for (int j = 0; j < Const.TileCntY; j++)
        {
            for (int i = 0; i < Const.TileCntX; i++)
            {
                int idx = i + j * Const.TileCntX;
                Tile tile = CreateTile(idx);
                mTileMap[idx] = tile;

                ETileType tile_type = ETileType.Default;
                if (mLoadMapData.Count > idx)
                    tile_type = (ETileType)mLoadMapData[idx];
                tile.SetTileType(tile_type);
            }
        }
    }

    public Tile CreateTile(int index)
    {
        GameObject tile_obj = Instantiate(TileObj);
        tile_obj.transform.parent = mTileMapFactoryObject.transform;

        Tile _tile = tile_obj.GetComponent<Tile>();
        _tile.CreateTile(index);

        return _tile;
    }

    public List<int> LoadData()
    {
        string data = JsonFactory.Load(Const.MapDataName);
        MapData _data = JsonUtility.FromJson<MapData>(data);

        if (_data == null) return null;
        return _data.MapDta;
    }
}
