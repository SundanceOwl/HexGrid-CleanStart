using UnityEngine;

// [CreateAssetMenu] erlaubt das Erstellen von Instanzen im Unity-Editor
[CreateAssetMenu(fileName = "NewUnitType", menuName = "Strategy/Unit Type Data")]
public class UnitTypeData : ScriptableObject
{
    // Die ID, die wir in Unit.cs und UnitData.cs speichern
    [Header("Type Identification")]
    public int unitTypeID = 0;
    public string typeName = "UnitName";

    // Die statischen Basiswerte (Stammdaten)
    [Header("Base Stats")]
    public int baseMaxHealth = 10;
    public int baseMovementPoints = 5;
    public int baseAttackStrength = 1;
    public int baseDefenseStrength = 1;

    [Header("Visuals")]
    public GameObject unitPrefab; // Welches 3D-Modell verwendet werden soll
}
