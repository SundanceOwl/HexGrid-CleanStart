using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Unit Stats")]
    public int movementPoints = 5;
    public int attackStrength = 1;
    public int defenseStrength = 1;
    public int ownerPlayer = 1;

    [Header("Position")]
    public HexCell currentCell;

    // Platziert die Einheit auf einer bestimmten Zelle
    public void Place(HexCell cell)
    {
        currentCell = cell;

        // Aktualisiert die Position der 3D-Einheit, damit sie auf dem Tile sitzt
        Vector3 newPosition = cell.transform.position;
        newPosition.y += 0.25f; // Leicht über die Zelle heben
        transform.position = newPosition;

        // Aktualisiert den Zustand der Zelle
        //cell.isOccupied = true;
        cell.playerOwner = ownerPlayer;
        cell.currentUnit = this;

        // Einheit wird Kind des Tiles (sauberere Hierarchie)
        transform.SetParent(cell.transform);
    }

    public void DestroyUnit()
    {
        if (currentCell != null)
        {
            //currentCell.isOccupied = false;
            currentCell.playerOwner = 0;
            currentCell.currentUnit = null;
        }
        Destroy(gameObject);
    }
}