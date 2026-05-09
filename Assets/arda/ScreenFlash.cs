using UnityEngine;

public class ScreenFlash : MonoBehaviour
{
    public CanvasGroup flash;

    void Update()
    {
        flash.alpha = Random.Range(0f, 0.08f);
    }
}