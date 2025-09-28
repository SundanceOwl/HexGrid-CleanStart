using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float panSpeed = 30f;
    public float panBorderThickness = 10f;
    public float edgeScrollSpeed = 20f;
    public bool enableEdgeScrolling = true; // NEU: Steuert das Rand-Scrollen

    [Header("Zoom Settings")]
    public float scrollSpeed = 50f;
    public float minY = 10f;
    public float maxY = 60f;

    private Vector3 lastMousePosition;
    private bool isPanning = false;

    void Update()
    {
        Vector3 pos = transform.position;

        // 1. Rand-Scrollen / WASD-Steuerung
        if (enableEdgeScrolling)
        {
            // 1. Rand-Scrollen / WASD-Steuerung
            if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness)
                pos.z += edgeScrollSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
                pos.z -= edgeScrollSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
                pos.x += edgeScrollSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness)
                pos.x -= edgeScrollSpeed * Time.deltaTime;
        }


        // 2. Mausrad-Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // 3. Panning (Kamera ziehen mit mittlerer Maustaste)
        if (Input.GetMouseButtonDown(2))
        {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            pos.x -= delta.x * panSpeed * Time.deltaTime * (pos.y / 25f);
            pos.z -= delta.y * panSpeed * Time.deltaTime * (pos.y / 25f);
            lastMousePosition = Input.mousePosition;
        }

        transform.position = pos;
    }
}
