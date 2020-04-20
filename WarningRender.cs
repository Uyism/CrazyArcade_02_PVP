using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningRender : MonoBehaviour
{

    #region Inspector Custom

    [SerializeField]
    GameObject WarningSprite;

    [SerializeField]
    [Header("위험 지역 표시")]
    bool DebugModeOn;

    [SerializeField, Range(0, 100)]
    int WarningTransparent = 1;

    [SerializeField, ColorUsage(true)]
    Color WarningColor;

    #endregion

    Dictionary<int, SpriteRenderer> mSpriteMap = new Dictionary<int, SpriteRenderer>();
    BombExplodeFactory mBombExplodeFactory;
    GameObject mWarningRenderObject;

    void Start()
    {
        mWarningRenderObject = new GameObject("WarningRenderObject");
        mBombExplodeFactory = GameObject.Find("SystemManager").GetComponent<BombExplodeFactory>();

        for (int j = 0; j < Const.TileCntY; j++)
        {
            for (int i = 0; i < Const.TileCntX; i++)
            {
                int idx = i + j * Const.TileCntX;
                
                // 생성
                GameObject obj = Instantiate(WarningSprite);
                obj.transform.parent = mWarningRenderObject.transform;

                // 위치
                // 타일 사이즈 1 로 고정, 타일의 가운데 위치 시킴
                // @TODO 유연하게 동작하도록 수정해야함
                obj.transform.position = new Vector3(0.5f + i, -0.5f - j, 0.5f);
                
                // 색상
                obj.GetComponent<SpriteRenderer>().color = WarningColor;
                mSpriteMap[idx] = obj.GetComponent<SpriteRenderer>();

                if (!DebugModeOn) obj.SetActive(false);
            }
        }

    }

    public void SetDebugTileColor(int index, Color color)
    {
        if (!mSpriteMap.ContainsKey(index)) return;
        mSpriteMap[index].color = color;
    }

    public void SetDebugTileDefault(int index)
    {
        if (!mSpriteMap.ContainsKey(index)) return;
        mSpriteMap[index].color = new Color(255, 1, 1, 0);
    }
    /*
    public void FixedUpdate()
    {
        Dictionary<int, List<BombExplode>> m_bomb_explode = mBombExplodeFactory.mExpectedBomExplodebMap;
        foreach (int index in m_bomb_explode.Keys)
        {
            // bomb_explode 없는 구역은 Defulat
            if (!m_bomb_explode.ContainsKey(index))
            {
                mSpriteMap[index].color = new Color(255, 1, 1, 0);
                continue;
            }

            // bomb_explode life time에 따라 투명도 조절
            float life_time = 0;
            if (mBombExplodeFactory.GetUrgentBombExplode(index))
                life_time = mBombExplodeFactory.GetUrgentBombExplode(index).GetLifeTime();

            if (!mSpriteMap.ContainsKey(index)) continue;

            WarningColor.a = life_time * 0.5f * WarningTransparent * 0.1f;
            mSpriteMap[index].color = WarningColor;
        }
    }
    */

    public void FixedUpdate()
    {
        Dictionary<int, List<BombExplode>> m_bomb_explode = mBombExplodeFactory.mExpectedBomExplodebMap;
        foreach (int index in mSpriteMap.Keys)
        {
            // bomb_explode 없는 구역은 Defulat
            if (!m_bomb_explode.ContainsKey(index))
            {
                mSpriteMap[index].color = new Color(255, 1, 1, 0);
                continue;
            }

            // bomb_explode life time에 따라 투명도 조절
            float life_time = 0;
            if (mBombExplodeFactory.GetUrgentBombExplode(index))
                life_time = mBombExplodeFactory.GetUrgentBombExplode(index).GetLifeTime();

            if (!mSpriteMap.ContainsKey(index)) continue;

            WarningColor.a = life_time * 0.5f * WarningTransparent * 0.1f;
            mSpriteMap[index].color = WarningColor;
        }
    }
}
