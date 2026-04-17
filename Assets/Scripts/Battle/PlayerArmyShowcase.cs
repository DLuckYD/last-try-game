using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitPrefabByType
{
    public UnitType type;
    public GameObject prefab;
}

public class PlayerArmyShowcase : MonoBehaviour
{
    [Header("Prefabs")]
    public UnitPrefabByType[] prefabs;

    [Header("Layout")]
    public float spacing = 2f;
    public float yPosition = 0f;

    private Dictionary<UnitType, GameObject> prefabDict;
    private List<GameObject> playerTeam = new List<GameObject>();

    private void Awake()
    {
        prefabDict = new Dictionary<UnitType, GameObject>();
        foreach (var p in prefabs)
        {
            if (!prefabDict.ContainsKey(p.type))
                prefabDict[p.type] = p.prefab;
        }
    }

    private void Start()
    {
        if (PlayerArmyScript.Instance == null)
        {
            Debug.LogError("[PlayerArmyShowcase] No PlayerArmyScript.Instance");
            return;
        }

        // 1️⃣ Беремо поточну (ще не оптимізовану) команду гравця
        var originalPlayerTypes = PlayerArmyScript.Instance.GetTeam();
        if (originalPlayerTypes == null || originalPlayerTypes.Count == 0)
        {
            Debug.LogWarning("[PlayerArmyShowcase] Player team is empty.");
            return;
        }

        // 2️⃣ Беремо типи ворога (ми їх теж десь зберігаємо як List<UnitType>)
        List<UnitType> enemyTypes = null;
        if (EnemyArmyScript.Instance != null)
        {
            enemyTypes = EnemyArmyScript.Instance.GetTeam();
        }

        // 3️⃣ Рахуємо оптимізований порядок
        var optimizedTypes = BuildOptimizedPlayerTypes(originalPlayerTypes, enemyTypes);

        Debug.Log("[PlayerArmyShowcase] Optimized player order:");
        for (int i = 0; i < optimizedTypes.Count; i++)
        {
            Debug.Log($"  Slot {i}: {optimizedTypes[i]}");
        }

        // 4️⃣ Зберігаємо оптимізований список у глобальний сінглтон
        PlayerArmyScript.Instance.SetTeam(optimizedTypes);

        // 5️⃣ Спавнимо юнітів по оптимізованому порядку
        SpawnShowcase(optimizedTypes);
    }

    private void SpawnShowcase(List<UnitType> types)
    {
        float totalWidth = (types.Count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < types.Count; i++)
        {
            UnitType type = types[i];

            if (!prefabDict.TryGetValue(type, out GameObject prefab))
            {
                Debug.LogError("[PlayerArmyShowcase] No prefab for type " + type);
                continue;
            }

            float x = startX + i * spacing;
            Vector3 pos = new Vector3(x, yPosition, 0f);

            GameObject instance = Instantiate(prefab, pos, Quaternion.identity);
            playerTeam.Add(instance);
        }
    }

    /// <summary>
    /// BattleScript буде звідси брати візуальні GameObject-и.
    /// </summary>
    public List<GameObject> GetArmyList()
    {
        return playerTeam;
    }

    // ---------------------------------------------------------
    // ЛОГІКА ОПТИМІЗАЦІЇ ПОРЯДКУ
    // ---------------------------------------------------------
    private List<UnitType> BuildOptimizedPlayerTypes(List<UnitType> playerTypes, List<UnitType> enemyTypes)
    {
        // Якщо ворога нема або він пустий — просто повертаємо оригінал.
        if (enemyTypes == null || enemyTypes.Count == 0)
        {
            Debug.LogWarning("[PlayerArmyShowcase] No enemyTypes, returning original order.");
            return new List<UnitType>(playerTypes);
        }

        // копія всіх юнітів гравця, які ще не використали
        List<UnitType> remaining = new List<UnitType>(playerTypes);
        List<UnitType> optimized = new List<UnitType>();

        int pairCount = Mathf.Min(remaining.Count, enemyTypes.Count);

        for (int i = 0; i < pairCount; i++)
        {
            UnitType enemy = enemyTypes[i];

            int winIndex = -1;
            int drawIndex = -1;

            // шукаємо кращого кандидата проти цього enemy
            for (int j = 0; j < remaining.Count; j++)
            {
                UnitType candidate = remaining[j];
                BattleResult res = Resolve(candidate, enemy);

                if (res == BattleResult.PlayerWins)
                {
                    winIndex = j;
                    break; // найкращий варіант
                }
                else if (res == BattleResult.Draw && drawIndex < 0)
                {
                    drawIndex = j; // перша потенційна нічия
                }
            }

            int chosenIndex;

            if (winIndex >= 0)
                chosenIndex = winIndex;
            else if (drawIndex >= 0)
                chosenIndex = drawIndex;
            else
                chosenIndex = 0; // без шансів — беремо будь-кого

            UnitType chosen = remaining[chosenIndex];
            optimized.Add(chosen);
            remaining.RemoveAt(chosenIndex);
        }

        // якщо у гравця юнітів більше, ніж у ворога — додаємо їх в хвіст
        optimized.AddRange(remaining);

        return optimized;
    }

    // Та ж сама таблиця "камінь-ножиці-папір", що й у BattleScript
    private BattleResult Resolve(UnitType player, UnitType enemy)
    {
        if (player == enemy)
            return BattleResult.Draw;

        switch (player)
        {
            case UnitType.Lance:
                if (enemy == UnitType.Shield) return BattleResult.PlayerWins;
                if (enemy == UnitType.Archer) return BattleResult.EnemyWins;
                break;

            case UnitType.Archer:
                if (enemy == UnitType.Lance) return BattleResult.PlayerWins;
                if (enemy == UnitType.Shield) return BattleResult.EnemyWins;
                break;

            case UnitType.Shield:
                if (enemy == UnitType.Archer) return BattleResult.PlayerWins;
                if (enemy == UnitType.Lance) return BattleResult.EnemyWins;
                break;
        }

        return BattleResult.Draw;
    }
}
