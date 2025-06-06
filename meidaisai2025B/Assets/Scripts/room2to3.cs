using UnityEngine;

public class room2to3 : MonoBehaviour

{
    public Transform cameraTransform;
    [SerializeField] GameObject closed0;
    [SerializeField] GameObject closed1;
    [SerializeField] GameObject opened0;
    [SerializeField] GameObject opened1;

    [SerializeField] GamePartController GPController;

    bool firstTime = true;
    bool doorOpened = false;

    [SerializeField] AudioClip roomMoveSE;
    [SerializeField] AudioClip openDoorSE;

    void Start()
    {
        
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            Debug.LogWarning("cameraTransform が null だったので Camera.main から取得しました。");
        }
    }
    
    void Update()
    {
        if (GPController.roomNum == 1 && GPController.isStart == true && doorOpened == false &&
                GPController.enemiesInRoom[1] <= (GPController.killInRoom[1] * 2))
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
        Debug.Log("衝突しました！オブジェクト名: " + other.gameObject.name);

        if (other.CompareTag("2to3"))
        {
            Debug.Log("SwitchTrigger に衝突しました！座標を変更します。");
            transform.position = new Vector2(22, 10.5f);
            GPController.audioSource.PlayOneShot(roomMoveSE);

            if (cameraTransform != null)
            {
                Debug.Log("カメラの座標変更前: " + cameraTransform.position);
                cameraTransform.position = new Vector3(22, 14, cameraTransform.position.z);
                Debug.Log("カメラの座標変更後: " + cameraTransform.position);
            }
            else
            {
                Debug.LogError("cameraTransform が null です！カメラが正しく設定されているか確認してください。");
            }

            GPController.roomNum = 2;
            if (firstTime == true)
            {
                GPController.CountVisibleObjects();
                firstTime = false;
            }
        }

    }
}

