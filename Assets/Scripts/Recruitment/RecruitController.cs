using System;
using TMPro;
using Tobii.GameIntegration.Net;
using UnityEngine;

public class RecruitController : MonoBehaviour
{
    //[Header("Gaze Debug")]
    //[SerializeField] private TMP_Text console;
    //[SerializeField] private GameObject debugCirclePrefab;

    [Header("Recruit Settings")]
    [SerializeField] private LayerMask unitsLayer;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private TimerScript timerScript;
    [SerializeField] private float recruitRadius = 0f;

    public KeyCode recruitKey = KeyCode.Space;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        CheckEndConditions();

        if (Input.GetKeyDown(recruitKey))
        {
            if (!timerScript.timerOn) return;

            if (!TryGetTobiiWorldPoint(out Vector2 point))
            {
                Debug.LogWarning("Failed to get Tobii gaze point.");
                return;
            }

            //SpawnDebugCircle(point);
            TryRecruitAtPoint(point);
        }
    }

    private bool TryGetTobiiWorldPoint(out Vector2 worldPoint)
    {
        worldPoint = Vector2.zero;

        if (cam == null) cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("[Recruit] No Main Camera tagged!");
            return false;
        }

        // === 1️⃣ TRY TOBII FIRST ===
        if (TobiiGameIntegrationApi.TryGetLatestGazePoint(out GazePoint gaze))
        {
            float pixelX = (gaze.X + 1f) * 0.5f * Screen.width;
            float pixelY = (gaze.Y + 1f) * 0.5f * Screen.height;

            Vector3 world = cam.ScreenToWorldPoint(
                new Vector3(pixelX, pixelY, -cam.transform.position.z)
            );

            worldPoint = new Vector2(world.x, world.y);

            Debug.Log($"[Recruit] Using TOBII gaze at {worldPoint}");
            //console.text += $"[Recruit] Using TOBII gaze at {worldPoint}";
            return true;
        }

        // === 2️⃣ DEBUG FALLBACK → MOUSE ===
        Vector3 mouse = Input.mousePosition;
        mouse.z = -cam.transform.position.z;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouse);
        worldPoint = new Vector2(mouseWorld.x, mouseWorld.y);

        Debug.LogWarning($"[Recruit] Tobii missing → using MOUSE at {worldPoint}");
        return true;
    }

    //private void SpawnDebugCircle(Vector2 pos)
    //{
    //    if (debugCirclePrefab == null) return;
    //    Instantiate(debugCirclePrefab, pos, Quaternion.identity);
    //}

    private void TryRecruitAtPoint(Vector2 pos)
    {
        if (playerManager == null) return;
        if (playerManager.RecruitedCount >= playerManager.maxRecruits) return;

        Collider2D hit;

        // 0 = точка, >0 = круг пошуку (якщо хочеш більшу зону)
        if (recruitRadius > 0f)
            hit = Physics2D.OverlapCircle(pos, recruitRadius, unitsLayer);
        else
            hit = Physics2D.OverlapPoint(pos, unitsLayer);

        if (hit == null)
        {
            Debug.Log("No unit under gaze!");
            //if (console != null) console.text = "No unit under gaze!";
            return;
        }

        Unit unit = hit.GetComponent<Unit>();
        if (unit == null)
        {
            Debug.Log("Hit object has no Unit!");
            //if (console != null) console.text = "Hit object has no Unit!";
            return;
        }

        Debug.Log($"Unit detected: {unit.name}");
        //if (console != null) console.text = $"Unit detected: {unit.name}";
        playerManager.EnqueueTarget(unit);
    }

    private void CheckEndConditions()
    {
        if (!timerScript.timerOn)
        {
            timerScript.EndPhase();
            enabled = false;
            return;
        }

        if (playerManager != null && playerManager.RecruitedCount >= playerManager.maxRecruits)
        {
            timerScript.EndPhase();
            enabled = false;
        }
    }

    public void RecruitUnit(Unit unit)
    {
        if (unit == null) return;
        if (unit.isRecruited) return;
        unit.Recruit();
    }
}