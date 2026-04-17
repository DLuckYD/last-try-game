using System.Collections.Generic;
using UnityEngine;

public class EnemyArmyScript : MonoBehaviour
{
    public static EnemyArmyScript Instance { get; private set; }

    [Header("Team settings")]
    [SerializeField] private int teamSize = 10;
    [SerializeField] private UnitType[] possibleTypes;

    private List<UnitType> savedTeamTypes = new List<UnitType>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        EnsureTeamGenerated();
    }

    public void EnsureTeamGenerated()
    {
        if (savedTeamTypes.Count == 0)
            GenerateRandomEnemyTeam();
    }

    private void GenerateRandomEnemyTeam()
    {
        savedTeamTypes.Clear();

        if (possibleTypes == null || possibleTypes.Length == 0)
        {
            Debug.LogError("[EnemyArmyScript] possibleTypes array is EMPTY!");
            return;
        }

        for (int i = 0; i < teamSize; i++)
        {
            UnitType randomType = possibleTypes[Random.Range(0, possibleTypes.Length)];
            savedTeamTypes.Add(randomType);
        }

        Debug.Log("[EnemyArmyScript] Generated enemy team of " + savedTeamTypes.Count + " units.");
    }

    public int GetTeamSize() => teamSize;

    public void SetTeam(List<UnitType> types)
    {
        savedTeamTypes.Clear();
        if (types != null)
            savedTeamTypes.AddRange(types);

        Debug.Log("[EnemyArmyScript] SAVED enemy team TYPES: " + savedTeamTypes.Count);
    }

    public List<UnitType> GetTeam()
    {
        return savedTeamTypes;
    }

    public void ResetAndGenerate()
    {
        savedTeamTypes.Clear();
        GenerateRandomEnemyTeam();
    }
}
