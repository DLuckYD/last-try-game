using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class BackgroundGenerator : MonoBehaviour
{
    [Header("Размер текстуры (в пикселях)")]
    [SerializeField] private int textureWidth = 512;
    [SerializeField] private int textureHeight = 512;

    [Header("Целевой размер фона в мире (юниты)")]
    [SerializeField] private Vector2 targetWorldSize = new Vector2(36f, 22f);

    [Header("Оттенки травы (0 – фон, 1 – остров, 2 – ядро)")]
    [SerializeField] private Color[] grassColors; // минимум 3 цвета

    [Header("Шум для ОСТРОВОВ (слой 1)")]
    [SerializeField, Range(4f, 40f)] private float baseNoiseScale = 16f;
    [SerializeField, Range(0f, 1f)]  private float baseThreshold = 0.45f;

    [Header("Шум для ЯДРА внутри островов (слой 2)")]
    [SerializeField, Range(2f, 40f)] private float innerNoiseScale = 8f;
    [SerializeField, Range(0f, 1f)]  private float innerThreshold = 0.7f;

    [Header("Seed и плотность")]
    [SerializeField] private int seed = 0;
    [SerializeField] private bool randomSeedOnPlay = true;
    [SerializeField] private float pixelsPerUnit = 32f;

    [Header("Размытие (блюр)")]
    [SerializeField, Range(0f, 100f)] private float blurStrength = 0f; // 0 – нет, 100 – макс.
    [SerializeField] private int maxBlurRadius = 6; // максимальный радиус блюра в пикселях

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Если спрайта нет — создаём временный пустой 2×2
        if (spriteRenderer.sprite == null)
        {
            Texture2D tmp = new Texture2D(2, 2);
            tmp.SetPixel(0, 0, Color.white);
            tmp.SetPixel(1, 0, Color.white);
            tmp.SetPixel(0, 1, Color.white);
            tmp.SetPixel(1, 1, Color.white);
            tmp.Apply();
        
            spriteRenderer.sprite = Sprite.Create(tmp, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 32f);
        }

        spriteRenderer.color = Color.white;
    }


    private void Start()
    {
        if (randomSeedOnPlay)
            seed = Random.Range(0, 100000);

        Generate();
    }

    [ContextMenu("Generate Background")]
    public void Generate()
    {
        if (grassColors == null || grassColors.Length < 3)
        {
            Debug.LogWarning("[BackgroundGenerator] Нужно минимум 3 цвета (фон, остров, ядро)");
            return;
        }

        Texture2D tex = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point; // пиксель-арт, после блюра можно оставить так же

        // Генерация “яичницы”
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                Color c = GetColorForPixel(x, y);
                c.a = 1f;
                tex.SetPixel(x, y, c);
            }
        }

        // ⬇️ Пост-обработка: размытие по желанию
        ApplyBlurIfNeeded(tex);

        tex.Apply();

        // Создаём спрайт и подгоняем размер
        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, textureWidth, textureHeight),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit
        );

        spriteRenderer.sprite = sprite;
        FitToWorldSize(sprite);
    }

    // --------- ЛОГИКА ОСТРОВОВ (фон → остров → ядро) ---------
    private Color GetColorForPixel(int x, int y)
    {
        // 1) Крупный шум – определяем, попали ли в остров
        float bx = (x + seed) / baseNoiseScale;
        float by = (y + seed) / baseNoiseScale;
        float baseNoise = Mathf.PerlinNoise(bx, by); // 0..1

        if (baseNoise < baseThreshold)
        {
            // Вне острова → фон
            return grassColors[0];
        }

        // 2) Внутри острова считаем второй шум для ядра
        float ix = (x + seed * 2) / innerNoiseScale;
        float iy = (y + seed * 2) / innerNoiseScale;
        float innerNoise = Mathf.PerlinNoise(ix, iy);

        if (innerNoise > innerThreshold)
        {
            // ядро
            return grassColors[2];
        }
        else
        {
            // тело острова
            return grassColors[1];
        }
    }
    // ---------------------------------------------------------

    // Подгоняем объект под размер мира
    private void FitToWorldSize(Sprite sprite)
    {
        Vector2 spriteSize = sprite.bounds.size;
        float scaleX = targetWorldSize.x / spriteSize.x;
        float scaleY = targetWorldSize.y / spriteSize.y;
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    // --------- БЛЮР ---------
    private void ApplyBlurIfNeeded(Texture2D tex)
    {
        if (blurStrength <= 0.01f || maxBlurRadius <= 0)
            return;

        int radius = Mathf.RoundToInt(maxBlurRadius * (blurStrength / 100f));
        radius = Mathf.Clamp(radius, 1, maxBlurRadius);

        int w = tex.width;
        int h = tex.height;

        Color[] src = tex.GetPixels();
        Color[] tmp = new Color[src.Length];
        Color[] dst = new Color[src.Length];

        // Горизонтальный проход
        for (int y = 0; y < h; y++)
        {
            int row = y * w;
            for (int x = 0; x < w; x++)
            {
                Vector4 sum = Vector4.zero;
                int count = 0;

                int xMin = Mathf.Max(0, x - radius);
                int xMax = Mathf.Min(w - 1, x + radius);

                for (int sx = xMin; sx <= xMax; sx++)
                {
                    Color c = src[row + sx];
                    sum.x += c.r;
                    sum.y += c.g;
                    sum.z += c.b;
                    sum.w += c.a;
                    count++;
                }

                tmp[row + x] = new Color(sum.x / count, sum.y / count, sum.z / count, sum.w / count);
            }
        }

        // Вертикальный проход
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Vector4 sum = Vector4.zero;
                int count = 0;

                int yMin = Mathf.Max(0, y - radius);
                int yMax = Mathf.Min(h - 1, y + radius);

                for (int sy = yMin; sy <= yMax; sy++)
                {
                    Color c = tmp[sy * w + x];
                    sum.x += c.r;
                    sum.y += c.g;
                    sum.z += c.b;
                    sum.w += c.a;
                    count++;
                }

                dst[y * w + x] = new Color(sum.x / count, sum.y / count, sum.z / count, sum.w / count);
            }
        }

        tex.SetPixels(dst);
    }
}
