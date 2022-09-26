using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthBehaviour : MonoBehaviour
{
    Animator _myAnimator;
    SkinnedMeshRenderer _myMeshRend;
    bool _isTongueLengthChanging = false;
    bool _isTongueStartedChanging = false;
    int _tongueLength = 0;
    float _speedTongueLengthChange = 5;  
    float _walkAnimSpeed = 1f;
    float _idleAnimSpeed = 0.1f;
    float _animSpeed = 0.1f;
    string _strSpeed = "speed";

    void Start()
    {
        AssignVariables();        
    }

    void Update()
    {
        TongueMovement();        
    }

    public void SetAnimSpeed(float newSpeed)
    {
        _animSpeed = newSpeed;
        if (!_myAnimator)
        {
            _myAnimator = GetComponent<Animator>();
        }
        _myAnimator.SetFloat(_strSpeed, _animSpeed);
    }
    public float GetWalkAnimSpeed()
    {
        return _walkAnimSpeed;
    }
    public float GetIdleAnimSpeed()
    {
        return _idleAnimSpeed;
    }
    IEnumerator ChangeTongueLength()
    {
        _isTongueStartedChanging = true;
        yield return new WaitForSeconds(Random.Range(2, 6));
        _isTongueLengthChanging = true;
        _tongueLength = Random.Range(1, 60);
        _isTongueStartedChanging = false;
    }    
    void TongueMovement()
    {
        if (!_isTongueLengthChanging)
        {
            if (!_isTongueStartedChanging)
            {
                StartCoroutine(ChangeTongueLength());
            }
        }
        else
        {
            if (_myMeshRend.GetBlendShapeWeight(0) > _tongueLength + (_speedTongueLengthChange / 2))
            {
                _myMeshRend.SetBlendShapeWeight(0, _myMeshRend.GetBlendShapeWeight(0) - _speedTongueLengthChange * Time.deltaTime);
            }
            else if (_myMeshRend.GetBlendShapeWeight(0) < _tongueLength - (_speedTongueLengthChange / 2))
            {
                _myMeshRend.SetBlendShapeWeight(0, _myMeshRend.GetBlendShapeWeight(0) + _speedTongueLengthChange * Time.deltaTime);
            }
            else
            {
                _isTongueLengthChanging = false;
            }
        }
    }
    void AssignVariables()
    {
        _myAnimator = GetComponent<Animator>();
        _myMeshRend = GetComponent<SkinnedMeshRenderer>();
        _animSpeed = _idleAnimSpeed;
        SetAnimSpeed(_animSpeed);
        _tongueLength = Random.Range(1, 90);
    }
}
