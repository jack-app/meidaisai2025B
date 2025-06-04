using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void GoToPaintScene()
    {
        SceneManager.LoadScene("MinaScene");
    }

    public void GoToGameScene()
    {
        SceneManager.LoadScene("hotaru_scene"); // ゲーム画面へ遷移
    }

    public void GoToResultScene()
    {
        SceneManager.LoadScene("ResultScene"); // 結果画面へ遷移
    }
}