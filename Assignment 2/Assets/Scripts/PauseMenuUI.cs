using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel; // Panel with Resume/Main Menu
    [SerializeField] private GameObject pauseButtonUI;  // The small Pause button

    public static bool GameIsPaused { get; private set; }

    private void Start()
    {
        // Clean start state
        Time.timeScale = 1f;
        GameIsPaused = false;

        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (pauseButtonUI) pauseButtonUI.SetActive(true);
    }

    // Called by Pause button (OnClick)
    public void OnPauseButtonClicked()
    {
        if (UIManager.GameIsOver) return; // <--- prevent pause

        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
        if (pauseButtonUI) pauseButtonUI.SetActive(false);

        Time.timeScale = 0f;      // Freeze gameplay
        GameIsPaused = true;
        // Optional: AudioListener.pause = true;
    }

    // Called by Resume button (OnClick)
    public void OnResumeButtonClicked()
    {
        if (UIManager.GameIsOver) return; // <--- prevent resume

        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (pauseButtonUI) pauseButtonUI.SetActive(true);

        Time.timeScale = 1f;      // Unfreeze gameplay
        GameIsPaused = false;
        // Optional: AudioListener.pause = false;
    }

    // Called by Main Menu button (OnClick)
    public void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f;      // Always reset before changing scenes
        GameIsPaused = false;
        SceneManager.LoadScene(0); // Your Main Menu is scene 0
    }

    // Safety: if object gets disabled (scene change), ensure time is normal
    private void OnDisable()
    {
        if (GameIsPaused)
        {
            Time.timeScale = 1f;
            // AudioListener.pause = false;
        }
    }
}
