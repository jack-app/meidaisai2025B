using UnityEngine;

public class room1to2 : MonoBehaviour

{
    public Transform cameraTransform; // カメラの Transform を指定

    void Start()
    {
        // カメラがインスペクターで設定されていない場合、自動で取得
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            Debug.LogWarning("cameraTransform が null だったので Camera.main から取得しました。");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("衝突しました！オブジェクト名: " + other.gameObject.name); // 衝突を確認

        if (other.CompareTag("1to2")) // 衝突対象のオブジェクトのタグをチェック
        {
            Debug.Log("SwitchTrigger に衝突しました！座標を変更します。");

            // プレイヤーの位置を変更
            transform.position = new Vector2(14.5f, 0);

            // カメラの位置を変更
            if (cameraTransform != null)
            {
                Debug.Log("カメラの座標変更前: " + cameraTransform.position);
                cameraTransform.position = new Vector3(22, 0, cameraTransform.position.z);
                Debug.Log("カメラの座標変更後: " + cameraTransform.position);
            }
            else
            {
                Debug.LogError("cameraTransform が null です！カメラが正しく設定されているか確認してください。");
            }
        }
    }
}