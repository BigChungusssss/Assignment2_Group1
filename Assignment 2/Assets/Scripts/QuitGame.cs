using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quit button pressed");

        // Quit the built game
        Application.Quit();

#if UNITY_EDITOR
        // Quit playmode in the editor (so it works while testing)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
