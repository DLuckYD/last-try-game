using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArmyShowcase : MonoBehaviour
{
    [Header("Prefabs")]
    public UnitPrefabByType[] prefabs;

    [Header("Layout")]
    public float spacing = 2f;
    public float yPosition = 0f;

    private Dictionary<UnitType, GameObject> prefabDict;
    private List<GameObject> armyTeam = new List<GameObject>();

    private void Awake()
    {
        prefabDict = new Dictionary<UnitType, GameObject>();
        foreach (var p in prefabs)
        {
            if (p.prefab == null) continue;
            prefabDict[p.type] = p.prefab;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(BuildWhenReady());
    }

    private IEnumerator BuildWhenReady()
    {
        // 1) дочекайся, поки Instance з’явиться
        while (EnemyArmyScript.Instance == null)
            yield return null;

        // 2) дочекайся, поки команда реально згенерується
        while (EnemyArmyScript.Instance.GetTeam() == null || EnemyArmyScript.Instance.GetTeam().Count == 0)
            yield return null;

        BuildShowcase();
    }

    private void BuildShowcase()
    {
        // чистимо старе (важливо при повторних заходах)
        foreach (var go in armyTeam)
            if (go != null) Destroy(go);
        armyTeam.Clear();

        var team = EnemyArmyScript.Instance.GetTeam();

        float totalWidth = (team.Count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < team.Count; i++)
        {
            var type = team[i];

            if (!prefabDict.TryGetValue(type, out var prefab))
            {
                Debug.LogError("[EnemyArmyShowcase] No prefab for type " + type);
                continue;
            }

            float x = startX + i * spacing;
            Vector3 pos = new Vector3(x, yPosition, 0f);

            var instance = Instantiate(prefab, pos, Quaternion.identity, transform);
            armyTeam.Add(instance);
        }

        Debug.Log($"[EnemyArmyShowcase] Spawned {armyTeam.Count} units.");
    }

    public List<GameObject> GetArmyList() => new List<GameObject>(armyTeam);
}