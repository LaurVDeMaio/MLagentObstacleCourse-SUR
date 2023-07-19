using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerController : Agent
{
    Rigidbody rb;
    bool isGrounded;
    Transform originalParent;

    //Human Mode
    public bool inHumanControl;

    //Agent Mode
    GameObject goal, startPlat,firstPlat, secPlat, thirdPlat;
    float lastDistance;
    float moveSpeed = 0.10f;
    public static int episodeNum = 0;

    public override void OnEpisodeBegin()
    {
        episodeNum++;

        Debug.Log("Beginning Episode: " + episodeNum);

        goal = transform.parent.Find("Goal").gameObject;
        startPlat = transform.parent.Find("StartFloor").gameObject;
        firstPlat = transform.parent.Find("firstPlatform").gameObject;
        secPlat = transform.parent.Find("secondPlatform").gameObject;
        thirdPlat = transform.parent.Find("thirdPlatform").gameObject;

        lastDistance = Vector3.Distance(transform.localPosition, goal.transform.localPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); //X,Y,Z of the agent
        sensor.AddObservation(goal.transform.localPosition); //X,Y,Z of the goal

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //get two numbers from the model 
        var x = actions.DiscreteActions[0];
        var y = actions.DiscreteActions[1];
        var z = actions.DiscreteActions[2];

        //move the agent
        transform.Translate(x * moveSpeed, y * moveSpeed, z * moveSpeed);

        var dist = Vector3.Distance(transform.localPosition, goal.transform.localPosition);

        if (dist < lastDistance)
        {
            SetReward(0.01f); //tiny reward for getting close
        }
        else
        {
            SetReward(-0.01f); //penalty for moving away 
        }

        lastDistance = dist;

    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalParent = transform.parent;

        inHumanControl = false;

    }

    void Update()
    {
        if (inHumanControl)
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
        }

        if(rb.position.y < -3)
        {
            //string currentScene = "SampleScene";
            //UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene);

            EndEpisode();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Ground" || collision.collider.tag == "Goal")
        {
            isGrounded = true;
            this.transform.parent = collision.gameObject.transform;
        }

        if (collision.gameObject.CompareTag("Goal"))
        {
            Debug.Log("<color=#00ff00>GOALLLLL</color>");
            SetReward(1.0f);
            EndEpisode();
        }
 //TO DO: what do we want to decrease award for?
        //else if (collision.gameObject.CompareTag("Wall"))
        //{
        //    Debug.Log("<color=#ff0000>WALLLL</color>");
        //    SetReward(-1.0f);
        //    EndEpisode();
        //}
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

