using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CoronaBall;

namespace CoronaBall
{
    public enum AmebaColor
    { red, green, turkus, blue, orange }
    public enum Difficulty
    { veryEasy, easy, normal, hard, veryHard }
}

public class GameMgr : MonoBehaviour
{
    static GameMgr _instance;
    public static GameMgr Instance { get { return _instance; } }

    [SerializeField] Difficulty _difficulty = Difficulty.normal;
    [SerializeField] AmebaColor _playerColor = AmebaColor.blue, _enemyColor = AmebaColor.orange;
    float _scaleX, _scaleY;
    bool _isLoaded = false;  
    string _strSoundVolume = "SoundVolume", _strMusicVolume = "MusicVolume";

    void Awake()
    {
        SetSingleton();
        Application.targetFrameRate = 60;
        SetScaleXY();
    }
    void Start()
    {   
        LoadSettings();
    }

    public Difficulty GetDifficulty()
    {
        return _difficulty;
    }
    public AmebaColor GetPlayerColor()
    {
        return _playerColor;
    }
    public AmebaColor GetEnemyColor()
    {
        return _enemyColor;
    }
    public void SetPlayerColor(AmebaColor newColor)
    {
        _playerColor = newColor;
    }
    public void SetEnemyColor(AmebaColor newColor)
    {
        _enemyColor = newColor;
    }
    public void RescaleWithScreenSize(RectTransform element)
    {
        if (element)
        {
            element.localScale = new Vector3(element.localScale.x * _scaleX, element.localScale.y * _scaleY, element.localScale.z);
        }
    }
    public void RescaleTextWithScreenSize(Text text)
    {
        SetScaleXY();
        text.fontSize = Mathf.RoundToInt((float)text.fontSize * _scaleY);
        text.resizeTextMaxSize = text.fontSize;
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(_strSoundVolume, AudioMgr.Instance.GetSoundVolume());
        PlayerPrefs.SetFloat(_strMusicVolume, AudioMgr.Instance.GetMusicVolume());
    }
    public bool GetIsLoaded()
    {
        return _isLoaded;
    }
    public void LoadSettings()
    {
        if(PlayerPrefs.HasKey(_strSoundVolume))
        {
            AudioMgr.Instance.SetSoundVolume(PlayerPrefs.GetFloat(_strSoundVolume));
        }
        if(PlayerPrefs.HasKey(_strMusicVolume))
        {
            AudioMgr.Instance.SetMusicVolume(PlayerPrefs.GetFloat(_strMusicVolume)*3);
        }
        _isLoaded = true;
    }
    public void SetDifficulty(Difficulty newDifficulty)
    {
        _difficulty = newDifficulty;
    }
    public void KeepPositionUI(RectTransform myObject)
    {
        myObject.anchoredPosition = new Vector2(myObject.anchoredPosition.x * _scaleX, myObject.anchoredPosition.y * _scaleY);
    }
    void SetScaleXY()
    {
        if (_scaleX == 0)
        {
            _scaleX = Screen.width / 2960f;
        }
        if (_scaleY == 0)
        {
            _scaleY = Screen.height / 1440f;
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
