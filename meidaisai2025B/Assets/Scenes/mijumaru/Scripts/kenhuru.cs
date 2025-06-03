using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class kenhuru : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        parentScript = transform.parent.GetComponent<player_moves>();

       
        ugoki2 = parentScript.GetComponent<player_moves>();
       //このスクリプトは子オブジェクトにつけて親オブジェクトからデータを取ってくる
    }

    // Update is called once per frame
    private void OnFire()
    {
        
        Debug.Log("件が出現");

         // コルーチン開始
    StartCoroutine(Kenmodosu());




    }

    void Update()
    {
        //[剣がプレイヤーの周りを回るようにする]//
        mypositon = ugoki2.playerpositions;
        Vector2 moveDir = ugoki2.veloc2.normalized;
        // 進行方向が有効なときのみ回転
        if (moveDir.sqrMagnitude > 0.001f)
        {
            angle = angularSpeed;//* Time.deltaTime;
            float rad = angle * Mathf.Deg2Rad;

            // プレイヤーの進行方向（forward）とそれに直交するベクトル（right）
            Vector2 forward = moveDir;
            Vector2 right = new Vector2(-forward.y, forward.x); // 時計回り方向の直交ベクトル

            // offset = 前方×cos + 右方向×sin（回転円周上の位置）
            //Vector2 offset = forward * Mathf.Cos(rad) + right * Mathf.Sin(rad);
            Vector2 offset = forward;
            offset *= radius;


            // 三角形の位置をプレイヤーの周囲に配置
            transform.position = (Vector2)ugoki2.playerpositions + offset;
            sankakuposition = transform.position;


            // 三角形の「先端」が円周の方向を向くように（SpriteのY軸を正面にしておく）
            transform.up = offset;

        }
        

    }


    //剣の振るモーション
    private IEnumerator Kenmodosu()
    {
       
    isRotating = true;
    Debug.Log("isRotating on " + isRotating);

    // 現在の回転を保存（四元数）
    Quaternion originalRotation = transform.rotation;

    // Z軸方向に60度回転（加算）
    transform.Rotate(0f, 0f, 120f);

    // 2秒待機
    yield return new WaitForSeconds(1f);

    // 元の回転に戻す
    transform.rotation = originalRotation;

    isRotating = false;
    Debug.Log("isRotating off " + isRotating);

    

}
}
