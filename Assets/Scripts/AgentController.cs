using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    // Pellet variables
    [SerializeField] private Transform target; // Reference to a target (not currently used but can be extended for specific target behaviors)
    public int pelletCount; // Number of pellets to spawn
    public GameObject food; // Pellet prefab
    [SerializeField] private List<GameObject> spawnedPelletsList = new List<GameObject>(); // List of currently spawned pellets

    // Agent variables
    [SerializeField] private float moveSpeed = 4f; // Movement speed of the agent
    private Rigidbody rb; // Rigidbody for handling physics-based movement

    // Environment variables
    [SerializeField] private Transform environmentLocation; // Reference to the environment's transform
    Material envMaterial; // Material of the environment (used to change color based on outcomes)
    public GameObject env; // Environment GameObject reference

    // Time keeping variables
    [SerializeField] private int timeForEpisode; // Maximum time allowed for an episode
    private float timeLeft; // Tracks remaining time for the current episode

    // Initialize variables and references at the start
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        envMaterial = env.GetComponent<Renderer>().material; // Get the material of the environment
    }

    // Resets the environment and agent at the start of each episode
    public override void OnEpisodeBegin()
    {
        // Randomize agent's position
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));

        // Spawn pellets in the environment
        CreatePellet();
        
        // Start the episode timer
        EpisodeTimerNew();
    }

    // Update is called once per frame to check if the timer has expired
    private void Update()
    {
        CheckRemainingTime();
    }

    // Creates and spawns pellets at random positions in the environment
    private void CreatePellet()
    {
        // If pellets already exist, remove them
        if(spawnedPelletsList.Count != 0)
        {
            RemovePellet(spawnedPelletsList);
        }

        // Spawn the specified number of pellets
        for(int i = 0; i < pelletCount; i++)
        {
            int counter = 0; // Limit retries to avoid infinite loops
            bool distanceGood;
            bool alreadyDecremented = false;

            // Instantiate a new pellet
            GameObject newPellet = Instantiate(food);

            // Parent the pellet to the environment
            newPellet.transform.parent = environmentLocation;

            // Generate a random position for the pellet
            Vector3 pelletLocation = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));

            // Ensure the pellet doesn't overlap with other pellets or the agent
            if(spawnedPelletsList.Count != 0)
            {
                for(int k = 0; k < spawnedPelletsList.Count; k++)
                {
                    if(counter < 10) // Retry up to 10 times
                    {
                        distanceGood = CheckOverlap(pelletLocation, spawnedPelletsList[k].transform.localPosition, 5f);
                        if(distanceGood == false)
                        {
                            pelletLocation = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));
                            k--;
                            alreadyDecremented = true;
                        }

                        distanceGood = CheckOverlap(pelletLocation, transform.localPosition, 5f);
                        if(distanceGood == false)
                        {
                            pelletLocation = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));
                            if(alreadyDecremented == false)
                            {
                                k--;
                            }
                        }

                        counter++;
                    }
                    else
                    {
                        k = spawnedPelletsList.Count; // Exit loop if retries are exhausted
                    }
                }
            }

            // Assign the final position to the pellet
            newPellet.transform.localPosition = pelletLocation;

            // Add the pellet to the list
            spawnedPelletsList.Add(newPellet);
        }
    }

    // Checks if the position is a sufficient distance from another object
    private bool CheckOverlap(Vector3 objectWeWantToAvoidOverlapping, Vector3 alreadyExistingObject, float minDistanceWanted)
    {
        float DistanceBetweenObjects = Vector3.Distance(objectWeWantToAvoidOverlapping, alreadyExistingObject);
        if(minDistanceWanted <= DistanceBetweenObjects)
        {
            return true;
        }
        return false;
    }

    // Removes all pellets and clears the list
    private void RemovePellet(List<GameObject> toBeDeletedGameObjectList)
    {
        foreach(GameObject i in toBeDeletedGameObjectList)
        {
            Destroy(i.gameObject);
        }
        toBeDeletedGameObjectList.Clear();
    }

    // Collect observations for the neural network
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); // Observe the agent's position
    }

    // Receives actions from the neural network and applies them to the agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0]; // Rotation action
        float moveForward = actions.ContinuousActions[1]; // Forward movement action

        // Apply movement and rotation
        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);
    }

    // Manual control for debugging or testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal"); // Horizontal input for rotation
        continuousActions[1] = Input.GetAxisRaw("Vertical"); // Vertical input for forward/backward movement
    }

    // Handles collision events with pellets or walls
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Pellet")
        {
            // Remove the pellet from the list and destroy it
            spawnedPelletsList.Remove(other.gameObject);
            Destroy(other.gameObject);

            // Reward the agent
            AddReward(10f);

            // Check if all pellets are collected
            if(spawnedPelletsList.Count == 0)
            {
                envMaterial.color = Color.green; // Change environment color to green
                RemovePellet(spawnedPelletsList);
                AddReward(5f); // Additional reward
                EndEpisode(); // End the episode
            }
        }
        if(other.gameObject.tag == "Wall")
        {
            envMaterial.color = Color.red; // Change environment color to red
            RemovePellet(spawnedPelletsList);
            AddReward(-15f); // Penalize the agent
            EndEpisode(); // End the episode
        }
    }

    // Resets the timer for the episode
    private void EpisodeTimerNew()
    {
        timeLeft = Time.time + timeForEpisode; // Set the remaining time
    }

    // Checks if the episode timer has expired
    private void CheckRemainingTime()
    {
        if(Time.time >= timeLeft)
        {
            envMaterial.color = Color.black; // Change environment color to black
            AddReward(-15f); // Penalize the agent
            RemovePellet(spawnedPelletsList);
            EndEpisode(); // End the episode
        }
    }
}