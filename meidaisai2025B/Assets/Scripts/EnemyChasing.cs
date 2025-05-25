using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChasing : MonoBehaviour
{
    Transform playerTr; // プレイヤーのTransform
    [SerializeField] float speed = 2; // 敵の動くスピード

    [SerializeField]
    private Animator anim;
    public bool useAnim = true;

    public float ChaseRange = 6.0f;

    public bool randamWalk = true;

    [SerializeField]
    private float randomSpan = 2.0f;

    private float time = 5.0f;

    private float x, y;

    private void Start()
    {
        // プレイヤーのTransformを取得（プレイヤーのタグをPlayerに設定必要）
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // プレイヤーとの距離が0.1f未満になったらそれ以上実行しない
        if (Vector2.Distance(transform.position, playerTr.position) > ChaseRange)
        {

            if (randamWalk == true)
            {
                time += Time.deltaTime;
                if (time >= randomSpan)
                {
                    x = Random.Range(-1, 2);
                    y = Random.Range(-1, 2);

                    time = 0.0f;
                }
                

                transform.position = Vector2.MoveTowards(
                    transform.position,
                    new Vector2(transform.position.x + x*100, transform.position.y + y*100),
                    speed * Time.deltaTime
                );

                anim.speed = speed / 4.0f;
                anim.SetFloat("X", x);
                anim.SetFloat("Y", y);
            }
            else
            {
                anim.speed = 0.0f;
            }
            
            return;
        }
        else
        {
            // プレイヤーに向けて進む
            transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(playerTr.position.x, playerTr.position.y),
            speed * Time.deltaTime);

            anim.speed = speed / 4.0f;
            Vector2 direction = playerTr.position - transform.position;
            anim.SetFloat("X", direction.x);
            anim.SetFloat("Y", direction.y);
        }
    }
}
