using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Unit : MonoBehaviour
{
    [Header("Data")]
    public UnitData data;

    public UnitType Type        => data ? data.type        : UnitType.Archer;
    public string   DisplayName => data ? data.displayName : name;

    [Header("State")]
    public bool isRecruited;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer; // ВИЗУАЛ
    [SerializeField] private Transform visualRoot;           // корень визуала

    private void Awake()
    {
        isRecruited = false;

        // автоматически ищем визуал
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (visualRoot == null && spriteRenderer != null)
            visualRoot = spriteRenderer.transform;
    }

    public void Recruit()
    {
        if (isRecruited) return;
        isRecruited = true;

        // ❄ стопаем idle корректно
        var idle = GetComponent<UnitIdleAnimator>();
        if (idle != null) idle.StopIdle();

        Debug.Log($"{DisplayName} has been recruited!");

        StartCoroutine(SolarFlashAndDestroy());
    }

    private IEnumerator SolarFlashAndDestroy()
    {
        if (spriteRenderer == null)
        {
            Destroy(gameObject);
            yield break;
        }

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Color baseColor = spriteRenderer.color;
        Vector3 baseScale = visualRoot.localScale;

        Color sunColor = new Color(1f, 0.9f, 0.4f); // золотистый

        float duration = 2f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);

            // цвет + яркость
            Color tinted = Color.Lerp(baseColor, sunColor, k);
            float brightness = Mathf.Lerp(1f, 2f, k);

            spriteRenderer.color = new Color(
                tinted.r * brightness,
                tinted.g * brightness,
                tinted.b * brightness,
                Mathf.Lerp(1f, 0f, k)
            );

            // ⚠ масштабируем ТОЛЬКО ВИЗУАЛ
            visualRoot.localScale = baseScale * Mathf.Lerp(1f, 1.3f, k);

            yield return null;
        }

        Destroy(gameObject);
    }
}
