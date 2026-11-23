using UnityEngine;
using UnityEngine.UI;

public class MiniHistoryGraph : MonoBehaviour
{
    public RawImage graphImage;
    public int textureWidth = 400;
    public int textureHeight = 120;
    public Color lineColor = Color.cyan;
    public Color backgroundColor = new Color(0, 0, 0, 0.3f);

    private Texture2D _texture;

    private void Awake()
    {
        _texture = new Texture2D(textureWidth, textureHeight);
        _texture.filterMode = FilterMode.Point;
        graphImage.texture = _texture;
    }

    public void Draw(float[] values, float minValue, float maxValue)
    {
        // background init
        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        _texture.SetPixels(pixels);

        if (values.Length < 2)
        {
            _texture.Apply();
            return;
        }

        float range = Mathf.Max(maxValue - minValue, 0.01f); // prevent division by zero
        // TODO : 어떻게 그리는건지 공부하기
        float graphHeight = textureHeight / 3f;
        float yOffset = (textureHeight - graphHeight) / 2f; // 중앙 정렬

        for (int i = 0; i < values.Length - 1; i++)
        {
            int x0 = Mathf.RoundToInt((float)i / (values.Length - 1) * (textureWidth - 1));
            int x1 = Mathf.RoundToInt((float)(i + 1) / (values.Length - 1) * (textureWidth - 1));
            int y0 = Mathf.RoundToInt(((values[i] - minValue) / range) * (graphHeight - 1) + yOffset);
            int y1 = Mathf.RoundToInt(((values[i + 1] - minValue) / range) * (graphHeight - 1) + yOffset);

            DrawLine(x0, y0, x1, y1);
        }

        _texture.Apply();
    }

    // draw lines between points
    // TODO : 어떻게 돌아가는건지 공부하기
    // bresenham's line algorithm은 픽셀 단위로 직선을 그릴 때 효율적, 선의 기울기에 따라 픽셀을 선택
    void DrawLine(int x0, int y0, int x1, int y1)
    {
        int dx = Mathf.Abs(x1 - x0), dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if (x0 >= 0 && x0 < textureWidth && y0 >= 0 && y0 < textureHeight)
            {
                _texture.SetPixel(x0, y0, lineColor);
            }
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }
}
