using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPosRotSca : MonoBehaviour
{
    public GameObject Parent;
    string _strPlayerRoot = "PlayerRoot";

    void Start()
    {
        if(!Parent)
        {
            Parent = GameObject.FindGameObjectWithTag(_strPlayerRoot);
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = Parent.transform.position;
        transform.rotation = Parent.transform.rotation;
        transform.localScale = Parent.transform.lossyScale;
    }
}
