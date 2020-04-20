using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Jh_Lib;

// 외부 파일 MapData.json Json 형식
public class MapData
{
    public string MapName;
    public List<int> MapDta;
}

public class MapTool : MonoBehaviour
{
    public Camera Camera;
    public GameObject BoundBox;
    TileMap mTileMap;
    TileMapFactory mTileFactory;

    public bool mSelecting = false;
    ETileType mTileType;

    private void Start()
    {
        mTileMap = this.GetComponent<TileMap>();
        mTileFactory = this.GetComponent<TileMapFactory>();

        BoundBox.SetActive(true);

        // 타일맵 세팅
        Dictionary<int, Tile> map_tile = mTileFactory.MakeTileMap();
        this.GetComponent<TileMap>().SetTileMap(map_tile);
    }

    void Update()
    {
        Vector3 pos = Input.mousePosition;
        pos = Camera.ScreenToWorldPoint(pos);

        // 맵 영역 밖은 터치X
        if (pos.x < 0) return;
        else if (pos.x > Const.TileCntX) return;
        else if (pos.y > 0) return;
        else if (pos.y < -15) return;

        // 터치할 선택 영역 표시
        pos = TileMap.getTileCenterPos(pos);
        pos = Camera.WorldToScreenPoint(pos);
        BoundBox.transform.position = pos;

        // 타일 설치
        if (Input.GetMouseButtonDown(0))
        {
            SetTileType();
        }

        // 뒤로 가기
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("Intro");
            }
        }
    }

    public void SelectTileType(int number)
    {
        mTileType = (ETileType)number;
        string res = Tile.TILE_TEXTURE[mTileType];
        BoundBox.GetComponent<Image>().sprite = Resources.Load<Sprite>(res);
    }

    public void SetTileType()
    {
        Vector3 pos = BoundBox.transform.position;
        pos = Camera.ScreenToWorldPoint(pos);
        int index = TileMap.PosToIndex(pos);

        // 타일 변경 & 설치
        Dictionary<int, Tile> tile_map = mTileMap.GetTileMap();
        Tile tile = null;
        if (tile_map.ContainsKey(index))
            tile = tile_map[index];

        if (tile == null)
            tile = mTileFactory.CreateTile(index);
 
        // 타일 등록
        tile.SetTileType(mTileType);
        mTileMap.SetTile(index, tile);
    }

    public void SaveData()
    {
        Dictionary<int, Tile> tile_map = mTileMap.GetTileMap();
        List<int> map_tile_list = new List<int>();
        for (int i = 0; i < Const.TileCntY; i++)
        {
            for (int j = 0; j < Const.TileCntX; j++)
            {
                int index = (i * Const.TileCntX) + j;
                Tile tile = null;
                if (tile_map.ContainsKey(index))
                    tile = tile_map[index];

                if (tile == null)
                    map_tile_list.Add(0);
                else
                    map_tile_list.Add((int)tile.GetTileType());
            }
        }

        MapData map_data = new MapData();
        map_data.MapName = Const.MapDataName;
        map_data.MapDta = map_tile_list;

        JsonFactory.Write(Const.MapDataName, map_data);
    }

    public void Reset()
    {
        Dictionary<int, Tile> tile_map = mTileMap.GetTileMap();
        for (int i = 0; i < Const.TileCntY; i++)
        {
            for (int j = 0; j < Const.TileCntX; j++)
            {
                int index = (i * Const.TileCntX) + j;
                if (tile_map.ContainsKey(index))
                {
                    tile_map[index].SetTileType(ETileType.Default);
                    mTileMap.SetTile(index, null);
                }
            }
        }
    }


}
