using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{

    public DoorSwitch doorSwitch;

    void Start()
    {
        doorSwitch.OnContact += Open;
    }

    private void Open()
    {
        transform.Translate(Vector3.up * 3f);
    }

    void OnDisable()
    {
        doorSwitch.OnContact -= Open;
    }

}
