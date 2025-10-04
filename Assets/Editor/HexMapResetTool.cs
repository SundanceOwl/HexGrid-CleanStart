using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class HexMapResetTool : EditorWindow
{
    [MenuItem("Tools/Hex Map/Reset and Reload")]
    public static void ResetAndReloadMap()
    {
        // 1. Alle Hex-Zellen l�schen
        var allHexCells = GameObject.FindObjectsByType<HexCell>(FindObjectsSortMode.None);

        int deleted = 0;

        foreach (var cell in allHexCells)
        {
            DestroyImmediate(cell.gameObject);
            deleted++;
        }

        UnityEngine.Debug.Log($"[HexMapResetTool] {deleted} Hex-Zellen gel�scht.");

        // 2. MapLoader finden
        var mapLoader = UnityEngine.Object.FindAnyObjectByType<MapLoader>();
        if (mapLoader == null)
        {
            UnityEngine.Debug.LogError("[HexMapResetTool] Kein MapLoader in der Szene gefunden.");
            return;
        }

        // 3. Map neu laden
        mapLoader.LoadMap();

        UnityEngine.Debug.Log("[HexMapResetTool] Map wurde erfolgreich neu geladen.");
    }
}
