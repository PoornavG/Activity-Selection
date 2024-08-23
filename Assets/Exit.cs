using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit(); // This will close the application when running outside the Unity Editor.
        // If you're in the Unity Editor, this won't work. In that case, you may want to add alternative behavior.
    }
}
