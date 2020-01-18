using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseController : MonoBehaviour
{
    GameObject player; //player reference
    AudioSource sound; //sound source for the mouse
    bool runMode; //is the mouse in run mode
    bool startRun; //flag for if this is the start of the run
    [Tooltip("<velocity, angular velocity, acceleration>")]
    public Vector3 runningMetrics; //angular velocity to use when running, high for snappy turns
    Vector3 normalMetrics; //normal movement metrics
    public NavMeshAgent agent; //agent to move the mouse
    public List<Transform> waypoints; //waypoints to navigate to
    public float minDistance; //min distance to waypoint
    public float minDistanceToRun; //how close the player needs to be at minimum to run
    public float maxDistanceToRun; //how far the mouse will try to get away before stopping running
    public bool switchAtEnd; //will the mouse switch direction at the end of the list?
    bool up; //if switched direction, go up or down by requirement
    int i; //index of travel

    // Start is called before the first frame update
    void Start()
    {
        //set up sound
        sound = GetComponent<AudioSource>();

        //set initial position
        i = 0;
        agent.SetDestination(waypoints[i].position);
        i++;

        //set normal metrics
        normalMetrics[0] = agent.speed;
        normalMetrics[1] = agent.angularSpeed;
        normalMetrics[2] = agent.acceleration;

        startRun = false;
        runMode = false;
        up = true;
        player = LevelManager.Instance.player;

        //connect messenger
        agent.enabled = true;
        Messenger.AddListener(GameEvents.INCREASED_SIZE, CheckSize);
    }

    //update sets the path of the agent
    private void Update()
    {
        //if not running
        if (!runMode)
        {
            NewMove();
        }//else we runnin' bois, get to da chedder
        else
        {
            //player is close on first time seen, RUN
            if(!startRun && (transform.position - player.transform.position).magnitude <= minDistanceToRun)
            {
                startRun = true;

                //TODO: play sound
                sound.Play();

                //set running angular velocity
                agent.speed = runningMetrics[0];
                agent.angularSpeed = runningMetrics[1];
                agent.angularSpeed = runningMetrics[2];
                agent.SetDestination((transform.position - player.transform.position).normalized * maxDistanceToRun);
            }
            else if(startRun && (player.transform.position - transform.position).magnitude <= maxDistanceToRun)
            {
                agent.SetDestination(transform.position + (transform.position - player.transform.position).normalized * maxDistanceToRun);
            }
            else //else far enough away, go back to normal
            {
                //if first time going back to safety
                if (startRun)
                {
                    //set metrics
                    agent.speed = normalMetrics[0];
                    agent.angularSpeed = normalMetrics[1];
                    agent.angularSpeed = normalMetrics[2];

                    //reset flag
                    startRun = false;
                }
                NewMove();
            }
        }
    }

    //attempts movement to new position
    void NewMove()
    {
        //if at last index and we want to switch, set flag
        if (i == waypoints.Count - 1 && switchAtEnd)
        {
            up = false;
        }
        else if (i == 0 && !up) //else if going down and at last index
        {
            up = true;
        }
        else //else not switching, check extreme
        {
            if(i >= waypoints.Count)
            {
                i = 0;
            }
        }

        //if close enough to go to next index
        if ((transform.position - waypoints[i].position).magnitude <= minDistance)
        {
            //set next position
            agent.SetDestination(waypoints[i].position);

            //increment or decrement
            if (up) i++;
            else i--;
        }
        else
        {
            agent.SetDestination(waypoints[i].position);
        }
    }

    /*//set up listeners
    private void OnEnable()
    {
        agent.enabled = true;
        Messenger.AddListener(GameEvents.INCREASED_SIZE, CheckSize);
    }*/

    //removes listener
    private void OnDisable()
    {
        Messenger.RemoveListener(GameEvents.INCREASED_SIZE, CheckSize);
    }

    //when picked up, disables this behavior
    public void PickedUp()
    {
        agent.enabled = false;
        this.enabled = false;
    }

    //checks the size of the player, sets to run mode if player is bigger
    void CheckSize()
    {
        if(player.GetComponent<Pickup>().size >= GetComponent<Pickupable>().size)
        {
            runMode = true;
        }
    }
}
