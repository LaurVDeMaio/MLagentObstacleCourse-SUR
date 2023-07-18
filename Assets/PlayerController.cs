using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    bool isGrounded;

    Transform originalParent;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalParent = transform.parent;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(0, 10f, 0, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            rb.AddForce(-5.0f, 0, 0, ForceMode.VelocityChange);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            rb.AddForce(5.0f, 0, 0, ForceMode.VelocityChange);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.AddForce(0, 0, 5.0f, ForceMode.VelocityChange);
        }

        if(rb.position.y < -5)
        {
            string currentScene = "SampleScene";
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Ground")
        {
            isGrounded = true;
            this.transform.parent = collision.gameObject.transform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGrounded = false;
            this.transform.parent = originalParent;
        }
    }
}

