using UnityEngine;

public class HexCell : MonoBehaviour
{
    [Header("Cell Properties")]
    public int x, y; // Grid coordinates
    public int elevation = 0;
    public bool isOccupied = false;
    public int playerOwner = 0; // 0 = empty, 1 = Player 1, 2 = Player 2
    public Unit currentUnit; // NEU: Referenz zur Unit

    [Header("Visual Components")]
    public Renderer cellRenderer;
    public GameObject playerPiece;

    [Header("Materials")]
    // Diese Materialien müssen im Inspector zugewiesen werden
    public Material player1Material;
    public Material player2Material;
    public Material highlightMaterial;

    private Material originalMaterial;
    private bool isHighlighted = false;

    void Start()
    {
        if (cellRenderer == null)
            cellRenderer = GetComponent<Renderer>();

        // Speichere das Material, das vom Prefab mitgebracht wird, als Original-Terrain-Material.
        if (cellRenderer != null && cellRenderer.sharedMaterial != null)
        {
            originalMaterial = cellRenderer.sharedMaterial;
        }
    }

    // Maus-Events für das Hovern
    void OnMouseEnter() { SetHighlight(true); }
    void OnMouseExit() { SetHighlight(false); }
    void OnMouseDown()
    {
        if (GameManager.Instance != null)
        {
            // ANPASSUNG: Muss den Aufruf an HexGame.cs weiterleiten!
            GameManager.Instance.OnCellClicked(this);
        }
    }

    // Setzt das Material der Zelle (Hervorhebung/Besitzer)
    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;

        if (cellRenderer == null) return;

        if (highlight && highlightMaterial != null)
        {
            SetCellMaterial(highlightMaterial);
        }
        else
        {
            // Rückkehr zum Besitzer-Material oder zum Original-Terrain-Material
            if (playerOwner == 1 && player1Material != null)
            {
                SetCellMaterial(player1Material);
            }
            else if (playerOwner == 2 && player2Material != null)
            {
                SetCellMaterial(player2Material);
            }
            else if (originalMaterial != null)
            {
                // Wenn die Zelle leer ist
                SetCellMaterial(originalMaterial);
            }
        }
    }

    // Setzt den Besitzer der Zelle
    public void SetOwner(int player)
    {
        playerOwner = player;
        isOccupied = (player != 0);
        SetHighlight(isHighlighted);
    }

    private void SetCellMaterial(Material material)
    {
        if (cellRenderer != null && material != null)
        {
            cellRenderer.material = material;
        }
    }

    // Die SetElevation-Methode ist nicht mehr notwendig, da Prefabs das Material mitbringen.
}