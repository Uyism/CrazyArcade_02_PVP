using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController_mobile : Controller
{   
    GameObject mBombBtn;
    GameObject mNiddleBtn;
    Character mCharacter;


    override protected void Init()
    {
        mCharacter = this.GetComponent<Character>();
        
        mBombBtn = GameObject.Find("BombBtn");
        mNiddleBtn = GameObject.Find("NiddleBtn");

        // 폭탄 버튼 등록
        Button.ButtonClickedEvent niddle_btn_event = mNiddleBtn.GetComponent<Button>().onClick;
        niddle_btn_event.AddListener(() => { mCharacter.SetResurect(); });

        // 바늘 버튼 등록
        Button.ButtonClickedEvent bomb_btn_event = mBombBtn.GetComponent<Button>().onClick;
        bomb_btn_event.AddListener(() => { SetBomb(); });
    }

    override public void MoveX(float diff_x)
    {
        if (diff_x > 0)
            mCharacter.Move(EDirection.Left);
        else
            mCharacter.Move(EDirection.Right);
    }

    override public void MoveY(float diff_y)
    {
        if (diff_y > 0)
            mCharacter.Move(EDirection.Down);
        else
            mCharacter.Move(EDirection.UP);
    }

    public void SetBomb()
    {
        mCharacter.BombAttack();   
    }
}
