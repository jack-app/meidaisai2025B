using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sannkaku : MonoBehaviour
{
    private player_moves ugoki;
    Vector3 mypositon;
    public Vector2 sankakuposition;

     public float radius = 1.5f;         // 円軌道の半径
    public float angularSpeed = 0;   // 度/秒
    private float angle = 0f;           // 現在の角度（度）
    private player_moves parentScript;

    // Start is called before the first frame update
    void Start()
    {
        parentScript = transform.parent.GetComponent<player_moves>();
       
        ugoki = parentScript.GetComponent<player_moves>();
        
    }

    // Update is called once per frame
    void Update()
    {
        mypositon = ugoki.playerpositions;
        Vector2 moveDir = ugoki.veloc2.normalized;
        // 進行方向が有効なときのみ回転
        if (moveDir.sqrMagnitude > 0.001f)
        {
            angle = angularSpeed;//* Time.deltaTime;
            float rad = angle * Mathf.Deg2Rad;

            // プレイヤーの進行方向（forward）とそれに直交するベクトル（right）
            Vector2 forward = moveDir;
            Vector2 right = new Vector2(-forward.y, forward.x); // 時計回り方向の直交ベクトル

            // offset = 前方×cos + 右方向×sin（回転円周上の位置）
            Vector2 offset = forward * Mathf.Cos(rad) + right * Mathf.Sin(rad);
            offset *= radius;


            // 三角形の位置をプレイヤーの周囲に配置
             transform.position = (Vector2)ugoki.playerpositions +  offset;
           
           
            sankakuposition = transform.position;


            // 三角形の「先端」が円周の方向を向くように（SpriteのY軸を正面にしておく）
            transform.up =  offset;
        }
        //Debug.Log("ミジュマル");
       // Debug.Log(ugoki.playerpositions);
        //Debug.Log(ugoki.veloc2);
        //transform.position = ugoki.playerpositions;

        
        
    }
}
