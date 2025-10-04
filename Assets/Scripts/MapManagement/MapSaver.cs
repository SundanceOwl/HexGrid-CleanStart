using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;


public class MapSaver : MonoBehaviour
{
    // Die interne Klasse MUSS HIER definiert werden, da sie keine eigene Datei hat, 
    // aber von der äußeren Klasse verwendet wird.
    [System.Serializable]
    public class MapFile
    {
        public HexData[] Hexes;
        public int Width;
        public int Height;
        public bool[] isCity;
    }

    // Referenz zum HexGame-Skript, um das Grid abzurufen
    public HexGame hexGame;

    public string fileName = "Map_01.json";

    // --- Die Speichern-Funktion ---
    [ContextMenu("Save Map to File")] // <--- DIESE ZEILE HINZUFÜGEN!
    public void SaveMap()
    {
        if (hexGame == null)
        {
            Debug.LogError("HexGame-Referenz fehlt im MapSaver!");
            return;
        }

        // Die Liste aller HexCell-Objekte aus dem HexGame-Dictionary abrufen
        List<HexCell> allCells = hexGame.cells.Values.ToList();

        if (allCells.Count == 0)
        {
            Debug.LogWarning("Das Hex-Grid ist leer. Nichts zu speichern.");
            return;
        }

        MapFile mapData = new MapFile();

        // 1. Daten aus den lebenden HexCell-Objekten auslesen und in HexData konvertieren
        mapData.Hexes = allCells
            .Select(cell => new HexData
            {
                Q = cell.x,
                R = cell.y,
                // Wir speichern nur den Index, der die Höhe und den Prefab-Typ definiert
                ElevationIndex = cell.elevation
            })
            .ToArray();

        // Neu: isCity-Array initialisieren mit false für alle Hexe
        mapData.isCity = new bool[allCells.Count];
        for (int i = 0; i < mapData.isCity.Length; i++)
        {
            mapData.isCity[i] = false;
        }

        // 2. Grid-Dimensionen speichern (aus HexGame.cs)
        mapData.Width = hexGame.gridWidth;
        mapData.Height = hexGame.gridHeight;

        // 3. Map-Container in JSON umwandeln
        string json = JsonUtility.ToJson(mapData, true);

        // 4. Pfad zum Speichern festlegen (Assets/Maps/)
        string path = Path.Combine(Application.dataPath, "Maps", fileName);

        // Ordner erstellen, falls er noch nicht existiert
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        // 5. In die Datei schreiben
        File.WriteAllText(path, json);

        Debug.Log($"Map '{fileName}' erfolgreich gespeichert unter: {path}");

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}