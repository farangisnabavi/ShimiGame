using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// MenuManager now has a single responsibility: menu flow and scene navigation.
// All button feedback (hover, click animation, colors, sounds) is handled entirely
// by ButtonInteractionEffects on each individual button - MenuManager no longer
// knows or cares about audio at all.
public class MenuManager : MonoBehaviour
{
    // ============================================================
    // SCENE MANAGEMENT
    // ============================================================
    [Header("Scene Management")]
    [Tooltip("The exact name of the gameplay scene to load. Must be added to Build Settings.")]
    [SerializeField] private string gameplaySceneName;

    // ============================================================
    // SETTINGS PANEL
    // ============================================================
    [Header("Settings Panel")]
    [Tooltip("The Settings Panel GameObject. Must have a CanvasGroup component for fading.")]
    [SerializeField] private CanvasGroup settingsPanel;

    [Tooltip("How long the fade/scale transition takes, in seconds.")]
    [SerializeField] private float transitionDuration = 0.25f;

    // The scale the panel animates from/to when closed (slightly smaller, like a pop-in effect).
    [SerializeField] private float closedScale = 0.9f;

    // ============================================================
    // INTERNAL STATE
    // ============================================================
    private Coroutine activeTransition; // Tracks the currently running fade/scale coroutine, if any
    private RectTransform settingsRectTransform; // Cached to avoid repeated GetComponent calls

    void Awake()
    {
        // Cache the settings panel's RectTransform once, since we need it every transition.
        if (settingsPanel != null)
        {
            settingsRectTransform = settingsPanel.GetComponent<RectTransform>();

            // Start with the panel fully hidden and slightly scaled down.
            settingsPanel.alpha = 0f;
            settingsPanel.interactable = false;
            settingsPanel.blocksRaycasts = false;
            if (settingsRectTransform != null)
            {
                settingsRectTransform.localScale = Vector3.one * closedScale;
            }
            settingsPanel.gameObject.SetActive(false);
        }
    }

    // ============================================================
    // START BUTTON
    // ============================================================

    // Connect this to the Start Button's OnClick event.
    public void PlayGame()
    {
        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            Debug.LogError("MenuManager: Gameplay Scene Name is empty. Assign it in the Inspector.");
            return;
        }

        if (!IsSceneInBuildSettings(gameplaySceneName))
        {
            Debug.LogError("MenuManager: Scene '" + gameplaySceneName +
                            "' was not found in Build Settings. Add it via File > Build Settings.");
            return;
        }

        Debug.Log("MenuManager: Loading gameplay scene '" + gameplaySceneName + "'...");
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Checks whether a given scene name is actually included in Build Settings,
    // preventing a silent failure or crash if it was forgotten.
    private bool IsSceneInBuildSettings(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneNameFromPath == sceneName)
            {
                return true;
            }
        }

        return false;
    }

    // ============================================================
    // SETTINGS BUTTON
    // ============================================================

    // Connect this to the Settings Button's OnClick event.
    public void OpenSettings()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("MenuManager: Settings Panel reference is missing.");
            return;
        }

        settingsPanel.gameObject.SetActive(true);
        StartTransition(true);
    }

    // Connect this to the Settings Panel's own "Close" or "Back" Button OnClick event.
    public void CloseSettings()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("MenuManager: Settings Panel reference is missing.");
            return;
        }

        StartTransition(false);
    }

    // Starts the fade/scale coroutine, stopping any previous one first
    // so opening and closing rapidly never causes overlapping animations.
    private void StartTransition(bool opening)
    {
        if (activeTransition != null)
        {
            StopCoroutine(activeTransition);
        }

        activeTransition = StartCoroutine(AnimateSettingsPanel(opening));
    }

    // Smoothly fades alpha and scales the panel in or out over transitionDuration seconds.
    private IEnumerator AnimateSettingsPanel(bool opening)
    {
        float startAlpha = settingsPanel.alpha;
        float targetAlpha = opening ? 1f : 0f;

        Vector3 startScale = settingsRectTransform != null ? settingsRectTransform.localScale : Vector3.one;
        Vector3 targetScale = opening ? Vector3.one : Vector3.one * closedScale;

        // While opening, allow interaction immediately so the panel feels responsive.
        // While closing, block interaction right away so buttons behind it can't be double-clicked mid-fade.
        settingsPanel.interactable = opening;
        settingsPanel.blocksRaycasts = opening;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            settingsPanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            if (settingsRectTransform != null)
            {
                settingsRectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            }

            yield return null;
        }

        // Snap to exact final values to avoid floating point drift.
        settingsPanel.alpha = targetAlpha;
        if (settingsRectTransform != null)
        {
            settingsRectTransform.localScale = targetScale;
        }

        // Only fully deactivate the GameObject after closing finishes,
        // so the fade-out is actually visible instead of vanishing instantly.
        if (!opening)
        {
            settingsPanel.gameObject.SetActive(false);
        }

        activeTransition = null;
    }

    // ============================================================
    // QUIT BUTTON
    // ============================================================

    // Connect this to the Quit Button's OnClick event.
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Quitting doesn't work in the Editor - stop Play Mode instead so testing feels natural.
        Debug.Log("MenuManager: Stopping Play Mode (Editor only).");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("MenuManager: Quitting application.");
        Application.Quit();
#endif
    }

    // ============================================================
    // FUTURE EXPANSION (placeholders)
    // ============================================================

    // Connect this to a future Credits Button's OnClick event.
    public void OpenCredits()
    {
        Debug.Log("Credits menu not implemented yet.");
    }

    // Connect this to a future "Back" button from any sub-menu.
    public void BackToMainMenu()
    {
        Debug.Log("Already in Main Menu.");
    }
}