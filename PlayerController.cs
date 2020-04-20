using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jh_Lib;

[RequireComponent (typeof(Character))]
public class PlayerController : MonoBehaviour
{
    Character mCharacter;

    private void Start()
    {
        mCharacter = GetComponent<Character>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 방향키
        if (Input.GetKey(KeyCode.UpArrow)) { mCharacter.Move(EDirection.UP); }
        else if (Input.GetKey(KeyCode.DownArrow)) { mCharacter.Move(EDirection.Down); }
        else if (Input.GetKey(KeyCode.RightArrow)) { mCharacter.Move(EDirection.Right); }
        else if (Input.GetKey(KeyCode.LeftArrow)) { mCharacter.Move(EDirection.Left); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            mCharacter.BombAttack();

        if (Input.GetKeyDown(KeyCode.Return))
            mCharacter.SetResurect();
    }
}
