using UnityEngine;
using System.Collections.Generic;


public class SpawnManager : MonoBehaviour
{
    [Header("Unit Settings")]
    [SerializeField] private GameObject[] UnitPrefabs;
    [SerializeField] private Transform king;
    [SerializeField] private float emptyChance = 0f;
    [SerializeField] private float jitter = 0.4f;
    
    [Header("Terrain Settings")]
    [SerializeField] private GameObject[] TerrainPrefabs;
    [SerializeField] private int terrainCount = 60;
    [SerializeField] private Transform terrainParent;
    [SerializeField] private float sameTypeMinDistance  = 0.7f;
    [SerializeField] private float terrainMinDistance = 0.6f;
    
    

    [Header("Spawn Ranges")]
    [SerializeField] private Vector2 minWorld = new Vector2(-18f, -11f);
    [SerializeField] private Vector2 maxWorld = new Vector2(18f, 11f);

    [Header("Map Grid")]
    public int rows = 6;
    public int columns = 7;

   
    
    public void Start()
    {
        SpawnUnits();
        SpawnTerrain();
    }

    [ContextMenu("Spawn All")]
    public void SpawnUnits()
    {
        float cellW = (maxWorld.x - minWorld.x) / columns;
        float cellH = (maxWorld.y - minWorld.y) / rows;


        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                //skip the cell based on empty chance
                if (Random.value < emptyChance) continue;

                //cell center
                float cx = minWorld.x + (c + 0.5f) * cellW;
                float cy = minWorld.y + (r + 0.5f) * cellH;

                //random position within the cell
                float jx = (Random.value * 2f - 1f) * cellW * jitter;
                float jy = (Random.value * 2f - 1f) * cellH * jitter;

                Vector3 pos = new Vector3(cx + jx, cy + jy, 0f);
                
                //avoid spawning too close to the king
                if (Vector3.Distance(pos, king.position) < 5f) continue;
                
                //random unit type
                GameObject prefab = UnitPrefabs[Random.Range(0, UnitPrefabs.Length)];
                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }
    
    /*public void SpawnTerrain()
    {
        float cellW = (maxWorld.x - minWorld.x) / columns;
        float cellH = (maxWorld.y - minWorld.y) / rows;


        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                //skip the cell based on empty chance
                //if (Random.value < emptyChance) continue;

                //cell center
                float cx = minWorld.x + (c + 0.5f) * cellW;
                float cy = minWorld.y + (r + 0.5f) * cellH;

                //random position within the cell
                float jx = (Random.value * 2f - 1f) * cellW * jitter;
                float jy = (Random.value * 2f - 1f) * cellH * jitter;

                Vector3 pos = new Vector3(cx + jx, cy + jy, 0f);

                
                //random terrain type
                GameObject prefab = TerrainPrefabs[Random.Range(0, TerrainPrefabs.Length)];
                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }*/
    
    private struct TerrainInstance
    {
        public Vector3 position;
        public int prefabIndex;
    }

    private List<TerrainInstance> terrainInstances = new List<TerrainInstance>();
    
    public void SpawnTerrain()
{
    terrainInstances.Clear();

    int spawned = 0;
    int safety = 0;

    while (spawned < terrainCount && safety < terrainCount * 30)
    {
        safety++;

        // 1) случайная позиция в пределах карты
        float x = Random.Range(minWorld.x, maxWorld.x);
        float y = Random.Range(minWorld.y, maxWorld.y);
        Vector3 pos = new Vector3(x, y, 0f);

        // 2) случайный тип террейна
        int prefabIndex = Random.Range(0, TerrainPrefabs.Length);

        // --- сначала проверяем ГЛОБАЛЬНУЮ дистанцию (любой тип) ---
        bool tooCloseAny = false;
        foreach (var t in terrainInstances)
        {
            float dist = Vector3.Distance(pos, t.position);
            if (dist < terrainMinDistance)
            {
                tooCloseAny = true;
                break;
            }
        }
        if (tooCloseAny)
            continue; // позиция плохая, пробуем следующую

        // --- затем пытаемся подобрать тип, чтобы не был тем же рядом ---
        const int maxPrefabTries = 4;
        for (int attempt = 0; attempt < maxPrefabTries; attempt++)
        {
            bool tooCloseSameType = false;

            foreach (var t in terrainInstances)
            {
                if (t.prefabIndex != prefabIndex) 
                    continue;

                float dist = Vector3.Distance(pos, t.position);
                if (dist < sameTypeMinDistance)
                {
                    tooCloseSameType = true;
                    break;
                }
            }

            if (!tooCloseSameType)
                break; // этот тип подходит

            // пробуем другой тип
            prefabIndex = Random.Range(0, TerrainPrefabs.Length);
        }

        // 3) создаём объект
        GameObject prefab = TerrainPrefabs[prefabIndex];
        GameObject instance = Instantiate(prefab, pos, Quaternion.identity, terrainParent);

        // опц.: рандомный флип для разнообразия
        TryRandomFlip(instance);

        // 4) запоминаем
        terrainInstances.Add(new TerrainInstance
        {
            position = pos,
            prefabIndex = prefabIndex
        });

        spawned++;
    }

    if (safety >= terrainCount * 30)
    {
        Debug.LogWarning("[SpawnManager] Terrain spawn stopped by safety guard.");
    }
}

    
    
    private void TryRandomFlip(GameObject instance)
    {
        // ищем SpriteRenderer либо на корне, либо в дочерних
        SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = instance.GetComponentInChildren<SpriteRenderer>();

        if (sr == null)
            return;

        // 50% шанс отзеркалить по X
        if (Random.value < 0.5f)
            sr.flipX = !sr.flipX;
        
    }



}
