using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    GameObject mPlayer;
    int mOffset = 8; // Following 멈추는 간격

    void Start()
    {
        mPlayer = GameObject.Find("SystemManager").GetComponent<CharacterManager>().GetPlayer().gameObject;
    }

    void Update()
    {
        int player_index = mPlayer.GetComponent<Character>().CurIndex;
        int player_index_x = player_index % Const.TileCntX;
        int player_index_y = player_index / Const.TileCntX;
        
        if ((player_index_x > mOffset * 2) && (player_index_x < Const.TileCntX - mOffset * 2)) // 화면 비율상 X 방향이 더 많이 보여야함
            gameObject.transform.position = new Vector3(mPlayer.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);

        if ((player_index_y > mOffset) && (player_index_y < Const.TileCntY - mOffset))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, mPlayer.transform.position.y, gameObject.transform.position.z);
    }
}
