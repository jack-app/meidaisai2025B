using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    Transform playerTr; // プレイヤーのTransform
    [SerializeField] float speed = 2; // 敵の動くスピード

    [SerializeField]
    private Animator anim;

    public float ChaseRange = 6.0f;     // この距離に入るとプレイヤーを追いかける
    public float ChaseEndRange = 4.0f;  // この距離より近く(fleeRangeより遠い)で追跡を停止し射撃
    public float fleeRange = 2.0f;      // この距離に入るとプレイヤーから逃げる

    public bool randamWalk = true; // 範囲外でランダムウォークをするか

    [SerializeField]
    private float randomSpan = 2.0f; // ランダムウォークの方向転換間隔

    private float currentRandomWaitTime = 0.0f;
    private float randomMoveX, randomMoveY;

    [Header("Shooting Settings")]
    [SerializeField]
    private GameObject shootedPrefab; // 射撃する弾のPrefab
    [SerializeField]
    private Transform firePoint;      // 弾の発射位置 (指定がなければ自身のTransformを使用)
    [SerializeField]
    private float fireRate = 1.0f;    // 1秒あたりの発射数
    private float nextFireTime = 0f;  // 次に射撃可能な時間

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTr = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found. Make sure your player GameObject has the 'Player' tag.");
            enabled = false;
            return;
        }

        if (anim == null)
        {
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogWarning("Animator component not found or not assigned on " + gameObject.name + ". Animations will not play properly.");
            }
        }

        if (shootedPrefab == null)
        {
            Debug.LogWarning("Shooted Prefab not assigned on " + gameObject.name + ". Enemy will not be able to shoot.");
        }
        if (firePoint == null)
        {
            firePoint = transform; // firePointが未設定なら自身の位置を発射地点とする
        }

        currentRandomWaitTime = randomSpan;
    }

    void Update()
    {
        if (playerTr == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTr.position);
        Vector2 directionToPlayer = (playerTr.position - transform.position).normalized;

        if (distanceToPlayer <= fleeRange) // 1. 逃走範囲内か？
        {
            // --- 逃走処理 ---
            Vector2 fleeDirection = (transform.position - playerTr.position).normalized;
            transform.position = Vector2.MoveTowards(
                transform.position,
                transform.position + (Vector3)fleeDirection * 100,
                0.7f * speed * Time.deltaTime
            );

            if (anim != null)
            {
                anim.speed = speed / 4.0f;
                anim.SetFloat("X", fleeDirection.x);
                anim.SetFloat("Y", fleeDirection.y);
            }
        }
        else if (distanceToPlayer <= ChaseEndRange) // 2. 射撃・停止範囲内か？ (fleeRangeよりは遠い)
        {
            // --- 停止し、プレイヤーの方向を向き、射撃 ---
            // 停止
            if (anim != null)
            {
                anim.speed = 0f; // アニメーションの再生を停止
            }

            // プレイヤーの方向を向く (アニメーターパラメータで制御)
            if (anim != null)
            {
                anim.SetFloat("X", directionToPlayer.x);
                anim.SetFloat("Y", directionToPlayer.y);
                // 注意: anim.speed = 0 の状態でSetFloatがアニメーションの見た目に即時反映されるかは
                // Animator Controllerの設定によります。
                // もし向きが変わらない場合、anim.speedを極小にするか、
                // transform.rotationを直接操作するなどの対応が必要になることがあります。
            }
            else // アニメーターがない場合は直接GameObjectの向きを変える (2Dトップダウン用例)
            {
                 float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f; // スプライトが上向き基準の場合-90度
                 transform.rotation = Quaternion.Euler(0, 0, angle);
            }


            // 射撃 (クールダウン考慮)
            TryShoot(directionToPlayer);
        }
        else if (distanceToPlayer <= ChaseRange) // 3. 追跡範囲内か？ (ChaseEndRangeよりは遠い)
        {
            // --- プレイヤー追跡処理 ---
            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTr.position,
                speed * Time.deltaTime
            );

            if (anim != null)
            {
                anim.speed = speed / 4.0f;
                anim.SetFloat("X", directionToPlayer.x);
                anim.SetFloat("Y", directionToPlayer.y);
            }
        }
        else // 4. 索敵範囲外
        {
            // --- ランダムウォークまたは待機 ---
            HandleRandomWalk();
        }
    }

    void TryShoot(Vector2 directionToPlayer) // directionToPlayer は EnemyShooter が計算済み
    {
        if (shootedPrefab == null) return;

        if (Time.time >= nextFireTime)
        {
            // 弾を生成 (回転はデフォルトの Quaternion.identity で、向きはまだ設定されていない)
            GameObject bulletInstance = Instantiate(shootedPrefab, firePoint.position, Quaternion.identity);

            // 生成した弾から BulletController コンポーネントを取得
            BulletController bulletCtrl = bulletInstance.GetComponent<BulletController>();

            if (bulletCtrl != null)
            {
                // BulletController の初期化メソッドを呼び出し、発射方向を渡す
                bulletCtrl.InitializeDirection(directionToPlayer);
            }
            else
            {
                Debug.LogError("射撃Prefab '" + shootedPrefab.name + "' に BulletController スクリプトがアタッチされていません。");
                Destroy(bulletInstance); // BulletController がないと弾が機能しないため破棄
                return; // 射撃失敗
            }

            nextFireTime = Time.time + 1f / fireRate;
        }
}

    void HandleRandomWalk()
    {
        if (randamWalk)
        {
            currentRandomWaitTime += Time.deltaTime;
            if (currentRandomWaitTime >= randomSpan)
            {
                randomMoveX = Random.Range(-1, 2);
                randomMoveY = Random.Range(-1, 2);
                currentRandomWaitTime = 0.0f;
            }

            Vector2 targetPosition = new Vector2(transform.position.x + randomMoveX * 100, transform.position.y + randomMoveY * 100);
            if (randomMoveX == 0 && randomMoveY == 0)
            {
                targetPosition = transform.position;
            }

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            if (anim != null)
            {
                if (randomMoveX != 0 || randomMoveY != 0)
                {
                    anim.speed = speed / 4.0f;
                    anim.SetFloat("X", randomMoveX);
                    anim.SetFloat("Y", randomMoveY);
                }
                else
                {
                    anim.speed = 0.0f;
                }
            }
        }
        else
        {
            if (anim != null)
            {
                anim.speed = 0.0f;
            }
        }
    }
}
