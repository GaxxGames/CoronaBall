using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusAvatarUI : MonoBehaviour
{
    [SerializeField] VirusEyes _eyeL;
    [SerializeField] VirusEyes _eyeR;
    [SerializeField] VirusMouth _myMouth;
    Rigidbody2D _myRigidbody;
    bool _isMoving = true;

    void Start()
    {
        _myRigidbody = GetComponent<Rigidbody2D>();
        if (_isMoving)
        {
            int forceX = 3;
            int forceY = 3;
            if(Random.Range(0,100)>50)
            {
                forceX = -3;
            }
            if (Random.Range(0, 100) > 50)
            {
                forceY = -3;
            }
            _myRigidbody.AddForce(new Vector2(forceX, forceY));
            _myRigidbody.angularVelocity = -10f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ChooseIsRotating();
        _eyeR.Blink();
        _eyeL.Blink();
        _myMouth.CollisionClose();
    }

    void ChooseIsRotating()
    {
        if (Random.Range(0,100)>50)
        {
            _myRigidbody.angularVelocity = Random.Range(7, 21);
        }
        else
        {
            _myRigidbody.angularVelocity = Random.Range(-21, -7);
        }
    }
}
