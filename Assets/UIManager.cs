using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InputField activityNameInput;
    public InputField durationInput;
    public InputField priorityInput;
    public GameObject resourcesPanel;

    public void ClearUI()
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
    }
}
