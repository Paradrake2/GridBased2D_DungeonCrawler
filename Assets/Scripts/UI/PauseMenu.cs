using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused;

    void Awake()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);
        if (quitButton != null)
            quitButton.onClick.AddListener(Quit);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
