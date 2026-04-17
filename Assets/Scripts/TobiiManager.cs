using System;
using System.Runtime.InteropServices;
using TMPro;
using Tobii.GameIntegration.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class TobiiTest : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    public static TobiiTest Instance { get; private set; }

    //private Rigidbody2D rb;
    //private Camera cam;
    private bool tobiiInitialized;

    private void Awake()
    {
        //rb = GetComponent<Rigidbody2D>();
        //cam = Camera.main;
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        InitializeTobii();
        //try
        //{
        //    InitializeTobii();
        //    tobiiInitialized = true;
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError("[Tobii] Initialization failed: " + ex.Message);
        //    tobiiInitialized = false;
        //}
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    
#if !UNITY_EDITOR
        if (tobiiInitialized)
            TobiiGameIntegrationApi.Shutdown();
#endif
    }

    private void Update()
    {
        if (tobiiInitialized)
            TobiiGameIntegrationApi.Update();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeTobii();
    }

    //private void FixedUpdate()
    //{
    //    if (!tobiiInitialized) return;

    //    if (!TobiiGameIntegrationApi.TryGetLatestGazePoint(out GazePoint gaze)) return;


    //    float pixelX = (gaze.X + 1f) * 0.5f * Screen.width;
    //    float pixelY = (gaze.Y + 1f) * 0.5f * Screen.height;

    //    Vector3 screenPos = new Vector3(pixelX, pixelY, -cam.transform.position.z);

    //    rb.position = cam.ScreenToWorldPoint(screenPos);
    //}

    private void InitializeTobii()
    {
        try
        {
            Debug.Log("[Tobii] Init started.");
            Debug.Log("[Tobii] Will try to load DLL: " + TobiiGameIntegrationApi.LoadedDll);


            TobiiGameIntegrationApi.SetApplicationName("LastTry");

            //IntPtr hwnd = GetForegroundWindow();
            //Debug.Log("[Tobii] Foreground window handle: " + hwnd);

            //#if !UNITY_EDITOR
            //        if (hwnd != IntPtr.Zero)
            //        {
            //            bool trackingStarted = TobiiGameIntegrationApi.TrackWindow(hwnd);
            //            Debug.Log("[Tobii] TrackWindow result = " + trackingStarted);

            //            TrackerInfo info = TobiiGameIntegrationApi.GetTrackerInfo();
            //            Debug.Log("[Tobii] Tracker info: " + info);
            //        }
            //#else
            //            Debug.LogWarning("[Tobii] TrackWindow disabled in Unity Editor (check it in build).");
            //#endif

#if !UNITY_EDITOR
            IntPtr hwnd = GetForegroundWindow();
            Debug.Log("[Tobii] Foreground window handle: " + hwnd);

            if (hwnd == IntPtr.Zero)
            {
                Debug.LogWarning("[Tobii] hwnd == 0 (вікно не у фокусі?)");
                return;
            }

            bool trackingStarted = TobiiGameIntegrationApi.TrackWindow(hwnd);
            Debug.Log("[Tobii] TrackWindow result = " + trackingStarted);

            TrackerInfo info = TobiiGameIntegrationApi.GetTrackerInfo();
            Debug.Log("[Tobii] Tracker info: " + info);

            tobiiInitialized = true;
#else
            Debug.LogWarning("[Tobii] TrackWindow disabled in Unity Editor (check it in build).");
            // В Editor все одно тримаємо true, щоб Update() крутився
            tobiiInitialized = true;
#endif
        }
        catch (Exception ex)
        {
            tobiiInitialized = false;
            Debug.LogError("[Tobii] Initialization failed: " + ex.Message);
        }
    }
}
