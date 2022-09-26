using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrisBehaviour : MonoBehaviour
{
    public GameObject Ball;
    float _maxDeviationX = 0.05f;
    bool _isRoteted180 = false;
    float _speed = 3;
    float _newX = 0;
    Transform _parentTransform;
    Vector2 _myInitialScale;
    Vector3 _parentInitialScale;

    void Start()
    {
        AssignVariables();     
    }

    void LateUpdate()
    {
        FocusOnBall();
    }

    void FocusOnBall()
    {
        transform.rotation = _parentTransform.rotation;
        if(_isRoteted180)
        {
            transform.Rotate(0, 180, 0);
        }
        transform.localScale = _myInitialScale * _parentTransform.lossyScale / _parentInitialScale;

        if(Ball.transform.position.x > transform.position.x)
        {
            _newX = transform.position.x + _speed * Time.deltaTime;
        }
        else if(Ball.transform.position.x < transform.position.x)
        {
            _newX = transform.position.x - _speed * Time.deltaTime;
        }

        if(_newX > _parentTransform.position.x + _maxDeviationX)
        {
            _newX = _parentTransform.position.x + _maxDeviationX;
        }
        else if(_newX < _parentTransform.position.x - _maxDeviationX)
        {
            _newX = _parentTransform.position.x - _maxDeviationX;
        }
        transform.position = new Vector3(_newX, _parentTransform.position.y, _parentTransform.position.z);
    }
    void AssignVariables()
    {
        if (transform.parent.GetComponent<TagEnemy>())
        {
            _isRoteted180 = true;
        }
        else
        {
            _isRoteted180 = false;
        }

        if(transform.name == "Teczowka_L") // Left eye is bigger, so deviation must be greater
        {
            _maxDeviationX = 0.08f;
        }

        _parentTransform = transform.parent;
        transform.SetParent(null);

        _myInitialScale = transform.lossyScale;
        _parentInitialScale = _parentTransform.lossyScale;

        Ball = LvlManager.Instance.Ball;
        _newX = transform.position.x;
    }
}
