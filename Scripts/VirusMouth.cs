using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusMouth : MonoBehaviour
{
    float _speed = 50f;
    float _currentPose;
    bool _isClosing = false;
    float _weightStep = 1;
    int _targetPose = 100;
    SkinnedMeshRenderer _myRenderer;
    // Start is called before the first frame update
    void Start()
    {
        AssignVariables();        
    }

    // Update is called once per frame
    void Update()
    {
        MouthBehaviour(); 
    }

    public void CollisionClose()
    {
        _currentPose = 100;
        _myRenderer.SetBlendShapeWeight(0, 100);
        _isClosing = false;
    }
    void MouthBehaviour()
    {
        if (_isClosing)
        {
            if (_myRenderer.GetBlendShapeWeight(0) < _targetPose - _weightStep)
            {
                _currentPose += _speed * Time.deltaTime;
                if (_currentPose > _myRenderer.GetBlendShapeWeight(0) + _weightStep)
                {
                    _myRenderer.SetBlendShapeWeight(0, _myRenderer.GetBlendShapeWeight(0) + _weightStep);
                }
            }
            else
            {
                _targetPose = 0;
                _isClosing = false;
            }
        }
        else
        {
            if (_myRenderer.GetBlendShapeWeight(0) > _targetPose + _weightStep)
            {
                _currentPose -= _speed * Time.deltaTime;
                if (_currentPose < _myRenderer.GetBlendShapeWeight(0) - _weightStep)
                {
                    _myRenderer.SetBlendShapeWeight(0, _myRenderer.GetBlendShapeWeight(0) - _weightStep);
                }
            }
            else
            {
                _targetPose = 100;
                _isClosing = true;
            }

        }
    }
    void AssignVariables()
    {
        _myRenderer = GetComponent<SkinnedMeshRenderer>();
        _currentPose = _myRenderer.GetBlendShapeWeight(0);
        if (_currentPose == _targetPose)
        {
            if (_targetPose == 0)
            {
                _targetPose = 100;
            }
            else
            {
                _targetPose = 0;
            }
        }
        if (_currentPose < _targetPose)
        {
            _isClosing = true;
        }
        else
        {
            _isClosing = false;
        }
    }
}
