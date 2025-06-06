using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector2 vec = new Vector2(3.0f, 3.0f);

        Rigidbody2D rigidbody = this.GetComponent<Rigidbody2D>();
        rigidbody.velocity = vec;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
