using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SimpleCompass : MonoBehaviour
{
    [Header("Hand + Compass")]
    public GameObject handCompassRoot;
    public Transform needle;
    public Camera compassCamera;

    [Header("UI")]
    public CanvasGroup compassCanvasGroup;

    [Header("Animasyon Ayarları")]
    public float animDuration = 0.3f;

    [Header("Pozisyon")]
    public Vector3 shownPosition = new Vector3(0.5f, -0.4f, 0.8f);
    public Vector3 shownRotation = new Vector3(0f, 0f, 0f);

    [Header("Settings")]
    public string targetTag = "Monster";
    public float rotateSmooth = 8f;
    public float needleOffset = 0f;

    private Transform targetTransform;
    private bool compassActive = false;
    private Coroutine animCoroutine;

    void Start()
    {
        handCompassRoot.transform.localPosition = shownPosition;
        handCompassRoot.transform.localRotation = Quaternion.Euler(shownRotation);

        if (compassCanvasGroup != null) compassCanvasGroup.alpha = 0;
        handCompassRoot.SetActive(false);
        if (compassCamera != null) compassCamera.enabled = false;
    }

    void Update()
    {
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            compassActive = !compassActive;

            if (animCoroutine != null) StopCoroutine(animCoroutine);

            if (compassActive)
            {
                handCompassRoot.SetActive(true);
                if (compassCamera != null) compassCamera.enabled = true;
                animCoroutine = StartCoroutine(FadeAnim(0f, 1f, closeAfter: false));
            }
            else
            {
                animCoroutine = StartCoroutine(FadeAnim(1f, 0f, closeAfter: true));
            }
        }

        if (compassActive) UpdateNeedle();
    }

    // Sadece fade in/out — pozisyon/rotasyon animasyonu yok
    IEnumerator FadeAnim(float fromAlpha, float toAlpha, bool closeAfter)
    {
        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);

            if (compassCanvasGroup != null)
                compassCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);

            yield return null;
        }

        if (compassCanvasGroup != null)
            compassCanvasGroup.alpha = toAlpha;

        if (closeAfter)
        {
            handCompassRoot.SetActive(false);
            if (compassCamera != null) compassCamera.enabled = false;
        }
    }

    void UpdateNeedle()
    {
        if (targetTransform == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag(targetTag);
            if (obj != null) targetTransform = obj.transform;
        }

        if (targetTransform == null || needle == null) return;

        Vector3 localTargetPos = needle.parent.InverseTransformPoint(targetTransform.position);
        localTargetPos.y = 0;

        if (localTargetPos.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(localTargetPos);
            needle.localRotation = Quaternion.Slerp(
                needle.localRotation,
                targetRotation * Quaternion.Euler(0, needleOffset, 0),
                Time.deltaTime * rotateSmooth
            );
        }
    }
}