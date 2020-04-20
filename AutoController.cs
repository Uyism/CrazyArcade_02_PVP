using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Jh_Lib;

public class AutoController : MonoBehaviour
{
    GameObject mSystemManager;
    BombFactory mBombFactory;
    BombExplodeFactory mBombExplodeFactory;
    CharacterManager mCharacterManager;
    ItemFactory mItemFactory;

    TileMap mTileMap;
    Character mCharacter;
    
    public int mCurIndex = 0;
    public int mNextIndex = 0;

    void Start()
    {
        mSystemManager = GameObject.Find("SystemManager");
        mBombFactory = mSystemManager.GetComponent<BombFactory>();
        mBombExplodeFactory = mSystemManager.GetComponent<BombExplodeFactory>();
        mCharacterManager = mSystemManager.GetComponent<CharacterManager>();
        mItemFactory = mSystemManager.GetComponent<ItemFactory>();

        mTileMap = mSystemManager.GetComponent<TileMap>();       
        mCharacter = this.GetComponent<Character>();

        mCurIndex = TileMap.PosToIndex(transform.position);
        mNextIndex = mCurIndex;

        CalcBehavior();
    }

    public void CalcBehavior()
    {
        // Debug Render
        mSystemManager.GetComponent<WarningRender>().SetDebugTileDefault(mCurIndex);       
        
        mNextIndex = CalacWhereToGo();

        // Debug Render
        mSystemManager.GetComponent<WarningRender>().SetDebugTileColor(mNextIndex, Color.blue);
    }

    void FixedUpdate()
    {
        // 다음 위치 도달 시, 다음 행동 계산
        mCurIndex = TileMap.PosToIndex(transform.position);
        if (mCurIndex == mNextIndex)
        {
            CalcBehavior();
            CalcPutBomb();        
        }
        mCharacter.Move(mNextIndex);
    }

    #region Inner Function

    int CalacWhereToGo()
    {
        // 1. 인접 인덱스 리스트
        int near_range_size = 1;
        Dictionary<int, float> near_index_list = GetNearIndex(mCurIndex, near_range_size);

        // 2. 인접 인덱스들 각각 위험도 계산 (값 낮을 수록 위험)
        AvoidDanger(ref near_index_list);

        // 3. 가장 높은 점수
        float best_score = float.MinValue;
        foreach (int index in near_index_list.Keys)
        {
            if (best_score < near_index_list[index])
            {
                best_score = near_index_list[index];
            }
        }

        // 4. 높은 점수를 가진 인덱스가 여러개인 경우, 랜덤으로 채택
        List<int> best_index_list = new List<int>();
        foreach (int index in near_index_list.Keys)
        {
            if (best_score == near_index_list[index])
            {
                best_index_list.Add(index);
            }
        }

        if (best_index_list.Count == 0)
            return mCurIndex;

        int random_index = Random.Range(0, best_index_list.Count);      
        if (best_index_list.Contains(best_index_list[random_index]))
            return best_index_list[random_index];
        
        return mCurIndex;
    }

    Dictionary<int, float> GetNearIndex(int cur_index, int near_range_size)
    {
        /*
        * near_index_list // 인접 인덱스 리스트
        * ex) near_range_size = 2
        * 0 0 0 0 0
        * 0 0 0 0 0
        * 0 0 # 0 0
        * 0 0 0 0 0
        * 0 0 0 0 0
        */
        Dictionary<int, float> near_index_list = new Dictionary<int, float>();

        AddNearIndex(ref near_index_list, cur_index, EDirection.UP, near_range_size);
        AddNearIndex(ref near_index_list, cur_index, EDirection.Down, near_range_size);
        AddNearIndex(ref near_index_list, cur_index, EDirection.Right, near_range_size);
        AddNearIndex(ref near_index_list, cur_index, EDirection.Left, near_range_size);

        near_index_list.Add(cur_index, 0); // 자기 자신 포함

        return near_index_list;
    }

    void AddNearIndex(ref Dictionary<int, float> near_index_list, int center_index, EDirection dir, int near_range_size)
    {
        for (int i = 1; i <= near_range_size; i++)
        {
            int index = center_index;
            switch (dir)
            {
                case EDirection.Left:
                    index = center_index - i;
                    if (index / Const.TileCntX != center_index / Const.TileCntX) return; // 같은 층에서만
                    break;

                case EDirection.Right:
                    index = center_index + i;
                    if (index / Const.TileCntX != center_index / Const.TileCntX) return; // 같은 층에서만
                    break;

                case EDirection.UP:
                    index = center_index + Const.TileCntX * i;
                    break;

                case EDirection.Down:
                    index = center_index - Const.TileCntX * i;
                    break;
            }

            
            if (mTileMap.IsWalkable(index))
                near_index_list.Add(index, 0);

            // 걸을 수 없는 지형을 만났을 경우 return
            else
                return;
        }
    }

    void CalcPutBomb()
    {
        int random = Random.Range(0, 1000);
        if (random > 10) return;

        if (mCharacter.BombCnt > 0) 
            mCharacter.BombAttack();
    }

    void AvoidDanger(ref Dictionary<int, float> near_index_list)
    {
        int[] keys = new int[near_index_list.Count];
        near_index_list.Keys.CopyTo(keys, 0);
        int keys_count = keys.Length;

        // Expected Bomb
        for (int i = 0; i < keys_count; ++i)
        {
            BombExplode expected_bomb_explode = mBombExplodeFactory.GetUrgentBombExplode(keys[i]);
            if (expected_bomb_explode != null)
            {
                // Expected Bomb
                // 폭탄 터질 시간 (0 ~ Const.BombLifeTime)
                // life time 늘어날 수록 위험
                near_index_list[keys[i]] -= expected_bomb_explode.GetLifeTime();
            }

            // BombExplode 구간
            BombExplode bomb_explode = mBombExplodeFactory.GetBombExplode(keys[i]);
            if (bomb_explode != null)
            {
                near_index_list[keys[i]] -= Const.BombLifeTime;
            }
        }

    }

    void PreferItem(ref Dictionary<int, float> near_index_list)
    {
        int[] keys = new int[near_index_list.Count];
        near_index_list.Keys.CopyTo(keys, 0);
        int keys_count = keys.Length;

        for (int i = 0; i < keys_count; ++i)
        {
            Item item = mItemFactory.GetItembMap(keys[i]);
            if (item != null)
            {
                near_index_list[keys[i]] += 1;
            }
        }
    }

    void AvoidOpponent(ref Dictionary<int, float> near_index_list)
    {
        Character charac = mCharacterManager.GetPlayer();
        EDirection dir = JUtils.GetDir(this.transform.position, charac.transform.position);
        int next_index = 0;
        switch (dir)
        {
            case EDirection.UP:
                next_index = mCurIndex - Const.TileCntX;
                break;
            case EDirection.Down:
                next_index = mCurIndex + Const.TileCntX;
                break;
            case EDirection.Right:
                next_index = mCurIndex - 1;
                break;
            case EDirection.Left:
                next_index = mCurIndex + 1;
                break;
        }

        int[] keys = new int[near_index_list.Count];
        near_index_list.Keys.CopyTo(keys, 0);
        int iCount = keys.Length;

        for (int i = 0; i < iCount; ++i)
        {
            near_index_list[keys[i]] += 1;
        }
        if (near_index_list.ContainsKey(next_index))
            near_index_list[next_index] += -1f;
    }

    #endregion
}
