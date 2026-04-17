using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DiceSystem : MonoBehaviour
{
    [SerializeField] private Image uiImage;

    [Header("Sprites")]
    [Tooltip("Index 0 = EMPTY, Index 1..20 = faces 1..20")]
    [SerializeField] private Sprite[] faces; // must be 21

    [Header("Roll Animation")]
    [SerializeField] private float rollDuration = 1.0f;
    [SerializeField] private int flickerSteps = 12; // swaps count during roll
    [SerializeField] private bool startWithEmpty = true;

    [Header("Fail Logic")]
    [Range(0f, 100f)]
    [SerializeField] private float failChancePercent = 30f;

    public bool IsRolling { get; private set; }
    public int LastRoll { get; private set; } = 8;

    public event Action<int, bool> OnRollFinished; // (roll, isFail)

    private Coroutine rollRoutine;

    // -------------------------------------------------------
    // PUBLIC API
    // -------------------------------------------------------

    /// <summary>
    /// Ролл і ЧЕКАТИ, поки анімація завершиться. Повертає roll + isFail.
    /// Викликається як: yield return dice.RollAndWait(result => {...});
    /// </summary>
    public IEnumerator RollAndWait(Action<int, bool> onResult = null)
    {
        int finalRoll = UnityEngine.Random.Range(1, 21);
        yield return RollToAndWait(finalRoll, onResult);
    }

    /// <summary>
    /// Контрольований ролл (наприклад, для тестів), і теж чекати.
    /// </summary>
    public IEnumerator RollToAndWait(int finalRoll, Action<int, bool> onResult = null)
    {
        finalRoll = Mathf.Clamp(finalRoll, 1, 20);

        if (IsRolling)
            yield break;

        yield return StartRollRoutine(finalRoll, onResult);
    }

    /// <summary>
    /// Старт без очікування (fire-and-forget). Якщо треба паралельно.
    /// </summary>
    public void RollAsync(Action<int, bool> onResult = null)
    {
        if (IsRolling) return;

        int finalRoll = UnityEngine.Random.Range(1, 21);
        StartCoroutine(StartRollRoutine(finalRoll, onResult));
    }

    /// <summary>
    /// Зупинити ролл (якщо треба).
    /// </summary>
    public void StopRoll()
    {
        if (rollRoutine != null)
            StopCoroutine(rollRoutine);

        rollRoutine = null;
        IsRolling = false;
    }

    // -------------------------------------------------------
    // INTERNAL ROLL
    // -------------------------------------------------------

    private IEnumerator StartRollRoutine(int finalRoll, Action<int, bool> onResult)
    {
        // safety
        if (faces == null || faces.Length < 21)
        {
            Debug.LogError("[DiceSystem] faces array must be size 21 (0=empty, 1..20=faces).");
            yield break;
        }

        IsRolling = true;

        flickerSteps = Mathf.Max(2, flickerSteps);
        float stepTime = rollDuration / flickerSteps;

        // animation: empty/random/empty/random...
        for (int i = 0; i < flickerSteps; i++)
        {
            bool shouldBeEmpty = startWithEmpty ? (i % 2 == 0) : (i % 2 != 0);

            if (shouldBeEmpty)
                SetEmpty();
            else
                SetFace(UnityEngine.Random.Range(1, 21));

            yield return new WaitForSeconds(stepTime);
        }

        // final
        SetFace(finalRoll);
        LastRoll = finalRoll;

        bool isFail = IsFail(finalRoll, failChancePercent);

        IsRolling = false;
        rollRoutine = null;

        onResult?.Invoke(finalRoll, isFail);
        OnRollFinished?.Invoke(finalRoll, isFail);
    }

    private bool IsFail(int roll, float failChance)
    {
        // 20 faces => 5% per face
        int failCount = Mathf.Clamp(Mathf.RoundToInt(20f * (failChance / 100f)), 0, 20);
        return roll <= failCount;
    }

    private void SetEmpty()
    {
        ApplySprite(faces[0]);
    }

    private void SetFace(int value)
    {
        value = Mathf.Clamp(value, 1, 20);
        ApplySprite(faces[value]);
    }

    private void ApplySprite(Sprite s)
    {
        if (uiImage != null)
            uiImage.sprite = s;
    }

    public void SetFailChance(float percent)
    {
        failChancePercent = Mathf.Clamp(percent, 0f, 100f);
    }
}