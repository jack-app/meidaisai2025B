using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GamePartController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Image playerImage;

    [SerializeField] private GameObject status;

    private GameObject[] taggedObjects;
    public List<int> enemiesInRoom;
    public List<int> killInRoom;

    public string targetTag = "Enemy"; // カウントしたいオブジェクトのタグを指定
    private Camera mainCamera;

    public AudioSource audioSource;

    [SerializeField] private AudioClip charge;
    [SerializeField] private AudioClip StartSE;

    //部屋管理
    public int roomNum = 0;


    public bool isStart = false;
    private PlayerAttack playerAttack;

    // Start is called before the first frame update
    void Start()
    {
        var playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        // 読み込み元を Application.persistentDataPath に変更
        string playerImagePathS = Path.Combine(Application.persistentDataPath, "PlayerSmall.png");

        playerSpriteRenderer.sprite = ConvertPathToSprite(playerImagePathS);
        if (playerSpriteRenderer.sprite == null)
        {
            Debug.LogWarning("Player image could not be loaded from: " + playerImagePathS + ". Displaying default or empty.");
            // 必要であれば、ここにデフォルト画像を設定するなどのフォールバック処理を記述
        }

        player.AddComponent<PolygonCollider2D>(); // PolygonCollider2Dの追加は通常一度だけで良い
        player.AddComponent<CompositeCollider2D>();

        string playerImagePath = Path.Combine(Application.persistentDataPath, "Player.png");

        Sprite playerUiSprite = ConvertPathToSprite(playerImagePath); ;
        if (playerUiSprite != null)
        {
            playerImage.sprite = playerUiSprite;
        }
        else
        {
            Debug.LogError("Player UI sprite not found in Resources.");
        }

        audioSource = GetComponent<AudioSource>();

        mainCamera = Camera.main;

        enemiesInRoom = new List<int> { 0, 0, 0 };
        killInRoom = new List<int> { 0, 0, 0 };

        playerAttack = player.transform.GetChild(0).GetComponent<PlayerAttack>();
        playerAttack.enabled = false;

        StartCoroutine(GameStart());
    }

    // Update is called once per frame
    void Update()
    {
        //if (isStart == true) 
        //CountKilledObjects();
    }

    public static Sprite ConvertPathToSprite(string path)
    {
        Sprite sprite = null;

        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        return sprite;
    }

    public void CountVisibleObjects()
    {
        enemiesInRoom[roomNum] = 0;
        taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);

        Debug.Log(taggedObjects.Length);
        foreach (GameObject obj in taggedObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();

            if (renderer != null && renderer.isVisible)
            {
                // より厳密にメインカメラに映っているか確認する場合の簡易的な追加チェック (オプション)
                Vector3 viewportPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
                if (viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1)
                {
                    enemiesInRoom[roomNum]++;
                }
            }
        }
    }

    IEnumerator GameStart()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        var statusHP = status.transform.GetChild(1).GetComponent<Text>();
        var statusPower = status.transform.GetChild(2).GetComponent<Text>();
        var statusSpeed = status.transform.GetChild(3).GetComponent<Text>();

        //ステータスを表示
        yield return new WaitForSecondsRealtime(1.0f);
        StartCoroutine(DisplayStatus(statusHP, 150));
        yield return new WaitForSecondsRealtime(1.6f);
        StartCoroutine(DisplayStatus(statusPower, 50));
        yield return new WaitForSecondsRealtime(1.6f);
        StartCoroutine(DisplayStatus(statusSpeed, 300));

        yield return new WaitForSecondsRealtime(3.0f);

        GameObject displayStatus = GameObject.Find("DisplayStatus");
        displayStatus.SetActive(false);

        yield return new WaitForSecondsRealtime(0.2f);
        audioSource.PlayOneShot(StartSE);

        yield return new WaitForSecondsRealtime(0.3f);
        GameObject startLetters = GameObject.Find("GameStart");
        startLetters.GetComponent<Text>().enabled = true;

        yield return new WaitForSecondsRealtime(1.3f);

        GameObject forStart = GameObject.Find("ForStart");
        forStart.SetActive(false);

        isStart = true;

        CountVisibleObjects();

        // 時間を元に戻す
        Time.timeScale = originalTimeScale; // 元のTimeScaleに戻すのがより安全です
        playerAttack.enabled = true;
    }

    IEnumerator DisplayStatus(Text status, int statusNum)
    {
        var statusSum = status.transform.GetChild(0).GetComponent<Text>();
        var statusBar = status.transform.GetChild(1).GetComponent<Image>();
        var BarRT = statusBar.GetComponent<RectTransform>();

        if (audioSource != null && charge != null)
        {
            audioSource.PlayOneShot(charge);
        }


        for (int DisplayNum = 0; DisplayNum <= statusNum; DisplayNum++)
        {
            statusSum.text = DisplayNum.ToString();

            int ImageWidth = DisplayNum * 2; // バーの幅を更新
            BarRT.anchoredPosition = new Vector2(DisplayNum, BarRT.anchoredPosition.y);
            BarRT.sizeDelta = new Vector2(ImageWidth, BarRT.sizeDelta.y); // 高さは元の値を維持

            // Time.timeScaleの影響を受けないWaitForSecondsRealtimeを使用
            yield return new WaitForSecondsRealtime((float)(1.0f / statusNum));
        }
    }
}
