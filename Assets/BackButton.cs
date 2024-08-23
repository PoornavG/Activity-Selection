using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ResetAndBack : MonoBehaviour
{
    public InputField activityNameInput;
    public InputField durationInput;
    public InputField priorityInput;
    public GameObject resourcesPanel;
    public Text resultText;
    public DataStorage dataStorage; // Reference to the DataStorage script

    // List to store activities
    private List<ActivitySelection.Activity> activities = new List<ActivitySelection.Activity>();
    // Dictionary to track resource availability
    private Dictionary<string, bool> resourceAvailability = new Dictionary<string, bool>();

    // Method to clear stored data
    private void ClearData()
    {
        activities.Clear();
        resourceAvailability.Clear();
    }

    // Method to clear UI elements
    private void ClearUI()
    {
        // Clear input fields
        activityNameInput.text = "";
        durationInput.text = "";
        priorityInput.text = "";

        // Reset toggle states
        foreach (Transform child in resourcesPanel.transform)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            toggle.isOn = false;
        }

        // Clear result text
        resultText.text = "";
    }

    // Attach this method to the back button's OnClick event in the Unity Editor
    public void OnBackButtonClicked()
    {
        // Load the home screen scene or perform any other action
        SceneManager.LoadScene("Home"); // Replace "HomeScreen" with the name of your home screen scene
    }

    // Attach this method to the reset button's OnClick event in the Unity Editor
    public void OnResetButtonClicked()
    {
        // Clear stored data
        ClearData();

        // Clear UI elements
        ClearUI();

        // Clear data stored by DataStorage script
        dataStorage.ClearStoredData();
    }
}
