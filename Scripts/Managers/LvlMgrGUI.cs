using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CoronaBall;

public class LvlMgrGUI : MonoBehaviour
{
    static LvlMgrGUI _instance;
    public static LvlMgrGUI Instance { get { return _instance; } }

    [SerializeField] GameObject _ball, _arrow, _playPanel, _endgamePanel, _loadingPanel, _pausePanel;
    [SerializeField] Renderer _arrowAwatar;
    [SerializeField] Transform _ballAvatar;
    [SerializeField] RectTransform _pausePanelButtons, _menuButton;
    [SerializeField] Sprite[] _playerColors, _lvlIcons;
    [SerializeField] Sprite _shieetCup;
    [SerializeField] Image _playerNewColor, _enemyNewColor, _lvlAvatar, _cupAvatar;
    [SerializeField] Slider _musicVolumeSlider, _soundVolumeSlider;
    [SerializeField] Text _endgameText, _difficultyText, _playerPointsText, _enemyPointsText;

    bool _isPointsTextScaling = false;
    bool _isPointTextAlphaChanging = false;
    int _lvlAvatarId = 0;
    float _speedPointsScaling = 3f;
    float _scalePointsText = 3f;
    float _arrowAvatarColorAlpha = 0;
    float _textAlpha;
    Slider _loadingSlider;
    Text _currentPointsToScaleText = null;
    Text _playerPointsBackground, _enemyPointsBackground;
    string[] _difficultyNames = { "very easy", "easy", "normal", "hard", "very hard" };
    string _strMenu = "Menu";
    string _strWinText = "You Win!!!";
    string _strLoseText = "You Lose!!!";

    void Awake()
    {
        SetSingleton();
    }

    void Update()
    {
        CheckPausePanel();
        CheckIfEscapePressed();
    }
    void FixedUpdate()
    {
        PointsTextBehaviour();
        ArrowBehaviour();
    }
    public GameObject GetArrow()
    {
        return _arrow;
    }
    public void SetTextAlpha(float alpha)
    {
        _textAlpha = alpha;
    }
    public float GetScalePointsText()
    {
        return _scalePointsText;
    }
    public void SetIsPointsTextScaling(bool isScaling)
    {
        _isPointsTextScaling = isScaling;
    }
    public bool GetIsPointsTextScaling()
    {
        return _isPointsTextScaling;
    }
    public Text GetEnemyPointsBackground()
    {
        return _enemyPointsBackground;
    }
    public void SetEnemyPointsBackground(Text enemyPointsBackground)
    {
        _enemyPointsBackground = enemyPointsBackground;
    }
    public Text GetPlayerPointsBackground()
    {
        return _playerPointsBackground;
    }
    public void SetPlayerPointsBackground(Text playerPointsBackground)
    {
        _playerPointsBackground = playerPointsBackground;
    }
    public Text GetCurrentPointsToScaleText()
    {
        return _currentPointsToScaleText;
    }
    public void SetCurrentPointsToScaleText(Text pointsToScaleText)
    {
        _currentPointsToScaleText = pointsToScaleText;
    }
    public Slider GetLoadingSlider()
    {
        return _loadingSlider;
    }
    public void SetLoadingSlider(Slider newSlider)
    {
        _loadingSlider = newSlider;
    }
    public GameObject GetPlayPanel()
    {
        return _playPanel;
    }
    public GameObject GetEndgamePanel()
    {
        return _endgamePanel;
    }
    public GameObject GetLoadingPanel()
    {
        return _loadingPanel;
    }
    public GameObject GetPausePanel()
    {
        return _pausePanel;
    }
    public RectTransform GetPausePanelButtons()
    {
        return _pausePanelButtons;
    }
    public RectTransform GetMenuButton()
    {
        return _menuButton;
    }
    public Slider GetMusicVolumeSlider()
    {
        return _musicVolumeSlider;
    }
    public Slider GetSoundVolumeSlider()
    {
        return _soundVolumeSlider;
    }
    public Text GetPlayerPointsText()
    {
        return _playerPointsText;
    }
    public Text GetEnemyPointsText()
    {
        return _enemyPointsText;
    }
    public void ClickMenuButton()
    {
        LvlManager.Instance.PauseStart();
        _pausePanel.SetActive(true);
        _menuButton.gameObject.SetActive(false);
    }
    public void ClickReturnToMainManuButton()
    {
        StartCoroutine(LvlManager.Instance.LoadLevel(_strMenu));
    }
    public void ClickPausePanelPlayButton()
    {
        _menuButton.gameObject.SetActive(true);
        _pausePanel.SetActive(false);
        LvlManager.Instance.PauseEnd();
    }
    public void ClickPlayAgainButton()
    {
        StartCoroutine(LvlManager.Instance.LoadLevel(SceneManager.GetActiveScene().name));
    }
    public void ClickPlay_EndgamePanel()
    {
        if (!LvlManager.Instance.GetIfWin())
        {
            //AdManager.Instance.ShowInterstitialAd();

            AdMgr.Instance.ShowAd();
        }
        _endgamePanel.SetActive(false);
        _playPanel.SetActive(true);
    }
    public void ClickPLayerColorArrowButton(bool isArrowRight)
    {
        int colorID = 0;
        for (int i = 0; i < _playerColors.Length; i++)
        {
            if (_playerColors[i] == _playerNewColor.sprite)
            {
                colorID = i;
            }
        }
        if (isArrowRight)
        {
            if (colorID + 1 < _playerColors.Length)
            {
                colorID++;
            }
            else
            {
                colorID = 0;
            }
        }
        else
        {
            if (colorID - 1 >= 0)
            {
                colorID--;
            }
            else
            {
                colorID = _playerColors.Length - 1;
            }
        }
        SetPlayerColorPanel(colorID);
    }
    public void ClickEnemyColorArrowButton(bool isArrowRight)
    {
        int colorID = 0;
        for (int i = 0; i < _playerColors.Length; i++)
        {
            if (_playerColors[i] == _enemyNewColor.sprite)
            {
                colorID = i;
            }
        }
        if (isArrowRight)
        {
            if (colorID + 1 < _playerColors.Length)
            {
                colorID++;
            }
            else
            {
                colorID = 0;
            }
        }
        else
        {
            if (colorID - 1 >= 0)
            {
                colorID--;
            }
            else
            {
                colorID = _playerColors.Length - 1;
            }
        }
        SetEnemyColorPanel(colorID);
    }
    public void ClickChangeDifficultyButtonRight()
    {
        int difficultyIndex = GetDifficultyTextIndex(_difficultyText.text);
        if (difficultyIndex < _difficultyNames.Length - 1)
        {
            difficultyIndex++;
        }
        else
        {
            difficultyIndex = 0;
        }
        _difficultyText.text = _difficultyNames[difficultyIndex];
        LvlManager.Instance.SetDifficultyLevel(difficultyIndex);
    }
    public void ClickChangeDifficultyButtonLeft()
    {
        int difficultyIndex = GetDifficultyTextIndex(_difficultyText.text);
        if (difficultyIndex > 0)
        {
            difficultyIndex--;
        }
        else
        {
            difficultyIndex = _difficultyNames.Length - 1;
        }
        _difficultyText.text = _difficultyNames[difficultyIndex];
        LvlManager.Instance.SetDifficultyLevel(difficultyIndex);
    }
    public void ClickChangeLvlButtonLeft()
    {
        AssignLvlAvatarID();
        if (_lvlAvatarId <= 0)
        {
            _lvlAvatarId = _lvlIcons.Length - 1;
        }
        else
        {
            _lvlAvatarId--;
        }
        SetLvlAvatar();
    }
    public void ClickChangeLvlButtonRight()
    {
        AssignLvlAvatarID();
        if (_lvlAvatarId >= _lvlIcons.Length - 1)
        {
            _lvlAvatarId = 0;
        }
        else
        {
            _lvlAvatarId++;
        }
        SetLvlAvatar();

    }
    public void ClickPlayNewLvl()
    {
        StartCoroutine(LvlManager.Instance.LoadLevel(_lvlAvatar.sprite.name));
    }
    public void SetPlayerColorPanel(int colorID)
    {
        switch (colorID)
        {
            case 0:
                {
                    GameMgr.Instance.SetPlayerColor(AmebaColor.blue);
                }
                break;
            case 1:
                {
                    GameMgr.Instance.SetPlayerColor(AmebaColor.green);
                }
                break;
            case 2:
                {
                    GameMgr.Instance.SetPlayerColor(AmebaColor.orange);
                }
                break;
            case 3:
                {
                    GameMgr.Instance.SetPlayerColor(AmebaColor.red);
                }
                break;
            case 4:
                {
                    GameMgr.Instance.SetPlayerColor(AmebaColor.turkus);
                }
                break;
        }
        _playerNewColor.sprite = _playerColors[colorID];
    }
    public void SetEnemyColorPanel(int colorID)
    {
        switch (colorID)
        {
            case 0:
                {
                    GameMgr.Instance.SetEnemyColor(AmebaColor.blue);
                }
                break;
            case 1:
                {
                    GameMgr.Instance.SetEnemyColor(AmebaColor.green);
                }
                break;
            case 2:
                {
                    GameMgr.Instance.SetEnemyColor(AmebaColor.orange);
                }
                break;
            case 3:
                {
                    GameMgr.Instance.SetEnemyColor(AmebaColor.red);
                }
                break;
            case 4:
                {
                    GameMgr.Instance.SetEnemyColor(AmebaColor.turkus);
                }
                break;
        }
        _enemyNewColor.sprite = _playerColors[colorID];
    }
    public void SetEnemyColorAvatar(AmebaColor enemyColor)
    {
        switch (enemyColor)
        {
            case AmebaColor.blue:
                {
                    _enemyNewColor.sprite = _playerColors[0];
                }
                break;
            case AmebaColor.green:
                {
                    _enemyNewColor.sprite = _playerColors[1];
                }
                break;
            case AmebaColor.orange:
                {
                    _enemyNewColor.sprite = _playerColors[2];
                }
                break;
            case AmebaColor.red:
                {
                    _enemyNewColor.sprite = _playerColors[3];
                }
                break;
            case AmebaColor.turkus:
                {
                    _enemyNewColor.sprite = _playerColors[4];
                }
                break;
        }
    }
    public void SetPlayerColorAvatar(AmebaColor playerColor)
    {
        switch (playerColor)
        {
            case AmebaColor.blue:
                {
                    _playerNewColor.sprite = _playerColors[0];
                }
                break;
            case AmebaColor.green:
                {
                    _playerNewColor.sprite = _playerColors[1];
                }
                break;
            case AmebaColor.orange:
                {
                    _playerNewColor.sprite = _playerColors[2];
                }
                break;
            case AmebaColor.red:
                {
                    _playerNewColor.sprite = _playerColors[3];
                }
                break;
            case AmebaColor.turkus:
                {
                    _playerNewColor.sprite = _playerColors[4];
                }
                break;
        }
    }
    public void SetDifficultyText()
    {
        Difficulty gameDifficulty = GameMgr.Instance.GetDifficulty();
        switch (gameDifficulty)
        {
            case Difficulty.veryEasy:
                {
                    _difficultyText.text = _difficultyNames[0];
                }
                break;
            case Difficulty.easy:
                {
                    _difficultyText.text = _difficultyNames[1];
                }
                break;
            case Difficulty.normal:
                {
                    _difficultyText.text = _difficultyNames[2];
                }
                break;
            case Difficulty.hard:
                {
                    _difficultyText.text = _difficultyNames[3];
                }
                break;
            case Difficulty.veryHard:
                {
                    _difficultyText.text = _difficultyNames[4];
                }
                break;
        }
    }
    public void SetLvlAvatar()
    {
        _lvlAvatar.sprite = _lvlIcons[_lvlAvatarId];
        AssignLvlAvatarID();
    }
    public void AssignLvlAvatarID()
    {
        for (int i = 0; i < _lvlIcons.Length; i++)
        {
            if (_lvlAvatar.sprite == _lvlIcons[i])
            {
                _lvlAvatarId = i;
            }
        }
    }
    public void AssignPlayerPanelVariables(bool ifwin)
    {
        if (ifwin)
        {
            _playPanel.SetActive(true);
            _endgameText.text = _strWinText;
            for (int i = 0; i < _lvlIcons.Length; i++)
            {
                if (_lvlIcons[i].name[3] == SceneManager.GetActiveScene().name[3])
                    _lvlAvatarId = i;
            }
            SetLvlAvatar();
            SetDifficultyText();
            ClickChangeLvlButtonRight();
            SetPlayerColorAvatar(GameMgr.Instance.GetPlayerColor());
            SetEnemyColorAvatar(GameMgr.Instance.GetEnemyColor());
            if (GameMgr.Instance.GetDifficulty() != Difficulty.veryHard)
            {
                ClickChangeDifficultyButtonRight();
            }
            _playPanel.SetActive(false);
        }
        else
        {
            _playPanel.SetActive(true);
            if (_shieetCup != null)
            {
                _cupAvatar.sprite = _shieetCup;
            }
            _endgameText.text = _strLoseText;
            for (int i = 0; i < _lvlIcons.Length; i++)
            {
                if (_lvlIcons[i].name[3] == SceneManager.GetActiveScene().name[3])
                    _lvlAvatarId = i;
            }
            SetLvlAvatar();
            SetDifficultyText();
            SetPlayerColorAvatar(GameMgr.Instance.GetPlayerColor());
            SetEnemyColorAvatar(GameMgr.Instance.GetEnemyColor());
            _playPanel.SetActive(false);
        }
    }
    public void Endgame(bool ifWin)
    {
        _endgamePanel.SetActive(true);
        AssignPlayerPanelVariables(ifWin);
        _menuButton.gameObject.SetActive(false);
    }
    public void AdjustTextAlpha(Text myText, float targetAlpha)
    {
        if (myText.color.a > targetAlpha)
        {
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, myText.color.a - 0.3f * _speedPointsScaling * Time.fixedDeltaTime);
        }
        else if (myText.color.a < targetAlpha)
        {
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, targetAlpha);
            _isPointTextAlphaChanging = false;
        }
    }    
    public IEnumerator ChangeTextAlpha()
    {
        yield return new WaitForSeconds(0.5f);
        _isPointTextAlphaChanging = true;
    }
    public void AssignVariables(float pointsTextDistanceFromCorner)
    {
        GetPausePanel().SetActive(true);
        GetSoundVolumeSlider().value = AudioMgr.Instance.GetSoundVolume();
        GetMusicVolumeSlider().value = AudioMgr.Instance.GetMusicVolume() * 3;
        GetPlayerPointsText().transform.position = new Vector2(6 * pointsTextDistanceFromCorner * Screen.height, Screen.height - 2 * pointsTextDistanceFromCorner * Screen.height);
        GetEnemyPointsText().transform.position = new Vector2(Screen.width - 6 * pointsTextDistanceFromCorner * Screen.height, Screen.height - 2 * pointsTextDistanceFromCorner * Screen.height);
        SetEnemyPointsBackground(GetEnemyPointsText().transform.GetChild(0).GetComponent<Text>());
        SetPlayerPointsBackground(GetPlayerPointsText().transform.GetChild(0).GetComponent<Text>());
        GetMenuButton().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.height / 10);
        GetMenuButton().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height / 10);
        SetTextAlpha(GetPlayerPointsText().color.a);
        GetPausePanelButtons().transform.localScale = new Vector3(Screen.height / 1440f, Screen.height / 1440f, Screen.height / 1440f);
        GetLoadingPanel().SetActive(true);
        SetLoadingSlider(GetLoadingPanel().GetComponentInChildren<Slider>());
    }
    public void Scored(bool ifPlayer, int playerPoints, int enemyPoints)
    {
        if (ifPlayer)
        {
            GetPlayerPointsText().text = playerPoints.ToString();
            Color text1Color = GetPlayerPointsText().color;
            Color text2Color = GetPlayerPointsBackground().color;
            GetPlayerPointsBackground().text = playerPoints.ToString();
            SetCurrentPointsToScaleText(GetPlayerPointsText());
            GetCurrentPointsToScaleText().transform.localScale
                = new Vector3(GetScalePointsText(), GetScalePointsText(), GetScalePointsText());
            GetPlayerPointsText().color = new Color(text1Color.r, text1Color.g, text1Color.b, 1);
            GetPlayerPointsBackground().color = new Color(text2Color.r, text2Color.g, text2Color.b, 1);
            SetIsPointsTextScaling(true);
            StartCoroutine(ChangeTextAlpha());
        }
        else
        {
            GetEnemyPointsText().text = enemyPoints.ToString();
            Color text1Color = GetEnemyPointsText().color;
            Color text2Color = GetEnemyPointsBackground().color;
            GetEnemyPointsBackground().text = enemyPoints.ToString();
            SetCurrentPointsToScaleText(GetEnemyPointsText());
            GetCurrentPointsToScaleText().transform.localScale
                = new Vector3(GetScalePointsText(), GetScalePointsText(), GetScalePointsText());
            GetEnemyPointsText().color = new Color(text1Color.r, text1Color.g, text1Color.b, 1);
            GetEnemyPointsBackground().color = new Color(text2Color.r, text2Color.g, text2Color.b, 1);
            SetIsPointsTextScaling(true);
            StartCoroutine(ChangeTextAlpha());
        }
    }
    public void ClosePanels()
    {
        if (GetLoadingPanel().activeSelf)
            GetLoadingPanel().SetActive(false);

        if (GetPlayPanel() && GetPlayPanel().activeSelf)
            GetPlayPanel().SetActive(false);

        if (GetEndgamePanel() && GetEndgamePanel().activeSelf)
            GetEndgamePanel().SetActive(false);
        
        if(GetPausePanel() && GetPausePanel().activeSelf)
            GetPausePanel().SetActive(false);
    }
    int GetDifficultyTextIndex(string difficultyText)
    {
        int index = 0;

        for (int i = 0; i < _difficultyNames.Length; i++)
        {
            if (difficultyText == _difficultyNames[i])
            {
                index = i;
            }
        }

        return index;
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
    void CheckPausePanel()
    {
        if (_pausePanel.activeSelf)
        {
            if (_musicVolumeSlider.value != AudioMgr.Instance.GetMusicVolume())
            {
                AudioMgr.Instance.SetMusicVolume(_musicVolumeSlider.value);
            }
            if (_soundVolumeSlider.value != AudioMgr.Instance.GetSoundVolume())
            {
                AudioMgr.Instance.SetSoundVolume(_soundVolumeSlider.value);
            }
        }
    }
    void CheckIfEscapePressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_pausePanel.activeSelf)
            {
                ClickMenuButton();
            }
            else
            {
                ClickPausePanelPlayButton();
            }
        }
    }
    void PointsTextBehaviour()
    {
        if (_isPointsTextScaling && _currentPointsToScaleText)
        {
            if (_currentPointsToScaleText.transform.localScale.x > 1)
            {
                _currentPointsToScaleText.transform.localScale = new Vector3(_currentPointsToScaleText.transform.localScale.x - _speedPointsScaling * Time.deltaTime,
                    _currentPointsToScaleText.transform.localScale.y - _speedPointsScaling * Time.fixedDeltaTime,
                    _currentPointsToScaleText.transform.localScale.z - _speedPointsScaling * Time.fixedDeltaTime);
            }
            else
            {
                _currentPointsToScaleText.transform.localScale = new Vector3(1, 1, 1);
                Text text2 = _currentPointsToScaleText.transform.GetChild(0).GetComponent<Text>();
                _isPointsTextScaling = false;
                _currentPointsToScaleText = null;
            }
        }
        if (_isPointTextAlphaChanging)
        {
            AdjustTextAlpha(GetPlayerPointsText(), _textAlpha);
            AdjustTextAlpha(GetEnemyPointsText(), _textAlpha);
            AdjustTextAlpha(GetPlayerPointsBackground(), _textAlpha);
            AdjustTextAlpha(GetEnemyPointsBackground(), _textAlpha);
        }
    }
    void ArrowBehaviour()
    {
        if (_ball.transform.position.y - 0.7f > _arrow.transform.position.y)
        {
            if (!_arrow.activeSelf)
            {
                _arrow.SetActive(true);
            }
            if (_ball.transform.position.y - 0.7f < _arrow.transform.position.y + 2)
            {
                _arrowAvatarColorAlpha = (_ball.transform.position.y - 0.7f - _arrow.transform.position.y) / 2;
                _arrowAwatar.material.color = new Color(_arrowAwatar.material.color.r, _arrowAwatar.material.color.g, _arrowAwatar.material.color.b, _arrowAvatarColorAlpha);
            }
        }
        else
        {
            if (_arrow.activeSelf)
            {
                _arrow.SetActive(false);
            }
        }

        if (_arrow.activeSelf)
        {
            _arrow.transform.position = new Vector3(_ball.transform.position.x, _arrow.transform.position.y);
        }
    }
}
