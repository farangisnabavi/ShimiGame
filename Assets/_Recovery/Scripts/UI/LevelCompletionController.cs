using UnityEngine;
using UnityEngine.SceneManagement;

// LevelCompletionController is a small, fully independent system that shows
// a manually-designed ConfirmationPanel when the level is completed, and waits
// for the player to press OK before loading the next scene (or the main menu,
// if this was the final level).
//
// This script never touches text, UI content, or design - it only controls
// the panel's active state and scene navigation. It has no dependency on
// HUDController, MoleculeCompletionChecker, or any other existing system.
public class LevelCompletionController : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("The confirmation panel shown on level completion. Should be disabled at level start.")]
    [SerializeField] private GameObject confirmationPanel;

    [Header("Scene Navigation")]
    [Tooltip("Scene to load after the OK button is pressed on the final level.")]
    [SerializeField] private string mainMenuSceneName;

    // Prevents CompleteLevel() from opening the panel more than once.
    private bool hasCompleted = false;

    void Awake()
    {
        // Guarantee the panel starts hidden, regardless of how it was left in the Editor.
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("LevelCompletionController: Confirmation Panel reference is missing.");
        }
    }

    // Call this from your existing level-completion system when the level is finished:
    //   levelCompletionController.CompleteLevel();
    public void CompleteLevel()
    {
        // Guard against multiple triggers - once completion has started, ignore further calls.
        if (hasCompleted)
        {
            return;
        }
        hasCompleted = true;

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }

        // No text, scoring, or scene loading happens here - only the panel is shown.
        // The next scene only loads once the player presses OK.
    }

    // Connect this to the OKButton's OnClick() event.
    public void OnOKButtonPressed()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        int nextIndex = currentIndex + 1;

        if (nextIndex < totalScenes)
        {
            // Another scene exists after this one - load it by build index,
            // so no scene names are ever hard-coded here.
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            // This was the last scene in Build Settings - return to the main menu instead.
            if (string.IsNullOrEmpty(mainMenuSceneName))
            {
                Debug.LogError("LevelCompletionController: Main Menu Scene Name is empty. Assign it in the Inspector.");
                return;
            }

            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}