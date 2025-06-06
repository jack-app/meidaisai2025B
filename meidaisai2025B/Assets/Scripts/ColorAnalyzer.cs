using UnityEngine;

public class ColorAnalyzer : MonoBehaviour
{
    public Texture2D texture;

    void Start()
    {
        AnalyzeImageColors(texture);
    }

    void AnalyzeImageColors(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("テクスチャが指定されていません。");
            return;
        }

        Color[] pixels = texture.GetPixels();
        float totalR = 0f;
        float totalG = 0f;
        float totalB = 0f;

        foreach (Color color in pixels)
        {
            totalR += color.r;
            totalG += color.g;
            totalB += color.b;
        }

        float total = totalR + totalG + totalB;

        if (total == 0)
        {
            Debug.LogWarning("画像にRGB情報が含まれていないか、すべてが黒です。");
            return;
        }

        float rRatio = (totalR / total) * 100f;
        float gRatio = (totalG / total) * 100f;
        float bRatio = (totalB / total) * 100f;

        Debug.Log($"RGB比率: R = {rRatio:F2}%, G = {gRatio:F2}%, B = {bRatio:F2}%");

        float attack = 10 + totalR / total * 90;
        float hp = 30 + totalG / total * 270;
        float speed = 10 + totalB / total * 90;

        Debug.Log($"ステータス：攻撃 = {attack:F2}/100, 体力 = {hp:F2}/300, 速度 = {speed:F2}/100");
    }
}