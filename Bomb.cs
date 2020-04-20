using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Jh_Lib;

public class Bomb : PoolObject
{
    Character mCharacter;
    GameObject mSystemManager;

    float mLifeTime = Const.BombLifeTime;
    public float LifeTime { get => mLifeTime; set => mLifeTime = value; }

    int mBombSize = 2;
    int mIndex = 0;

    // @PoolObject
    // 생성 시점이 active 시점 보다 빠를 수 있음
    private void Awake()
    {
        mSystemManager = GameObject.Find("SystemManager");
    }

    public void StartBomb(int index, int size, Character character)
    {
        mIndex = index;
        mBombSize = size;
        mCharacter = character;

        gameObject.SetActive(true);

        // BombExplode 미리 생성
        MakeBombExplode(mLifeTime);

        StartCoroutine("RunBomb");
    }

    IEnumerator RunBomb()
    {
        yield return new WaitForSeconds(mLifeTime);
       
        ObjectSleep(); // ObjectPool 사용 해제

        gameObject.SetActive(false);
        mCharacter.PickUpBombCnt(); // 캐릭터가 사용 가능한 폭탄 갯수 회수 시킴
    }

    #region Inner Function

    public void MakeBombExplode(float life_time)
    {
        int index = TileMap.PosToIndex(transform.position);
        mSystemManager.GetComponent<BombExplodeFactory>().MakeBombExplode(index, mBombSize, mLifeTime);
    }

    // @ 옵저버 패턴
    // ObjectPool 사용 해제될 때 폭탄 회수 알림
    public override void SleepCustome()
    {
        mSystemManager.GetComponent<BombFactory>().RemoveBomb(mIndex);
    }

    #endregion
}
