using UnityEngine;
using System.Collections;

public class DoorSwitch : MonoBehaviour
{

    public delegate void Contact();
    public event Contact OnContact;

    private void OnTriggerEnter(Collider other)
    {
        if (OnContact != null)
            OnContact();
    }

}
