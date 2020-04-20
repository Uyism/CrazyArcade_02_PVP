using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapToolCameraController : Controller
{
    public Camera mCamera;
    float mCameraSpeed = 0.1f;

    // 맵툴은 터치하는 부분이 많아서 컨트롤러 중심이 움직이지 않게 설정
    override protected void Init()
    {
        mIsControllPoseLock = true;
    }

    override public void MoveX(float diff_x)
    {
        if (diff_x > 0)
            mCamera.transform.Translate(Vector3.left * mCameraSpeed);
        else
            mCamera.transform.Translate(Vector3.right * mCameraSpeed);
    }

    override public void MoveY(float diff_y)
    {
        if (diff_y > 0)
            mCamera.transform.Translate(Vector3.down * mCameraSpeed);
        else
            mCamera.transform.Translate(Vector3.up * mCameraSpeed);
    }
}
