using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// これを親オブジェクトにつけると子オブジェクトも含む Collider2D を可視化する
public class ColliderVisualizer2D : MonoBehaviour
{
    Dictionary<Collider2D, GameObject> dict = new Dictionary<Collider2D, GameObject>();

    private GameObject CreatePrimitive2D(PrimitiveType type)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        Destroy(obj.GetComponent<Collider>());
        return obj;
    }

    void UpdatePrimitive(GameObject primitive, CircleCollider2D collider)
    {
        primitive.transform.localPosition = (Vector3)collider.offset;
        primitive.transform.localScale = new Vector3(collider.radius * 2f, collider.radius * 2f, 1f);
    }

    void UpdatePrimitive(GameObject primitive, BoxCollider2D collider)
    {
        primitive.transform.localPosition = (Vector3)collider.offset;
        primitive.transform.localScale = new Vector3(collider.size.x, collider.size.y, 1f);
    }

    void UpdatePrimitive(GameObject primitive, CapsuleCollider2D collider)
    {
        primitive.transform.localPosition = (Vector3)collider.offset;

        Vector2 size = collider.size;
        primitive.transform.localScale = new Vector3(size.x, size.y, 1f);

        if (collider.direction == CapsuleDirection2D.Horizontal)
            primitive.transform.rotation = Quaternion.Euler(0, 0, 90);
        else
            primitive.transform.rotation = Quaternion.identity;
    }

    void Awake()
    {
        var colliders = GetComponentsInChildren<Collider2D>();

        foreach (var collider in colliders)
        {
            if (!collider.enabled) continue;

            GameObject primitive;

            if (collider is CircleCollider2D)
            {
                primitive = CreatePrimitive2D(PrimitiveType.Sphere);
                primitive.transform.SetParent(collider.transform, false);
                UpdatePrimitive(primitive, collider as CircleCollider2D);
            }
            else if (collider is BoxCollider2D)
            {
                primitive = CreatePrimitive2D(PrimitiveType.Cube);
                primitive.transform.SetParent(collider.transform, false);
                UpdatePrimitive(primitive, collider as BoxCollider2D);
            }
            else if (collider is CapsuleCollider2D)
            {
                primitive = CreatePrimitive2D(PrimitiveType.Capsule);
                primitive.transform.SetParent(collider.transform, false);
                UpdatePrimitive(primitive, collider as CapsuleCollider2D);
            }
            else
            {
                continue;
            }

            var material = primitive.GetComponent<Renderer>().material;
            material.shader = Shader.Find("Sprites/Default");
            primitive.GetComponent<MeshRenderer>().material.color = new Color(0.3f, 0.8f, 0.3f, 0.3f);

            dict.Add(collider, primitive);
        }
    }

    void Update()
    {
        foreach (var kv in dict)
        {
            Collider2D collider = kv.Key;
            GameObject primitive = kv.Value;

            if (collider is CircleCollider2D)
            {
                UpdatePrimitive(primitive, collider as CircleCollider2D);
            }
            else if (collider is BoxCollider2D)
            {
                UpdatePrimitive(primitive, collider as BoxCollider2D);
            }
            else if (collider is CapsuleCollider2D)
            {
                UpdatePrimitive(primitive, collider as CapsuleCollider2D);
            }
        }
    }
}
