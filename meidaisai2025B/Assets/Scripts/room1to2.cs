using UnityEngine;

public class room1to2 : MonoBehaviour

{
    public Transform cameraTransform; // カメラの Transform を指定
    [SerializeField] GameObject closed0;
    [SerializeField] GameObject closed1;
    [SerializeField] GameObject opened0;
    [SerializeField] GameObject opened1;

    [SerializeField] GamePartController GPController;

    private bool firstTime = true;
    private bool doorOpened = false;

    [SerializeField] AudioClip roomMoveSE;
    [SerializeField] AudioClip openDoorSE;

    void Start()
    {
        // カメラがインスペクターで設定されていない場合、自動で取得
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            Debug.LogWarning("cameraTransform が null だったので Camera.main から取得しました。");
        }
    }

    void Update()
    {
        if (GPController.roomNum == 0 && GPController.isStart == true && doorOpened == false &&
                GPController.enemiesInRoom[0] <= (GPController.killInRoom[0] * 2))
        {
            closed0.SetActive(false);
            closed1.SetActive(false);
            opened0.SetActive(true);
            opened1.SetActive(true);
            GPController.audioSource.PlayOneShot(openDoorSE);
            doorOpened = true;  
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("衝突しました！オブジェクト名: " + other.gameObject.name); // 衝突を確認

        if (other.CompareTag("1to2")) // 衝突対象のオブジェクトのタグをチェック
        {
            Debug.Log("SwitchTrigger に衝突しました！座標を変更します。");

            // プレイヤーの位置を変更
            transform.position = new Vector2(15.0f, 0);
            GPController.audioSource.PlayOneShot(roomMoveSE);

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

            GPController.roomNum = 1;
            if (firstTime == true)
            {
                GPController.CountVisibleObjects();
                firstTime = false;
            }
        }
    }
}