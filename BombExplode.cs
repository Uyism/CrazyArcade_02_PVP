using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Jh_Lib;

public class BombExplode : PoolObject
{
    GameObject mSystemManager;
    
    int mIndex;
    float mLifeTime = Const.BombLifeTime;
    public bool mEnd = false; // BombExplode 끝남 여부

    EXPLODE_EFFECT_TYPE mBombType; // ex) 방향, 중간, 끝
    Dictionary<EXPLODE_EFFECT_TYPE, string> mExplodeAnim = new Dictionary<EXPLODE_EFFECT_TYPE, string>(); // ex) 방향, 중간, 끝에 해당하는 애니메이션 

    // @PoolObject
    // 생성 시점이 active 시점 보다 빠를 수 있음
    void Awake()
    {
        mExplodeAnim[EXPLODE_EFFECT_TYPE.BombDefault] = "default";
        mExplodeAnim[EXPLODE_EFFECT_TYPE.BombDir] = "isDir";
        mExplodeAnim[EXPLODE_EFFECT_TYPE.BombCenter] = "isStart";
        mExplodeAnim[EXPLODE_EFFECT_TYPE.BombEnd] = "isEnd";
        mSystemManager = GameObject.Find("SystemManager");
    }

    public void InitBombExplode(EXPLODE_EFFECT_TYPE bomb_type, int index, float life_time)
    {
        mIndex = index;
        mLifeTime = life_time;
        mBombType = bomb_type;
        mEnd = false;

        // BombExplode 생성 시점은 Bomb과 같음, 랜더링은 나중에
        this.GetComponent<SpriteRenderer>().enabled = false;
    } 

    void FixedUpdate()
    {
        if (mEnd) return;

        mLifeTime -= 0.02f;
        if (mLifeTime < 0)
        {
            RunBombExplode();
            mEnd = true;
            mLifeTime = Const.BombLifeTime;
        }
    }

    void RunBombExplode()
    {
        // 1. 타일 폭파
        mSystemManager.GetComponent<TileMap>().HitTile(mIndex);

        // 2. BombExplodeFactory에 등록
        mSystemManager.GetComponent<BombExplodeFactory>().SetBombExplodeMap(mIndex, this);

        // 3. 애니메이션 출력
        StartBombExplodeAnim();

        mSystemManager.GetComponent<AudioManager>().PlayBombExplodeAudio();
    }

    void StartBombExplodeAnim()
    {
        // @TODO BombExplode가 터질 때 CurIndex에 아직 Bomb이 있음 (IsWalk == true 라도 랜더링 되도록 예외처리)
        // @TODO Bomb 제거 후 터지도록 수정해야함
        if (mBombType != EXPLODE_EFFECT_TYPE.BombCenter)
        {
            // IsWakable == false 일 경우 해당 객체 종료
            if (!mSystemManager.GetComponent<TileMap>().IsWalkable(mIndex))
            {                
                endAnim();
                return;
            }
        }

        this.GetComponent<Animator>().SetBool(mExplodeAnim[mBombType], true);
        this.GetComponent<SpriteRenderer>().enabled = true;
    }

    #region Inner Function

    void endAnim()
    {
        // 초기화
        mEnd = true;
        mLifeTime = Const.BombLifeTime;

        // ObjectPool 사용 해제
        ObjectSleep();
        gameObject.SetActive(false);
    }

    // @ 옵저버 패턴
    // ObjectPool 사용 해제될 때 폭탄 회수 알림
    public override void SleepCustome()
    {
        mSystemManager.GetComponent<BombExplodeFactory>().RemoveFromBombExplodeMap(mIndex);
    }

    public float GetLifeTime()
    {
        return mLifeTime;
    }

    #endregion
}
