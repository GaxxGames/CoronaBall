using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoronaBall;

public class Player : MonoBehaviour
{
    public GameObject Ball;
    [SerializeField] MouthBehaviour _mouth;
    [SerializeField] Animator _myAnimator;
    [SerializeField] MovingTexture _myOutline;
    Rigidbody2D _myRigidbody;
    bool _isKeyDownRightArrow = false;
    bool _isKeyDownLeftArrow = false;
    float _idleAnimSpeed = 0.5f;
    float _walkAnimSpeed = 3f;
    float _speed, _jumpSpeed, _defaultOutlineTexSpeedX;
    float _moveX = 0;
    string _strIsPunching = "isPunching";
    string _strSpeed = "speed";
    string _strJump = "Jump";
    string _strIsWalking = "isWalking";

    void Start()
    {
        AssignVariables();
    }

    void Update()
    {
        HandleInput();
    }
    void FixedUpdate()
    {
        _myRigidbody.velocity = new Vector2(_moveX * _speed, _myRigidbody.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<TagBall>())
        {
            _myAnimator.SetBool(_strIsPunching, true);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<TagBall>())
        {
            _myAnimator.SetBool(_strIsPunching, false);
        }
    }

    public void Jump()
    {
        if (Mathf.Abs(_myRigidbody.velocity.y) < 0.05f)
        {
            _myRigidbody.AddForce(new Vector2(0, _jumpSpeed), ForceMode2D.Impulse);
            _myAnimator.SetBool(_strIsPunching, true);
            StartCoroutine(SetIsPunchingFalse());
        }
    }
    public void MoveLeft()
    {
        _moveX = -_speed;
        _myAnimator.SetFloat(_strSpeed, -_walkAnimSpeed);
        _myAnimator.SetBool(_strIsWalking, true);
        _myOutline.SetSpeed(new Vector2(-3f * _defaultOutlineTexSpeedX, 0));
        _mouth.SetAnimSpeed(_mouth.GetWalkAnimSpeed());
    }
    public void MoveRight()
    {
        _moveX = _speed;
        _myAnimator.SetFloat(_strSpeed, _walkAnimSpeed);
        _myAnimator.SetBool(_strIsWalking, true);
        _myOutline.SetSpeed(new Vector2(3f * _defaultOutlineTexSpeedX, 0));
        _mouth.SetAnimSpeed(_mouth.GetWalkAnimSpeed());
    }
    public void MoveStop()
    {
        _moveX = 0;
        _myAnimator.SetFloat(_strSpeed,Random.Range(_idleAnimSpeed - (0.2f * _idleAnimSpeed), _idleAnimSpeed + (0.2f * _idleAnimSpeed)));
        _myAnimator.SetBool(_strIsWalking, false);       
        _myOutline.SetSpeed(new Vector2(_defaultOutlineTexSpeedX, 0));
        _mouth.SetAnimSpeed(_mouth.GetIdleAnimSpeed());
    }
    public void CheckButtons()
    {
        if (Input.GetButtonDown(_strJump))
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _isKeyDownLeftArrow = true;
            MoveLeft();
        }
        else
        {
            _isKeyDownLeftArrow = false;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            _isKeyDownRightArrow = true;
            MoveRight();
        }
        else
        {
            _isKeyDownLeftArrow = false;
        }
    }
    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }
    IEnumerator SetIsPunchingFalse()
    {
        yield return new WaitForSeconds(0.04f);
        _myAnimator.SetBool(_strIsPunching, false);
    }
    void AssignVariables()
    {
        _myRigidbody = GetComponent<Rigidbody2D>();

        _speed = LvlManager.Instance.GetPlayerSpeed();
        if (GameMgr.Instance.GetDifficulty() == Difficulty.veryEasy)
        {
            SetSpeed(LvlManager.Instance.GetPlayerSpeed() / 0.7f);
        }
        else if (GameMgr.Instance.GetDifficulty() == Difficulty.easy)
        {
            SetSpeed(LvlManager.Instance.GetPlayerSpeed() / 0.8f);
        }
        else if (GameMgr.Instance.GetDifficulty() == Difficulty.normal)
        {
            SetSpeed(LvlManager.Instance.GetPlayerSpeed() / 0.9f);
        }
        _jumpSpeed = LvlManager.Instance.GetPlayerJumpSpeed();
        _defaultOutlineTexSpeedX = _myOutline.GetDefaultSpeedX();
    }
    void HandleInput()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            if (Input.GetButtonDown(_strJump))
            {
                Jump();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _isKeyDownLeftArrow = true;
                MoveLeft();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _isKeyDownRightArrow = true;
                MoveRight();
            }
            if (!(_isKeyDownLeftArrow && _isKeyDownRightArrow))
            {
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    _isKeyDownLeftArrow = false;
                    MoveStop();
                }

                if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    _isKeyDownRightArrow = false;
                    MoveStop();
                }
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    _isKeyDownLeftArrow = false;
                }

                if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    _isKeyDownRightArrow = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                TouchReaction(Input.touches[i]);
            }
        }
    }
    void TouchReaction(Touch dotyk)
    {
        if (dotyk.position.x < Screen.width / 2)
        {
            Vector3 dotyk1Pos = Camera.main.ScreenToWorldPoint(dotyk.position);
            if (transform.position.x > dotyk1Pos.x + 0.1f)
            {
                MoveLeft();
            }
            else if (transform.position.x < dotyk1Pos.x - 0.1f)
            {
                MoveRight();
            }
            else
            {
                MoveStop();
            }

            if (dotyk.phase == TouchPhase.Ended || dotyk.phase == TouchPhase.Canceled)
            {
                MoveStop();
            }
        }
        else
        {
            Jump();
        }
    }
}

