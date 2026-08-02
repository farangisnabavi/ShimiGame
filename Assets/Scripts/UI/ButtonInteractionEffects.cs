using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// A complete, reusable UI button interaction script for menus.
// Handles hover scaling, hover color transitions, and physically accurate
// press/release feedback - all in code with Vector3.Lerp / Color.Lerp,
// with no Animator or animation clips involved.
//
// Attach directly to any UI Button GameObject. Each instance manages its own state,
// so it can be reused across every button in a menu without shared dependencies.
[RequireComponent(typeof(RectTransform))]
public class ButtonInteractionEffects : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // ============================================================
    // HOVER SCALE SETTINGS
    // ============================================================
    [Header("Hover Scale")]
    [Tooltip("How large the button becomes when hovered (1.1 = 10% bigger).")]
    [SerializeField] private float hoverScaleMultiplier = 1.1f;

    [Tooltip("How quickly the button scales towards its target size when hovering/unhovering.")]
    [SerializeField] private float scaleSpeed = 8f;

    // ============================================================
    // HOVER COLOR SETTINGS
    // ============================================================
    [Header("Hover Color")]
    [Tooltip("The Image component whose color changes on hover. Usually the button's own background Image.")]
    [SerializeField] private Image targetImage;

    [Tooltip("The button's color when not hovered.")]
    [SerializeField] private Color normalColor = Color.white;

    [Tooltip("The button's color while hovered.")]
    [SerializeField] private Color hoverColor = new Color(0.85f, 0.85f, 0.85f, 1f);

    [Tooltip("How quickly the color transitions. Higher = faster.")]
    [SerializeField] private float colorSpeed = 8f;

    // ============================================================
    // CLICK PRESS SETTINGS
    // ============================================================
    [Header("Click Press Effect")]
    [Tooltip("How much the button shrinks while actively pressed (0.9 = 10% smaller).")]
    [SerializeField] private float clickScaleMultiplier = 0.9f;

    [Tooltip("How quickly the button shrinks down on press. Higher = snappier.")]
    [SerializeField] private float pressScaleSpeed = 14f;

    [Tooltip("How quickly the button returns to normal/hover size after release. Higher = faster.")]
    [SerializeField] private float releaseScaleSpeed = 8f;

    // ============================================================
    // AUDIO SETTINGS
    // ============================================================
    [Header("Audio (Optional)")]
    [Tooltip("Sound played when the mouse enters the button. Leave empty to disable.")]
    [SerializeField] private AudioClip hoverSound;

    [Tooltip("Sound played when the button is pressed. Leave empty to disable.")]
    [SerializeField] private AudioClip clickSound;

    [Tooltip("AudioSource used to play the sounds above. If left empty, one is added automatically only if needed.")]
    [SerializeField] private AudioSource audioSource;

    // ============================================================
    // INTERNAL STATE
    // ============================================================
    private RectTransform rectTransform;   // Cached for performance
    private Vector3 originalScale;         // The button's resting scale, captured at Awake
    private Vector3 targetScale;           // The scale we are currently animating towards
    private bool isHovering = false;       // True while the pointer is over the button
    private bool isPressed = false;        // True while the mouse/finger is actively held down

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        targetScale = originalScale;

        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage != null)
        {
            targetImage.color = normalColor;
        }

        // Audio is optional - only add an AudioSource if a clip might actually be played.
        if (audioSource == null && (hoverSound != null || clickSound != null))
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }
    }

    void Update()
    {
        HandleScaleAnimation();
        HandleColorAnimation();
    }

    // ------------------------------------------------------------
    // Smoothly moves the button's actual scale towards targetScale every frame.
    // Uses pressScaleSpeed while actively held down (fast, snappy shrink),
    // and releaseScaleSpeed for every other transition (hover in/out, release).
    // Because this is now driven by real pointer state instead of a timer,
    // it can never desync from what the player is actually doing.
    // ------------------------------------------------------------
    private void HandleScaleAnimation()
    {
        float speed = isPressed ? pressScaleSpeed : releaseScaleSpeed;
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * speed);
    }

    // ------------------------------------------------------------
    // Smoothly transitions the target Image's color between normalColor and hoverColor.
    // Independent of scale/press state entirely, so it can never conflict with them.
    // Skipped safely if no Image is assigned.
    // ------------------------------------------------------------
    private void HandleColorAnimation()
    {
        if (targetImage == null)
        {
            return;
        }

        Color desiredColor = isHovering ? hoverColor : normalColor;
        targetImage.color = Color.Lerp(targetImage.color, desiredColor, Time.deltaTime * colorSpeed);
    }

    // Recalculates the correct target scale based on current hover/press state.
    // Press always takes priority - if the button is actively held down, it should
    // stay shrunk regardless of hover state, and only expand once released.
    private void RefreshTargetScale()
    {
        if (isPressed)
        {
            targetScale = originalScale * clickScaleMultiplier;
        }
        else if (isHovering)
        {
            targetScale = originalScale * hoverScaleMultiplier;
        }
        else
        {
            targetScale = originalScale;
        }
    }

    private void PlaySoundIfAvailable(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // ============================================================
    // EVENT SYSTEM CALLBACKS
    // Require a Canvas with a Graphic Raycaster and an EventSystem in the scene.
    // ============================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        RefreshTargetScale();
        PlaySoundIfAvailable(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        RefreshTargetScale();
    }

    // Fires the instant the mouse/finger presses down on the button -
    // this is the real, physically accurate start of the press animation.
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        RefreshTargetScale();
        PlaySoundIfAvailable(clickSound);
    }

    // Fires the instant the mouse/finger is released, whether over the button or not.
    // Correctly resolves back to hover scale (if still hovering) or normal scale.
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        RefreshTargetScale();
    }
}
