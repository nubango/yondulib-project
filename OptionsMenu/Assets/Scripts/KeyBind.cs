using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBind : MonoBehaviour
{
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    //struct InputInfo
    //{
    //    int id; // ID unico e interno
    //    string name; // nombre descriptivo para la accion
    //    delegate void inputDelegate(); // delegate que se llama con la accion
    //}

    public Text up, left, down, right, jump;

    private GameObject currentKey;

    private Color32 idle = new Color32(255, 98, 0, 255);
    private Color32 selected = new Color32(255, 198, 18, 255);

    void Start()
    {
        // [W,A,S,D,Space] preset
        keys.Add("Up", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", "W"))); // "UpArrow" -> KeyCode.UpArrow
        keys.Add("Down", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down", "A")));
        keys.Add("Left", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", "S")));
        keys.Add("Right", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", "D")));
        keys.Add("Jump", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump", "Space")));

        up.text = keys["Up"].ToString(); // KeyCode.W == "W"
        down.text = keys["Down"].ToString();
        left.text = keys["Left"].ToString();
        right.text = keys["Right"].ToString();
        jump.text = keys["Jump"].ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(keys["Up"]))
        {
            // Logica del juego
            Debug.Log("Up");
        }
        if (Input.GetKeyDown(keys["Down"]))
        {
            Debug.Log("Down");
        }
        if (Input.GetKeyDown(keys["Left"]))
        {
            Debug.Log("Left");
        }
        if (Input.GetKeyDown(keys["Right"]))
        {
            Debug.Log("Right");
        }
        if (Input.GetKeyDown(keys["Jump"]))
        {
            Debug.Log("Jump");
        }
    }

    private void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                keys[currentKey.name] = e.keyCode;
                currentKey.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                currentKey.GetComponent<Image>().color = idle;
                currentKey = null;
            }
        }
    }

    public void ChangeKey(GameObject clicked)
    {
        if (currentKey != null)
            currentKey.GetComponent<Image>().color = idle;

        currentKey = clicked;
        currentKey.GetComponent<Image>().color = selected;
    }

    public void SaveKeys()
    {
        foreach (var key in keys)
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }

        PlayerPrefs.Save();
    }
}
