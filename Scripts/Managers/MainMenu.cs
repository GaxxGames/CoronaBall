using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CoronaBall;

public class MainMenu : MonoBehaviour
{
    static MainMenu _instance;
    public static MainMenu Instance { get { return _instance; } }
    [SerializeField] Sprite[] _playerColors, _lvlIcons;
    [SerializeField] Image _playerNewColor, _enemyNewColor, _lvlAvatar;
    [SerializeField] Transform _floor, _wallR;
    [SerializeField] RectTransform _title;
    [SerializeField] GameObject _menuMain, _creditsPanel, _settingsPanel, _playPanel, _loadingPanel;
    [SerializeField] Slider _loadingSlider, _musicVolumeSlider, _soundVolumeSlider;
    [SerializeField] Text _difficultyText;
    [SerializeField] bool _isTitleVisible = true;
    int _lvlAvatarId = 0;
    string[] _difficultyNames = { "very easy", "easy", "normal", "hard", "very hard" };
    Vector2 _titleInitialPosMax, _titleInitialPosMin, _titleInitialScale;
    GameObject[] _panels;
    enum TitlePosition { _initialPos, _centerPos }

    void Awake()
    {
        SetSingleton();
    }

    void Start()
    {
        Load();
        AssignVariables();
    }

    void Update()
    {
        if (_playPanel.activeSelf)
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

    public void ClickPlayButtonMainManu()
    {
        SetObjectActive(_menuMain, false);
        DeactivateAllPanels();
        SetObjectActive(_playPanel, true);
        MoveTitle(TitlePosition._centerPos);
        _title.localScale = _titleInitialScale / 2;
    }
    public void ClickPLayButtonPlayPanel()
    {
        DeactivateAllPanels();
        MoveTitle(TitlePosition._centerPos);
        _title.localScale = _titleInitialScale;
        SetObjectActive(_loadingPanel, true);
        StartCoroutine(LoadLevel(_lvlAvatar.sprite.name));
    }
    public void ClickSettingsButton()
    {
        SetObjectActive(_menuMain, false);
        DeactivateAllPanels();
        SetObjectActive(_settingsPanel, true);
        MoveTitle(TitlePosition._centerPos);
        _title.localScale = _titleInitialScale / 2;
    }
    public void ClickExitButton()
    {
        GameMgr.Instance.SaveSettings();
        DeactivateAllPanels();
        Application.Quit();
    }
    public void ClickCreditsButton()
    {
        DeactivateAllPanels();
        SetObjectActive(_creditsPanel, true);
        MoveTitle(TitlePosition._initialPos);
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
        SetPLayerColor(colorID);
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
        SetEnemyColor(colorID);
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
        SetDifficultyLevel(difficultyIndex);
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
        SetDifficultyLevel(difficultyIndex);
    }
    public void ClickChangeLvlButtonLeft()
    {
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
    public void SetPLayerColor(int colorID)
    {
        switch (colorID)
        {
            case 0:
                {
                    GameMgr.Instance.SetPlayerColor(AmebaColor.blue);
                }break;
            case 1:
                {
                    GameMgr.Instance.SetPlayerColor(AmebaColor.green);
                }break;
            case 2:
                {
                    GameMgr.Instance.SetPlayerColor(AmebaColor.orange);
                } break;
            case 3:
                {
                    GameMgr.Instance.SetPlayerColor(AmebaColor.red);
                }break;
            case 4:
                {
                    GameMgr.Instance.SetPlayerColor(AmebaColor.turkus);
                }break;
        }
        _playerNewColor.sprite = _playerColors[colorID];
    }
    public void SetEnemyColor(int colorID)
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
    public void SetPlayerColorAvatar(AmebaColor playerColor)
    {
        switch(playerColor)
        {
            case AmebaColor.blue:
                {
                    _playerNewColor.sprite = _playerColors[0];
                }break;
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
    public void SetDifficultyText()
    {
        Difficulty gameDifficulty = GameMgr.Instance.GetDifficulty();
        switch(gameDifficulty)
        {
            case Difficulty.veryEasy:
                {
                    _difficultyText.text = _difficultyNames[0];
                }break;
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
    public void SetLvlAvatar()
    {
        _lvlAvatar.sprite = _lvlIcons[_lvlAvatarId];
    }
    public void GoBackToMainMenu()
    {
        DeactivateAllPanels();
        SetObjectActive(_menuMain, true);
        MoveTitle(TitlePosition._initialPos);
        _title.localScale = _titleInitialScale;
    }
    public void AssignLvlAvatarID()
    {
        for(int i=0; i< _lvlIcons.Length; i++)
        {
            if(_lvlAvatar.sprite == _lvlIcons[i])
            {
                _lvlAvatarId = i;
            }
        }
    }

    IEnumerator LoadLevel(string levelName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
        while (!operation.isDone)
        {
            yield return null;
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            _loadingSlider.value = progress;
        }
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
    void AssignVariables()
    {
        AssignLvlAvatarID();
        Time.timeScale = 1;
        SetPlayerColorAvatar(GameMgr.Instance.GetPlayerColor());
        SetEnemyColorAvatar(GameMgr.Instance.GetEnemyColor());
        _panels = new GameObject[] { _creditsPanel, _loadingPanel, _settingsPanel, _playPanel };
        SetObjectActive(_title.gameObject, true);
        _titleInitialPosMin = _title.anchorMin;
        _titleInitialPosMax = _title.anchorMax;
        GameMgr.Instance.RescaleWithScreenSize(_title);
        _titleInitialScale = _title.localScale;
        SetObjectActive(_creditsPanel, true);
        GameMgr.Instance.RescaleWithScreenSize(_loadingSlider.GetComponent<RectTransform>());
        _playPanel.SetActive(true);
        _soundVolumeSlider.value = AudioMgr.Instance.GetSoundVolume();
        _musicVolumeSlider.value = AudioMgr.Instance.GetMusicVolume() * 3;
        SetDifficultyText();
        DeactivateAllPanels();
        if (!_isTitleVisible)
        {
            _title.gameObject.SetActive(false);
        }
    }
    void SetObjectActive(GameObject gamObj, bool isActive)
    {
        if (gamObj.activeSelf != isActive)
        {
            gamObj.SetActive(isActive);
        }
    }
    void MoveTitle(TitlePosition titlePos)
    {
        if (titlePos == TitlePosition._centerPos && _title.anchorMin != new Vector2(0, 0))
        {
            _title.anchorMin = new Vector2(0, 0);
            _title.anchorMax = new Vector2(0, 0);
            _title.anchoredPosition = new Vector2(Screen.width / 2, Screen.height - Screen.height / 7);
        }
        if (titlePos == TitlePosition._initialPos && _title.anchorMin != _titleInitialPosMin)
        {
            _title.anchorMin = _titleInitialPosMin;
            _title.anchorMax = _titleInitialPosMax;
            _title.anchoredPosition = new Vector2(0, 0);
        }
    }
    void DeactivateAllPanels()
    {
        for (int i = 0; i < _panels.Length; i++)
        {
            if (_panels[i].activeSelf)
            {
                SetObjectActive(_panels[i], false);
            }
        }
    }
    void Load()
    {
        if (!GameMgr.Instance.GetIsLoaded())
        {
            GameMgr.Instance.LoadSettings();
        }
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
