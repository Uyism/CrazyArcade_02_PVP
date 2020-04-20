using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jh_Lib;
using UnityEngine.Diagnostics;

public class NetCharacterManager : MonoBehaviour
{
    Character player1;
    Character player2;

    ClientNet mNEt;

    private void Start()
    {
        // 소켓 설치
        gameObject.AddComponent<ClientNet>();
        mNEt = this.GetComponent<ClientNet>();

        // 로컬 데이터에 들어있는 유저 정보 저장
        string data = JsonFactory.Load(Const.UserDataName);
        StructUserData _data = JsonUtility.FromJson<StructUserData>(data);
        Const.UserData = _data;
    }

    public void SetPlayer1(Character character)
    {
        player1 = character;
        player1.SetBombAttackCallBack(SendBombAttack);

        // 위치 지정
        if (Const.UserData.uid == "1")
            player1.transform.position = TileMap.IndexToPos(Const.Player1PosIndex);
        else
            player1.transform.position = TileMap.IndexToPos(Const.Player2PosIndex);

    }

    public void SetPlayer2(Character character)
    {
        player2 = character;

        // 위치 지정
        if (Const.UserData.uid == "1")
            player2.transform.position = TileMap.IndexToPos(Const.Player1PosIndex);
        else
            player2.transform.position = TileMap.IndexToPos(Const.Player2PosIndex);

    }

    private void Update()
    {
        // @request 위치 전달/수신
        Vector3 player1_pos = player1.transform.position;
        StructRequest request = new StructRequest();
        request.uid = Const.UserData.uid;
        request.request_url = URL.SyncMovement.ToString();
        request.parameter = new Dictionary<string, string>();
        request.parameter["posX"] = player1.gameObject.transform.position.x.ToString();
        request.parameter["posY"] = player1.gameObject.transform.position.y.ToString();
        request.parameter["opponentUid"] = Const.UserData.opponentUid;

        void CallBack(StructRequest response)
        {
            if (!response.parameter.ContainsKey("opponentPosX"))
                return;

            // 받은 player2 위치 적용
            float x = float.Parse(response.parameter["opponentPosX"]);
            float y = float.Parse(response.parameter["opponentPosY"]);
            Vector3 next_pos = new Vector3(x, y, player2.transform.position.z);
            EDirection dir =  JUtils.GetDir(next_pos, player2.transform.position);
            player2.Move(dir);
            player2.transform.position = next_pos;
        }


        mNEt.SetCallBack(CallBack);
        mNEt.RequestMsg(request);
    }

    void SendBombAttack(int index)
    {
        player2.BombAttack(index);
    }

    void SendRecurrect(int index)
    { }
}
