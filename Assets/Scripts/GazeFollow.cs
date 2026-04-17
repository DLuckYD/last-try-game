using Tobii.GameIntegration.Net;
using UnityEngine;

public class GazeFollow : MonoBehaviour
{
    [SerializeField] float offsetX;
    [SerializeField] float offsetY;
    [SerializeField] private float freezeRadius = 100f;

    private Rigidbody2D rb;
    private Camera cam;

    //private RectTransform rectTransform;

    void Awake()
    {
        //rectTransform = GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update()
    {
        if(cam == null)
        {
            cam = Camera.main;
        }

        if (!TobiiGameIntegrationApi.TryGetLatestGazePoint(out GazePoint gaze))
            return;

        float pixelX = (gaze.X + 1f) * 0.5f * Screen.width;
        float pixelY = (gaze.Y + 1f) * 0.5f * Screen.height;

        Vector3 screenPos = new Vector3(pixelX + offsetX, pixelY + offsetY, -cam.transform.position.z);
        rb.position = cam.ScreenToWorldPoint(screenPos);
    }
}
