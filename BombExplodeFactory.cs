using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Jh_Lib;
using TMPro;

public class BombExplodeFactory : ObjectPool<BombExplodeFactory, BombExplode>
{



    public GameObject BombExplode;
    GameObject SystemManager;

    // BombExplode 터지기 전에 담아두는 Map
    // BombExplode 터진 후 담아두는 Map
    public Dictionary<int, List<BombExplode>> mExpectedBomExplodebMap = new Dictionary<int, List<BombExplode>>();
    Dictionary<int, BombExplode> mBomExplodebMap = new Dictionary<int, BombExplode>();
    
    private void Start()
    {
        SystemManager = GameObject.Find("SystemManager");
    }

    public void MakeBombExplode(int index, int bomb_size, float life_time)
    {
        // 폭발 중앙
        SetCenterBombEffect(index, life_time);

        // 사방향
        SetDirBombEffect(index, EDirection.UP, bomb_size, life_time);
        SetDirBombEffect(index, EDirection.Down, bomb_size, life_time);
        SetDirBombEffect(index, EDirection.Right, bomb_size, life_time);
        SetDirBombEffect(index, EDirection.Left, bomb_size, life_time);   
    }

    public void SetBombExplodeMap(int index, BombExplode obj)
    {
        mBomExplodebMap[index] = obj;
    }

    public void RemoveFromBombExplodeMap(int index)
    {
        // 맵에서 제거
        if (mBomExplodebMap.ContainsKey(index))
            mBomExplodebMap[index] = null;

        // "ExpectedMap" 에서 이미 Run된 값 제거
        List<BombExplode> remove_bomb_explode = new List<BombExplode>();
        foreach (BombExplode _bomb_explode in mExpectedBomExplodebMap[index])
        {
            if (_bomb_explode.mEnd)
                remove_bomb_explode.Add(_bomb_explode);              
        }

        foreach (BombExplode _bomb_explode in remove_bomb_explode)
        {
            mExpectedBomExplodebMap[index].Remove(_bomb_explode);
        }
    }

    // 터지기 일보 직전인 BombExplode 반환
    public BombExplode GetUrgentBombExplode(int index)
    {
        if (!mExpectedBomExplodebMap.ContainsKey(index)) return null;

        float min_life_time = float.MaxValue;
        BombExplode bomb_explode = null;
        foreach (BombExplode _bomb_explode in mExpectedBomExplodebMap[index])
        {
            if (_bomb_explode.GetLifeTime() < min_life_time) // 터지는 시간이 가장 적게 남은 _bomb_explode
                bomb_explode = _bomb_explode;
        }
        return bomb_explode;
    }

    public BombExplode GetBombExplode(int index)
    {
        if (!mBomExplodebMap.ContainsKey(index)) return null;

        return mBomExplodebMap[index];
    }

    public bool IsBombExplodeTile(int index)
    {
        BombExplode bomb = GetBombExplode(index);
        if (bomb == null) return false;
        else return true;
    }

    #region Inner Function

    void SetCenterBombEffect(int index, float life_time)
    {
        StartBombEffect(index, EXPLODE_EFFECT_TYPE.BombCenter, life_time);
    }

    void SetDirBombEffect(int center_index, EDirection dir, int bomb_size, float life_time)
    {

        bool encounter_obstacle = false;
        for (int i = 1; i <= bomb_size; i++)
        {
            int index = center_index;
            int rot = 0;
            switch (dir)
            {
                case EDirection.Left:
                    index = center_index - i;
                    if (index / Const.TileCntX != center_index / Const.TileCntX) return; // 가로 폭탄은 같은 층에서만

                    rot = 270;
                    break;

                case EDirection.Right:
                    index = center_index + i;
                    if (index / Const.TileCntX != center_index / Const.TileCntX) return; // 가로 폭탄은 같은 층에서만

                    rot = 90;
                    break;

                case EDirection.UP:
                    index = center_index + Const.TileCntX * i;
                    rot = 0;
                    break;

                case EDirection.Down:
                    index = center_index - Const.TileCntX * i;
                    rot = 180;
                    break;
            }

            // 막힌 구간 만나면 더이상 Bomb Eplode를 생성하지 않는다.
            if (SystemManager.GetComponent<TileMap>().IsBlocked(index))
                return;

            // 가장자리 애니메이션 적용 여부
            EXPLODE_EFFECT_TYPE bomb_type = EXPLODE_EFFECT_TYPE.BombDir;
            if (i == bomb_size)
                bomb_type = EXPLODE_EFFECT_TYPE.BombEnd;

            // BombEffect 생성
            GameObject obj = StartBombEffect(index, bomb_type, life_time);
            if (obj == null) return;

            // 방향
            obj.transform.rotation = Quaternion.Euler(0, 0, rot);

        }
    }

    GameObject StartBombEffect(int index, EXPLODE_EFFECT_TYPE bomb_type, float life_time)
    {
        // ObjectPool 할당
        GameObject obj = Push(BombExplode);

        // 생성된 Object를 "ExpectedMap"에 미리 맵에 등록
        if (!mExpectedBomExplodebMap.ContainsKey(index))
            mExpectedBomExplodebMap[index] = new List<BombExplode>();

        mExpectedBomExplodebMap[index].Add(obj.GetComponent<BombExplode>());

        // 위치
        Vector3 pos = TileMap.IndexToPos(index);
        obj.transform.position = pos;

        // 초기화 ("Run" 아님)
        obj.SetActive(true);
        obj.GetComponent<BombExplode>().InitBombExplode(bomb_type, index, life_time);
        return obj;
    }

    #endregion
}
