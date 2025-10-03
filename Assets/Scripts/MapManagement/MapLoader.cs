using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    // Die MapFile-Struktur MUSS mit der im MapSaver und der JSON-Datei übereinstimmen!
    [System.Serializable]
    public class MapFile
    {
        public HexData[] Hexes;
        public int Width;
        public int Height;
    }

    public HexGame hexGame;
    public string fileName = "Map_01.json";

    [ContextMenu("Load Map from File")]
    public void LoadMap()
    {
        if (hexGame == null)
        {
            Debug.LogError("HexGame-Referenz fehlt im MapLoader!");
            return;
        }

        string path = Path.Combine(Application.dataPath, "Maps", fileName);

        if (!File.Exists(path))
        {
            Debug.LogError($"Map-Datei nicht gefunden unter: {path}. Kann nicht geladen werden.");
            return;
        }

        // 1. Datei einlesen und deserialisieren
        string json = File.ReadAllText(path);
        MapFile mapData = JsonUtility.FromJson<MapFile>(json);

        if (mapData == null || mapData.Hexes == null)
        {
            Debug.LogError("Fehler beim Deserialisieren der Map-Daten.");
            return;
        }

        // 2. Bestehendes Gitter löschen
        hexGame.ClearGrid();

        // 3. Grid-Dimensionen setzen
        hexGame.gridWidth = mapData.Width;
        hexGame.gridHeight = mapData.Height;

        // 4. Hex-Gitter Zelle für Zelle neu aufbauen
        BuildGridFromData(mapData);

        Debug.Log($"Map '{fileName}' erfolgreich geladen und das Grid neu aufgebaut.");
    }

    private void BuildGridFromData(MapFile mapData)
    {
        // Das Dictionary für die neuen Zellen neu initialisieren
        hexGame.cells = new Dictionary<int, HexCell>();

        foreach (HexData hexData in mapData.Hexes)
        {
            // Die Höhe aus dem Index und Prefabs ableiten
            int elevation = hexData.ElevationIndex;

            // Korrektes Prefab auswählen
            int index = Mathf.Clamp(elevation, 0, hexGame.elevationPrefabs.Length - 1);
            GameObject cellPrefabToUse = hexGame.elevationPrefabs[index];

            // Position mithilfe der (jetzt öffentlichen) HexGame-Methode berechnen
            Vector3 position = hexGame.GetCellPosition(hexData.Q, hexData.R);

            // Instanziieren und Konfigurieren der Zelle
            GameObject cellGO = Instantiate(cellPrefabToUse, position, Quaternion.identity, hexGame.transform);

            HexCell cell = cellGO.GetComponent<HexCell>();
            cell.x = hexData.Q;
            cell.y = hexData.R;
            cell.elevation = elevation; // Wichtig: Elevation setzen für Bewegungslogik!

            // Füge die Zelle zum HexGame-Dictionary hinzu
            int key = cell.y * hexGame.gridWidth + cell.x;
            hexGame.cells.Add(key, cell);

            cellGO.name = $"Hex {cell.x},{cell.y} E{cell.elevation} (Loaded)";
        }

        // Führe die Startinitialisierung des Spiels erneut aus (z.B. Einheiten platzieren)
        hexGame.InitializeGameStart();
    }
}