using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleChange : MonoBehaviour
{
    float _minScale = 0.8f;
    float _maxScale = 1.6f;
    float _targetScale = 1f;
    float _speed = 0.1f;
    float _margin = 0.1f;
    string _strTitle = "Title";

    void Start()
    {
        AssignVariables();        
    }

    void Update()
    {
        Scaling();        
    }

    void Scaling()
    {
        if (transform.localScale.x > _targetScale + _margin)
        {
            transform.localScale = new Vector2(transform.localScale.x - _speed * Time.deltaTime, transform.localScale.y - _speed * Time.deltaTime);
        }
        else if (transform.localScale.x < _targetScale - _margin)
        {
            transform.localScale = new Vector2(transform.localScale.x + _speed * Time.deltaTime, transform.localScale.y + _speed * Time.deltaTime);
        }
        else
        {
            ChooseScale();
        }
    }
    void ChooseScale()
    {
        _targetScale = Random.Range(_minScale, _maxScale);
    }
    void AssignVariables()
    {
        if (transform.parent.name == _strTitle)
        {
            _minScale = 0.9f;
            _maxScale = 1.1f;
            _targetScale = 1f;
            _speed = 0.1f;
            _margin = 0.03f;
        }
        else if (GetComponent<Button>())
        {
            _minScale = 0.9f;
            _maxScale = 1.1f;
            _targetScale = 1f;
            _speed = 0.02f;
            _margin = 0.01f;
        }
    }
}
