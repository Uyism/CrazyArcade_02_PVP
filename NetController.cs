using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jh_Lib;

public class NetController : MonoBehaviour
{
    ClientNet mNet;
    ClientNet.mDelegateType mCallBack;

    // Start is called before the first frame update
    void Start()
    {
        mNet = new ClientNet();
        mNet.SetCallBack(mCallBack);
    }

    // Update is called once per frame
    void Update()
    {
        StructRequest request = new StructRequest();
        request.uid = "2";
        request.request_url = "character/getClientMove";

        mNet.RequestMsg(request);
    }
}
