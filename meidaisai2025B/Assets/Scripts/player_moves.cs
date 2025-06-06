using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class player_moves : MonoBehaviour
{
    
    //bool cooltimes = false;
    // int amari = 0;

    [SerializeField] public float HP_player = 30, ATK_player = 10, SPD_player = 10;
    //public atariyou Atariyou;
    
    /*ここでplayerのステータスを入力
    ほかから持ってくる場合は
    HP_player= HP;
    ATK_player= ATK;
    SPD_player= SPD;
    みたいに代入を適切なところでする
    */
    private Vector2 velocity_;
    public Vector2 veloc2;
    private Rigidbody2D rb;

    public Vector3 playerpositions;

    private PlayerAttack playerAttack;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerpositions = transform.position;

        playerAttack = transform.GetChild(0).GetComponent<PlayerAttack>();

        //0 ダッシュ使える

    }
    private void OnMove(InputValue value)
    {
        var axis = value.Get<Vector2>().normalized;
        velocity_ = new Vector2(axis.x * SPD_player, axis.y * SPD_player);
        veloc2 = new Vector2(velocity_.x, velocity_.y);
        //Debug.Log("Move input: " + axis);
    }
    private void OnDash()
    {

        rb.AddForce(velocity_ * 50 * SPD_player);

        //cooltimes = true;

    }

    private void OnFire()
    {
        playerAttack.OnFire();
    }


    void Update()
    {
        if (HP_player <= 0)
        {
            Debug.Log("ゲームオーバー");
            SPD_player = 0;//スピード値を0にして動けない様にしてる

            //ゲームオーバー時
            //何か起こしたいならここ
        }



    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = velocity_;
        playerpositions = transform.position;

        // transform.position += new Vector3(velocity_.x, velocity_.y, 0) * Time.deltaTime;
    }
      private void OnCollisionStay2D(Collision2D collision)
    {
        //"Enemy"タグで自分の攻撃を相手に当たったかを行っている.
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // プレイヤーがこのオブジェクトに触れたときの処理
            //Debug.Log("Player has entered the trigger area.フタチマル");
            // ここにプレイヤーが触れたときの処理を追加
            //Atariyouは敵のステータス参照
            atariyou Atariyou = collision.gameObject.GetComponent<atariyou>();
            if (Atariyou != null)
            {
                HP_player -= Atariyou.ATK_enemy;
                Debug.Log("自分の残りHPは" + HP_player);
            }
            else
            {
                Debug.LogWarning("player_moves component not found on this GameObject.");
            }

        }
        //Debug.Log("ぶつかった");
    }
     
}
