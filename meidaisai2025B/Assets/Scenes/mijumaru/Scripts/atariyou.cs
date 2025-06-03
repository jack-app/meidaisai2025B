using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
//敵に取り付けます
//敵のステータスと攻撃判定のプログラム
public class atariyou : MonoBehaviour
{
    //ここに敵のステータスを入力
    [SerializeField] public float HP_enemy = 100, ATK_enemy = 10, SPD_enemy = 10;
    //敵を倒すと獲得するスコア
    [SerializeField] private float SCORE=100;
    
        public player_moves player_Moves;


    // Start is called before the first frame update
    //Collider2D　にしないと反応しないため注意
    //OnTriggerEnter2DにするかOnTriggerStay2Dにするか変更可能
    private void OnTriggerEnter2D(Collider2D other)
    {
        //"atarutag"で自分の攻撃を相手に当たったかを行っている.

        if (other.CompareTag("atarutag"))
        {
            // プレイヤーがこのオブジェクトに触れたときの処理
            Debug.Log("Player has entered the trigger area.ミジュ");
            // ここにプレイヤーが触れたときの処理を追加
            //player_moves player_Moves = GetComponent<player_moves>();
            if (player_Moves != null)
            {
                HP_enemy -= player_Moves.ATK_player;
                Debug.Log("敵の残りHPは" + HP_enemy);
            }
            else
            {
                Debug.LogWarning("player_moves component not found on this GameObject.");
            }

        }
        Debug.Log("ぶつかった");
    }

    void Start()
    {
        // 初期化処理が必要ならここに記述
    }

    // Update is called once per frame
    void Update()
    {
        if (HP_enemy <= 0)
        {
            Destroy(gameObject);
            // Scorekanri　+=SCORE;
            //スコア入れるならここ
        }

    }
}
