//using UnityEngine;

//public class MouseFollow : MonoBehaviour
//{
//    [SerializeField] private float moveSpeed = 10f;

//    private Rigidbody2D rb;
//    private Camera cam;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        cam = Camera.main;
//    }

//    void FixedUpdate()
//    {
//        rb.position = cam.ScreenToWorldPoint(Input.mousePosition);
//    }
//}