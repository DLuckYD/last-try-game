using UnityEngine;
using UnityEngine.UI;

public class GazeButtonClick : MonoBehaviour
{
    [Header("Cursor source (choose one)")]
    [SerializeField] private Transform worldCursor;        // MousePosition (world object)

    [Header("Input")]
    [SerializeField] private KeyCode clickKey = KeyCode.Space;

    private Button button;
    private RectTransform buttonRect;
    private Canvas canvas;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (!Input.GetKeyDown(clickKey)) return;

        if (IsCursorOverButton())
        {
            button.onClick.Invoke();
        }
    }

    private bool IsCursorOverButton()
    {

        // 2) якщо Ї world курсор (MousePosition) Ч конвертуЇмо world -> screen
        if (worldCursor != null)
        {
            var cam = Camera.main;
            if (cam == null) return false;

            Vector2 screenPoint = cam.WorldToScreenPoint(worldCursor.position);

            return RectTransformUtility.RectangleContainsScreenPoint(
                buttonRect,
                screenPoint,
                GetUiCamera()
            );
        }

        return false;
    }

    private Camera GetUiCamera()
    {
        // ƒл€ Screen Space - Overlay треба null
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        // ƒл€ Screen Space - Camera / World Space треба camera з Canvas
        return canvas != null ? canvas.worldCamera : null;
    }
}