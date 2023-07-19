using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerController : Agent
{
    Rigidbody rb;
    bool isGrounded = false;
    Transform originalParent;

    float lastDist;

    Vector3 startingPos;

    //Human Mode
    public bool inHumanControl;

    //Agent Mode
    GameObject goal, startPlat,firstPlat, secPlat, thirdPlat;
    float lastDistance;
    GameObject trainingArea;
    GameObject environment;
    float moveSpeed = 0.10f;
    public static int episodeNum = 0;

    public override void OnEpisodeBegin()
    {
        episodeNum++;

        Debug.Log("Beginning Episode: " + episodeNum);

        firstPlat.transform.Find("trigger").GetComponent<BoxCollider>().enabled = true;
        secPlat.transform.Find("trigger").GetComponent<BoxCollider>().enabled = true;
        thirdPlat.transform.Find("trigger").GetComponent<BoxCollider>().enabled = true;

        transform.position = startingPos;
        lastDist = Mathf.Abs(goal.transform.localPosition.z - transform.localPosition.z);
    }

    public override void CollectObservations(VectorSensor sensor) //telling agent about its environment
    {
        if (inHumanControl) return;
        sensor.AddObservation(transform.localPosition); //X,Y,Z of the agent
        sensor.AddObservation(startPlat.transform.localPosition);
        sensor.AddObservation(firstPlat.transform.localPosition);
        sensor.AddObservation(secPlat.transform.localPosition);
        sensor.AddObservation(thirdPlat.transform.localPosition);
        sensor.AddObservation(goal.transform.localPosition);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (inHumanControl) return;

        //get two numbers from the model 
        var jump = actions.DiscreteActions[0];
        var move = actions.DiscreteActions[1];

       // Debug.Log(jump + " " + move);

        if(jump == 1 && isGrounded)
        {
            //Debug.Log("Jumped!");
            rb.AddForce(0, 5f, 0, ForceMode.Impulse);
        }

        if(move == 0)
        {
            //do nothing, no movement
        }

        else if(move == 1)
        {
            rb.AddForce(-5.0f, 0, 0, ForceMode.Force);
        }
        else if (move == 2)
        {
            rb.AddForce(5.0f, 0, 0, ForceMode.Force);
        }

        else if (move == 3)
        {
            rb.AddForce(0, 0, 5.0f, ForceMode.Force);
        }
        else if (move == 4)
        {
            rb.AddForce(0, 0, -5.0f, ForceMode.Force);
        }

        var curdist = Mathf.Abs(goal.transform.localPosition.z - transform.localPosition.z);
        
        if(curdist < lastDist)
        {
            SetReward(0.03f);
        }
        else
        {
            SetReward(-0.01f);
        }

        lastDist = curdist;
    }

    void Start()
    {
        startingPos = transform.position;
        rb = GetComponent<Rigidbody>();
        originalParent = transform.parent;
        isGrounded = false;

        Debug.Log(startingPos + " " + inHumanControl);

        trainingArea = transform.parent.gameObject;
        if (trainingArea.name != "TrainingArea") trainingArea = trainingArea.transform.parent.gameObject;

        environment = trainingArea.transform.Find("Environment").gameObject;

        goal = trainingArea.transform.Find("Goal").gameObject;
        startPlat = environment.transform.Find("StartFloor").gameObject;
        firstPlat = environment.transform.Find("firstPlatform").gameObject;
        secPlat = environment.transform.Find("secondPlatform").gameObject;
        thirdPlat = environment.transform.Find("thirdPlatform").gameObject;
    }

    void Update()
    {
        if (inHumanControl)
        {
            if (Input.GetKey(KeyCode.Space) && isGrounded)
            {
                rb.AddForce(0, 1f, 0, ForceMode.Impulse);
            }

            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(-5.0f, 0, 0, ForceMode.Force);
            }

            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(5.0f, 0, 0, ForceMode.Force);
            }

            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(0, 0, 5.0f, ForceMode.Force);
            }
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
            SetReward(3.0f);
            EndEpisode();
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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Death")
        {
            Debug.Log("<color=#ff0000>OH NOOOO I have fallen</color>");
            SetReward(-1.0f);
            EndEpisode();
        }

        else if(other.tag == "Reward1")
        {
            Debug.Log("<color=#0000ff>Reward1</color>");
            SetReward(0.2f);
            other.enabled = false;
        }

        else if (other.tag == "Reward2")
        {
            Debug.Log("<color=#ffff00>Reward2</color>");
            SetReward(0.3f);
            other.enabled = false;
        }

        else if (other.tag == "Reward3")
        {
            Debug.Log("<color=#ff00ff>Reward3</color>");
            SetReward(0.5f);
            other.enabled = false;
        }
    }
}

