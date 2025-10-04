using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton-Instanz
    public static GameManager Instance { get; private set; }
    // Referenz zum HexGame-Skript, wo die eigentliche Logik ist
    public HexGame hexGame;

    private Unit selectedUnit;


    void Awake()
    {
        // Singleton-Logik
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Sicherstellen, dass die Referenz gesetzt ist
        hexGame = GetComponent<HexGame>();
        if (hexGame == null)
        {
            UnityEngine.Debug.LogError("GameManager benötigt ein HexGame-Skript auf demselben GameObject.");
        }
    }

    // Weiterleiten des Mausklicks an das HexGame-Skript
    public void OnCellClicked(HexCell cell)
    {
        if (hexGame != null)
        {
            hexGame.HandleCellClick(cell);
        }
    }

    public void SelectUnit(Unit unit)
    {
        selectedUnit = unit;
        UnityEngine.Debug.Log("Einheit ausgewählt: " + unit.unitName);

        // Pathfinding starten
        Dictionary<HexCell, int> reachable = GameManager.Instance.hexGame.GetReachableCells(unit);
        foreach (HexCell tile in reachable.Keys)
        {
            tile.SetHighlight(true);
        }
    }
}