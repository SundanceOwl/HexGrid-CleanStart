using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class HexGame : MonoBehaviour
{
    [Header("Grid Setup")]
    public int gridWidth = 20;
    public int gridHeight = 20;
    public GameObject hexCellPrefab; // HIER: Das niedrigste Prefab (z.B. Hex_E0_Sea) zuweisen!

    [Header("Prefabs & Resources")]
    public GameObject[] elevationPrefabs; // HIER: Alle 5 Höhen-Prefabs zuweisen!
    public Unit unitPrefab; // HIER: Das Unit Prefab zuweisen!

    [Header("Precision Compensation")]
    [Range(0.8f, 1.2f)] // Range von 0.9 bis 1.1 erlaubt größere Anpassungen!
    public float xCompensate = 1.106f;
    [Range(0.8f, 1.2f)]
    public float zCompensate = 0.863f;

    [Header("Terrain Data")]
    public TerrainData terrainData; // DIESE ZEILE MUSS HIER STEHEN!

    [Header("Runtime Data")]
    public Dictionary<int, HexCell> cells = new Dictionary<int, HexCell>();
    private HexCell selectedCell = null;


    void Start()
    {
        GenerateGrid();
        InitializeGameStart();
    }


    /// <summary>
    /// Löscht alle existierenden HexCell-GameObjects aus der Szene und leert das Dictionary.
    /// </summary>
    public void ClearGrid()
    {
        if (cells.Count == 0) return;

        // Zerstöre alle GameObjects und deren Einheiten
        foreach (HexCell cell in cells.Values)
        {
            if (cell.currentUnit != null)
            {
                cell.currentUnit.DestroyUnit(); // Einheit entfernen
            }
            // Zerstöre das HexCell-GameObject
            Destroy(cell.gameObject);
        }

        cells.Clear(); // Leere das Dictionary
    }

    // Erstellt das Hex-Grid
    public void GenerateGrid()
    {
        cells.Clear();
        if (elevationPrefabs == null || elevationPrefabs.Length == 0)
        {
            UnityEngine.Debug.LogError("Bitte weisen Sie alle Hexagon Prefabs im Inspector zu!");
            return;
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = GetCellPosition(x, y);

                // Generierung des Höhenwerts (z.B. einfacher Noise-Wert)
                int elevation = GetElevation(x, y);

                // Das korrekte Prefab basierend auf der Höhe auswählen
                GameObject cellPrefabToUse = elevationPrefabs[Mathf.Clamp(elevation, 0, elevationPrefabs.Length - 1)];

                GameObject cellGO = Instantiate(cellPrefabToUse, position, Quaternion.identity, transform);

                HexCell cell = cellGO.GetComponent<HexCell>();
                cell.x = x;
                cell.y = y;
                // Die SetElevation-Methode ist nicht mehr notwendig, da das Prefab die Höhe repräsentiert
                cell.elevation = elevation;
                cells.Add(y * gridWidth + x, cell);
                cellGO.name = "Hex " + x + "," + y + " E" + elevation;
            }
        }
    }

    // NEU: Wird im Editor aufgerufen, wenn sich eine Variable ändert
    private void OnValidate()
    {
        // Diese Methode wird nur im Editor und Play Mode aufgerufen, 
        // wenn eine public Variable im Inspector geändert wird.
        if (UnityEngine.Application.isPlaying)
        {
            UpdateGridPositions();
        }
    }

    // NEU: Aktualisiert die Position aller Hexagone im Grid
    private void UpdateGridPositions()
    {
        // Wichtig: Nur wenn das Grid bereits existiert!
        if (cells.Count == 0) return;

        foreach (KeyValuePair<int, HexCell> entry in cells)
        {
            HexCell cell = entry.Value;

            // Berechnet die neue Position mit den aktuellen Inspector-Werten
            Vector3 newPosition = GetCellPosition(cell.x, cell.y);

            // Verschiebt die Zelle an die neue Position
            cell.transform.position = newPosition;

            // Wenn Einheiten platziert sind, müssen diese auch mitziehen
            if (cell.currentUnit != null)
            {
                // Unit.Place() aktualisiert die Unit-Position und setzt sie 
                // 0.25f über die Zelle (wie in Unit.cs definiert)
                cell.currentUnit.Place(cell);
            }
        }
    }

    // Einfache Perlin Noise Höhenberechnung
    private int GetElevation(int x, int y)
    {
        float noiseValue = Mathf.PerlinNoise((float)x * 0.1f, (float)y * 0.1f);
        // Nutzt 5 Höhenstufen (0 bis 4)
        return Mathf.FloorToInt(noiseValue * elevationPrefabs.Length);
    }

    public Vector3 GetCellPosition(int x, int y)
    {
        // Wir setzen voraus, dass das Modell 90° gedreht ist (X=1.732, Z=2.0)
        float r = 1.0f;
        const float SQRT3 = 1.73205080757f;

        // Die Kompensationen skalieren die Basisabmessungen
        float width = SQRT3 * r * xCompensate; // Passt die Breite (X) an
        float height = 2.0f * r * zCompensate; // Passt die Höhe (Z) an

        // 1. Horizontale Verschiebung (X)
        float x_spacing = width * 0.75f;
        float finalX = x * x_spacing;

        // 2. Vertikale Hauptverschiebung (Z)
        float finalZ = y * height;

        // 3. Versatz in Z-Richtung für ungerade Spalten (x)
        if (x % 2 != 0)
        {
            finalZ += height * 0.5f;
        }

        return new Vector3(finalX, 0, finalZ);
    }

    /// Hilfsfunktion: Ruft eine Zelle anhand ihrer X/Y-Koordinaten ab
    public HexCell GetCell(int x, int y)
    {
        // Die Grid-Schlüssel sind y * gridWidth + x
        int key = y * gridWidth + x;

        if (cells.ContainsKey(key))
        {
            return cells[key];
        }
        return null;
    }

    /// Hilfsfunktion: Gibt alle existierenden Nachbarn einer Zelle zurück
    public List<HexCell> GetNeighbors(HexCell cell)
    {
        List<HexCell> neighbors = new List<HexCell>();

        // Definiert die 6 möglichen Richtungen in einem Pointy-Top-Grid
        // Die Offsets sind abhängig davon, ob die Spalte 'x' gerade oder ungerade ist
        int x = cell.x;
        int y = cell.y;
        int parity = x % 2; // 0 für gerade, 1 für ungerade Spalten

        // Offsets für die 6 Nachbarn
        int[,] offsets = new int[6, 2];

        if (parity == 0) // Gerade Spalten (x=0, 2, 4...)
        {
            offsets = new int[,] {
            { 0, 1 }, { 1, 0 }, { 1, -1 },
            { 0, -1 }, { -1, -1 }, { -1, 0 }
        };
        }
        else // Ungerade Spalten (x=1, 3, 5...)
        {
            offsets = new int[,] {
            { 0, 1 }, { 1, 1 }, { 1, 0 },
            { 0, -1 }, { -1, 0 }, { -1, 1 }
        };
        }

        for (int i = 0; i < 6; i++)
        {
            int neighborX = x + offsets[i, 0];
            int neighborY = y + offsets[i, 1];

            // Prüfen, ob der Nachbar innerhalb der Grid-Grenzen liegt
            if (neighborX >= 0 && neighborX < gridWidth &&
                neighborY >= 0 && neighborY < gridHeight)
            {
                HexCell neighbor = GetCell(neighborX, neighborY);
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    // Platziert die Start-Units für Testzwecke
    public void InitializeGameStart()
    {
        // Beispiel: Platziere Player 1 Unit bei (1, 1) und Player 2 Unit bei (18, 18)
        PlaceUnit(1, 1, 1);
        PlaceUnit(gridWidth - 2, gridHeight - 2, 2);
    }

    /// Berechnet alle Zellen, die die Unit innerhalb ihrer MovementPoints erreichen kann (Dijkstra)
    public Dictionary<HexCell, int> GetReachableCells(Unit unit)
    {
        // Der Rückgabewert: Speichert alle erreichbaren Zellen und die kumulierten Kosten
        // Wert = Kosten von der Startzelle bis zu dieser Zelle
        Dictionary<HexCell, int> distances = new Dictionary<HexCell, int>();

        // Eine Liste aller noch zu prüfenden Zellen. Am Anfang nur die Startzelle.
        List<HexCell> frontier = new List<HexCell> { unit.currentCell };

        // Die Kosten zur Startzelle sind 0
        distances[unit.currentCell] = 0;

        int maxMovement = unit.movementPoints;

        while (frontier.Count > 0)
        {
            // Sortieren Sie die Frontier, um die Zelle mit den geringsten Gesamtkosten zu erhalten (Dijkstra)
            // Linq-Sortierung simuliert eine Priority Queue
            HexCell currentCell = frontier.OrderBy(cell => distances[cell]).First();
            frontier.Remove(currentCell);

            int costToCurrent = distances[currentCell];

            // Wenn die Kosten bereits das Limit überschreiten, beenden wir diesen Pfad
            if (costToCurrent >= maxMovement) continue;

            // Prüfen Sie alle Nachbarn der aktuellen Zelle
            foreach (HexCell neighbor in GetNeighbors(currentCell))
            {
                int elevationIndex = neighbor.elevation;

                // 1. Unpassierbares Terrain prüfen
                if (!terrainData.IsPassable(elevationIndex)) continue;

                // 2. Besetzte Zelle prüfen (Einheiten können nicht auf besetzte Zellen ziehen)
                if (neighbor.isOccupied && neighbor != unit.currentCell) continue;

                // 3. Kosten für das Betreten des Nachbar-Terrains abrufen
                int costToEnterNeighbor = terrainData.GetCost(elevationIndex);

                // Gesamtkosten bis zum Nachbarn
                int newDistance = costToCurrent + costToEnterNeighbor;

                // Prüfen, ob diese Gesamtkosten das Bewegungslimit überschreiten
                if (newDistance <= maxMovement)
                {
                    // Prüfen, ob wir diese Zelle noch nicht besucht haben ODER ob wir einen billigeren Pfad gefunden haben
                    if (!distances.ContainsKey(neighbor) || newDistance < distances[neighbor])
                    {
                        distances[neighbor] = newDistance; // Kosten speichern

                        // Zelle zur Frontier hinzufügen, wenn sie noch nicht zur Verarbeitung ansteht
                        if (!frontier.Contains(neighbor))
                        {
                            frontier.Add(neighbor);
                        }
                    }
                }
            }
        }

        return distances;
    }

    private void PlaceUnit(int x, int y, int playerOwner)
    {
        if (cells.TryGetValue(y * gridWidth + x, out HexCell cell) && !cell.isOccupied)
        {
            // Unit instantiieren und platzieren
            Unit newUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
            newUnit.ownerPlayer = playerOwner;
            newUnit.Place(cell);
        }
    }

    public void HandleCellClick(HexCell cell)
    {
        // Phase 1: Hat der Spieler bereits eine Unit ausgewählt?
        if (selectedCell != null)
        {
            // Fall A: Spieler klickt auf eine neue Zelle (potenzieller Zug)

            // **Hervorhebungen entfernen**
            HighlightReachableCells(selectedCell.currentUnit, false);

            // **Prüfen, ob der Zug gültig ist**
            Unit unit = selectedCell.currentUnit;
            Dictionary<HexCell, int> reachableCells = GetReachableCells(unit);

            if (reachableCells.ContainsKey(cell) && !cell.isOccupied)
            {
                // Zug ist gültig: Bewege die Unit

                // Zustand der alten Zelle zurücksetzen
                
                selectedCell.playerOwner = 0;
                selectedCell.currentUnit = null;

                // Unit bewegen (Unit.Place wird das Cell.isOccupied/Owner setzen)
                unit.Place(cell);
            }

            // **Auswahl aufheben**
            selectedCell = null;
        }
        else
        {
            // Phase 2: Keine Unit ausgewählt. Prüfe, ob die geklickte Zelle eine Unit enthält.
            if (cell.isOccupied)
            {
                // Unit auswählen
                selectedCell = cell;

                // **Erreichbare Zellen hervorheben**
                HighlightReachableCells(selectedCell.currentUnit, true);
            }
        }
    }

    // NEU: Hilfsfunktion zur Steuerung der Hervorhebung
    private void HighlightReachableCells(Unit unit, bool highlight)
    {
        if (unit == null) return;

        Dictionary<HexCell, int> reachable = GetReachableCells(unit);

        foreach (HexCell cell in reachable.Keys)
        {
            // Nur Zellen hervorheben, die nicht die aktuelle Zelle sind und nicht besetzt sind
            if (cell != unit.currentCell && !cell.isOccupied)
            {
                cell.SetHighlight(highlight);
            }
        }
        // Die aktuelle Zelle selbst hervorheben, falls gewünscht
        unit.currentCell.SetHighlight(highlight);
    }
}

