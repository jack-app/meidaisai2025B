using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public void GoToGameScene()
    {
        SceneManager.LoadScene("GameScene"); // ゲーム画面へ遷移
    }

    public void GoToResultScene()
    {
        SceneManager.LoadScene("ResultScene"); // 結果画面へ遷移
    }
}