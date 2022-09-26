using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBehaviour : MonoBehaviour
{
    public GameObject Eyebrow;
    float _blinkSpeed = 500;
    int _weightChangeStep = 10;
    int _blinkIntervalCount = 20;
    bool _isBlinking = false;
    bool _isBlinkingStarted = false;
    bool _areEyelidsDescending  = true;
    int _eyelidTargetPosUp = 100;
    int _eyelidTargetPosDown = 100;
    float _eyelidCurrentPosUp = 0;
    float _eyelidCurrentPosDown = 0;
    SkinnedMeshRenderer _myMeshRend;
    SkinnedMeshRenderer _eyebrowMeshRend;

    
    void Start()
    {
        AssignVariables();
    }

    void Update()
    {
        if (_isBlinking)
        {
            if(_areEyelidsDescending)
            {
                if(_myMeshRend.GetBlendShapeWeight(0) < 100)
                {
                    _eyelidCurrentPosUp += _blinkSpeed * Time.deltaTime;

                    if(_eyelidCurrentPosUp >= _myMeshRend.GetBlendShapeWeight(0) + _weightChangeStep)
                    {
                        _myMeshRend.SetBlendShapeWeight(0, _myMeshRend.GetBlendShapeWeight(0) + _weightChangeStep);
                        _eyebrowMeshRend.SetBlendShapeWeight(0, _myMeshRend.GetBlendShapeWeight(0));
                    }
                }
                if (_myMeshRend.GetBlendShapeWeight(1) < 100)
                {
                    _eyelidCurrentPosDown += _blinkSpeed * Time.deltaTime; 

                    if (_eyelidCurrentPosDown >= _myMeshRend.GetBlendShapeWeight(1) + _weightChangeStep)
                    {
                        _myMeshRend.SetBlendShapeWeight(1, _myMeshRend.GetBlendShapeWeight(1) + _weightChangeStep);
                    }
                }

                if(_myMeshRend.GetBlendShapeWeight(0) >= 100 && _myMeshRend.GetBlendShapeWeight(1) >= 100)
                {
                    _myMeshRend.SetBlendShapeWeight(0,100);
                    _eyebrowMeshRend.SetBlendShapeWeight(0, _myMeshRend.GetBlendShapeWeight(0));
                    _myMeshRend.SetBlendShapeWeight(1,100);
                    _eyelidCurrentPosUp = 100;
                    _eyelidCurrentPosDown = 100;
                    _areEyelidsDescending = false;
                }
            }
            else
            {
                if(_myMeshRend.GetBlendShapeWeight(0) >= _eyelidTargetPosUp)
                {
                    if( _eyelidCurrentPosUp >= _eyelidTargetPosUp)
                    {
                        _eyelidCurrentPosUp -= _blinkSpeed * Time.deltaTime;
                    }                    

                    if (_eyelidTargetPosUp <= _myMeshRend.GetBlendShapeWeight(0) - _weightChangeStep)
                    {
                        _myMeshRend.SetBlendShapeWeight(0, _myMeshRend.GetBlendShapeWeight(0) - _weightChangeStep);
                        _eyebrowMeshRend.SetBlendShapeWeight(0, _myMeshRend.GetBlendShapeWeight(0));
                    }
                }

                if (_myMeshRend.GetBlendShapeWeight(1) >= _eyelidTargetPosDown)
                {
                    if (_eyelidCurrentPosDown >= _eyelidTargetPosDown)
                    {
                        _eyelidCurrentPosDown -= _blinkSpeed * Time.deltaTime;
                    }

                    if (_eyelidCurrentPosDown <= _myMeshRend.GetBlendShapeWeight(1) - _weightChangeStep)
                    {
                        _myMeshRend.SetBlendShapeWeight(1, _myMeshRend.GetBlendShapeWeight(1) - _weightChangeStep);
                    }
                }

                if(_myMeshRend.GetBlendShapeWeight(0) <= _eyelidTargetPosUp + _weightChangeStep
                    && _myMeshRend.GetBlendShapeWeight(1) <= _eyelidTargetPosDown + _weightChangeStep)
                {
                    _myMeshRend.SetBlendShapeWeight(0, _eyelidTargetPosUp);
                    _eyebrowMeshRend.SetBlendShapeWeight(0, _myMeshRend.GetBlendShapeWeight(0));
                    _myMeshRend.SetBlendShapeWeight(1, _eyelidTargetPosDown);
                    _eyelidCurrentPosUp = _eyelidTargetPosUp;
                    _eyelidCurrentPosDown = _eyelidTargetPosDown;
                    _isBlinking = false;
                    _areEyelidsDescending = true;
                }

            }
        }
        else
        {
            if (!_isBlinkingStarted)
            {
                StartCoroutine(startBlink());
                _isBlinkingStarted = true;
            }
        }
    }

    IEnumerator startBlink()
    {
        float waitingTime = LvlManager.Instance.BlinkInterval[_blinkIntervalCount];
        _blinkIntervalCount++;
        if(_blinkIntervalCount>=LvlManager.Instance.BlinkInterval.Length)
        {
            _blinkIntervalCount = 0;
        }
        yield return new WaitForSeconds(waitingTime);        
        _isBlinking = true;
        _eyelidTargetPosUp = Random.Range(0, 100);
        if (_eyelidTargetPosUp > 50)
        {
            _eyelidTargetPosDown = Random.Range(0, 75);
        }
        else
        {
            _eyelidTargetPosDown = Random.Range(0, 30);
        }
        _isBlinkingStarted = false;
    }
    void AssignVariables()
    {
        _myMeshRend = gameObject.GetComponent<SkinnedMeshRenderer>();
        _myMeshRend.SetBlendShapeWeight(0, _eyelidCurrentPosUp);
        _myMeshRend.SetBlendShapeWeight(1, _eyelidCurrentPosDown);
        _eyebrowMeshRend = Eyebrow.GetComponent<SkinnedMeshRenderer>();
        _eyebrowMeshRend.SetBlendShapeWeight(0, _eyelidCurrentPosUp);
    }
}
