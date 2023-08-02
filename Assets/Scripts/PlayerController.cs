using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerController : Agent
{
    Rigidbody rb;
    public bool isGrounded = false;
    Transform originalParent;

    //Human Mode
    public bool inHumanControl;

    //Agent Mode
    GameObject goal, startPlat, firstPlat, secPlat, thirdPlat, fourthPlat, goalPlat;
    Vector3 startingPos;
    GameObject trainingArea;
    GameObject environment;
    float lastDist;
    Stats stats;

    private float jumpForce = 2.5f;
    private float moveForce = 15.0f;

    private float deathReward = 20.0f;
    private float r1 = 4.0f;
    private float r2 = 4.0f;
    private float r3 = 4.0f;
    private float r4 = 4.0f;
    private float goalReward = 12.0f;
    private float rCloser = 0.02f;
    private float rFurther = 0.05f;

    private GameObject lastPlat;

    RaycastHit hit;
    LayerMask ground;

    int jump = 0;
    int move = 0;

    int myepisode = 0;

    public Vector3 RandomizePlatform(Vector3 pos)
    {
        pos.y = Random.Range(-1.0f, 1.0f);
        return pos;
    }

    public void RandomizePlatforms()
    {
        if (firstPlat != null) firstPlat.transform.localPosition = RandomizePlatform(firstPlat.transform.localPosition);
        if (secPlat != null) secPlat.transform.localPosition = RandomizePlatform(secPlat.transform.localPosition);
        if (thirdPlat != null) thirdPlat.transform.localPosition = RandomizePlatform(thirdPlat.transform.localPosition);
        if (fourthPlat != null) fourthPlat.transform.localPosition = RandomizePlatform(fourthPlat.transform.localPosition);

        var pos = RandomizePlatform(goalPlat.transform.localPosition);
        goalPlat.transform.localPosition = pos;
        pos.y += 0.75f;
        goal.transform.localPosition = pos;
    }

    public override void OnEpisodeBegin()
    {
        myepisode = stats.StartEpisode();

        if (firstPlat != null) firstPlat.transform.Find("trigger").GetComponent<BoxCollider>().enabled = true;
        if (secPlat != null) secPlat.transform.Find("trigger").GetComponent<BoxCollider>().enabled = true;
        if (thirdPlat != null) thirdPlat.transform.Find("trigger").GetComponent<BoxCollider>().enabled = true;
        if (fourthPlat != null) fourthPlat.transform.Find("trigger").GetComponent<BoxCollider>().enabled = true;
        
        transform.position = startingPos;
        lastDist = Mathf.Abs(goal.transform.localPosition.z - transform.localPosition.z);

        jump = 0;
        move = 0;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // RandomizePlatforms();

        lastPlat = startPlat;
        
    }

    public GameObject GetNextPlat()
    {
        if (lastPlat == startPlat)
        {
            if (firstPlat == null) { return goalPlat; }
            else {return firstPlat;}
        }
        else if (lastPlat == firstPlat)
        {
            if(secPlat == null) { return goalPlat; }
            else { return secPlat; }
        }
        else if (lastPlat == secPlat)
        {
            if (thirdPlat == null) { return goalPlat; }
            else { return thirdPlat; }
        }
        else if (lastPlat == thirdPlat)
        {
            if (fourthPlat == null) { return goalPlat; }
            else { return fourthPlat; }
        }
        else
        {
            return goalPlat;
        }
    }
        

    public override void CollectObservations(VectorSensor sensor) //telling agent about its environment
    {
        if (inHumanControl) return;

        // 3 floats
        sensor.AddObservation(transform.localPosition); //X,Y,Z of the agent

        // 3 floats
        sensor.AddObservation(rb.velocity);

        /*
        // 1 value
        sensor.AddObservation(isGrounded);

        // 3 floats
        sensor.AddObservation(startPlat.transform.localPosition);

        // 3 floats
        if (firstPlat != null) sensor.AddObservation(firstPlat.transform.localPosition);
        if (secPlat != null) {sensor.AddObservation(secPlat.transform.localPosition);} // 3 if used
        if (thirdPlat != null) { sensor.AddObservation(thirdPlat.transform.localPosition); } // 3 if used
        if (fourthPlat != null) { sensor.AddObservation(fourthPlat.transform.localPosition); } // 3 if used


        // 3 floats
        sensor.AddObservation(goal.transform.localPosition);
        */

        sensor.AddObservation(GetNextPlat().transform.position);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (inHumanControl) return;

        //get two numbers from the model 
        jump = actions.DiscreteActions[0];
        move = actions.DiscreteActions[1];

       
    }

    void DoTheThing()
    {
        if (jump == 1 && isGrounded)
        {
            //Debug.Log("Jumped!");
            rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
        }

        if (move == 0)
        {
            //do nothing, no movement
        }
        else if (move == 1)
        {
            rb.AddForce(0, 0, moveForce, ForceMode.Force);
        }
        else if (move == 2)
        {
            rb.AddForce(0, 0, -moveForce, ForceMode.Force);
        }
        else if (move == 3)
        {
            rb.AddForce(-moveForce, 0, 0, ForceMode.Force);
        }
        else if (move == 4)
        {
            rb.AddForce(moveForce, 0, 0, ForceMode.Force);
        }


        var curdist = Mathf.Abs(goal.transform.localPosition.z - transform.localPosition.z);

        if (curdist < lastDist)
        {
            SetReward(rCloser);
        }
        else
        {
            SetReward(-rFurther);
        }

        lastDist = curdist;
    }

    void Start()
    {
        startingPos = transform.position;
        rb = GetComponent<Rigidbody>();
        originalParent = transform.parent;
        isGrounded = false;

        stats = GameObject.FindGameObjectWithTag("Stats").GetComponent<Stats>();

//        Debug.Log(startingPos + " " + inHumanControl);

        trainingArea = transform.parent.gameObject;
        if (trainingArea.name != "TrainingArea") trainingArea = trainingArea.transform.parent.gameObject;

        environment = trainingArea.transform.Find("Environment").gameObject;

        goal = trainingArea.transform.Find("Goal").gameObject;
        startPlat = FindPlat("StartFloor");
        firstPlat = FindPlat("firstPlatform");
        secPlat = FindPlat("secondPlatform");
        thirdPlat = FindPlat("thirdPlatform");
        fourthPlat = FindPlat("fourthPlatform");

        goalPlat = FindPlat("goalPlatform");

        ground = LayerMask.GetMask("Ground");
    }

    GameObject FindPlat(string s)
    {
        Transform t;

        t = environment.transform.Find(s);
        if (t == null) return null;
        else { return t.gameObject; }
    }

    void Update()
    {
        if (inHumanControl)
        {
            jump = 0;
            move = 0;

            if (Input.GetKey(KeyCode.Space) && isGrounded)
            {
                jump = 1;
            }

            if (Input.GetKey(KeyCode.W))
            {
                move = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                move = 2;
            }

            else if (Input.GetKey(KeyCode.A))
            {
                move = 3;
            }

            else if (Input.GetKey(KeyCode.D))
            {
                move = 4;
            }

           
        }


        
    }

    void FixedUpdate()
    {
        DoTheThing();
    }


    void OnCollisionEnter(Collision collision)
    {
        lastPlat = collision.collider.gameObject;


        if (collision.collider.tag == "Ground" || collision.collider.tag == "Goal")
        {
            isGrounded = true;
            this.transform.parent = collision.gameObject.transform;
        }

        if (collision.gameObject.CompareTag("Goal"))
        {
            Debug.Log("<color=#00ff00>GOALLLLL</color>");
            SetReward(goalReward);
            EndEpisode();

            stats.AddGoal(myepisode, 1);
        }
 
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "Goal")
        { 
            isGrounded = false;
            this.transform.parent = originalParent;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Death")
        {
            //Debug.Log("<color=#ff0000>OH NOOOO</color>");
            SetReward(-deathReward);
            EndEpisode();

            stats.AddGoal(myepisode, 0);
        }

        else if(other.tag == "Reward1")
        {
           // Debug.Log("<color=#0000ff>Reward1</color>");
            SetReward(r1);
            other.enabled = false;
        }

        else if (other.tag == "Reward2")
        {
           // Debug.Log("<color=#ffff00>Reward2</color>");
            SetReward(r2);
            other.enabled = false;
        }

        else if (other.tag == "Reward3")
        {
            //Debug.Log("<color=#ff00ff>Reward3</color>");
            SetReward(r3);
            other.enabled = false;
        }

        else if (other.tag == "Reward4")
        {
            //Debug.Log("<color=#ff00ff>Reward4</color>");
            SetReward(r4);
            other.enabled = false;
        }
    }
}

