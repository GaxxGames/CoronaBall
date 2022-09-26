using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoronaBall;

public class Enemy : MonoBehaviour
{
    public GameObject Ball;
    public List<Transform> Hitpoints;
    public List<Vector2> HitpointShifts;
    [SerializeField] GameObject _blockCollider;
    [SerializeField] Transform  _groundPoint;
    [SerializeField] Animator _myAnimator;
    [SerializeField] MouthBehaviour _mouth;
    [SerializeField] MovingTexture _myOutline;

    int _hitsToMistake = 0;
    int _mistakeFrequency = 0;
    int _chosenHitpointId = 3;
    int _fixedFramesToCollision;
    float _idleAnimSpeed = 0.5f;
    float _walkAnimSpeed = 3f;
    float _predictedCollisionPointX = 1988;
    float _centerOfEnemyHalf = 0;
    float _defaultOutlineTexSpeedX = 3;
    float _mistakeShift = -1.5f; 
    float _netPosX = 0;
    float _moveX = 0;
    float _ballRadious, _speed, _jumpSpeed, _height, _jumpHeight, _stepX;
    bool _isWaiting = true;
    bool _timeToMakeMistake = false;
    bool _isServing = false;
    bool _isCountingFramesToCollision = false;
    bool _canJump;
    string _isPunchingStr = "isPunching";
    string _speedStr = "speed";
    string _isWalkingStr = "isWalking";
    Difficulty _myDifficulty;
    Rigidbody2D _ballRB, _myRigidbody;
    Vector2 _chosenHitpointPos;

    void Start()
    {
        AssignVariables();
        ResetHitsToMistake();
    }

    void FixedUpdate()
    {
        CheckIfTimeToMakeMistake();
        ReactToBall();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<TagBall>())
        {
            _myAnimator.SetBool(_isPunchingStr, true);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<TagBall>())
        {
            _myAnimator.SetBool(_isPunchingStr, false);
        }
    }

    public void Jump()
    {
        if (Mathf.Abs(_myRigidbody.velocity.y) < 0.1f)
        {
            _myRigidbody.AddForce(new Vector2(0, _jumpSpeed), ForceMode2D.Impulse);
            _myAnimator.SetBool(_isPunchingStr, true);
            StartCoroutine(SetIsPunchingFalse());
        }
        SetCanJump(false);
        _fixedFramesToCollision = -1;
    }
    public void ForcedJump()
    {
        _myRigidbody.AddForce(new Vector2(0, _jumpSpeed), ForceMode2D.Impulse);
        _myAnimator.SetBool(_isPunchingStr, true);
        StartCoroutine(SetIsPunchingFalse());
        SetCanJump(false);
        _fixedFramesToCollision = -1;
    }
    public void MoveLeft()
    {
        _moveX = -_speed;
        _myAnimator.SetFloat(_speedStr, -_walkAnimSpeed);
        _myAnimator.SetBool(_isWalkingStr, true);
        _myOutline.SetSpeed(new Vector2(-3f * _defaultOutlineTexSpeedX, 0));
        _mouth.SetAnimSpeed(_mouth.GetWalkAnimSpeed());
    }
    public void MoveRight()
    {
        _moveX = _speed;
        _myAnimator.SetFloat(_speedStr, _walkAnimSpeed);
        _myAnimator.SetBool(_isWalkingStr, true);
        _myOutline.SetSpeed(new Vector2(3f * _defaultOutlineTexSpeedX, 0));
        _mouth.SetAnimSpeed(_mouth.GetWalkAnimSpeed());
    }
    public void MoveStop()
    {
        _moveX = 0;
        _myAnimator.SetFloat(_speedStr, Random.Range(_idleAnimSpeed - (0.2f * _idleAnimSpeed), _idleAnimSpeed + (0.2f * _idleAnimSpeed)));
        _myAnimator.SetBool(_isWalkingStr, false);
        _myOutline.SetSpeed(new Vector2(_defaultOutlineTexSpeedX, 0));
        _mouth.SetAnimSpeed(_mouth.GetIdleAnimSpeed());
    }
    public void SetChosenHitPointPos(int i)
    {
        _chosenHitpointPos = Hitpoints[i].position;
        _chosenHitpointId = i;        
    }    
    public void SetCanJump(bool ifCan)
    {
        _canJump = ifCan;
    }
    public void StartSet(float delayInSeconds)
    {
        StartCoroutine(StartSetCoroutine(delayInSeconds));
    }
    public bool GetIsServing()
    {
        return _isServing;
    }
    public void SetIsServing(bool isIt)
    {
        _isServing = isIt;
    }
    public void SetHitsToMistake(int hitsCount)
    {
        _hitsToMistake = hitsCount;
    }
    public void HitsToMistakeMinusOne()
    {
        _hitsToMistake--;
    }   
    public void SetMistakeFrequency()
    {
        if (_myDifficulty == Difficulty.veryEasy)
        {
            _mistakeFrequency = Random.Range(3, 10);
        }
        else if (_myDifficulty == Difficulty.easy)
        {
            _mistakeFrequency = Random.Range(6, 12);
        }
        else if (_myDifficulty == Difficulty.normal)
        {
            _mistakeFrequency = Random.Range(10, 20);
        }
        else if (_myDifficulty == Difficulty.hard)
        {
            _mistakeFrequency = Random.Range(15, 30);
        }
        else if (_myDifficulty == Difficulty.veryHard)
        {
            _mistakeFrequency = 100;
        }
    }
    public void ResetHitsToMistake()
    {
        SetMistakeFrequency();
        SetHitsToMistake(_mistakeFrequency);
    }
    public void SetTimeToMakeMistake(bool value)
    {
        _timeToMakeMistake = value;
    }
    public float GetHeight()
    {
        return _height;
    }
    void ReactToBall()
    {
        if (TrajectoryManager.Instance.GetIsTrajejectoryPredicted())
        {
            int predColPosIdHigh = TrajectoryManager.Instance.GetPredictedColPointId(LvlManager.Instance.GetFloorSurface() + _jumpHeight);
            int predColPosIdLow = TrajectoryManager.Instance.GetPredictedColPointId(LvlManager.Instance.GetFloorSurface() + _height);
            int predChangeDirPointID = TrajectoryManager.Instance.GetChangeDirPointId();
            int chosenColPosId;
            float predColPosHighX = TrajectoryManager.Instance.GetPredictedPos(predColPosIdHigh).x;
            float predColPosLowX = TrajectoryManager.Instance.GetPredictedPos(predColPosIdLow).x;

            if (_isCountingFramesToCollision)
            {
                _fixedFramesToCollision--;
            }
            else if (_fixedFramesToCollision < 0)
            {
                _fixedFramesToCollision = 0;
                _isCountingFramesToCollision = false;
            }
            if (predColPosHighX > _netPosX - 0.3 * _ballRadious || predColPosLowX > _netPosX - 0.3 * _ballRadious)
            {
                switch (_myDifficulty)
                {
                    case Difficulty.veryHard:
                        {
                            if (Mathf.Abs(_ballRB.velocity.x) < 1.5 && !_isServing)
                            {
                                SetChosenHitPointPos(0);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else if (Mathf.Abs(_ballRB.velocity.x) < 6 && (predColPosIdHigh < predChangeDirPointID || predChangeDirPointID == -1))
                            {
                                SetChosenHitPointPos(1);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else if (predColPosHighX >= _netPosX - 0.3 * _ballRadious && predColPosHighX <= _netPosX + 0.5)
                            {
                                SetChosenHitPointPos(1);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else
                            {
                                SetChosenHitPointPos(3);
                                SetCanJump(false);
                                chosenColPosId = predColPosIdLow;
                            }

                            FindCollisionPointX(TrajectoryManager.Instance.GetPredictedPos(chosenColPosId).y + HitpointShifts[_chosenHitpointId].y);
                        }
                        break;
                    case Difficulty.hard:
                        {
                            if (Mathf.Abs(_ballRB.velocity.x) < 1.5)
                            {
                                SetChosenHitPointPos(4);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(false);
                            }
                            else if (predColPosHighX >= _netPosX - 0.3 * _ballRadious && predColPosHighX <= _netPosX + 0.5)
                            {
                                SetChosenHitPointPos(1);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else if (Mathf.Abs(_ballRB.velocity.x) < 8 && (predColPosIdHigh < predChangeDirPointID || predChangeDirPointID == -1))
                            {
                                SetChosenHitPointPos(1);

                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else
                            {
                                SetChosenHitPointPos(3);
                                SetCanJump(false);
                                chosenColPosId = predColPosIdLow;
                            }
                            if (_isServing)
                            {
                                SetCanJump(true);
                            }

                            FindCollisionPointX(TrajectoryManager.Instance.GetPredictedPos(chosenColPosId).y + HitpointShifts[_chosenHitpointId].y);
                        }
                        break;
                    case Difficulty.normal:
                        {
                            if (Mathf.Abs(_ballRB.velocity.x) < 1.5)
                            {
                                SetChosenHitPointPos(4);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(false);
                            }
                            else if (predColPosHighX >= _netPosX - 0.3 * _ballRadious && predColPosHighX <= _netPosX + 0.5)
                            {
                                SetChosenHitPointPos(1);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else if (Mathf.Abs(_ballRB.velocity.x) < 8 && (predColPosIdHigh < predChangeDirPointID || predChangeDirPointID == -1))
                            {
                                SetChosenHitPointPos(1);

                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else
                            {
                                SetChosenHitPointPos(3);
                                SetCanJump(false);
                                chosenColPosId = predColPosIdLow;
                            }
                            if (_isServing)
                            {
                                SetCanJump(true);
                            }

                            FindCollisionPointX(TrajectoryManager.Instance.GetPredictedPos(chosenColPosId).y + HitpointShifts[_chosenHitpointId].y);
                        }
                        break;
                    case Difficulty.easy:
                        {
                            if (Mathf.Abs(_ballRB.velocity.x) < 1.5)
                            {
                                SetChosenHitPointPos(4);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(false);
                            }
                            else if (predColPosHighX >= _netPosX - 0.3 * _ballRadious && predColPosHighX <= _netPosX + 0.5)
                            {
                                SetChosenHitPointPos(1);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else if (Mathf.Abs(_ballRB.velocity.x) < 8 && (predColPosIdHigh < predChangeDirPointID || predChangeDirPointID == -1))
                            {
                                SetChosenHitPointPos(1);

                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else
                            {
                                SetChosenHitPointPos(3);
                                SetCanJump(false);
                                chosenColPosId = predColPosIdLow;
                            }
                            if (_isServing)
                            {
                                SetCanJump(true);
                            }

                            FindCollisionPointX(TrajectoryManager.Instance.GetPredictedPos(chosenColPosId).y + HitpointShifts[_chosenHitpointId].y);
                        }
                        break;
                    case Difficulty.veryEasy:
                        {
                            if (Mathf.Abs(_ballRB.velocity.x) < 1.5)
                            {
                                SetChosenHitPointPos(4);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(false);
                            }
                            else if (predColPosHighX >= _netPosX - 0.3 * _ballRadious && predColPosHighX <= _netPosX + 0.5)
                            {
                                SetChosenHitPointPos(1);
                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else if (Mathf.Abs(_ballRB.velocity.x) < 8 && (predColPosIdHigh < predChangeDirPointID || predChangeDirPointID == -1))
                            {
                                SetChosenHitPointPos(1);

                                chosenColPosId = predColPosIdHigh;
                                SetCanJump(true);
                            }
                            else
                            {
                                SetChosenHitPointPos(3);
                                SetCanJump(false);
                                chosenColPosId = predColPosIdLow;
                            }
                            if (_isServing)
                            {
                                SetCanJump(true);
                            }

                            FindCollisionPointX(TrajectoryManager.Instance.GetPredictedPos(chosenColPosId).y + HitpointShifts[_chosenHitpointId].y);
                        }
                        break;
                }
            }
        }
        else
        {
            _predictedCollisionPointX = 1988;
            _isWaiting = false;
        }

        if (_predictedCollisionPointX != 1988 && _predictedCollisionPointX > LvlManager.Instance.Net.position.x - 0.5 * Ball.transform.localScale.x
            && _predictedCollisionPointX < LvlManager.Instance.GetWallEdgeR())
        {
            if (!_isWaiting)
            {
                if (_timeToMakeMistake)
                {
                    if (_predictedCollisionPointX + _mistakeShift < _chosenHitpointPos.x)
                    {
                        MoveLeft();
                    }
                    else if (_predictedCollisionPointX + _mistakeShift > _chosenHitpointPos.x)
                    {
                        MoveRight();
                    }

                    if (_predictedCollisionPointX + _mistakeShift >= _chosenHitpointPos.x - 1.5f * _stepX
                        && _predictedCollisionPointX + _mistakeShift <= _chosenHitpointPos.x + 1.5f * _stepX)
                    {
                        _myRigidbody.MovePosition(new Vector2(_predictedCollisionPointX + _mistakeShift - Hitpoints[_chosenHitpointId].localPosition.x, transform.position.y));
                        MoveStop();
                        _isWaiting = true;
                    }
                    _timeToMakeMistake = false;
                }
                else
                {
                    if (_predictedCollisionPointX < _chosenHitpointPos.x)
                    {
                        MoveLeft();
                    }
                    else if (_predictedCollisionPointX > _chosenHitpointPos.x)
                    {
                        MoveRight();
                    }

                    if (_predictedCollisionPointX >= _chosenHitpointPos.x - 1.5f * _stepX
                        && _predictedCollisionPointX <= _chosenHitpointPos.x + 1.5f * _stepX)
                    {
                        _myRigidbody.MovePosition(new Vector2(_predictedCollisionPointX - Hitpoints[_chosenHitpointId].localPosition.x, transform.position.y));
                        MoveStop();
                        _isWaiting = true;
                    }
                }
            }
        }
        else
        {
            if (transform.position.x < _centerOfEnemyHalf - 0.5f)
            {
                MoveRight();
            }
            else if (transform.position.x > _centerOfEnemyHalf + 0.5f)
            {
                MoveLeft();
            }
            else
            {
                MoveStop();
            }
            _isWaiting = true;
        }

        if (_fixedFramesToCollision == 25)
        {
            if (_canJump && Ball.transform.position.x > _netPosX - Ball.transform.localScale.x)
            {
                Jump();
            }
            else if (Mathf.Abs(_ballRB.velocity.x) < 1.5 || transform.position.x <= _netPosX + Ball.transform.localScale.x)
            {
                ForcedJump();
            }
        }

        if (_myRigidbody.velocity.y > 13.14114)
        {
            _myRigidbody.velocity = new Vector2(_moveX * _speed, 13.14114f);
        }
        else
        {
            _myRigidbody.velocity = new Vector2(_moveX * _speed, _myRigidbody.velocity.y);
        }
    }
    void CheckIfTimeToMakeMistake()
    {
        if (_hitsToMistake <= 0)
        {
            _timeToMakeMistake = true;
        }
    }
    void AssignVariables()
    {
        _blockCollider.SetActive(true);
        _blockCollider.GetComponent<SpriteRenderer>().enabled = false;
        _blockCollider.SetActive(false);
        _myDifficulty = GameMgr.Instance.GetDifficulty();
        Ball = LvlManager.Instance.Ball;
        _ballRadious = Ball.transform.localScale.x * 0.6f;
        _myRigidbody = GetComponent<Rigidbody2D>();
        _ballRB = Ball.GetComponent<Rigidbody2D>();
        for (int i = 0; i < Hitpoints.Count; i++)
        {
            HitpointShifts.Add(Hitpoints[i].localPosition);
        }
        _speed = 1.8f;
        _jumpSpeed = LvlManager.Instance.GetPlayerJumpSpeed();
        _stepX = _speed * Time.fixedDeltaTime;
        _chosenHitpointPos = Hitpoints[3].transform.position;
        _height = Hitpoints[3].localPosition.y - _groundPoint.localPosition.y;
        _jumpHeight = 1f * (_height + 4);
        if (LvlManager.Instance.GetPlaygroundSectionInterval() == 0)
        {
            LvlManager.Instance.AssignVariables();
        }
        _netPosX = LvlManager.Instance.Net.position.x;
        _centerOfEnemyHalf = LvlManager.Instance.Net.position.x + 2 * LvlManager.Instance.GetPlaygroundSectionInterval();
        _defaultOutlineTexSpeedX = _myOutline.GetDefaultSpeedX();
        if (_myDifficulty == Difficulty.veryEasy)
        {
            _speed *= 0.9f;
        }
        else if (_myDifficulty == Difficulty.easy)
        {
            _speed *= 1f;
        }
        else if (_myDifficulty == Difficulty.hard)
        {
            _speed *= 1.1f;
        }
        else if (_myDifficulty == Difficulty.veryHard)
        {
            _speed *= 1.2f;
        }
    }
    void FindCollisionPointX(float y)
    {
        int id = TrajectoryManager.Instance.GetPredictedColPointId(_chosenHitpointPos.y);
        if (TrajectoryManager.Instance.GetPredictedPos(id).x != _predictedCollisionPointX)
        {
            _fixedFramesToCollision = id;
            _predictedCollisionPointX = TrajectoryManager.Instance.GetPredictedPos(id).x;
            _isCountingFramesToCollision = true;
            _isWaiting = false;
        }
    }
    IEnumerator StartSetCoroutine(float sec)
    {
        yield return new WaitForSeconds(sec);
        Jump();
    }
    IEnumerator SetIsPunchingFalse()
    {
        yield return new WaitForSeconds(0.04f);
        _myAnimator.SetBool(_isPunchingStr, false);
    }
}
