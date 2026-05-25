using Plane;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

[RequireComponent(typeof(RectTransform))]
public class JetCompass : MonoBehaviour
{
    public List<LabelRect> labelPool = new List<LabelRect>();
    public Color backgroundColor;
    public Color hudColor = Color.green;
    public float visibleDegrees = 60f;
    public int tickStep = 5;
    public Aerodynamics plane;
    public float yAngle;

    float _tapeWidth;
    float _pixelsPerDegree;
    float _halfWidth;

    void SpawnCompassScrollBar()
    {
        float height = 223;
        var go = MakeChild("CompassScrollBar");
        var image = go.AddComponent<Image>();
        image.color = hudColor;
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, height);
        rt.sizeDelta = new Vector2(_tapeWidth, 1.2f);
    }

    void SpawnHeadingTick()
    {
        float height = 236;
        var go = MakeChild("HeadingTick");
        var image = go.AddComponent<Image>();
        image.color = hudColor;
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, height);
        rt.sizeDelta = new Vector2(2, 20);
    }

    void SpawnLabel(float startX)
    {
        Font builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        float height = 236;

        var go = MakeChild("Tick");
        var image = go.AddComponent<Image>();
        image.color = hudColor;
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(startX, height);
        rt.sizeDelta = new Vector2(2, 15);

        var labelGO = MakeChild("Label");
        labelGO.GetComponent<RectTransform>().SetParent(rt, false);
        var label = labelGO.AddComponent<Text>();
        label.font = builtinFont;
        label.color = hudColor;
        label.alignment = TextAnchor.MiddleCenter;
        label.fontSize = 10;
        label.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 18);

        LabelRect labelRect = new LabelRect();
        labelRect.image = image;
        labelRect.text = label;
        labelRect.rect = rt;
        labelRect.startX = startX;

        labelPool.Add(labelRect);
    }

    void SpawnCompass()
    {
        // Pixel distance between ticks — same ratio as tickStep is to visibleDegrees
        float spacing = (tickStep / visibleDegrees) * _tapeWidth;

        // One extra tick so the wrap has no visible gap at either edge
        int tickCount = Mathf.CeilToInt(visibleDegrees / tickStep) + 1;

        for (int i = 0; i < tickCount; i++)
        {
            float startX = -_halfWidth + i * spacing;
            SpawnLabel(startX);
        }
    }

    private void Awake()
    {
        _tapeWidth = GetComponent<RectTransform>().rect.width;
        _pixelsPerDegree = _tapeWidth / visibleDegrees;
        _halfWidth = _tapeWidth * 0.5f;

        SpawnCompassScrollBar();
        SpawnHeadingTick();
        SpawnCompass();
    }
    private void FixedUpdate()
    {
        yAngle = plane.transform.eulerAngles.y;
    }
    private void LateUpdate()
    {
        // How many pixels the whole tape has shifted due to heading
        float headingOffset = yAngle * _pixelsPerDegree;

        for (int i = 0; i < labelPool.Count; i++)
        {
            LabelRect label = labelPool[i];

            // Shift this tick left by the heading offset from its spawn position
            float x = label.startX - headingOffset;

            // Periodic boundary — wrap x into [-halfWidth, halfWidth]
            // Same idea as a simulation box: particle exits one side, enters the other
            x = (x % _tapeWidth + _tapeWidth + _halfWidth) % _tapeWidth - _halfWidth;

            // Read the angle back out from screen position
            // Center of tape = current heading, so offset from center = offset in degrees
            float degree = MathHelpers.WrapDegrees(yAngle + x / _pixelsPerDegree);

            // Snap to nearest tick interval so labels don't show fractional degrees mid-scroll
            int snapped = Mathf.RoundToInt(degree / tickStep) * tickStep;
            snapped = ((snapped % 360) + 360) % 360;

            label.text.text = GetLabel(snapped);

            Vector2 pos = label.rect.anchoredPosition;
            pos.x = x;
            label.rect.anchoredPosition = pos;
            labelPool[i] = label;
        }
    }

    string GetLabel(int degree)
    {
        if (degree == 0) return "N";
        if (degree == 90) return "E";
        if (degree == 180) return "S";
        if (degree == 270) return "W";
        return degree.ToString();
    }

    public struct LabelRect
    {
        public RectTransform rect;
        public Image image;
        public Text text;
        public float startX;
        public float degreesToDisplay;
    }

    GameObject MakeChild(string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.GetComponent<RectTransform>().SetParent(transform, false);
        return go;
    }
}
