using UnityEngine;

public class move_mock_player : MonoBehaviour

{
    float speed = 3.0f;
 
    void Update()
    {
        // Wキー（前方移動）
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += speed * transform.up * Time.deltaTime;
        }
 
        // Sキー（後方移動）
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= speed * transform.up * Time.deltaTime;
        }
 
        // Dキー（右移動）
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += speed * transform.right * Time.deltaTime;
        }
 
        // Aキー（左移動）
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= speed * transform.right * Time.deltaTime;
        }
    }
}