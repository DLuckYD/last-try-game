using UnityEngine;

public enum UnitType { Lance, Archer, Shield }

[CreateAssetMenu(fileName = "UnitData", menuName = "Game/UnitData")]
public class UnitData : ScriptableObject
{
    public string displayName;
    public UnitType type;
}
