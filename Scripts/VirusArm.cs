using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusArm : MonoBehaviour
{
    float _speedRot = 5f;
    float _rotationTargetTime = 4;
    float _speedScale = 0.3f;
    float _maxScale = 1.3f;
    float _minScale = 0.7f;
    float _rotationTime = 0;
    float _targetScale, _initialSpeed, _initialTargetTime;
    bool _isScaling = false;
    bool _isRotating = true;
    bool _isRotatingRight = false;
    Vector3 _initialScale;

    void Start()
    {
        AssignVariables();
    }

    void FixedUpdate()
    {
        Rotating();
        Scaling();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            transform.localScale = new Vector3(0.5f * _initialScale.x, 0.5f * _initialScale.y, 0.5f * _initialScale.z);
        }
    }
    void ChooseRotDir()
    {
        int rand = Random.Range(0, 100);
        if(rand%2 == 0)
        {
            _isRotatingRight = true;
        }
        else
        {
            _isRotatingRight = false;
        }
    }
    void ChooseRotTime()
    {
        _rotationTargetTime = Random.Range(0.7f * _initialTargetTime, 1.3f * _initialTargetTime);
    }
    void ChooseSpeed()
    {
        _speedRot = Random.Range(0.7f * _initialSpeed, 1.3f * _initialSpeed);
    }
    void ChooseTargetScale()
    {
        _targetScale = Random.Range(_minScale, _maxScale);
    }
    void Rotating()
    {
        if (_isRotating)
        {
            if (_rotationTime < _rotationTargetTime)
            {
                _rotationTime += Time.fixedDeltaTime;
            }
            else
            {
                _isRotating = false;
            }

            if (_isRotatingRight)
            {
                transform.rotation *= Quaternion.Euler(Quaternion.Inverse(transform.rotation) * new Vector3(0, 0, _speedRot * Time.fixedDeltaTime));
            }
            else
            {
                transform.rotation *= Quaternion.Euler(Quaternion.Inverse(transform.rotation) * new Vector3(0, 0, -_speedRot * Time.fixedDeltaTime));
            }
        }
        else
        {
            _isRotating = true;
            _isRotatingRight = !_isRotatingRight;
            _rotationTime = 0f;
        }
    }
    void Scaling()
    {
        if (_isScaling)
        {
            if (transform.localScale.y > _targetScale + 0.03f)
            {
                float sc = _speedScale * Time.fixedDeltaTime;
                transform.localScale -= new Vector3(sc, sc, sc);
            }
            else if (transform.localScale.y < _targetScale - 0.03f)
            {
                float sc = _speedScale * Time.fixedDeltaTime;
                transform.localScale += new Vector3(sc, sc, sc);
            }
            else
            {
                _isScaling = false;
            }
        }
        else
        {
            ChooseTargetScale();
            _isScaling = true;
        }
    }
    void AssignVariables()
    {
        _initialTargetTime = _rotationTargetTime;
        _rotationTime = _rotationTargetTime / 2;
        _initialSpeed = _speedRot;
        ChooseRotDir();
        ChooseRotTime();
        ChooseSpeed();
        ChooseTargetScale();
        _initialScale = transform.localScale;
    }
}
