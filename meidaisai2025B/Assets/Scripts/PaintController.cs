using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class PaintController : MonoBehaviour
{
    private Vector2 m_disImagePos;

    [SerializeField]

    private RawImage m_image = null;

    private Texture2D m_texture = null;

    public Texture2D source_texture = null;

    public bool isErase = false;
    public bool isFill = false;

    /*[SerializeField]
    private int m_width = 4;
 
    [SerializeField]
    private int m_height = 4;*/

    public int m_radius = 4;

    public Color color;

    private Vector2 m_prePos;
    private Vector2 m_TouchPos;

    private float m_clickTime, m_preClickTime;

    //マウスらへんの機能
    private Image pointer;
    private Image eraser;
    private Image bucket;
    private RectTransform pointerRT;
    private RectTransform eraserRT;
    private RectTransform bucketRT;
    public Vector2 mousePos, worldPos;
    public void OnDrag(BaseEventData arg) //線を描画
    {
        PointerEventData _event = arg as PointerEventData; //タッチの情報取得

        // 押されているときの処理
        m_TouchPos = _event.position - m_disImagePos;
        //m_TouchPos = _event.position; //現在のポインタの座標
        m_clickTime = _event.clickTime; //最後にクリックイベントが送信された時間を取得

        float disTime = m_clickTime - m_preClickTime; //前回のクリックイベントとの時差

        //int width  = m_width;  //ペンの太さ(ピクセル)
        //int height = m_height; //ペンの太さ(ピクセル)

        int radius = m_radius;

        var dir = m_prePos - m_TouchPos; //直前のタッチ座標との差
        if (disTime > 0.01) dir = new Vector2(0, 0); //0.1秒以上間隔があいたらタッチ座標の差を0にする

        var dist = (int)dir.magnitude; //タッチ座標ベクトルの絶対値

        dir = dir.normalized; //正規化

        //指定のペンの太さ(ピクセル)で、前回のタッチ座標から今回のタッチ座標まで塗りつぶす
        for (int d = 0; d < dist; ++d)
        {
            var p_pos = m_TouchPos + dir * d; //paint position
            //p_pos.y -= radius;
            //p_pos.x -= radius;
            for (int h = -radius; h <= radius; ++h)
            {
                int y = (int)(p_pos.y + h);
                if (y < 0 || y > m_texture.height) continue; //タッチ座標がテクスチャの外の場合、描画処理を行わない

                for (int w = -radius; w <= radius; ++w)
                {
                    int x = (int)(p_pos.x + w);

                    if (x >= 0 && x <= m_texture.width)
                    {
                        if (w * w + h * h <= radius * radius)
                        {
                            if (isErase == true)
                            {
                                m_texture.SetPixel(x, y, new Color(0, 0, 0, 0));//消しゴム
                            }
                            else
                            {
                                m_texture.SetPixel(x, y, color); //線を描画
                            }
                        }
                    }
                }
            }
        }
        m_texture.Apply();
        m_prePos = m_TouchPos;
        m_preClickTime = m_clickTime;
    }

    public void OnTap(BaseEventData arg) //点を描画
    {
        PointerEventData _event = arg as PointerEventData; //タッチの情報取得

        // 押されているときの処理
        m_TouchPos = _event.position - m_disImagePos;
        //m_TouchPos = _event.position; //現在のポインタの座標

        int radius = m_radius;

        var p_pos = m_TouchPos; //paint position
        //p_pos.y -= radius;
        //p_pos.x -= radius;

        if (isFill == false)
        {
            for (int h = -radius; h <= radius; ++h)
            {
                int y = (int)(p_pos.y + h);
                if (y < 0 || y > m_texture.height) continue; //タッチ座標がテクスチャの外の場合、描画処理を行わない
                for (int w = -radius; w <= radius; ++w)
                {
                    int x = (int)(p_pos.x + w);
                    if (x >= 0 && x <= m_texture.width)
                    {
                        if (w * w + h * h <= radius * radius)
                        {
                            if (isErase == true)
                            {
                                m_texture.SetPixel(x, y, new Color(0, 0, 0, 0));//消しゴム
                            }
                            else
                            {
                                m_texture.SetPixel(x, y, color); //線を描画
                            }
                        }
                    }
                }
            }
        }
        else
        {
            PaintFill((int)p_pos.x, (int)p_pos.y, m_texture.GetPixel((int)p_pos.x, (int)p_pos.y));
        }

        m_texture.Apply();
    }


    private void PaintFill(int x, int y, Color targetColor)
    {
        int width = m_texture.width;
        int height = m_texture.height;

        Color[] pixels = m_texture.GetPixels();

        int index(int px, int py) => py * width + px;

        Color originalColor = pixels[index(x, y)];
        if (originalColor == color || originalColor != targetColor)
            return;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(x, y));

        while (queue.Count > 0)
        {
            Vector2Int p = queue.Dequeue();
            int px = p.x;
            int py = p.y;

            if (px < 0 || px >= width || py < 0 || py >= height)
                continue;

            int i = index(px, py);
            if (pixels[i] != targetColor)
                continue;

            pixels[i] = color;

            queue.Enqueue(new Vector2Int(px + 1, py));
            queue.Enqueue(new Vector2Int(px - 1, py));
            queue.Enqueue(new Vector2Int(px, py + 1));
            queue.Enqueue(new Vector2Int(px, py - 1));
        }

        m_texture.SetPixels(pixels);
        m_texture.Apply();
    }

    private void Start()
    {
        var rect = m_image.gameObject.GetComponent<RectTransform>().rect;
        m_texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);

        Graphics.CopyTexture(source_texture, m_texture);

        //下の行追加（2021/10/21）
        //WhiteTexture((int)rect.width, (int)rect.height);

        m_image.texture = m_texture;

        var m_imagePos = m_image.gameObject.GetComponent<RectTransform>().anchoredPosition;
        m_disImagePos = new Vector2(m_imagePos.x - rect.width / 2, m_imagePos.y - rect.height / 2);

        var rt = new RenderTexture(m_texture.width, m_texture.height, 32);

        //マウスらへんの機能
        //Cursor.visible = false;

        pointer = GameObject.Find("Pointer").GetComponent<Image>();
        eraser = GameObject.Find("Eraser").GetComponent<Image>();
        bucket = GameObject.Find("Bucket").GetComponent<Image>();

        pointerRT = GameObject.Find("Pointer").GetComponent<RectTransform>();
        eraserRT = GameObject.Find("Eraser").GetComponent<RectTransform>();
        bucketRT = GameObject.Find("Bucket").GetComponent<RectTransform>();
    }

    private void Update()
    {
        //仮仕様　本番はStart()のCursor.visible = false;を使用
        if (Input.GetKeyDown(KeyCode.P))
        {
            Cursor.visible = !Cursor.visible;
        }

        mousePos = Input.mousePosition;
        worldPos = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

        pointer.transform.position = mousePos;
    }

    //下の関数を追加（2021/10/21）
    //テクスチャを白色にする関数
    /*private void WhiteTexture(int width, int height)
    {
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                m_texture.SetPixel(w, h, Color.white);
            }
        }
        m_texture.Apply();
    }*/

    public void IsErase()
    {
        isErase = true;
        eraser.enabled = true;
        pointer.enabled = false;
        isFill = false;
        bucket.enabled = false;
    }

    public void IsBrush()
    {
        isErase = false;
        eraser.enabled = false;
        pointer.enabled = true;
        isFill = false;
        bucket.enabled = false;
    }

    public void IsFll()
    {
        isErase = false;
        eraser.enabled = false;
        pointer.enabled = false;
        isFill = true;
        bucket.enabled = true;
    }

    public void ChangeScale(BaseEventData data)
    {
        GameObject pointerObject = (data as PointerEventData).pointerClick;
        Vector2 size = pointerObject.GetComponent<RectTransform>().sizeDelta;

        m_radius = (int)size.x / 2;
        pointerRT.sizeDelta = size;
        eraserRT.sizeDelta = size;
        eraserRT.localPosition = new Vector2(size.x / 4, size.y / 4);
        bucketRT.sizeDelta = size;
        bucketRT.localPosition = new Vector2(size.x / 4, size.y / 4);
    }

    public void ChangeColor(BaseEventData data)
    {
        GameObject pointerObject = (data as PointerEventData).pointerClick;
        Color ObjectColor = pointerObject.GetComponent<Image>().color;

        color = ObjectColor;
        pointer.color = ObjectColor;
    }

    public void ToPNG()
    {
        var storagePath = Application.dataPath + "/" + "Player.png";

        //Texture2D resizedTexture = ResizeTexture(m_texture, 516, 516);
        byte[] png = m_texture.EncodeToPNG();

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        File.WriteAllBytes(storagePath, png);
        
        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds + "ms");
        
        //Debug.Log(storagePath);
    }
    
    Texture2D ResizeTexture(Texture2D srcTexture, int newWidth, int newHeight) {
        var resizedTexture = new Texture2D(newWidth, newHeight);
        Graphics.ConvertTexture(srcTexture, resizedTexture);
        return resizedTexture;
    }
}
