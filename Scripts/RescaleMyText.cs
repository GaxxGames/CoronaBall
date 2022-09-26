using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RescaleMyText : MonoBehaviour
{
    Text _myText;
    // Start is called before the first frame update
    void Start()
    {
        _myText = GetComponent<Text>();
        GameMgr.Instance.RescaleTextWithScreenSize(_myText);
        GetComponent<RescaleMyText>().enabled = false;
    }

}
