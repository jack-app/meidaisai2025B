using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

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
    // Start is called before the first frame update

    [SerializeField] private AudioClip piin;
    [SerializeField] private AudioClip dame;
    [SerializeField] private AudioClip fanfare;
    AudioSource audioSource;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HIGH_SCORE", 0);

        var playerPath = Application.dataPath + "/" + "Player.png";
        //byte[] fileData = System.IO.File.ReadAllBytes(playerPath); 
        PlayerImage.sprite = ConvertPathToSprite(playerPath);

        Title.text = GameData.isClear ? "ゲームクリア！" : "ゲームオーバー";
        StartCoroutine(ShowScores());
        
        audioSource = GetComponent<AudioSource>();
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
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
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

        audioSource.PlayOneShot(piin);
        Display(LeftHP, HPScale, GameData.leftHP);
        GameData.TotalScore += GameData.leftHP * HPScale;

        yield return new WaitForSeconds(ShowInterval * 4.0f);

        audioSource.PlayOneShot(piin);
        Display(KilledEnemy, EnemyScale, GameData.killedEnemy);
        GameData.TotalScore += GameData.killedEnemy * EnemyScale;

        yield return new WaitForSeconds(ShowInterval * 4.0f);

        if (GameData.isClear == true)
        {
            audioSource.PlayOneShot(piin);
            Display(LeftTime, TimeScale, GameData.leftTime);
            GameData.TotalScore += GameData.leftTime * TimeScale;
        }
        else
        {
            audioSource.PlayOneShot(dame);
            Image peke = LeftTime.transform.GetChild(2).GetComponent<Image>();
            peke.enabled = true;
            Display(LeftTime, 0, GameData.leftTime);
        }

        yield return new WaitForSeconds(ShowInterval * 8.0f);

        audioSource.PlayOneShot(piin);
        TotalScore_sum.text = GameData.TotalScore.ToString();

        yield return new WaitForSeconds(ShowInterval * 3.0f);

        if (GameData.TotalScore >= highScore)
        {
            audioSource.PlayOneShot(fanfare);
            PlayerPrefs.SetInt("HIGH_SCORE", GameData.TotalScore);
            HighScoreText.text = "HIGH SCORE!!!";
        }
        PlayerPrefs.Save();
    }
}
