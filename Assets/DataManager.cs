using UnityEngine;

public class DataStorage : MonoBehaviour
{
    private string storedActivityName;
    private int storedDuration;
    private int storedPriority;

    public void StoreData(string activityName, int duration, int priority)
    {
        storedActivityName = activityName;
        storedDuration = duration;
        storedPriority = priority;
    }

    public void ClearStoredData()
    {
        storedActivityName = "";
        storedDuration = 0;
        storedPriority = 0;
    }

    // You can add more methods here to retrieve stored data if needed
}
