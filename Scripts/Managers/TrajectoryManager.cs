using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour
{    
    static TrajectoryManager _instance;
    public static TrajectoryManager Instance {get {return _instance; } }

    [SerializeField] GameObject _ball, _trajectoryPointPref;
    [SerializeField] List<GameObject> _trajectoryBalls;
    [SerializeField] List<Vector2> _predictedPos, _realPos;
    Vector2 _netPos, _trajectoryTopPos, _startPoint;
    Rigidbody2D _ballRB;
    bool _ifGameStarted = false;
    bool _isTrajectoryPredicted = false;
    bool _isTrajectoryVisible = false;
    bool _canChangeTrajTopPos = true;
    int _changeDirPointID = -1;
    int _fixedFramesAheadNr = 200;
    float _stepX = 0;
    float _maxTrajectoryDeviation = 0.5f;
    float _wallEdgeR, _wallEdgeL, _floorSurface, _netTop, _netEdgeR, _netEdgeL;

    void Awake()
    {
        SetSingleton();
    }
    
    void Start()
    {
        AssignVariables();
    }
    
    void FixedUpdate()
    {
        if (!_ifGameStarted) return;
        SolveTrajectory();                
    }

    public void RemoveTrajectory()
    {
        _isTrajectoryPredicted = false;
        _changeDirPointID = -1;
        if (_trajectoryBalls.Count > 0)
        {
            for (int i = 0; i < _trajectoryBalls.Count; i++)
            {
                Destroy(_trajectoryBalls[i]);
            }
        }
        _predictedPos.Clear();
        _trajectoryBalls.Clear();
        _realPos.Clear();
    }
    public void SetIfGameStarted(bool ifStarted)
    {
        _ifGameStarted = ifStarted;
    }
    public void SetCanChangeTrajTopPos(bool isTrue)
    {
        _canChangeTrajTopPos = isTrue;
    }
    public int GetPredictedColPointId(float posY)
    {
        int id = 0;
        float[] distanceToPosY = new float[_predictedPos.Count];
        for (int i = 0; i < _predictedPos.Count; i++)
        {
            if (i > 0)
            {
                distanceToPosY[i] = Mathf.Abs(_predictedPos[i].y - posY);

                if (distanceToPosY[i] <= distanceToPosY[i - 1])
                {
                    id = i;
                }
            }
            else
            {
                id = 0;
            }
        }
        return id;
    }
    public int GetChangeDirPointId()
    {
        return _changeDirPointID;
    }
    public bool GetIsBallComingFromRight()
    {

        if(_predictedPos.Count>0 && GetTrajectoryTopPos().x > _predictedPos[_predictedPos.Count-1].x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool GetIsTrajejectoryPredicted()
    {
        return _isTrajectoryPredicted;
    }
    public Vector2 GetTrajectoryTopPos()
    {
        return _trajectoryTopPos;
    }
    public Vector2 GetPredictedPos(int listID)
    {
        return _predictedPos[listID];
    }
    public IEnumerator DelayedCalcTrajectory()
    {
        _startPoint = _ball.transform.position;
        yield return new WaitForSeconds(0.1f);
        CalculateTrajectory();
    }
    void SetTrajectoryTopPos()
    {
        float MaxY = -1000;
        int id = -1;

        if (GetIsTrajejectoryPredicted() && _predictedPos.Count > 3)
        {
            bool isChangingDirection = false;
            for (int i = 0; i < _predictedPos.Count; i++)
            {
                if (i > 1)
                {
                    if ((_predictedPos[i].x - _predictedPos[i - 1].x > 0 && _predictedPos[i - 1].x - _predictedPos[i - 2].x > 0)
                        || (_predictedPos[i].x - _predictedPos[i - 1].x < 0 && _predictedPos[i - 1].x - _predictedPos[i - 2].x < 0))
                    {
                        if (MaxY < _predictedPos[i].y)
                        {
                            MaxY = _predictedPos[i].y;
                            id = i;
                        }
                    }
                    else
                    {
                        isChangingDirection = true;
                        MaxY = _predictedPos[i].y;
                        id = i;
                        if (_changeDirPointID < 0)
                        {
                            _changeDirPointID = id;
                        }
                    }
                }
                else
                {
                    if (MaxY < _predictedPos[i].y)
                    {
                        MaxY = _predictedPos[i].y;
                        id = i;
                    }
                }
            }
            if (_canChangeTrajTopPos)
            {
                _trajectoryTopPos = _predictedPos[id];
                if (isChangingDirection)
                {
                    SetCanChangeTrajTopPos(false);
                }
            }
        }
    }
    void CalculateTrajectory()
    {
        _changeDirPointID = -1;
        if (_predictedPos.Count > 0)
        {
            RemoveTrajectory();
        }

        _stepX = _ballRB.velocity.x * Time.fixedDeltaTime;
        float velocityY = _ballRB.velocity.y;
        float predPosY = _ball.transform.position.y;

        int maxI = -666;

        if (_fixedFramesAheadNr > 0)
        {
            for (int i = 0; i < _fixedFramesAheadNr; i++)
            {
                float predPosX;
                predPosY += (Time.fixedDeltaTime * velocityY);
                Vector2 ballPos = _ball.transform.position;


                if (ballPos.x + (i) * _stepX >= _wallEdgeR)
                {
                    predPosX = _wallEdgeR - (i - maxI) * _stepX;
                }
                else if (ballPos.x + (i) * _stepX <= _wallEdgeL)
                {
                    predPosX = _wallEdgeL - (i - maxI) * _stepX;
                }
                else if (predPosY < _netTop)
                {
                    if (_predictedPos != null && _predictedPos.Count > 0)
                    {
                        if (_startPoint.x < _netPos.x && ballPos.x + (i) * _stepX >= _netEdgeR &&
                            _predictedPos[_predictedPos.Count - 1].x < _netEdgeR)
                        {
                            predPosX = _netEdgeR - (i - maxI) * _stepX;
                        }
                        else if (_startPoint.x > _netPos.x && ballPos.x + (i) * _stepX <= _netEdgeL &&
                            _predictedPos[_predictedPos.Count - 1].x > _netEdgeL)
                        {
                            predPosX = _netEdgeL - (i - maxI) * _stepX;
                        }
                        else
                        {
                            predPosX = _ball.transform.position.x + (i) * _stepX;
                            maxI = i;
                        }
                    }
                    else
                    {
                        predPosX = _ball.transform.position.x + (i) * _stepX;
                        maxI = i;
                    }
                }
                else
                {
                    predPosX = _ball.transform.position.x + (i) * _stepX;
                    maxI = i;
                    //Debug.Log("i: " + i + " maxI: " + maxI);
                }

                if (predPosY > _floorSurface)
                {
                    if (_isTrajectoryVisible)
                    {
                        _trajectoryBalls.Add(Instantiate(_trajectoryPointPref, new Vector3(predPosX, predPosY, 0), _ball.transform.rotation));
                    }
                    _predictedPos.Add(new Vector2(predPosX, predPosY));
                    velocityY -= Mathf.Abs(Time.fixedDeltaTime * Physics2D.gravity.y);
                }
                else
                {
                    //Exit *for{...}* to stop unnecesary calculation
                    i = _fixedFramesAheadNr - 1;
                }
            }
        }
        _isTrajectoryPredicted = true;
        SetTrajectoryTopPos();
    }
    void SolveTrajectory()
    {
        if (!_isTrajectoryPredicted)
        {
            StartCoroutine(DelayedCalcTrajectory());
        }
        else
        {
            if ((_realPos.Count < _fixedFramesAheadNr - 2) && (_predictedPos.Count > 0))
            {
                _realPos.Add(_ball.transform.position);
                if (_realPos.Count > 1 && _predictedPos.Count > _realPos.Count && _predictedPos.Count > 0)
                {
                    float devX = _realPos[_realPos.Count - 1].x - _predictedPos[_realPos.Count].x;
                    float devY = _realPos[_realPos.Count - 1].y - _predictedPos[_realPos.Count].y;

                    if (Mathf.Abs(devX) > _maxTrajectoryDeviation || Mathf.Abs(devY) > _maxTrajectoryDeviation)
                    {
                        RemoveTrajectory();
                    }
                }
            }
            else
            {
                RemoveTrajectory();
            }
        }
    }
    void AssignVariables()
    {
        if (!LvlManager.Instance.GetIfVariablesAssigned())
        {
            LvlManager.Instance.AssignVariables();
        }
        _wallEdgeL = LvlManager.Instance.GetWallEdgeL();
        _wallEdgeR = LvlManager.Instance.GetWallEdgeR();
        _floorSurface = LvlManager.Instance.GetFloorSurface();
        _netTop = LvlManager.Instance.GetNetTop();
        _netEdgeR = LvlManager.Instance.GetNetEdgeR();
        _netEdgeL = LvlManager.Instance.GetNetEdgeL();
        _netPos = LvlManager.Instance.Net.position;

        if (!_ball)
        {
            _ball = LvlManager.Instance.GetBall();
        }
        _ballRB = _ball.GetComponent<Rigidbody2D>();
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
}
