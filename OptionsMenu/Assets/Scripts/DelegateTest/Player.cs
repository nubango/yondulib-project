using UnityEngine;
using System.Collections;
public class Player : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            transform.Translate(2 * Time.deltaTime * Vector3.left);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(2 * Time.deltaTime * Vector3.right);
        if (Input.GetKey(KeyCode.W))
            transform.Translate(2 * Time.deltaTime * Vector3.up);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(2 * Time.deltaTime * Vector3.down);
    }
}
