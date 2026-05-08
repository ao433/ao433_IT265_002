using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => Debug.Log("BUTTON CLICKED!"));
            Debug.Log("Test button added to " + gameObject.name);
        }
    }
}