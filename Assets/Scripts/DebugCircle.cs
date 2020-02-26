using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class DebugCircle : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int value;
    // Start is called before the first frame update
    public void Update()
    {
        text.text = value.ToString();
    }
}
