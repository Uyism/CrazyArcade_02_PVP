using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    public GameObject CharacterObj;
    EPlayMode mGameMode; // AI / Contrl VS Contrl
    Character mCharacter;

    bool mIsAuto = false;
    bool mIsPlayerTeam = false;
    bool mIsPlayer = false;

    public void SetMode(EPlayMode game_mode)
    {
        mGameMode = game_mode;
    }

    public void SetAuto(bool is_auto)
    {
        mIsAuto = is_auto;
    }

    public void SetIsPlayer(bool is_player)
    {
        mIsPlayer = is_player;
    }

    public void SetIsPlayerTeam(bool is_player_team)
    {
        mIsPlayerTeam = is_player_team;
    }

    public Character MakeCharacter()
    {
        GameObject obj = Instantiate(CharacterObj);
        mCharacter = obj.GetComponent<Character>();

        mCharacter.IsPlayerTeam(mIsPlayerTeam);
        mCharacter.mIsPlayer = mIsPlayer;


        mCharacter.SetAutoControl(mIsAuto);
        mCharacter.transform.position = TileMap.IndexToPos(0);

        
        return mCharacter;
    }
}