using UnityEngine;

// Definiert die Zelle des Hex-Gitters
public class HexCell : MonoBehaviour
{
    [Header("Materialien")]
    public Material highlightMaterial; // Material für Hover-Effekt
    public Material cityMaterial;      // Material für Stadt-Zellen

    [Header("Status")]
    public bool isCity = false;          // NEU: Status für die Stadt-Logik

    [Header("Terrain Data")]
    public int elevation;       // FÜR MAPSAVER
    public int playerOwner;     // FÜR UNIT.CS UND CITY-LOGIC
    public Unit currentUnit;     // FÜR HEXGAME
    public bool isOccupied => currentUnit != null;

    [Header("Coordinates")]
    public int x;
    public int y;
    public int z;               // FÜR MAPSAVER

    // Private interne Felder
    private Material originalMaterial;
    private bool materialsInitialized = false;
    private Renderer _cellRenderer;

    [Header("Renderer Zuweisung")]
    public Renderer cellRenderer; // Bestehendes Feld beibehalten für Inspector

    private Renderer CellRenderer
    {
        get
        {
            if (_cellRenderer == null)
            {
                // Verwende Inspector-Wert, wenn gesetzt, sonst suche in Children
                _cellRenderer = cellRenderer != null ? cellRenderer : GetComponentInChildren<Renderer>();
                if (_cellRenderer == null)
                {
                    UnityEngine.Debug.LogError($"Fehler: Kein Renderer gefunden für HexCell {gameObject.name} (Elevation {elevation})");
                }
            }
            return _cellRenderer;
        }
    }

    // Setzt den Stadt-Status der Zelle
    public void SetIsCity(bool status)
    {
        isCity = status;

        // Aktualisiert das visuelle Erscheinungsbild sofort
        if (isCity)
        {
            SetHighlight(false); // Aktualisiert, falls gerade gehovert
            SetCellMaterial(cityMaterial);
        }
        else
        {
            SetHighlight(false); // Setzt auf Original-Zustand zurück
        }
    }

    // Steuert den Hover-Effekt und die Material-Priorität
    public void SetHighlight(bool highlight)
    {
        UnityEngine.Debug.Log($"[Hover] Highlighting für {gameObject.name} mit Elevation {elevation}");

        if (highlight)
        {
            if (highlightMaterial != null)
            {
                SetCellMaterial(highlightMaterial);
                UnityEngine.Debug.Log($"Highlighting aktiviert für {gameObject.name} (Elevation {elevation})");
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Kein highlightMaterial zugewiesen für {gameObject.name} (Elevation {elevation})");
            }
        }
        else
        {
            // Setzt die höchste dauerhafte Priorität: Stadt-Material
            if (isCity && cityMaterial != null)
            {
                SetCellMaterial(cityMaterial);
            }
            // Fallback auf das gespeicherte Originalmaterial
            else
            {
                SetCellMaterial(originalMaterial);
            }
        }
    }

    // Speichert/Setzt das Material auf den Renderer
    private void SetCellMaterial(Material material)
    {
        // 1. Initialisiere das Originalmaterial beim ersten Aufruf
        if (!materialsInitialized)
        {
            if (CellRenderer != null && CellRenderer.sharedMaterial != null)
            {
                originalMaterial = CellRenderer.sharedMaterial;
                materialsInitialized = true;
                UnityEngine.Debug.Log($"[Init] Originalmaterial gespeichert für {gameObject.name}: {originalMaterial.name}");
            }
            else
            {
                UnityEngine.Debug.LogError($"[Init] Kein gültiges sharedMaterial für {gameObject.name} gefunden!");
                return;
            }
        }

        // 2. Prüfe, ob Material gültig ist
        if (material == null)
        {
            UnityEngine.Debug.LogWarning($"[Set] Material ist null für {gameObject.name}, Abbruch.");
            return;
        }

        // 3. Renderer vorhanden?
        if (CellRenderer == null)
        {
            UnityEngine.Debug.LogError($"[Set] Kein Renderer gefunden für {gameObject.name}, Abbruch.");
            return;
        }

        // 4. Material setzen
        if (material == originalMaterial)
        {
            CellRenderer.sharedMaterial = material;
            UnityEngine.Debug.Log($"[Set] sharedMaterial gesetzt für {gameObject.name}: {material.name}");
        }
        else
        {
            CellRenderer.material = material;
            UnityEngine.Debug.Log($"[Set] material gesetzt für {gameObject.name}: {material.name}");
        }
    }

}