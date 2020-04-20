using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    List<Character> mPlayer1TeamList;
    List<Character> mPlayer2TeamList;

    Character mPlayer;
    public void AddPlayer1Team(Character character)
    {
        if (mPlayer1TeamList == null)
            return;

        mPlayer1TeamList.Add(character);
    }

    public void AddPlayer2Team(Character character)
    {
        if (mPlayer2TeamList == null)
            return;

        mPlayer2TeamList.Add(character);
    }

    public void SetPlayer(Character character)
    {
        mPlayer = character;
    
    }

    public Character GetPlayer()
    {
        return mPlayer;
    }

    public List<Character> GetOpponentList(bool is_player_team)
    {
        if (is_player_team)
            return mPlayer2TeamList;
        else
            return mPlayer1TeamList;
    }
}
