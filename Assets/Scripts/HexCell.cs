using UnityEngine;

// Definiert die Zelle des Hex-Gitters
public class HexCell : MonoBehaviour
{
    [Header("Materialien")]
    public Material highlightMaterial; // Material f�r Hover-Effekt
    public Material cityMaterial;      // Material f�r Stadt-Zellen

    [Header("Status")]
    public bool isCity = false;          // NEU: Status f�r die Stadt-Logik

    [Header("Terrain Data")]
    public int elevation;       // F�R MAPSAVER
    public int playerOwner;     // F�R UNIT.CS UND CITY-LOGIC
    public Unit currentUnit;     // F�R HEXGAME
    public bool isOccupied => currentUnit != null;

    [Header("Coordinates")]
    public int x;
    public int y;
    public int z;               // F�R MAPSAVER

    // Private interne Felder
    private Material originalMaterial;
    private bool materialsInitialized = false;
    private Renderer _cellRenderer;

    [Header("Renderer Zuweisung")]
    public Renderer cellRenderer; // Bestehendes Feld beibehalten f�r Inspector

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
                    UnityEngine.Debug.LogError($"Fehler: Kein Renderer gefunden f�r HexCell {gameObject.name} (Elevation {elevation})");
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
            SetHighlight(false); // Setzt auf Original-Zustand zur�ck
        }
    }

    // Steuert den Hover-Effekt und die Material-Priorit�t
    public void SetHighlight(bool highlight)
    {
        UnityEngine.Debug.Log($"[Hover] Highlighting f�r {gameObject.name} mit Elevation {elevation}");

        if (highlight)
        {
            if (highlightMaterial != null)
            {
                SetCellMaterial(highlightMaterial);
                UnityEngine.Debug.Log($"Highlighting aktiviert f�r {gameObject.name} (Elevation {elevation})");
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Kein highlightMaterial zugewiesen f�r {gameObject.name} (Elevation {elevation})");
            }
        }
        else
        {
            // Setzt die h�chste dauerhafte Priorit�t: Stadt-Material
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
                UnityEngine.Debug.Log($"[Init] Originalmaterial gespeichert f�r {gameObject.name}: {originalMaterial.name}");
            }
            else
            {
                UnityEngine.Debug.LogError($"[Init] Kein g�ltiges sharedMaterial f�r {gameObject.name} gefunden!");
                return;
            }
        }

        // 2. Pr�fe, ob Material g�ltig ist
        if (material == null)
        {
            UnityEngine.Debug.LogWarning($"[Set] Material ist null f�r {gameObject.name}, Abbruch.");
            return;
        }

        // 3. Renderer vorhanden?
        if (CellRenderer == null)
        {
            UnityEngine.Debug.LogError($"[Set] Kein Renderer gefunden f�r {gameObject.name}, Abbruch.");
            return;
        }

        // 4. Material setzen
        if (material == originalMaterial)
        {
            CellRenderer.sharedMaterial = material;
            UnityEngine.Debug.Log($"[Set] sharedMaterial gesetzt f�r {gameObject.name}: {material.name}");
        }
        else
        {
            CellRenderer.material = material;
            UnityEngine.Debug.Log($"[Set] material gesetzt f�r {gameObject.name}: {material.name}");
        }
    }

}