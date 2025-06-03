using UnityEngine;
using TMPro;
//敵キャラダメージ
public class DamagePopup : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float lifeTime = 1f;
    private TextMeshProUGUI text;
    public atariyou Atariyou;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void Setup(int damage)
    {
        //damege = Atariyou.
        text.text = damage + " ダメージ";
        Destroy(gameObject, lifeTime); // 一定時間後に消す
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }
}
