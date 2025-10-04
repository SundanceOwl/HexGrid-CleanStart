using UnityEngine;

public class HexCellInput : MonoBehaviour
{
    private HexCell lastHighlightedCell;
    private LayerMask hexLayer;

    void Awake()
    {
        hexLayer = LayerMask.GetMask("Game_Terrain"); // Safe to call here
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hexLayer)) // Fixed: Use hexLayer
        {
            HexCell cell = hit.collider.GetComponent<HexCell>();
            if (cell != null)
            {
                UnityEngine.Debug.Log($"Hover auf Zelle: Hex {cell.x},{cell.y} E{cell.elevation} " +
                    $"(Elevation {cell.elevation}, Layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}, " +
                    $"Hit Point: {hit.point}, Normal: {hit.normal})");

                if (lastHighlightedCell != cell)
                {
                    if (lastHighlightedCell != null)
                    {
                        lastHighlightedCell.SetHighlight(false);
                    }
                    cell.SetHighlight(true);
                    lastHighlightedCell = cell;
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Raycast traf Objekt ohne HexCell-Komponente: {hit.collider.gameObject.name}, " +
                    $"Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, Hit Point: {hit.point}");
                if (lastHighlightedCell != null)
                {
                    lastHighlightedCell.SetHighlight(false);
                    lastHighlightedCell = null;
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log($"Kein Raycast-Treffer, Highlight zurückgesetzt. " +
                $"Mausposition: {Input.mousePosition}, Kamera: {Camera.main.transform.position}, " +
                $"Ray Direction: {ray.direction}, Ray Origin: {ray.origin}");
            if (lastHighlightedCell != null)
            {
                lastHighlightedCell.SetHighlight(false);
                lastHighlightedCell = null;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hexLayer)) // Fixed: Use hexLayer
            {
                HexCell cell = hit.collider.GetComponent<HexCell>();
                if (cell != null)
                {
                    UnityEngine.Debug.Log($"Zelle geklickt: Hex {cell.x},{cell.y} E{cell.elevation}");
                    // Klick-Logik hier
                }
            }
        }
    }
}