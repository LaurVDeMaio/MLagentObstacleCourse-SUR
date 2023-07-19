using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownControl : MonoBehaviour
{
    public float upLimit = 2.5f;
    public float downLimit = -1.0f;
    public float speed = 2.0f;
    int direction = 1;

    void Update()
    {
        if (transform.localPosition.y > upLimit)
        {
            direction = -1;
        }
        else if (transform.localPosition.y < downLimit)
        {
            direction = 1;
        }
        var movement = Vector3.up * direction * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}
