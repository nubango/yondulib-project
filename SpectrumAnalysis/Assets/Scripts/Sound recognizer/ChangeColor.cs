using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangeColor : MonoBehaviour
{
    public void Click()
    {
        Color32 idle = new Color32(255, 98, 0, 255);
        GetComponent<UnityEngine.UI.Image>().color = idle;
    }
}
