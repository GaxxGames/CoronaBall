using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour
{
    [SerializeField] float _animSpeed = 1;
    Animator _myAnimator;
    string _strSpeed = "speed";

    void Start()
    {
        _myAnimator = GetComponent<Animator>();
        _myAnimator.SetFloat(_strSpeed, _animSpeed);
    }
}
