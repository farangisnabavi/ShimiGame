using UnityEngine;

public class SnapVFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SnapSystem3D snapSystem;
    [SerializeField] private ParticleSystem snapEffect;

    private void Awake()
    {
        if (snapSystem == null)
            snapSystem = GetComponent<SnapSystem3D>();
    }

    private void OnEnable()
    {
        if (snapSystem != null)
            snapSystem.OnSnapped += PlaySnapEffect;
    }

    private void OnDisable()
    {
        if (snapSystem != null)
            snapSystem.OnSnapped -= PlaySnapEffect;
    }

    private void PlaySnapEffect()
    {
        if (snapEffect == null)
        {
            Debug.LogWarning($"SnapVFX: Snap effect is not assigned on {gameObject.name}.");
            return;
        }

        snapEffect.Play();
    }
}