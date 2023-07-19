using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidetoSideControl : MonoBehaviour
{
    public float rightLimit = 2.5f;
    public float leftLimit = 1.0f;
    public float speed = 2.0f;
    int direction = 1;

    void Update()
    {
        if (transform.localPosition.x > rightLimit)
        {
            direction = -1;
        }
        else if (transform.localPosition.x < leftLimit)
        {
            direction = 1;
        }
        var movement = Vector3.right * direction * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}
