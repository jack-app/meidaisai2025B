using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    private Rigidbody2D rb;
    private bool isInitialized = false; // 初期化されたかどうかを示すフラグ

    void Awake() // Startより早くコンポーネントを取得するためにAwakeを使用することがあります
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("弾のPrefab '" + gameObject.name + "' に Rigidbody2D コンポーネントがアタッチされていません。");
            Destroy(gameObject); // Rigidbody2Dがない場合は動作しないためオブジェクトを破棄
        }
    }

    // EnemyShooterから呼び出される初期化メソッド
    public void InitializeDirection(Vector2 direction)
    {
        if (rb == null) return; // Rigidbody2D がなければ何もしない

        // 1. 弾の向きを発射方向に向ける
        // Vector2.zero の場合はエラーや警告を出すか、デフォルトの向きにする
        if (direction == Vector2.zero)
        {
            Debug.LogWarning("弾の方向がゼロベクトルです。進行方向を設定できません。");
            // transform.right (ローカルX軸正方向) をデフォルトの進行方向とするなど、フォールバック処理も可能
            // direction = transform.right; // この場合、Instantiate時の向きに依存する
            Destroy(gameObject); // 方向が不明な弾は破棄する例
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 2. 設定された方向に力を加える (速度を設定)
        rb.velocity = direction.normalized * speed; // 方向ベクトルを正規化してから速度を乗じる

        isInitialized = true; // 初期化完了

        // 一定時間後に弾を自動で消滅させる (初期化成功時のみ)
        Destroy(gameObject, lifetime);
    }

    void Start()
    {
        // InitializeDirection が Instantiate 直後に呼ばれることを期待しているため、
        // Start で isInitialized が false の場合、何らかの問題が発生している可能性がある。
        if (!isInitialized)
        {
            // EnemyShooter から InitializeDirection が呼ばれなかった場合の処理
            Debug.LogWarning("BulletController on '" + gameObject.name + "' was not initialized with a direction. Destroying bullet.");
            if(rb != null) rb.velocity = Vector2.zero; // 念のため停止
            Destroy(gameObject);
        }
    }

    // (任意) 弾が何かに衝突したときの処理 (変更なし)
    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     // ...
    // }
}
