using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] Transform _myBallAvatar;
    [SerializeField] VirusEyes _myEyeL, _myEyeR;
    [SerializeField] VirusMouth _myMouth;
    int _enemyHits = 0;
    int _playerHits = 0;
    float _squizeFactor = 0.25f;
    float _minPosY = 0;    
    float _ballForcePlayer, _ballForceEnemy, _ballForce, _netXPos, _netTopYPos;
    float _delay = 0.04f;
    Vector3 _scaleSpeedX = new Vector3(0.01f, 0, 0);
    Vector3 _scaleSpeedY = new Vector3(0, 0.01f, 0);
    Rigidbody2D _myRigidbody;
    AudioSource _myAudioSource;
    Collision2D _col;
    [SerializeField] Enemy _myEnemy;

    void Start()
    {
        AssignVariables();
    }
    void FixedUpdate()
    {
        BallScaleBack();        
    }
    void OnCollisionEnter2D(Collision2D currentCollision)
    {
        TrajectoryManager.Instance.RemoveTrajectory();
        _myEyeR.Blink();
        _myEyeL.Blink();
        _myMouth.CollisionClose();
        _myAudioSource.volume = AudioMgr.Instance.GetSoundVolume();
        _myAudioSource.Play();
        CheckCollisions(currentCollision);        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        _myRigidbody.isKinematic = false;
        _myRigidbody.mass = 0.9f;
        TrajectoryManager.Instance.SetIfGameStarted(true);
        for (int i = 0; i < GetComponents<Collider2D>().Length; i++)
        {
            if (GetComponents<Collider2D>()[i].isTrigger)
            {
                GetComponents<Collider2D>()[i].enabled = false;
            }
        }
    }

    public void ResetHits()
    {
        _playerHits = 0;
        _enemyHits = 0;
    }
    public void Squize()
    {
        if (_col != null)
        {
            _myBallAvatar.localScale = new Vector3(1 - _squizeFactor * _col.contacts[0].normal.x, 1 - _squizeFactor * _col.contacts[0].normal.y, _myBallAvatar.localScale.z);
        }
    }
    public void BallScaleBack()
    {
        if (_myBallAvatar.localScale.x < 0.99f)
        {
            _myBallAvatar.localScale += _scaleSpeedX;
        }
        else if (_myBallAvatar.localScale.x > 0.99f)
        {
            _myBallAvatar.localScale -= _scaleSpeedX; 
        }
        else
        {
            _myBallAvatar.localScale = new Vector3(1, _myBallAvatar.localScale.y, 1);
        }

        if (_myBallAvatar.localScale.y < 0.99f)
        {
            _myBallAvatar.localScale += _scaleSpeedY; 
        }
        else if (_myBallAvatar.localScale.y > 0.99f)
        {
            _myBallAvatar.localScale -= _scaleSpeedY; 
        }
        else
        {
            _myBallAvatar.localScale = new Vector3(_myBallAvatar.localScale.x, 1, _myBallAvatar.localScale.z);
        }
    }
    void AssignVariables()
    {
        _netXPos = LvlManager.Instance.GetNetEdgeR();
        _netTopYPos = LvlManager.Instance.GetNetTop();
        _myAudioSource = GetComponent<AudioSource>();
        _myEnemy = LvlManager.Instance.LvlEnemy;
        LvlManager.Instance.CreatePlayer();
        _myRigidbody = GetComponent<Rigidbody2D>();
        _ballForcePlayer = LvlManager.Instance.GetBallForcePlayer();
        _ballForceEnemy = LvlManager.Instance.GetBallForceEnemy();
        _ballForce = _ballForcePlayer;
    }
    void CheckCollisions(Collision2D collision)
    {
        _col = collision;
        if (!collision.gameObject.GetComponent<TagWall>()) // Using a component (empty class) is much faster and cleaner than using Tags
        {
            if (collision.gameObject.GetComponent<TagEnemy>())
            {
                _myEnemy.HitsToMistakeMinusOne();
                Squize();
                TrajectoryManager.Instance.SetCanChangeTrajTopPos(true);
                _ballForce = _ballForceEnemy;
                _playerHits = 0;
                if (_minPosY == 0)
                {
                    _minPosY = LvlManager.Instance.GetFloorSurface() + _myEnemy.GetHeight();
                }
                if ((transform.position.y <= _minPosY || TrajectoryManager.Instance.GetTrajectoryTopPos().y < _minPosY + 2
                    && TrajectoryManager.Instance.GetTrajectoryTopPos().y > _minPosY)
                    && GameMgr.Instance.GetDifficulty() == CoronaBall.Difficulty.veryHard)
                {
                    StartCoroutine(EmergencyHit());
                }
                if (transform.position.x >= _netXPos && transform.position.x <= _netTopYPos + transform.localScale.x && transform.position.y < _netTopYPos)
                {
                    StartCoroutine(UnderNetHit());
                }

                if (_enemyHits <= 2)
                {
                    _enemyHits++;
                }
                else
                {
                    _myEnemy.ResetHitsToMistake();
                    LvlManager.Instance.Scored(true);
                    _enemyHits = 0;
                };
                if (_myEnemy.GetIsServing())
                {
                    _myEnemy.SetIsServing(false);
                }

            }
            if (collision.gameObject.GetComponent<TagPlayer>())
            {
                Squize();
                _ballForce = _ballForcePlayer;
                TrajectoryManager.Instance.SetCanChangeTrajTopPos(true);
                _enemyHits = 0;
                if (_playerHits <= 2)
                {
                    _playerHits++;
                }
                else
                {
                    LvlManager.Instance.Scored(false);
                    _playerHits = 0;
                }
            }
            if (collision.gameObject.GetComponent<TagFloor>())
            {
                _myBallAvatar.transform.localScale = new Vector3(1, 1, _myBallAvatar.localScale.z);
                _myRigidbody.velocity /= new Vector2(1.5f, 3f);

                if (transform.position.x > LvlManager.Instance.Net.position.x)
                {
                    _myEnemy.ResetHitsToMistake();
                    LvlManager.Instance.Scored(true);
                }
                else
                {
                    LvlManager.Instance.Scored(false);
                }
                TrajectoryManager.Instance.enabled = false;

            }
            else if (collision.contacts[0].normal.x < 0.5f * collision.contacts[0].normal.y)
            {
                Squize();
                StartCoroutine(DelayAddForce_Physical());
            }
            else
            {
                Squize();
                StartCoroutine(DelayAddForce_Normal());
            }
        }
        else
        {
            Squize();
        }
    }
    IEnumerator DelayAddForce_Physical()
    {
        _myRigidbody.velocity = _ballForce * _myRigidbody.velocity.normalized;
        yield return new WaitForSeconds(_delay);
        Vector2 direction = _myRigidbody.velocity.normalized;
        _myRigidbody.velocity = new Vector2(0, 0);        
        _myRigidbody.AddForce(_ballForce * direction, ForceMode2D.Impulse);        
    }
    IEnumerator DelayAddForce_Normal()
    {
        yield return new WaitForSeconds(_delay);
        _myRigidbody.velocity = new Vector2(0, 0);
        _myRigidbody.AddForce(_ballForce * _col.contacts[0].normal, ForceMode2D.Impulse);
    }
    IEnumerator EmergencyHit()
    {
        yield return new WaitForFixedUpdate();
        _myRigidbody.velocity = new Vector2(0, 0);
        _myRigidbody.AddForce(_ballForce * new Vector2(0, 1), ForceMode2D.Impulse);
    }
    IEnumerator UnderNetHit()
    {
        yield return new WaitForFixedUpdate();
        _myRigidbody.velocity = new Vector2(0, 0);
        _myRigidbody.AddForce(_ballForce * new Vector2(-0.1f, 0.9f), ForceMode2D.Impulse);
    }
    public void SetMinPosY(float posY)
    {
        _minPosY = posY;
    }
}
