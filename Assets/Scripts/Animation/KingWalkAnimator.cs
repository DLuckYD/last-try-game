using UnityEngine;

public class KingWalkAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform spriteRoot;

    [Header("Качание")]
    [SerializeField] private float swayFrequency = 6f;
    [SerializeField] private float maxTiltAngle = 5f;
    [SerializeField] private float horizontalShift = 0.05f;

    [Header("Пульсация размера")]
    [SerializeField] private float scaleAmplitude = 0.05f;

    private bool _isMoving;
    private float _t;

    public void SetMoving(bool moving)
    {
        if (moving && !_isMoving)
            _t = 0f;

        _isMoving = moving;
    }

    private void Update()
    {
        if (!_isMoving)
        {
            // Возвращение в idle
            spriteRoot.localRotation = Quaternion.Lerp(spriteRoot.localRotation, Quaternion.identity, 10f * Time.deltaTime);
            spriteRoot.localScale = Vector3.Lerp(spriteRoot.localScale, Vector3.one, 10f * Time.deltaTime);
            spriteRoot.localPosition = Vector3.Lerp(spriteRoot.localPosition, Vector3.zero, 10f * Time.deltaTime);
            return;
        }

        _t += Time.deltaTime;

        float k = Mathf.Sin(_t * swayFrequency); // -1..1

        // Наклон
        spriteRoot.localRotation = Quaternion.Euler(0, 0, k * maxTiltAngle);

        // Сдвиг (верх короля чуть “шагает”)
        spriteRoot.localPosition = new Vector3(k * horizontalShift, 0, 0);

        // Пульсация размера (микродвижение при шаге)
        float scale = 1f + k * scaleAmplitude;
        spriteRoot.localScale = new Vector3(scale, scale, 1f);
    }
}