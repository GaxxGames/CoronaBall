using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusEyes : MonoBehaviour
{
    public Transform IrisTransform;
    bool _isRotating = false; 
    bool _isBlinking = false;
    float _blinkSpeed = 500;
    float _rotationSpeed = 75;
    float _eyelidCurrentPosUp = 0;
    float _eyelidCurrentPosDown = 0;
    float _targetRotation = 180;
    int _weightChangeStep = 10;
    int _RandIntervalID = 15;
    bool _isCounting = false;
    bool _isDescending = true;
    SkinnedMeshRenderer _myMeshRend;

    void Start()
    {
        AssignVariables();
        StartCoroutine(CountToNextRotation());
    }

    void Update()
    {
        BlinkingUpdate();
        Rotating();
    }


    public void Blink()
    {
        if (!_isBlinking)
        {
            _isBlinking = true;
            _eyelidCurrentPosUp = 100;
            _eyelidCurrentPosDown = 100;
            _myMeshRend.SetBlendShapeWeight(1, 100);
            _myMeshRend.SetBlendShapeWeight(0, 100);
            _isDescending = false;
        }
    }
    void BlinkingUpdate()
    {

        if (_isBlinking)
        {
            if (_isDescending)
            {
                if (_myMeshRend.GetBlendShapeWeight(0) < 100)
                {
                    _eyelidCurrentPosUp += _blinkSpeed * Time.deltaTime;

                    if (_eyelidCurrentPosUp >= _myMeshRend.GetBlendShapeWeight(0) + _weightChangeStep)
                    {
                        _myMeshRend.SetBlendShapeWeight(0, _myMeshRend.GetBlendShapeWeight(0) + _weightChangeStep);
                    }
                }
                else
                {
                    _myMeshRend.SetBlendShapeWeight(0, 100);
                    _eyelidCurrentPosUp = 100;
                }

                if (_myMeshRend.GetBlendShapeWeight(1) < 100)
                {
                    _eyelidCurrentPosDown += _blinkSpeed * Time.deltaTime;

                    if (_eyelidCurrentPosDown >= _myMeshRend.GetBlendShapeWeight(1) + _weightChangeStep)
                    {
                        _myMeshRend.SetBlendShapeWeight(1, _myMeshRend.GetBlendShapeWeight(1) + _weightChangeStep);
                    }
                }
                else
                {
                    _myMeshRend.SetBlendShapeWeight(1, 100);
                    _eyelidCurrentPosDown = 100;
                }

                if (_myMeshRend.GetBlendShapeWeight(0) >= 100 && _myMeshRend.GetBlendShapeWeight(1) >= 100)
                {
                    _isDescending = false;
                }
            }
            else
            {
                if (_myMeshRend.GetBlendShapeWeight(0) > 0)
                {
                    _eyelidCurrentPosUp -= _blinkSpeed * Time.deltaTime;

                    if (_eyelidCurrentPosUp <= _myMeshRend.GetBlendShapeWeight(0) - _weightChangeStep)
                    {
                        _myMeshRend.SetBlendShapeWeight(0, _myMeshRend.GetBlendShapeWeight(0) - _weightChangeStep);
                    }
                }
                else
                {
                    _myMeshRend.SetBlendShapeWeight(0, 0);
                    _eyelidCurrentPosUp = 0;
                }
                if (_myMeshRend.GetBlendShapeWeight(1) > 0)
                {
                    _eyelidCurrentPosDown -= _blinkSpeed * Time.deltaTime;

                    if (_eyelidCurrentPosDown <= _myMeshRend.GetBlendShapeWeight(1) - _weightChangeStep)
                    {
                        _myMeshRend.SetBlendShapeWeight(1, _myMeshRend.GetBlendShapeWeight(1) - _weightChangeStep);
                    }
                }
                else
                {
                    _myMeshRend.SetBlendShapeWeight(1, 0);
                    _eyelidCurrentPosDown = 0;
                }

                if (_myMeshRend.GetBlendShapeWeight(0) <= 0 && _myMeshRend.GetBlendShapeWeight(1) <= 0)
                {
                    _isBlinking = false;
                    _isDescending = true;
                }
            }
        }
        else
        {
            if (!_isCounting)
            {
                _isCounting = true;
                StartCoroutine(CountToNextBlink());
            }
        }
    }
    void Rotating()
    {
        if (!_isRotating) return;

        if (IrisTransform.localEulerAngles.z <= _targetRotation)
        {
            TeczowkaRotate(_rotationSpeed);
        }
        else if (IrisTransform.localEulerAngles.z > _targetRotation + 3)
        {
            TeczowkaRotate(-_rotationSpeed);
        }
        else
        {
            _isRotating = false;
            int random = Random.Range(0, 100);
            if (_targetRotation == 359)
            {
                if (random % 2 == 0)
                {
                    _targetRotation = 0;
                }
                else
                {
                    _targetRotation = 180;
                }
            }
            else if (_targetRotation == 180)
            {
                if (random % 2 == 0)
                {
                    _targetRotation = 0;
                }
                else
                {
                    _targetRotation = 359;
                }
            }
            else if (_targetRotation == 0)
            {
                if (random % 2 == 0)
                {
                    _targetRotation = 359;
                }
                else
                {
                    _targetRotation = 180;
                }

            }
            StartCoroutine(CountToNextRotation());
        }
    }
    void TeczowkaRotate(float speed)
    {
        IrisTransform.Rotate(new Vector3(0, 0, 1), speed * Time.deltaTime);
    }
    void AssignVariables()
    {

        _myMeshRend = gameObject.GetComponent<SkinnedMeshRenderer>();
        _myMeshRend.SetBlendShapeWeight(0, _eyelidCurrentPosUp);
        _myMeshRend.SetBlendShapeWeight(0, _eyelidCurrentPosDown);
    }
    IEnumerator CountToNextBlink()
    {
        yield return new WaitForSeconds(3);

        if(!_isBlinking)
        {
            _isBlinking = true;
        }
        _isCounting = false;
    }
    IEnumerator CountToNextRotation()
    {
        if (LvlManager.Instance)
        {
            yield return new WaitForSeconds(LvlManager.Instance.BlinkInterval[_RandIntervalID]);
            _RandIntervalID++;
            if (_RandIntervalID >= LvlManager.Instance.BlinkInterval.Length)
            {
                _RandIntervalID = 0;
            }
        }
        else
        {
            yield return new WaitForSeconds(3);
        }
        _isRotating = true;
    }
}
