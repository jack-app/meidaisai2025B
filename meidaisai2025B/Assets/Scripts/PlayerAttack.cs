using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Sword2;
    public Vector2 spawnposition;
    private player_moves ugoki2;
    Vector3 mypositon;
    public Vector2 sankakuposition;

    public float radius = 1.5f;         // 円軌道の半径
    public float angularSpeed = 0;   // 度/秒
    private float angle = 0f;           // 現在の角度（度）
    private player_moves parentScript;
    //親オブジェクトから持ってくる
    public float rotationSpeed = 360f; // 回転速度（度/秒）
    private bool isRotating = false;

    [SerializeField] private float slashDuration = 0.3f;  // 斬撃にかかる時間（秒）
    [SerializeField] private float slashAngle = 120f;     // 斬撃の回転角度（度）。正の値で反時計回り、負の値で時計回りを想定
    private bool isSlashing = false;    // 斬撃中かどうかのフラグ

    [SerializeField] private GameObject triangle;

    // Start is called before the first frame update
    void Start()
    {
        parentScript = transform.parent.GetComponent<player_moves>();

        //playerInput = transform.parent.GetComponent<PlayerInput>();

        ugoki2 = parentScript.GetComponent<player_moves>();
        //このスクリプトは子オブジェクトにつけて親オブジェクトからデータを取ってくる
    }

    // Update is called once per frame
    public void OnFire()
    {

        Debug.Log("剣が出現");

        // コルーチン開始
        if (!isSlashing)
            StartCoroutine(SwordSlash());

    }

    void FixedUpdate()
    {
        if(!isSlashing)
        SwordFollow();
    }

    public void SwordFollow()
    {
        // プレイヤーの現在位置と進行方向を取得
        Vector2 playerPos = ugoki2.playerpositions;
        Vector2 moveDir = ugoki2.veloc2.normalized;

        Vector2 playerDir = (Vector2)triangle.transform.position-playerPos;
        float moveDirAngleRad = Mathf.Atan2(playerDir.y, playerDir.x);

        //if (moveDir.sqrMagnitude > 0.001f) // プレイヤーが動いている場合
        //{
            // プレイヤーの進行方向の角度（ラジアン）
            //float moveDirAngleRad = Mathf.Atan2(moveDir.y, moveDir.x);

            // 剣の待機角度を計算 (プレイヤー進行方向から -slashAngle / 2 度回転)
            float targetIdleAngleRad = moveDirAngleRad - (slashAngle / 2f * Mathf.Deg2Rad);

            // 新しいオフセットベクトルを計算
            Vector2 offset = new Vector2(Mathf.Cos(targetIdleAngleRad), Mathf.Sin(targetIdleAngleRad)) * radius;

            // 剣の位置を更新
            transform.position = playerPos + offset;
            sankakuposition = transform.position; // これはデバッグ用か何かの変数でしょうか。更新しておきます。

            // 剣の「先端」がプレイヤーからのオフセット方向を向くように（SpriteのY軸を正面）
            transform.up = offset.normalized;

            // Z軸回転を45度加える (元の SwordFollow の仕様)
            transform.Rotate(0, 0, 45);
        //}
        /*
        else
        {
            // プレイヤーが静止している場合の剣の位置維持ロジック
            // (例: 最後に設定されたオフセットを維持するか、特定の位置に固定する)
            // 現在はプレイヤーが静止していると SwordFollow による位置更新は行われません。
            // 必要であれば、静止時の剣の振る舞いをここに追加してください。
            // 例えば、現在の剣の位置をプレイヤーの周りに維持し続ける場合：
            if (ugoki2 != null && ugoki2.playerpositions != null) // nullチェックを追加
            {
                Vector2 currentOffset = (Vector2)transform.position - playerPos;
                if (currentOffset.sqrMagnitude > 0.001f) // 既にオフセットがある場合
                {
                    transform.position = playerPos + currentOffset.normalized * radius;
                    transform.up = currentOffset.normalized;
                    transform.Rotate(0, 0, 45); // 角度も再適用
                }
                // else 完全に重なっている場合は何もしないか、デフォルト位置へ
            }
        }*/
    }

    public IEnumerator SwordSlash()
    {
        isSlashing = true;
        Debug.Log("SwordSlash 開始");

        Vector2 playerCurrentPosAtSlashStart = ugoki2.playerpositions; // 開始時のプレイヤー位置を一時保持
        Vector2 initialOffsetToPlayer = ((Vector2)transform.position - playerCurrentPosAtSlashStart).normalized;

        // 斬撃開始時のプレイヤーに対する剣の角度（度単位）
        float startAngleDeg = Mathf.Atan2(initialOffsetToPlayer.y, initialOffsetToPlayer.x) * Mathf.Rad2Deg;

        float elapsedTime = 0f;
        while (elapsedTime < slashDuration)
        {
            Vector2 playerPos = ugoki2.playerpositions;

            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / slashDuration); // 進行度 (0から1)

            // 現在の回転掃引角度 (0度から目標のslashAngleまで)
            float currentSweepAngleDeg = Mathf.Lerp(0, slashAngle, progress);

            // プレイヤーから見た剣の現在の絶対角度
            float currentAbsoluteAngleDeg = startAngleDeg + currentSweepAngleDeg;
            float currentAbsoluteAngleRad = currentAbsoluteAngleDeg * Mathf.Deg2Rad;

            // 新しいオフセットベクトルを計算
            Vector2 newOffset = new Vector2(Mathf.Cos(currentAbsoluteAngleRad), Mathf.Sin(currentAbsoluteAngleRad)) * radius;

            // 剣の位置を更新
            transform.position = playerPos + newOffset;

            // 剣の向きを更新
            // 斬撃の進行方向（円弧の接線方向）を計算
            Vector2 tangentDirection;

            if (slashAngle >= 0) // 角度が増加する方向（反時計回りを想定）
            {
                tangentDirection = new Vector2(-newOffset.y, newOffset.x).normalized;
                transform.up = tangentDirection;
                transform.Rotate(0f, 0f, -45f);
            }
            else // 角度が減少する方向（時計回りを想定）
            {
                tangentDirection = new Vector2(newOffset.y, -newOffset.x).normalized;
                transform.up = tangentDirection;
                transform.Rotate(0f, 0f, 45f);
            }

            yield return null; // 1フレーム待機
        }

        Debug.Log("SwordSlash 終了");
        isSlashing = false;

        // 斬撃終了後、剣の向きと位置をUpdate()のロジックに即座に合わせるため、
        // Update()内のロジックを一度ここで実行するか、
        // 次のUpdate()フレームで自然に更新されるのを待つ。
        // ここでは後者（何もしない）を選択。必要なら調整してください。
    }
    
    //剣の振るモーション
    /*private IEnumerator SwordSlash()
    {
        Vector2 DistanceV = transform.parent.position - transform.position;
        float distance = DistanceV.magnitude;

        Debug.Log("Distance:" + distance);

        isRotating = true;
        Debug.Log("isRotating on " + isRotating);

        // 現在の回転を保存（四元数）
        Quaternion originalRotation = transform.rotation;

        //transform.position += new Vector3(distance*Mathf.Sin(pi/3), distance*Mathf.Cos(pi/3));

        // Z軸方向に60度回転（加算）
        //transform.Rotate(0f, 0f, 120f);

        // 2秒待機
        yield return new WaitForSeconds(1f);

        // 元の回転に戻す
        transform.rotation = originalRotation;

        isRotating = false;
        Debug.Log("isRotating off " + isRotating);

    

    }*/
}
