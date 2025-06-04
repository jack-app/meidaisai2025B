using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // System.IO名前空間が必要です

public class DisplayScore : MonoBehaviour
{
    [SerializeField] private Text Title;
    [SerializeField] private Image PlayerImage;
    [SerializeField] private Text HighScoreText;

    private int highScore = 0;

    [SerializeField] private GameObject LeftHP;
    [SerializeField] private GameObject KilledEnemy;
    [SerializeField] private GameObject LeftTime;

    [SerializeField] private Text TotalScore_sum;

    [SerializeField] private int HPScale;
    [SerializeField] private int EnemyScale;
    [SerializeField] private int TimeScale;

    [SerializeField] private float ShowInterval = 0.2f;

    [SerializeField] private AudioClip piin;
    [SerializeField] private AudioClip dame;
    [SerializeField] private AudioClip fanfare;
    AudioSource audioSource;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HIGH_SCORE", 0);

        // 読み込み元を Application.persistentDataPath に変更
        string playerImagePath = Path.Combine(Application.persistentDataPath, "Player.png");
        
        PlayerImage.sprite = ConvertPathToSprite(playerImagePath);
        if (PlayerImage.sprite == null)
        {
            Debug.LogWarning("Player image could not be loaded from: " + playerImagePath + ". Displaying default or empty.");
            // 必要であれば、ここにデフォルト画像を設定するなどのフォールバック処理を記述
        }

        Title.text = GameData.isClear ? "ゲームクリア！" : "ゲームオーバー";
        audioSource = Camera.main.GetComponent<AudioSource>(); // audioSourceの初期化をShowScoresより前に移動
        StartCoroutine(ShowScores());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Sprite ConvertPathToSprite(string path)
    {
        Sprite sprite = null;
        if (File.Exists(path))
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2); // 初期サイズは小さくて良い。LoadImageでリサイズされる。
                if (tex.LoadImage(bytes)) // 画像データの読み込みに成功した場合
                {
                    sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    Debug.LogError("ConvertPathToSprite: Failed to load image data into texture from path: " + path);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("ConvertPathToSprite: Exception occurred while loading image from path: " + path + "\n" + e.ToString());
            }
        }
        else
        {
            Debug.LogWarning("ConvertPathToSprite: File not found at path: " + path);
        }
        return sprite;
    }

    private void Display(GameObject Parent, int Scale, int Score)
    {
        Text ScoreText = Parent.transform.GetChild(0).GetComponent<Text>();
        Text ScaleText = Parent.transform.GetChild(1).GetComponent<Text>();

        ScoreText.text = Score.ToString();
        ScaleText.text = "×" + Scale.ToString();
    }

    IEnumerator ShowScores()
    {
        yield return new WaitForSeconds(ShowInterval * 4.0f);

        if (audioSource != null) audioSource.PlayOneShot(piin);
        Display(LeftHP, HPScale, GameData.leftHP);
        GameData.TotalScore += GameData.leftHP * HPScale;

        yield return new WaitForSeconds(ShowInterval * 4.0f);

        if (audioSource != null) audioSource.PlayOneShot(piin);
        Display(KilledEnemy, EnemyScale, GameData.killedEnemy);
        GameData.TotalScore += GameData.killedEnemy * EnemyScale;

        yield return new WaitForSeconds(ShowInterval * 4.0f);

        if (GameData.isClear == true)
        {
            if (audioSource != null) audioSource.PlayOneShot(piin);
            Display(LeftTime, TimeScale, GameData.leftTime);
            GameData.TotalScore += GameData.leftTime * TimeScale;
        }
        else
        {
            if (audioSource != null) audioSource.PlayOneShot(dame);
            Image peke = LeftTime.transform.GetChild(2).GetComponent<Image>();
            if (peke != null) peke.enabled = true; // nullチェック追加
            Display(LeftTime, 0, GameData.leftTime);
        }

        yield return new WaitForSeconds(ShowInterval * 8.0f);

        if (audioSource != null) audioSource.PlayOneShot(piin);
        TotalScore_sum.text = GameData.TotalScore.ToString();

        yield return new WaitForSeconds(ShowInterval * 3.0f);

        if (GameData.TotalScore >= highScore)
        {
            if (audioSource != null) audioSource.PlayOneShot(fanfare);
            PlayerPrefs.SetInt("HIGH_SCORE", GameData.TotalScore); // highScore変数ではなく、GameData.TotalScoreを保存
            HighScoreText.text = "HIGH SCORE!!!";
        }
        PlayerPrefs.Save();
    }
}