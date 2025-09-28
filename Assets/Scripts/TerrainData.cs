using UnityEngine;

// [System.Serializable] erlaubt es, diese Klasse im Inspector zu sehen/bearbeiten
[System.Serializable]
public class TerrainCost
{
    // Die Index-Nummer entspricht dem Index im elevationPrefabs[] Array (0 bis 4)
    public int elevationIndex;
    // Der Name dient nur zur besseren Lesbarkeit im Inspector
    public string terrainName;
    // Die Kosten, um in dieses Feld einzutreten (1 ist Standard)
    public int movementCost = 1;
    // Soll die Bewegung über dieses Terrain blockiert werden?
    public bool isPassable = true;
}

// Dieses Skript ist nur ein Datencontainer, der die Kosten im Inspector hält
public class TerrainData : MonoBehaviour
{
    // HIER werden Sie im Inspector die Kosten für jedes Terrain definieren
    public TerrainCost[] terrainCosts;

    // Eine Hilfsfunktion, um die Kosten schnell abzurufen
    public int GetCost(int elevationIndex)
    {
        foreach (var cost in terrainCosts)
        {
            if (cost.elevationIndex == elevationIndex)
            {
                return cost.movementCost;
            }
        }
        // Fallback: Wenn Elevation nicht definiert, sind die Kosten 1
        return 1;
    }

    // Eine Hilfsfunktion, um die Passierbarkeit abzurufen
    public bool IsPassable(int elevationIndex)
    {
        foreach (var cost in terrainCosts)
        {
            if (cost.elevationIndex == elevationIndex)
            {
                return cost.isPassable;
            }
        }
        return true; // Standardmäßig passierbar
    }
}