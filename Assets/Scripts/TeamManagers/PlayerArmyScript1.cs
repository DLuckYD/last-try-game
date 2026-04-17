using System.Collections.Generic;
using UnityEngine;

public class PlayerArmyScript : MonoBehaviour
{
    public static PlayerArmyScript Instance { get; private set; }

    [SerializeField] private int teamSize = 10;

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

    public int GetTeamSize() => teamSize;

    public void SetTeam(List<UnitType> types)
    {
        savedTeamTypes = new List<UnitType>(types);
        Debug.Log("[PlayerArmyScript] Saved TYPES: " + savedTeamTypes.Count);
    }

    public List<UnitType> GetTeam()
    {
        return savedTeamTypes;
    }

    public void ResetTeam()
    {
        savedTeamTypes.Clear();
    }
}
