using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTexture : MonoBehaviour
{
    [SerializeField] Renderer _myRenderer;
    [SerializeField] float _speedX = 1;
    float _speedY = 0;
    Vector2 _offset;
    string _strMainTex = "_MainTex";
    string _strBaseMap = "_BaseMap";

    void Start()
    {
        if (!_myRenderer)
        {
            _myRenderer = GetComponent<Renderer>();
        }
        _offset = _myRenderer.material.mainTextureOffset;
    }

    void Update()
    {
        _offset += new Vector2(-_speedX, _speedY) * Time.deltaTime;
        _myRenderer.material.SetTextureOffset(_strMainTex, _offset);
        _myRenderer.material.SetTextureOffset(_strBaseMap, _offset);
    }

    public Vector2 GetTexSpeed()
    {
        return new Vector2(_speedX, _speedY);
    }
    public float GetDefaultSpeedX()
    {
        return _speedX;
    }
    public void SetSpeed(Vector2 newSpeed)
    {
        _speedX = newSpeed.x;
        _speedY = newSpeed.y;
    }
}
