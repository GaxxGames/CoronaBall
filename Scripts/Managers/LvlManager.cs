using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using CoronaBall;

public class LvlManager : MonoBehaviour
{
    static LvlManager _instance;
    public static LvlManager Instance { get { return _instance; } }

    public Enemy LvlEnemy;
    public Player LvlPlayer;
    public AmebaColor PlayerColor = AmebaColor.green;
    public AmebaColor EnemyColor = AmebaColor.orange;
    public VideoPlayer BackgroundPlayer;
    public SpriteRenderer BackgroundImage;
    public GameObject Ball;
    public GameObject PrefPlayerGreen, PrefPlayerRed, PrefPlayerBlue, PrefPlayerTurkus, PrefPlayerOrange;
    public GameObject PrefEnemyGreen, PrefEnemyRed, PrefEnemyBlue, PrefEnemyTurkus, PrefEnemyOrange;
    public PhysicsMaterial2D BallMatEnd, BallMat, FloorMat;
    public Transform Wall_L, Wall_R, Floor, Net, PlayerStartPos, EnemyStartPos;
    public float[] BlinkInterval;

    AudioSource _myAudioSource;    
    GameObject _player, _enemy;    
    Vector2 _ballToPlayerShift;   
    Quaternion _ballRot;
    Rigidbody2D _lvlEnemyRB, _lvlPlayerRB;
    Rigidbody2D _ballRB;
    Camera _cam;
    int PlayerPoints = 0;
    int EnemyPoints = 0;
    int PointsToWin = 5;
    float _jumpSpeed = 700f;
    float _ballForcePlayer = 9f;
    float _ballForceEnemy = 10f;
    float _wallEdgeR=0;
    float _wallEdgeL=0;
    float _floorSurface=0;
    float _netTop=0;
    float _ballRadious=0;
    float _netEdgeL=0;
    float _netEdgeR=0;
    float _playgroundSectionInterval = 0;
    float _speed = 2f;
    float _lvlTimescale = 1;
    bool _ifWin = true;
    bool _ifVariablesAssigned = false;
    bool _ifJustScored = false;
    bool _isPLayerStartSet;

    private void Awake()
    {
        SetSingleton();
        CreatePlayer();
        if (!LvlEnemy)
        {
            CreateEnemy();
        }
        else
        {
            _enemy = LvlEnemy.transform.parent.gameObject;
        }
        SyncEyes();        
    }

    void Start()
    {
        AdMgr.Instance.LoadAd();
        AssignVariables();        
        StartCoroutine(DeactivateBackgroundImage());
    }

    public GameObject GetBall()
    {
        return Ball;
    }
    public void AssignVariables()
    {
        if (!GetComponent<AudioSource>())
        {
            gameObject.AddComponent<AudioSource>();
        }
        _myAudioSource = GetComponent<AudioSource>();
        _myAudioSource.clip = AudioMgr.Instance.GetWhistleBlowAudioclip();        
        _myAudioSource.volume = AudioMgr.Instance.GetSoundVolume();
        Time.timeScale = 1;
        _cam = Camera.main;
        // Make Game works with all possible screen sizes by adjusting net, walls and floor:
        Vector3 ScreenCoordFloor = new Vector3(Screen.width / 2, 0.2f * Screen.height, 10);
        Vector3 ScreenCoordWall_L = new Vector3(0, 0, 10);
        Vector3 ScreenCoordWall_R = new Vector3(Screen.width, 0, 10);
        Floor.transform.position = _cam.ScreenToWorldPoint(ScreenCoordFloor);
        Wall_L.parent.transform.position = _cam.ScreenToWorldPoint(ScreenCoordWall_L);
        Wall_R.parent.transform.position = _cam.ScreenToWorldPoint(ScreenCoordWall_R);
        LvlMgrGUI.Instance.GetArrow().transform.position = _cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, 10));
        _ballRadious = 0.5f * Ball.transform.localScale.x;
        _wallEdgeL = Wall_L.position.x + 0.5f * Wall_L.localScale.y + _ballRadious;
        _wallEdgeR = Wall_R.position.x - 0.5f * Wall_L.localScale.y - _ballRadious;
        _floorSurface = Floor.position.y + 0.5f * Floor.localScale.y + _ballRadious;
        _netTop = Net.position.y + 0.5f * Net.localScale.x;
        _netEdgeL = Net.position.x - 0.5f * Net.localScale.y + _ballRadious;
        _netEdgeR = Net.position.x + 0.5f * Net.localScale.y - _ballRadious;
        //Half of playground is divided for 4 sections to program different enemy behaviour on each section
        _playgroundSectionInterval = Mathf.Abs(_wallEdgeR - Net.transform.position.x) / 4;
        _ifVariablesAssigned = true;
        _ballToPlayerShift = Ball.transform.position - _player.transform.position;
        _ballRot = Ball.transform.rotation;
        _ballRB = Ball.GetComponent<Rigidbody2D>();
        _lvlEnemyRB = LvlEnemy.GetComponent<Rigidbody2D>();
        float pointsTextDistanceFromCorner = 0.05f;
        LvlMgrGUI.Instance.AssignVariables(pointsTextDistanceFromCorner);
        LvlMgrGUI.Instance.ClosePanels();

        if (GameMgr.Instance.GetDifficulty() == Difficulty.veryEasy)
            _lvlTimescale = 0.7f;        
        else if (GameMgr.Instance.GetDifficulty() == Difficulty.easy)
            _lvlTimescale = 0.8f;        
        else if (GameMgr.Instance.GetDifficulty() == Difficulty.normal)
            _lvlTimescale = 0.9f;        
        else
            _lvlTimescale = 1f;       

        Time.timeScale = _lvlTimescale;

        if (BackgroundImage)
            BackgroundImage.gameObject.SetActive(true);
    }
    public void PauseStart()
    {
        Time.timeScale = 0;
    }
    public void PauseEnd()
    {
        Time.timeScale = _lvlTimescale;
    }
    public void Scored(bool ifPlScored)
    {
        if (!_ifJustScored)
        {
            _myAudioSource.volume = AudioMgr.Instance.GetSoundVolume()*0.4f;
            _myAudioSource.Play();
            if (ifPlScored)
            {
                _isPLayerStartSet = true;
                PlayerPoints++;
                LvlMgrGUI.Instance.Scored(true, PlayerPoints, EnemyPoints);
            }
            else
            {
                _isPLayerStartSet = false;
                EnemyPoints++;
                LvlMgrGUI.Instance.Scored(false, PlayerPoints, EnemyPoints);
            }
            TrajectoryManager.Instance.SetIfGameStarted(false);
            TrajectoryManager.Instance.enabled = false;

            LvlPlayer.MoveStop();
            LvlEnemy.MoveStop();
            LvlEnemy.enabled = false;
            LvlPlayer.enabled = false;
            Ball.layer = 10;
            Ball.GetComponent<Rigidbody2D>().sharedMaterial = BallMatEnd;
            Floor.GetComponent<Collider2D>().sharedMaterial = BallMatEnd;
            _ifJustScored = true;
            _lvlPlayerRB.velocity = new Vector2(0, 0);
            _lvlEnemyRB.velocity = new Vector2(0, 0);
            
            Ball.GetComponent<Ball>().ResetHits();
            if (PlayerPoints >= PointsToWin)
            {
                LvlMgrGUI.Instance.Endgame(true);
            }
            else if (EnemyPoints >= PointsToWin)
            {
                LvlMgrGUI.Instance.Endgame(false);
            }
            else
            {
                StartCoroutine(StartNewSetAfterXSec(3));
            }
        }    
    }
    public void StartNewSet()
    {
        LvlPlayer.transform.position = PlayerStartPos.position;
        LvlEnemy.transform.position = EnemyStartPos.position;

        if (_isPLayerStartSet)
        {
            Ball.transform.position = PlayerStartPos.position + new Vector3(_ballToPlayerShift.x, _ballToPlayerShift.y, PlayerStartPos.position.z);
        }
        else
        {
            Ball.transform.position = new Vector2(EnemyStartPos.position.x - _ballToPlayerShift.x, EnemyStartPos.position.y + _ballToPlayerShift.y);
            LvlEnemy.StartSet(2);
        }
        TrajectoryManager.Instance.enabled = true;
        LvlEnemy.enabled = true;
        LvlPlayer.enabled = true;
        LvlPlayer.CheckButtons();
        _ballRB.isKinematic = true;
        Ball.layer = 7;
        _ballRB.sharedMaterial = BallMat;
        Floor.GetComponent<Collider2D>().sharedMaterial = FloorMat;
        _ifJustScored = false;
        TrajectoryManager.Instance.SetIfGameStarted(true);
        for (int i = 0; i < Ball.GetComponents<Collider2D>().Length; i++)
        {
            if (Ball.GetComponents<Collider2D>()[i].isTrigger)
            {
                Ball.GetComponents<Collider2D>()[i].enabled = true;
            }
        }
        _ballRB.velocity = new Vector2(0, 0);
        Ball.transform.rotation = _ballRot;
        _ballRB.angularVelocity = 0;
        _lvlPlayerRB.velocity = new Vector2(0, 0);
        _lvlEnemyRB.velocity = new Vector2(0, 0);
        LvlPlayer.MoveStop();
        LvlEnemy.MoveStop();
        Ball.GetComponent<Ball>().ResetHits();
        LvlEnemy.SetIsServing(true);
    }
    public bool GetIfWin()
    {
        return _ifWin;
    }
    public float GetPlayerSpeed()
    {
        return _speed;
    }
    public float GetPlayerJumpSpeed()
    {
        return _jumpSpeed;
    }
    public float GetBallForcePlayer()
    {
        return _ballForcePlayer;
    }
    public float GetBallForceEnemy()
    {
        return _ballForceEnemy;
    }
    public float GetWallEdgeR()
    {
        return _wallEdgeR;
    }
    public float GetWallEdgeL()
    {
        return _wallEdgeL;
    }
    public float GetFloorSurface()
    {
        return _floorSurface;
    }
    public float GetNetTop()
    {
        return _netTop;
    }
    public float GetNetEdgeL()
    {
        return _netEdgeL;
    }
    public float GetNetEdgeR()
    {
        return _netEdgeR;
    }
    public float GetPlaygroundSectionInterval()
    {
        return _playgroundSectionInterval;
    }
    public bool GetIfVariablesAssigned()
    {
        return _ifVariablesAssigned;
    }   
    public void CreatePlayer()
    {
        PlayerColor = GameMgr.Instance.GetPlayerColor();

        if (!_player)
        {
            if (PlayerColor == AmebaColor.green)
            {
                _player = Instantiate(PrefPlayerGreen, PlayerStartPos.position, PlayerStartPos.localRotation, null);
            }
            else if(PlayerColor == AmebaColor.blue)
            {
                _player = Instantiate(PrefPlayerBlue, PlayerStartPos.position, PlayerStartPos.localRotation, null);
            }
            else if(PlayerColor == AmebaColor.red)
            {
                _player = Instantiate(PrefPlayerRed, PlayerStartPos.position, PlayerStartPos.localRotation, null);
            }
            else if(PlayerColor == AmebaColor.turkus)
            {
                _player = Instantiate(PrefPlayerTurkus, PlayerStartPos.position, PlayerStartPos.localRotation, null);
            }
            else if(PlayerColor == AmebaColor.orange)
            {
                _player = Instantiate(PrefPlayerOrange, PlayerStartPos.position, PlayerStartPos.localRotation, null);
            }
        }
        if (!LvlPlayer)
        {
            LvlPlayer = _player.GetComponentInChildren<Player>();
           _lvlPlayerRB = _player.GetComponentInChildren<Rigidbody2D>();
        }
    }
    public void CreateEnemy()
    {
        EnemyColor = GameMgr.Instance.GetEnemyColor();
        if(!_enemy)
        {
            switch(EnemyColor)
            {
                case AmebaColor.green:
                    {
                        _enemy = Instantiate(PrefEnemyGreen, EnemyStartPos.position, EnemyStartPos.localRotation, null);
                    }break;
                case AmebaColor.blue:
                    {
                        _enemy = Instantiate(PrefEnemyBlue, EnemyStartPos.position, EnemyStartPos.localRotation, null);
                    }
                    break;
                case AmebaColor.red:
                    {
                        _enemy = Instantiate(PrefEnemyRed, EnemyStartPos.position, EnemyStartPos.localRotation, null);
                    }
                    break;
                case AmebaColor.turkus:
                    {
                        _enemy = Instantiate(PrefEnemyTurkus, EnemyStartPos.position, EnemyStartPos.localRotation, null);
                    }
                    break;
                case AmebaColor.orange:
                    {
                        _enemy = Instantiate(PrefEnemyOrange, EnemyStartPos.position, EnemyStartPos.localRotation, null);
                    }
                    break;
            }
        }
        if(!LvlEnemy)
        {
            LvlEnemy = _enemy.GetComponentInChildren<Enemy>();
            _lvlEnemyRB = _enemy.GetComponentInChildren<Rigidbody2D>();
        }
    }
    public void SetPlayerColor(AmebaColor newColor)
    {
        PlayerColor = newColor;
    }
    public void SetEnemyColor(AmebaColor newColor)
    {
        EnemyColor = newColor;
    }
    public void SetDifficultyLevel(int lvlIndex)
    {
        switch (lvlIndex)
        {
            case 0:
                {
                    GameMgr.Instance.SetDifficulty(Difficulty.veryEasy);
                }
                break;
            case 1:
                {
                    GameMgr.Instance.SetDifficulty(Difficulty.easy);
                }
                break;
            case 2:
                {
                    GameMgr.Instance.SetDifficulty(Difficulty.normal);
                }
                break;
            case 3:
                {
                    GameMgr.Instance.SetDifficulty(Difficulty.hard);
                }
                break;
            case 4:
                {
                    GameMgr.Instance.SetDifficulty(Difficulty.veryHard);
                }
                break;
        }
    }
    public IEnumerator LoadLevel(string levelName)
    {
        LvlMgrGUI.Instance.GetLoadingPanel().SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
        while (!operation.isDone)
        {
            yield return null;
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            LvlMgrGUI.Instance.GetLoadingSlider().value = progress;
        }
    }    
    IEnumerator DeactivateBackgroundImage()
    {
        yield return new WaitForSeconds(1f);
        if (BackgroundImage)
        {
            StartCoroutine(FadeBackgroundImageOut());
        }
    }
    IEnumerator FadeBackgroundImageOut()
    {
        float colorChangeSpeed = 1;
        while(BackgroundImage.color.a > 0)
        {
            yield return null;
            BackgroundImage.color = new Color(BackgroundImage.color.r, BackgroundImage.color.g, BackgroundImage.color.b,
                BackgroundImage.color.a - colorChangeSpeed * Time.deltaTime);
        }
    }
    IEnumerator StartNewSetAfterXSec(float x)
    {
        yield return new WaitForSeconds(x);
        StartNewSet();
    }
    void SetSingleton()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void SyncEyes()
    {
        BlinkInterval = new float[30];
        for (int i = 0; i < BlinkInterval.Length; i++)
        {
            BlinkInterval[i] = Random.Range(2, 7);
        }
    }
}
