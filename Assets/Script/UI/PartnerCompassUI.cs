using UnityEngine;
using UnityEngine.UI;

// UI indikator arah pasangan (CCC-29) + progress hold teleport (CCC-30).
// Hanya membaca data dari DynamicCameraController, tidak mengubah apa pun di sana.
public class PartnerCompassUI : MonoBehaviour
{
    [System.Serializable]
    private class Indicator
    {
        public RectTransform root;   // pindah ke tepi layar (anchor bawah-kiri, pivot tengah)
        public RectTransform arrow;  // child root, sprite menghadap ATAS, dirotasi ke arah pasangan
        public Image holdFill;       // child root, Image Type = Filled, Radial 360
    }

    [Header("References")]
    [SerializeField] private DynamicCameraController _camera;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Indicator[] _indicators = new Indicator[2]; // 0 = Player1, 1 = Player2

    [Header("Layout")]
    [SerializeField, Range(0.02f, 0.3f)] private float _edgeMargin = 0.08f;

    [Header("Teleport")]
    [SerializeField] private float _teleportHoldSeconds = 3f; // samakan dengan DynamicCameraController

    private void Awake()
    {
        foreach (Indicator indicator in _indicators)
        {
            if (indicator == null || indicator.root == null) continue;

            indicator.root.anchorMin = Vector2.zero;
            indicator.root.anchorMax = Vector2.zero;
            indicator.root.pivot = new Vector2(0.5f, 0.5f);
            indicator.root.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (_camera == null || _canvas == null) return;

        bool show = _camera.CurrentMode == DynamicCameraController.CameraMode.Split
            && _camera.PartnerCompassVisible;

        for (int i = 0; i < _indicators.Length; i++)
        {
            UpdateIndicator(i, show);
        }
    }

    private void UpdateIndicator(int index, bool show)
    {
        Indicator indicator = _indicators[index];
        if (indicator == null || indicator.root == null) return;

        if (!show)
        {
            SetActive(indicator, false);
            return;
        }

        Vector3 view = _camera.GetPartnerViewportPosition(index);
        Vector2 point = new Vector2(view.x, view.y);
        bool behind = view.z < 0f;

        bool onScreen = !behind
            && point.x > _edgeMargin && point.x < 1f - _edgeMargin
            && point.y > _edgeMargin && point.y < 1f - _edgeMargin;

        if (onScreen)
        {
            SetActive(indicator, false); // pasangan terlihat di layar sendiri
            return;
        }

        Vector2 direction = point - new Vector2(0.5f, 0.5f);
        if (behind) direction = -direction;
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.up;

        float half = 0.5f - _edgeMargin;
        float scale = half / Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        Vector2 edge = new Vector2(0.5f, 0.5f) + direction * scale;

        // Tiap pemain punya setengah layar (split vertikal).
        float halfWidth = Screen.width * 0.5f;
        Vector2 screen = new Vector2(index * halfWidth + edge.x * halfWidth, edge.y * Screen.height);
        indicator.root.anchoredPosition = screen / _canvas.scaleFactor;

        if (indicator.arrow != null)
        {
            // Arah dihitung di ruang piksel supaya sudut tidak terdistorsi aspect ratio.
            Vector2 pixelDirection = new Vector2(direction.x * halfWidth, direction.y * Screen.height);
            float angle = Mathf.Atan2(pixelDirection.y, pixelDirection.x) * Mathf.Rad2Deg - 90f;
            indicator.arrow.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        UpdateHold(index, indicator);
        SetActive(indicator, true);
    }

    private void UpdateHold(int index, Indicator indicator)
    {
        if (indicator.holdFill == null) return;

        float hold = index == 0 ? _camera.Player1TeleportHold : _camera.Player2TeleportHold;

        indicator.holdFill.fillAmount = Mathf.Clamp01(hold / Mathf.Max(0.01f, _teleportHoldSeconds));
        indicator.holdFill.enabled = hold > 0.01f;
    }

    private static void SetActive(Indicator indicator, bool active)
    {
        if (indicator.root.gameObject.activeSelf != active) indicator.root.gameObject.SetActive(active);
    }
}