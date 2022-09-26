using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public bool isActive = false;
    [SerializeField] private Text _fpsText;
    [SerializeField] private float _hudRefreshRate = 0.1f;

    float _timer;
    string _strFPS = "FPS: ";

    void Start()
    {
        if (!_fpsText)
        {
            _fpsText = GameObject.FindGameObjectWithTag("FPSCounter").GetComponent<Text>();
        }
        if (!isActive)
        {
            _fpsText.gameObject.SetActive(false);
            gameObject.GetComponent<FPSCounter>().enabled = false;
        }
    }
    void Update()
    {
        if (Time.unscaledTime > _timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            _fpsText.text = _strFPS + fps;
            _timer = Time.unscaledTime + _hudRefreshRate;
        }
    }
}