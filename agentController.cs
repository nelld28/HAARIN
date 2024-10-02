
using UnityEngine;
using UnityEngine.AI;
using Microsoft.MixedReality.Toolkit;

public class agentController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform user;  
    public float userWaitDistance = 3.0f;   // Distance at which the agent waits for the user
    public float destinationThreshold = 0.1f; // Distance at which the agent considers it has reached the destination
    public float spawnDistanceFromUser = 5f; // Distance at which the agent appears from the user
    public LayerMask nonWalkableLayers;  // Layer mask to define non-walkable layers

    private Vector3 currentDestination;
    private bool hasDestination = false; // Track if a destination is set

    // Reference to the Animator
    private Animator animator;

    private bool isWalking;
    public AudioSource salsaAudio1;
    public AudioSource salsaAudio2;
    public AudioSource salsaAudio3;


    public UIManager uiManager;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Find AudioSources in child object
        AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
        
        if (audioSources.Length >= 2)
        {
            salsaAudio1 = audioSources[0];  // First AudioSource found in the child
            salsaAudio2 = audioSources[1];  // Second AudioSource found in the child
            salsaAudio3 = audioSources[2];  // Second AudioSource found in the child

        }   
        else
        {
            Debug.LogError("Not enough AudioSource components found in child objects!");
        }

        // Set the agent to appear at a certain distance from the user at the start
        PlaceAgentAtDistanceFromUser();
        
    }

    void Update()
    {
        if (!hasDestination) return; // Do nothing if no destination is set

        // Get the position of the user (either the assigned Transform or Unity's main camera)
        Vector3 userPosition;

        if (user != null)
        {
            userPosition = user.position;
        }
        else
        {
            // Use Unity's main camera instead of MRTK
            userPosition = Camera.main.transform.position;
        }

        // Calculate distances
        float distanceToUser = Vector3.Distance(transform.position, userPosition);
        float distanceToDestination = Vector3.Distance(transform.position, currentDestination);

        // Check if the agent has reached the destination
        if (distanceToDestination <= destinationThreshold)
        {
            agent.isStopped = true;
            hasDestination = false; // Reset destination once reached

            animator.SetBool("isWalking",false);

            Debug.Log("Destination reached");

            // Play the second salsa audio when the destination is reached
            if (salsaAudio2 != null && !salsaAudio2.isPlaying)
            {
                salsaAudio2.Play();
            }
            FaceUser(userPosition);
            uiManager.ShowSecondPanel();
            return;
        }

        

        // If the user is too far behind, the agent waits for the user
        if (hasDestination && distanceToUser > userWaitDistance )
        {
            agent.isStopped = true;
            animator.SetBool("isWalking",false);
            FaceUser(userPosition);
            // Play the second salsa audio when the destination is reached
            if (salsaAudio3 != null && !salsaAudio3.isPlaying)
            {
                salsaAudio3.Play();
            }



        }
        else
        {
            // If the user is within range, continue moving to the destination
            agent.isStopped = false;
            animator.SetBool("isWalking",true);
            
        }
        
    }

    // Method to set the destination and start guiding the user
    public void NavigateTo(Vector3 destination)
    {
        currentDestination = destination;
        hasDestination = true; // Mark that we have a destination
        agent.isStopped = false; // Make sure the agent starts moving
        agent.SetDestination(destination); // Update the agent's destination
        animator.SetBool("isWalking",false);
        
    }

    

    // Place the agent at a certain distance from the user at the start
    private void PlaceAgentAtDistanceFromUser()
    {
               
        Vector3 userPosition;
        Vector3 cameraForward;
        
        if (Camera.main != null)
        {
            
            userPosition = Camera.main.transform.position;
            cameraForward = Camera.main.transform.forward;
        }
        else
        {
            Debug.LogError("No camera or playspace found!");
            return;
        }
        

        // Calculate a spawn position at a specified distance in front of the user
        Vector3 spawnPosition = userPosition + cameraForward * spawnDistanceFromUser;
        
        // Place the agent at the calculated spawn position
        agent.transform.position = spawnPosition;

        // Ensure the agent is not moving initially
        agent.isStopped = true;
        FaceUser(userPosition);
        animator.SetBool("isWalking", false);
    }

     // Method to make the agent face the user
    private void FaceUser(Vector3 userPosition)
    {
        Vector3 lookDirection = userPosition - transform.position;
        lookDirection.y = 0; // Keep the agent upright
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
    
}
