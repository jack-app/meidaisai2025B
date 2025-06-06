using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI操作に必要

public class PlayerHealth : MonoBehaviour
{
    // プレイヤーの最大HPと現在のHP
    public int maxHp = 300;
    private int currentHp;
    private int a;

    public int GetcurrentHp()
    {
        return currentHp;
    }
    // スライダーの参照
    public Slider hpSlider;

    void Start()
    {
        // 初期設定
        currentHp = maxHp; // HPを最大値に設定
        hpSlider.maxValue = maxHp; // スライダーの最大値を設定
        hpSlider.value = currentHp; // 現在のHPを反映    
    }

    public void TakeDamage(int damage)
    {
        // HPを減らす処理
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;

        // スライダーに現在のHPを反映
        hpSlider.value = currentHp;

        // HPが0になったときの処理
        if (currentHp == 0)
        {
            Debug.Log("ゲームオーバー！");
            // ここにゲームオーバーの処理を追加
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // スペースでダメージ判定の条件
        {
            a = Random.Range(1,100);
            TakeDamage(a); // ランダムでHPを減らす
            Debug.Log(a);
        }
    }
}