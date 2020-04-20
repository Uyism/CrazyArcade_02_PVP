using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Jh_Lib;

public class Character : MonoBehaviour
{
    #region Parameter

    CharacterManager mCharacterManager;
    GameObject mSystemManager;
    public GameObject PlayerSign;

    [HideInInspector]
    public float mWalkSpeed = Const.WalkSpeed;
    float mOriWalkSpeed = Const.WalkSpeed;
    float mWaterBallTime = Const.WaterBallTime;
    float mOriWaterBallTime = Const.WaterBallTime;

    bool mIsAuto;
    [HideInInspector]
    public bool mIsPlayer;
    bool mIsPlayerTeam;

    // 폭탄 설치 순간은 자유로운 이동 가능
    // 이후로는 폭탄 있는 곳에서 걸을 수 없음
    bool mIsWalkBlockFreeTime = false;
    float mWalkingBlockFreeTime = Const.WalkBlockFreeTime;
    float mOriWalkingBlockFreeTime = Const.WalkBlockFreeTime;

    Animator anim;

    // Inspector
    [HideInInspector]
    public EPlayerState mPlayerState = EPlayerState.EWalk;

    [Header("캐릭터")]
    public int CurIndex = 0;

    [Header("보유 아이템")]
    [SerializeField]
    int NiddleCnt = Const.NiddleCount;
    public int BombCnt = Const.BombCount;
    public int BombSize = Const.BombSize;

    // Net
    public delegate void mDelegateType(int index);
    mDelegateType mBombAttackCallBack;
    mDelegateType mRecurrectCallBAck;
    ClientNet mNet;

    #endregion

    public int GetSetNiddleCnt { get => NiddleCnt; set { NiddleCnt = value; SetNiddleCntText(); } }

    private void Start()
    {
        mSystemManager = GameObject.Find("SystemManager");
        mCharacterManager = mSystemManager.GetComponent<CharacterManager>();

        anim = GetComponent<Animator>();
        GetSetNiddleCnt = 2;

        // 쉐이더로 팀 색상 구분해서 랜더링
        if (mIsPlayerTeam)
            this.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Material/character_red");
        else
            this.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Material/character_blue");
    }

    public void SetAutoControl(bool is_auto)
    {
        mIsAuto = is_auto;
        if (mIsAuto)
            gameObject.AddComponent<AutoController>();

        if (!mIsPlayerTeam)
            return;

        // 수동
        if (!mIsAuto)
        {
            if (Application.platform == RuntimePlatform.Android)
                gameObject.AddComponent<PlayerController_mobile>();
            else
                gameObject.AddComponent<PlayerController>();
        }

        // 내 캐릭터 표시
        if (mIsPlayer)
            PlayerSign.SetActive(true);
    }

    void Update()
    {
        switch (mPlayerState)
        {
            case EPlayerState.EWalk:
                CurIndex = TileMap.PosToIndex(transform.position);
                CheckTrapInWaterBall();
                break;

            case EPlayerState.EWaterBall:
                CheckOpponentTouchWaterBall();
                break;

            case EPlayerState.EDead:
                break;

        }
    }

    public void Move(EDirection dir)
    {
        if (mPlayerState != EPlayerState.EWalk) return;

        Vector3 pos_dir = Vector3.zero;
        switch (dir)
        {
            case EDirection.UP: pos_dir = Vector3.up; break;
            case EDirection.Down: pos_dir = Vector3.down; break;
            case EDirection.Right: pos_dir = Vector3.right; break;
            case EDirection.Left: pos_dir = Vector3.left; break;
        }
        SetMoveAnim(pos_dir);

        bool is_free = CheckIsWalkableFree(transform.position + pos_dir.normalized / 2);
        if (!is_free)
            return;

        transform.Translate(pos_dir * mWalkSpeed * Time.deltaTime);
    }

    public void Move(int index)
    {
        if (mPlayerState != EPlayerState.EWalk) return;

        Vector3 target_pos = TileMap.IndexToPos(index);
        Vector3 cur_pos = transform.position;
        Vector3 dir = target_pos - cur_pos;
        SetMoveAnim(dir);

        bool is_free = CheckIsWalkableFree(transform.position + dir.normalized / 2);
        if (!is_free)
            return;

        transform.Translate(dir.normalized * mWalkSpeed * Time.deltaTime);
    }

    public void SetBombAttackCallBack(mDelegateType call_back)
    {
        // 소켓 설치
        mBombAttackCallBack = call_back;
        gameObject.AddComponent<ClientNet>();
        mNet = GetComponent<ClientNet>();

        // @request 소켓을 서버에 등록
        StructRequest request = new StructRequest();
        request.uid = Const.UserData.uid;
        request.request_url = URL.SetBombSocket.ToString();

        mNet.SetCallBack(NetBombAttackCallBack); // 상대 클라이언트에서 물풍선 설치했을 때 받을 콜백
        mNet.RequestMsg(request);
    }

    public void SetRecurrectCallBack(mDelegateType call_back)
    {
        mRecurrectCallBAck = call_back;
    }

    #region Inner Function

    void CheckTrapInWaterBall()
    {
        bool is_explode_tile = mSystemManager.GetComponent<BombExplodeFactory>().IsBombExplodeTile(TileMap.PosToIndex(transform.position));
        if (is_explode_tile == true)
        {
            mPlayerState = EPlayerState.EWaterBall;
            anim.SetBool("water_ball", true);

            TestShaderStart();
        }
    }

    

    void CheckOpponentTouchWaterBall()
    {
        // 적과 만났을  경우 Dead
        List<Character> opponent_list = mCharacterManager.GetOpponentList(mIsPlayerTeam);
        if (opponent_list == null)
            return;

        foreach (Character charac in opponent_list)
        {
            if (charac.mPlayerState == EPlayerState.EWalk)
            {
                if (CurIndex == charac.CurIndex)
                    SetDead();
            }
        }

        // 아군과 만났을 경우 부활
        List<Character> my_team_list = mCharacterManager.GetOpponentList(!mIsPlayerTeam);
        foreach (Character charac in my_team_list)
        {
            if (charac.mPlayerState == EPlayerState.EWalk)
            {
                if (CurIndex == charac.CurIndex)
                    SetResurect();
            }
        }
    }

    void SetMoveAnim(Vector3 dir) 
    {
        EDirection e_dir = JUtils.GetDir(dir, Vector3.zero);
        switch (e_dir)
        {
            case EDirection.UP:  anim.SetTrigger("up"); break;
            case EDirection.Down:  anim.SetTrigger("down"); break;
            case EDirection.Right:  anim.SetTrigger("right"); break;
            case EDirection.Left: anim.SetTrigger("left"); break;
        }
    }

    public bool CheckIsWalkableFree(Vector3 pos_dir)
    {
        // 물폭탄 구역 지나가도 되는 타임인가
        if (mIsWalkBlockFreeTime)
        {
            mWalkingBlockFreeTime -= 1;
            if (mWalkingBlockFreeTime < 0) mIsWalkBlockFreeTime = false;

            return mSystemManager.GetComponent<TileMap>().IsWalkableFree(pos_dir);
        }
        else
        {
            mWalkingBlockFreeTime = 0;
            bool isWalkable = mSystemManager.GetComponent<TileMap>().IsWalkable(pos_dir);
            if (!isWalkable) return false;
        }

        return true;
    }


    void NetBombAttackCallBack(StructRequest response)
    {
        // 상대 클라이언트에서 물풍선 설치했다는 통신을 받음
        if (response.request_url == URL.AttackBomb.ToString())
        {
            string bomb_index = response.parameter["bombIndex"];

            if (mBombAttackCallBack != null)
                mBombAttackCallBack(int.Parse(bomb_index));
        }

    }

    public virtual void BombAttack()
    {
        if (mPlayerState != EPlayerState.EWalk) return;

        if (BombCnt <= 0) return;

        int cur_index = TileMap.PosToIndex(this.transform.position);
        mSystemManager.GetComponent<BombFactory>().MakeBomb(cur_index, BombSize, this);

        BombCnt -= 1;
        mWalkingBlockFreeTime = mOriWalkingBlockFreeTime;
        mIsWalkBlockFreeTime = true;

        // PVP 모드에서만 동작
        if (mNet == null) return;

        // @request 물풍선 설치
        StructRequest request = new StructRequest();
        request.uid = Const.UserData.uid;
        request.request_url = URL.AttackBomb.ToString();
        request.parameter = new Dictionary<string, string>();
        request.parameter["bombIndex"] = cur_index.ToString();

        mNet.RequestMsg(request);
    }


    public virtual void BombAttack(int index)
    {
        if (mPlayerState != EPlayerState.EWalk) return;

        if (BombCnt <= 0) return;

        int cur_index = index;
        mSystemManager.GetComponent<BombFactory>().MakeBomb(cur_index, BombSize, this);

        BombCnt -= 1;
        mWalkingBlockFreeTime = mOriWalkingBlockFreeTime;
        mIsWalkBlockFreeTime = true;

 
    }

    public void PickUpBombCnt()
    {
        BombCnt += 1;
    }

    public void SpeedUp()
    {
        mWalkSpeed += 1f;
    }

    public void BombSizeUp()
    {
        BombCnt += 1;
    }

    public void BombLengthUp()
    {
        BombSize += 1;
    }

    public void AddNiddle()
    {
        GetSetNiddleCnt += 1;
    }

    void SetNiddleCntText()
    {
        if (!mIsAuto)
            GameObject.Find("NiddleBtn").GetComponentInChildren<Text>().text = NiddleCnt.ToString();
    }

    public void IsPlayerTeam(bool is_player_team)
    {
        mIsPlayerTeam = is_player_team;
    }

    public void SetDead()
    {
        mPlayerState = EPlayerState.EDead;
        anim.SetBool("water_ball", false);
        anim.SetTrigger("dead");

        mSystemManager.GetComponent<AudioManager>().PlayWaterBallEndAudio();
    }

    public void RemoveCharacter()
    {
        gameObject.SetActive(false);
    }

    // 바늘로 부활
    public void SetResurect()
    {
        if (GetSetNiddleCnt <= 0) return;

        GetSetNiddleCnt -= 1;
        mPlayerState = EPlayerState.EWalk;
        mWalkSpeed = mOriWalkSpeed;

        anim.SetTrigger("resurrect");
        anim.SetBool("water_ball", false);

        mSystemManager.GetComponent<AudioManager>().PlayWaterBallEndAudio();


        int cur_index = TileMap.PosToIndex(transform.position);
        if (mRecurrectCallBAck != null)
            mRecurrectCallBAck(cur_index);
    }

    void TestShaderStart()
    {
        if (!mIsPlayerTeam)
        {
            this.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Material/character_blue_blue");
            StartCoroutine("TestShaderReturn");
        }
    }

    IEnumerator TestShaderReturn()
    {
        yield return new WaitForSeconds(2);
        this.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Material/character_blue");
    }

    #endregion
}
