using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Jh_Lib;

public class BombFactory : ObjectPool<BombFactory, Bomb>
{
    public GameObject bomb;
    GameObject mSystemManager;

    Dictionary<int, Bomb> mBombMap = new Dictionary<int, Bomb>();
    
    public void Start()
    {
        mSystemManager = GameObject.Find("SystemManager");
    }

    public void MakeBomb(int index, int size, Character charac)
    {
        if (!mSystemManager.GetComponent<TileMap>().IsWalkable(index)) return;

        mSystemManager.GetComponent<AudioManager>().PlayBombAudio();

        // 1. 오브젝트 풀에 할당
        GameObject obj = Push(bomb);

        // 2. 폭탄 맵에 저장
        mBombMap[index] = obj.GetComponent<Bomb>();

        // 3. 위치 지정
        Vector3 res_pos = TileMap.IndexToPos(index);
        obj.transform.position = res_pos;

        // 4. Bomb 시작
        obj.GetComponent<Bomb>().StartBomb(index, size, charac);
    }

    public void RemoveBomb(int index)
    {
        if (!mBombMap.ContainsKey(index)) return;

        mBombMap[index] = null;
    }

    public Bomb GetBomb(int index)
    {
        if (!mBombMap.ContainsKey(index)) return null;

        return mBombMap[index];
    }

    public bool isBombTile(int index)
    {
        Bomb bomb = GetBomb(index);
        if (bomb == null) return false;
        else return true;
    }
}
