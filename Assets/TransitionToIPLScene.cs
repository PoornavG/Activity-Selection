using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionToIPLScene : MonoBehaviour
{
    public void LoadIPLScene()
    {
        SceneManager.LoadScene("IPL");
    }
}
