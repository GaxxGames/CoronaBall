using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescaleMyRectTransform : MonoBehaviour
{
    public bool KeepPosition = false;
    RectTransform _myRectTransform;

    void Start()
    {
        _myRectTransform = GetComponent<RectTransform>();
        GameMgr.Instance.RescaleWithScreenSize(_myRectTransform);
        GetComponent<RescaleMyRectTransform>().enabled = false;
        if(KeepPosition)
        {
            GameMgr.Instance.KeepPositionUI(_myRectTransform);
        }
    }

}
