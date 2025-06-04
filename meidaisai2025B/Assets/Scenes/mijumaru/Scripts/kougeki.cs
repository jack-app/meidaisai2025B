using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class kougeki : MonoBehaviour
{
    [SerializeField] private float attacktime = 2.0f;
    PolygonCollider2D atarihanteicol;
    
    // Start is called before the first frame update
    void Start()
    {
        atarihanteicol = GetComponent<PolygonCollider2D>();
        atarihanteicol.enabled = false;
        Debug.Log("開始時攻撃判定オフ");
        Debug.Log(atarihanteicol);
    }

    // Update is called once per frame
    void OnFire()
    {

        StartCoroutine(AttackCoroutine());

    }

    private IEnumerator AttackCoroutine()
    {
       
        
        atarihanteicol.enabled = true;
        Debug.Log("攻撃判定開始");
        yield return new WaitForSeconds(attacktime);  // ここで2秒待つ
        atarihanteicol.enabled = false;
        Debug.Log("攻撃判定オフ");
    }
    void Update()
    {

    }
   
    
}
