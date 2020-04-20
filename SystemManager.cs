using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    void Awake()
    {
        // 0. 유저 데이터 세팅
        SetUserData();

        // 1. 맵 생성
        MakeMap();


        // 2. 캐릭터 생성
        // PVP 모드로 고정 상태
        MakeCharacter();
    }

    void SetUserData()
    {       
        string data = JsonFactory.Load(Const.UserDataName);
        StructUserData _data = JsonUtility.FromJson<StructUserData>(data);
        Const.UserData = _data;
    }

    void MakeMap()
    {
        // PVP모드에서는 서버에서 받은 MapData를 사용
        TileMapFactory factory_map = this.GetComponent<TileMapFactory>();
        Dictionary<int, Tile> map_data = factory_map.MakeTileMap();
        this.GetComponent<TileMap>().SetTileMap(map_data);
    }

    void MakeCharacter()
    {
        // 주인공
        CharacterFactory player1_fac = this.GetComponent<CharacterFactory>();
        player1_fac.SetMode(EPlayMode.EPVP);
        player1_fac.SetAuto(false);
        player1_fac.SetIsPlayerTeam(true);
        player1_fac.SetIsPlayer(true);
        Character player1 = player1_fac.MakeCharacter();

        // 상대
        CharacterFactory player2_fac = this.GetComponent<CharacterFactory>();
        player2_fac.SetMode(EPlayMode.EPVP);
        player2_fac.SetAuto(false);
        player2_fac.SetIsPlayerTeam(false);
        Character player2 = player2_fac.MakeCharacter();

        // PVP 모드일 경우
        NetCharacterManager net_character_manager = this.GetComponent<NetCharacterManager>();
        net_character_manager.SetPlayer1(player1);
        net_character_manager.SetPlayer2(player2);

        // 생성된 유저 관리
        CharacterManager character_manager = this.GetComponent<CharacterManager>();
        character_manager.AddPlayer1Team(player1);
        character_manager.AddPlayer2Team(player2);
        character_manager.SetPlayer(player1);
    }

    private void Update()
    {
        // 뒤로 가기
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("Intro");
            }
        }
    }

}
