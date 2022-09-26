using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    static AdInitializer  _instance;
    public static AdInitializer  Instance { get { return _instance; } }
#if UNITY_IOS
    string _gameId = "4144866";
#elif UNITY_ANDROID
    string _gameId = "4144867";
#endif

    bool _testMode = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        //Debug.Log("Unity Ads initialization complete.");
    }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        //Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}
