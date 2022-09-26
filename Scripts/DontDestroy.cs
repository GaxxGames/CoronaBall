using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [SerializeField] bool _isGameMgrsParent = false;
    string _strCheckChildren = "CheckIfHasChidren";
    void Awake()
    {
        DontDestroyOnLoad(this);
        if(_isGameMgrsParent)
            Invoke(_strCheckChildren, 0.1f);
    }

    void CheckIfHasChidren()
    {
        if (transform.childCount == 0)
            Destroy(gameObject);
    }
}
