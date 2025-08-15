using UnityEngine;
using UnityEngine.UI;
// If using TextMeshPro, replace "Text" with "TMPro.TextMeshProUGUI"

public class SimplePause : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] private Button pauseResumeButton;
    [SerializeField] private TMPro.TextMeshProUGUI buttonText;
    // Change to TMPro.TextMeshProUGUI if using TextMeshPro

    void Start()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        buttonText.text = "PAUSE";

        // Link the button click
        pauseResumeButton.onClick.AddListener(TogglePause);
    }

    void Update()
    {
        // Press Esc to toggle pause
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        buttonText.text = "RESUME";
    }

    void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        buttonText.text = "PAUSE";
    }
}
