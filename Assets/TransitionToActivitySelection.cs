using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionToActivitySelection : MonoBehaviour
{
    public void LoadActivitySelectionScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
    
}
