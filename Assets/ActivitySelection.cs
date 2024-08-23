using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ActivitySelection : MonoBehaviour
{
    public InputField activityNameInput;
    public InputField durationInput;
    public InputField priorityInput;
    public GameObject resourcesPanel; // Panel containing resource toggles
    public GameObject tableContainer; // Container for the table rows
    public GameObject rowTemplate; // Template for a table row
    public Text resultText; // Text field to display output

    private List<Activity> activities = new List<Activity>();
    private Dictionary<string, bool> resourceAvailability = new Dictionary<string, bool>();
    private float currentTime = 0f; // Track current time

    // Class to represent an activity
    public class Activity
    {
        public string name;
        public int priority;
        public int duration;
        public List<string> requiredResources; // List of required resources

        public Activity(string name, int priority, int duration, List<string> requiredResources)
        {
            this.name = name;
            this.priority = priority;
            this.duration = duration;
            this.requiredResources = requiredResources;
        }
    }

    // PriorityQueue implementation
    private class PriorityQueue<T>
    {
        private SortedDictionary<int, Queue<T>> _dictionary = new SortedDictionary<int, Queue<T>>();

        public int Count { get; private set; }

        public void Enqueue(T item, int priority)
        {
            if (!_dictionary.ContainsKey(priority))
                _dictionary[priority] = new Queue<T>();
            _dictionary[priority].Enqueue(item);
            Count++;
        }

        public T Dequeue()
        {
            if (Count == 0)
                throw new System.InvalidOperationException("Queue empty.");
            foreach (var key in _dictionary.Keys.Reverse()) // Reverse iteration to dequeue higher priority items first
            {
                var queue = _dictionary[key];
                if (queue.Count > 0)
                {
                    Count--;
                    return queue.Dequeue();
                }
            }
            throw new System.InvalidOperationException("Queue empty.");
        }
    }

    // Add activity data to memory and clear input fields
    public void AddActivity()
    {
        try
        {
            string name = activityNameInput.text;
            int duration = int.Parse(durationInput.text);
            int priority = int.Parse(priorityInput.text);
            List<string> requiredResources = new List<string>(); // Initialize list of required resources

            // Add required resources based on toggled checkboxes
            foreach (Transform child in resourcesPanel.transform)
            {
                Toggle toggle = child.GetComponent<Toggle>();
                if (toggle.isOn)
                {
                    requiredResources.Add(toggle.GetComponentInChildren<Text>().text);
                }
            }

            Activity newActivity = new Activity(name, priority, duration, requiredResources);
            activities.Add(newActivity);

            // Clear input fields
            ClearInputFields();

            Debug.Log("Activity added: " + newActivity.name + ", Priority: " + newActivity.priority + ", Duration: " + newActivity.duration);

            // Update the table
            UpdateTable();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error adding activity: " + e.Message);
        }
    }

    // Clear input fields including toggles
    private void ClearInputFields()
    {
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

    // Update the table with current activities
    private void UpdateTable()
    {
        // Clear existing rows
        foreach (Transform child in tableContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Add rows for each activity
        foreach (Activity activity in activities)
        {
            GameObject newRow = Instantiate(rowTemplate, tableContainer.transform);
            Text[] columns = newRow.GetComponentsInChildren<Text>();
            columns[0].text = activity.name;
            columns[1].text = activity.priority.ToString();
            columns[2].text = activity.duration.ToString();
            columns[3].text = string.Join(", ", activity.requiredResources);
        }
    }

    // Schedule activities without executing them
    private void ScheduleActivities(PriorityQueue<Activity> activityQueue)
    {
        // Clear previous schedules
        currentTime = 0f;
        resultText.text = "";

        // Initialize resource availability
        resourceAvailability.Clear();
        foreach (var activity in activities)
        {
            foreach (var resource in activity.requiredResources)
            {
                resourceAvailability[resource] = true;
            }
        }

        // Dictionary to track the latest end time for activities requiring the same resources
        Dictionary<string, float> maxEndTimeForResource = new Dictionary<string, float>();

        while (activityQueue.Count > 0)
        {
            // Dequeue the highest priority activity
            Activity currentActivity = activityQueue.Dequeue();

            // Calculate start time based on the maximum end time of dependencies
            float startTime = 0f;
            foreach (string resource in currentActivity.requiredResources)
            {
                // Get the maximum end time for activities requiring the same resource
                if (maxEndTimeForResource.ContainsKey(resource))
                {
                    startTime = Mathf.Max(startTime, maxEndTimeForResource[resource]);
                }
            }

            // Calculate end time
            float endTime = startTime + currentActivity.duration;

            // Update the maximum end time for each resource
            foreach (string resource in currentActivity.requiredResources)
            {
                if (maxEndTimeForResource.ContainsKey(resource))
                {
                    maxEndTimeForResource[resource] = Mathf.Max(maxEndTimeForResource[resource], endTime);
                }
                else
                {
                    maxEndTimeForResource[resource] = endTime;
                }
            }

            // Update the result text with activity information
            resultText.text += "Activity: " + currentActivity.name + ", Start: " + startTime + ", End: " + endTime + ", Priority: " + currentActivity.priority + ", Resources: " + string.Join(", ", currentActivity.requiredResources.ToArray()) + "\n";

            // Update current time for the next activity
            currentTime = endTime;
        }
    }

    // Start activity selection based on priority
    public void StartActivitySelection()
    {
        // Clear the result text
        resultText.text = "";

        // Sort activities by priority (lower priority first)
        activities.Sort((a1, a2) => a1.priority.CompareTo(a2.priority));

        // Use a priority queue for efficient activity selection
        PriorityQueue<Activity> activityQueue = new PriorityQueue<Activity>();

        // Enqueue activities with their priority as the key
        foreach (Activity activity in activities)
        {
            activityQueue.Enqueue(activity, activity.priority);
        }

        ScheduleActivities(activityQueue);
    }

    public void ClearAllData()
    {
        // Clear activities list
        activities.Clear();

        // Clear input fields
        ClearInputFields();

        // Clear result text
        resultText.text = "";

        // Clear resource availability dictionary
        resourceAvailability.Clear();

        // Reset current time
        currentTime = 0f;

        // Update the table
        UpdateTable();
    }
}
