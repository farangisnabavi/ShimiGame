using UnityEngine;
using UnityEngine.SceneManagement;

// HUDController manages switching between the main HUD and the Help panel,
// and handles navigating back to the main menu scene.
//
// It only toggles GameObject active states - it never touches the Scroll View
// or its contents, and never searches the scene for references (all references
// come from the Inspector).
public class HUDController : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("The main HUD panel, visible during normal gameplay.")]
    [SerializeField] private GameObject hudPanel;

    [Tooltip("The Help panel, containing the Scroll View with tutorial images and the Back button.")]
    [SerializeField] private GameObject helpPanel;

    [Header("Scene Navigation")]
    [Tooltip("The exact name of the main menu scene to load. Must be added to Build Settings.")]
    [SerializeField] private string menuSceneName;

    void Awake()
    {
        // Set the correct initial state as soon as the scene loads:
        // HUD visible, Help panel hidden. This guarantees HelpPanel can never
        // accidentally appear at startup, regardless of how it was left in the Editor.
        InitializeUI();
    }

    // Ensures the HUD is showing and the Help panel is hidden.
    private void InitializeUI()
    {
        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("HUDController: HUD Panel reference is missing.");
        }

        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("HUDController: Help Panel reference is missing.");
        }
    }

    // Connect this to HelpButton's OnClick() event.
    // Only toggles panel active states - the Scroll View and its content
    // inside HelpPanel are never touched, so their state persists as-is.
    public void OpenHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
        }

        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }
    }

    // Connect this to BackButton's OnClick() event (inside HelpPanel).
    public void CloseHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }

        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }
    }

    // Connect this to MenuButton's OnClick() event.
    // Validates the scene name before attempting to load, so a missing/misspelled
    // name fails with a clear Console error instead of a silent or crashing load.
    public void OpenMainMenu()
    {
        if (string.IsNullOrEmpty(menuSceneName))
        {
            Debug.LogError("HUDController: Menu Scene Name is empty. Assign it in the Inspector.");
            return;
        }

        SceneManager.LoadScene(menuSceneName);
    }
}