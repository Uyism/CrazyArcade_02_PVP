using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    GameObject mControllArea;
    GameObject mControllMenu;
    GameObject mController;

    bool isTouching = false;
    float mScreanWidth;
    bool mIsControllLock = false;
    protected bool mIsControllPoseLock = false; // 컨트롤러 중심부 움직임 여부

    protected void Start()
    {
        mControllArea = GameObject.Find("ControllArea");
        mControllMenu = GameObject.Find("ControllMenu");
        mController = GameObject.Find("Controller");

        mControllMenu.SetActive(true);

        // 해상도 가로 크기
        Resolution resolution = Screen.currentResolution;
        mScreanWidth = resolution.width;

        Init();
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector3 touch_pos = Input.GetTouch(0).position;

            // 컨트롤러는 화면 중앙 전까지만 움직임 가능
            if (touch_pos.x > mScreanWidth / 2) return;

            if (!mIsControllPoseLock)
            {
                mControllArea.transform.position = touch_pos;
                mController.transform.position = touch_pos;
            }

            isTouching = true;
        }

        // 조작 중
        if (isTouching)
        {
            Vector3 touch_pos = Input.GetTouch(0).position;
            SetControllerPos(touch_pos);

            float diff_x = mControllArea.transform.position.x - touch_pos.x;
            float diff_y = mControllArea.transform.position.y - touch_pos.y;

            if (mIsControllPoseLock)
            {
                if (mIsControllLock)
                    return;
            }

            if (Mathf.Abs(diff_x) > Mathf.Abs(diff_y))
                MoveX(diff_x);
            else
                MoveY(diff_y);
        }
        // 조작 끝
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            isTouching = false;
        }
    }

    void SetControllerPos(Vector3 touch_pos)
    {
        Vector3 center_pos = mControllArea.transform.position;
        Vector3 dir = touch_pos - center_pos;
        float distance = dir.magnitude;
        float controll_area_size = mControllArea.GetComponent<RectTransform>().rect.width;

        if (distance < controll_area_size / 2)
        {
            mController.transform.position = touch_pos;
            mIsControllLock = false;
        }
        // 컨트롤러가 조작 범위 넘어갔을 경우 방향만 조절
        else
        {
            if (!mIsControllPoseLock)
            {
                mController.transform.position = center_pos + dir.normalized * controll_area_size;
                mIsControllLock = true;
            }
        }
    }

    virtual protected void Init()
    {

    }

    virtual public void MoveX(float diff_x)
    {
        
    }

    virtual public void MoveY(float diff_y)
    {
       
    }
}
