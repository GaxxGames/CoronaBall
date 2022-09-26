using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    static AudioMgr _instance;
    public static AudioMgr Instance { get { return _instance; } }

    [SerializeField] float _soundVolume = 1f, _musicVolume = 1f;
    [SerializeField] AudioClip[] _audioClips;
    [SerializeField] AudioClip _whistleBlow;
    [SerializeField] AudioSource _myAudioSource, _myAudioSource2;
    bool _isFirstAudioSourcePlaying = true;

    void Awake()
    {
        SetSingleton();
        SetAudioSources();        
    }

    void Start()
    {
        SetAudio();
    }

    public AudioClip GetWhistleBlowAudioclip()
    {
        return _whistleBlow;
    }
    public void SetSoundVolume(float volume)
    {
        _soundVolume = volume;
    }
    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume / 3;
        _myAudioSource.volume = _musicVolume;
        _myAudioSource2.volume = _musicVolume;
    }
    public float GetSoundVolume()
    {
        return _soundVolume;
    }
    public float GetMusicVolume()
    {
        return _musicVolume;
    }

    IEnumerator WaitForClipFinish(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        ChangeAudioClip();
    }
    void ChangeAudioClip()
    {
        if (_isFirstAudioSourcePlaying)
        {
            _myAudioSource.Stop();
            _myAudioSource.clip = _audioClips[Random.Range(0, _audioClips.Length - 1)];
            while (_myAudioSource.clip == _myAudioSource2.clip)
            {
                _myAudioSource.clip = _audioClips[Random.Range(0, _audioClips.Length - 1)];
            }
            _myAudioSource2.Play();
            _isFirstAudioSourcePlaying = false;
            StartCoroutine(WaitForClipFinish(_myAudioSource2.clip.length));

        }
        else
        {
            _myAudioSource2.Stop();
            _myAudioSource2.clip = _audioClips[Random.Range(0, _audioClips.Length - 1)];
            while (_myAudioSource2.clip == _myAudioSource.clip)
            {
                _myAudioSource2.clip = _audioClips[Random.Range(0, _audioClips.Length - 1)];
            }
            _myAudioSource.Play();
            _isFirstAudioSourcePlaying = true;
            StartCoroutine(WaitForClipFinish(_myAudioSource.clip.length));
        }
    }
    void SetAudio()
    {
        _myAudioSource.clip = _audioClips[Random.Range(0, _audioClips.Length - 1)];

        do{
            _myAudioSource2.clip = _audioClips[Random.Range(0, _audioClips.Length - 1)];
        } while (_myAudioSource2.clip == null || _myAudioSource2.clip == _myAudioSource.clip);

        if (!_myAudioSource.isPlaying)
        {
            _myAudioSource.Play();
        }
        if (_myAudioSource2.isPlaying)
        {
            _myAudioSource2.Stop();
        }
        StartCoroutine(WaitForClipFinish(_myAudioSource.clip.length));
    }
    void SetAudioSources()
    {
        if (!_myAudioSource)
        {
            _myAudioSource = GetComponents<AudioSource>()[0];
        }
        if (!_myAudioSource2)
        {
            _myAudioSource2 = GetComponents<AudioSource>()[1];
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
