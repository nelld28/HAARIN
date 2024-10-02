

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour, IMixedRealitySpeechHandler
{
    public agentController agent; // Reference to your agent controller
    public Transform[] destinations; // List of destination points
    public GameObject firstPanel; // Reference to the first UI Panel
    public GameObject secondPanel; // Reference to the second UI Panel

    // Start by showing the first panel
    private void Start()
    {
        firstPanel.SetActive(true);
        secondPanel.SetActive(false);
    }

    // Function to switch from the first panel to the second panel
    public void ShowSecondPanel()
    {
        Debug.Log("ShowSecondPanel called");
        firstPanel.SetActive(false); // Hide the first panel
        secondPanel.SetActive(true); // Show the second panel
    }

    // Function to navigate to a room based on room index
    public void NavigateToRoom(int roomIndex)
    {
        if (roomIndex >= 0 && roomIndex < destinations.Length)
        {
            Debug.Log("Navigating to room " + roomIndex);
            agent.NavigateTo(destinations[roomIndex].position);
            secondPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Invalid room index: " + roomIndex);
        }
    }

    public void ExitScene()
    {
        Debug.Log("Exiting scene...");
        // Load the desired scene (e.g., main menu or exit scene)
        SceneManager.LoadScene("Mains"); // Replace with your scene name
    }

    // Register the speech handler when the GameObject is enabled
    private void OnEnable()
    {
        MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>()?.RegisterHandler<IMixedRealitySpeechHandler>(this);

    }

    // Unregister the speech handler when the GameObject is disabled
    private void OnDisable()
    {
        MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>()?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
    }

    // Implement the speech keyword recognition
    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        Debug.Log("Voice command recognized: " + eventData.Command.Keyword);

        switch (eventData.Command.Keyword.ToLower())
        {
            case "okay":
                ShowSecondPanel(); // When the user says "Okay", switch to the second panel
                break;

            case "room one":
                NavigateToRoom(0); // Navigate to the first room
                break;

            case "room two":
                NavigateToRoom(1); // Navigate to the second room
                break;

            case "room three":
                NavigateToRoom(2); // Navigate to the third room (add more rooms as needed)
                break;

            case "exit scene":
                ExitScene();
                break;

            default:
                Debug.Log("Unrecognized voice command");
                break;
        }
    }
}

