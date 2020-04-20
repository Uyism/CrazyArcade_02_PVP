using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StructUserData
{
    public string uid;
    public string opponentUid;
}

public enum EDirection
{
    Default,
    UP,
    Down,
    Right,
    Left
}

public enum ETileType

{
    Default = 0,
    Rock,
    Forest_1,
    Flower,
    Basket
}


public enum EXPLODE_EFFECT_TYPE { BombCenter, BombEnd, BombDir, BombDefault } // ex) Bomb_Explode 애니메이션 타입 :방향, 중간, 끝
public enum EPlayerState { EWalk, EWaterBall, EDead }
public enum EPlayMode { EPVP, EAI }

public class Const
{
    // Character value
    public const float StartPosX = 0;
    public const float StartPosY = 0;
    public const int TileCntX = 30;
    public const int TileCntY = 20;
    public const int BombLifeTime = 2;
    public const float WalkSpeed = 3.0f;
    public const float WaterBallTime = 5.0f;
    public const float WalkBlockFreeTime = 10;
    public const float ItemDropPossiblity= 0.5f;
    public const int BombSize = 1;
    public const int BombCount = 2;
    public const int NiddleCount = 2;
    
    // 로컬 데이터 파일 이름
    public const string MapDataName = "MapData.json";
    public const string UserDataName = "UserData.json";

    public const int Player1PosIndex = 316;
    public const int Player2PosIndex = 313;

    // 알맞은 자리를 찾을 때까지 잠시...
    static public StructUserData UserData;
}
