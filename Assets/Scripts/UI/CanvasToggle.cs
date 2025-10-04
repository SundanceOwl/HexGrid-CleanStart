using UnityEngine;

public class CanvasToggle : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            bool isVisible = canvasGroup.alpha > 0;

            canvasGroup.alpha = isVisible ? 0 : 1;
            canvasGroup.interactable = !isVisible;
            canvasGroup.blocksRaycasts = !isVisible;
        }
    }
}
