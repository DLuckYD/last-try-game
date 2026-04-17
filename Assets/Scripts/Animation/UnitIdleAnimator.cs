using UnityEngine;

public class UnitIdleAnimator : MonoBehaviour
{
    public enum IdleMode { MoveUpDown, ScaleBreathing }

    [Header("Что анимируем (двигаем/скейлим ТОЛЬКО визуал)")]
    [SerializeField] private Transform spriteRoot;

    [Header("Режим анимации (рандом выбирается автоматически)")]
    public IdleMode mode;

    [Header("Дыхание через Scale (1.0–1.1–0.9)")]
    public float scaleAmplitude = 0.10f;

    [Header("Поднимание вверх-вниз (в ЛОКАЛЬНЫХ координатах)")]
    public float moveAmplitude = 0.08f;

    [Header("Скорость анимации")]
    public float frequency = 1.6f;

    private Vector3 baseLocalPos;
    private Vector3 baseLocalScale;
    private float phase;

    private void Reset()
    {
        // Если забудешь назначить — пусть по умолчанию будет ТОЛЬКО визуальная часть
        // (обычно SpriteRenderer/Graphics). Но если ее нет — оставим transform.
        if (spriteRoot == null)
            spriteRoot = GetComponentInChildren<SpriteRenderer>() ? GetComponentInChildren<SpriteRenderer>().transform : transform;
    }

    private void Awake()
    {
        if (spriteRoot == null)
            spriteRoot = GetComponentInChildren<SpriteRenderer>() ? GetComponentInChildren<SpriteRenderer>().transform : transform;

        // ВАЖНО: сохраняем локальные, а не мировые.
        baseLocalPos = spriteRoot.localPosition;
        baseLocalScale = spriteRoot.localScale;

        // рандом фаза
        phase = Random.Range(0f, Mathf.PI * 2f);

        // рандом режим
        mode = (Random.value < 0.5f) ? IdleMode.MoveUpDown : IdleMode.ScaleBreathing;

        // рандом частота
        frequency *= Random.Range(0.85f, 1.15f);
    }

    private void OnEnable()
    {
        // Если объект двигали/репарентили — берем текущие значения как базу,
        // чтобы idle НЕ тянул назад и не "ломал" позицию.
        if (spriteRoot == null) return;
        baseLocalPos = spriteRoot.localPosition;
        baseLocalScale = spriteRoot.localScale;
    }

    private void LateUpdate()
    {
        // LateUpdate — чтобы idle применялся ПОСЛЕ всех перемещений/скриптов за кадр.
        if (spriteRoot == null) return;

        float t = Time.time * frequency + phase;
        float s = Mathf.Sin(t);

        // всегда отталкиваемся от актуальной базы (baseLocalPos/baseLocalScale)
        if (mode == IdleMode.MoveUpDown)
        {
            spriteRoot.localPosition = baseLocalPos + new Vector3(0f, s * moveAmplitude, 0f);
            spriteRoot.localScale = baseLocalScale;
        }
        else
        {
            float factor = 1f + s * scaleAmplitude;
            spriteRoot.localScale = baseLocalScale * factor;
            spriteRoot.localPosition = baseLocalPos;
        }
    }

    public void StopIdle()
    {
        enabled = false;
        RestoreBase();
    }

    public void SetIdleFrozen(bool frozen)
    {
        enabled = !frozen;
        if (frozen) RestoreBase();
        else CaptureBase(); // когда размораживаем — берем текущую позу как новую базу
    }

    private void CaptureBase()
    {
        if (spriteRoot == null) return;
        baseLocalPos = spriteRoot.localPosition;
        baseLocalScale = spriteRoot.localScale;
    }

    private void RestoreBase()
    {
        if (spriteRoot == null) return;
        spriteRoot.localPosition = baseLocalPos;
        spriteRoot.localScale = baseLocalScale;
    }
}
