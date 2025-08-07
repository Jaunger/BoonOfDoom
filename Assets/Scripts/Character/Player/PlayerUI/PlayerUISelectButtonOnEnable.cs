using UnityEngine;
using UnityEngine.UI;

public class PlayerUISelectButtonOnEnable : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.Select();
            //button.OnSelect(null);
        }
    }
}
