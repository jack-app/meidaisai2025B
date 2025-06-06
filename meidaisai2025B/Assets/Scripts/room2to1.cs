using UnityEngine;

public class room2to1 : MonoBehaviour

{
    public Transform cameraTransform;
    [SerializeField] GamePartController GPController;

    [SerializeField] AudioClip roomMoveSE;

    void Start()
    {
        
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            Debug.LogWarning("cameraTransform が null だったので Camera.main から取得しました。");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("衝突しました！オブジェクト名: " + other.gameObject.name);

        if (other.CompareTag("2to1"))
        {
            Debug.Log("SwitchTrigger に衝突しました！座標を変更します。");
            transform.position = new Vector2(7.0f, 0);
            GPController.audioSource.PlayOneShot(roomMoveSE);


            if (cameraTransform != null)
            {
                Debug.Log("カメラの座標変更前: " + cameraTransform.position);
                cameraTransform.position = new Vector3(0, 0, cameraTransform.position.z);
                Debug.Log("カメラの座標変更後: " + cameraTransform.position);
            }
            else
            {
                Debug.LogError("cameraTransform が null です！カメラが正しく設定されているか確認してください。");
            }
            
            GPController.roomNum = 0;
        }

    }
}
