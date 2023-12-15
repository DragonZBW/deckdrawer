using UnityEngine;
using TMPro;

public class ErrorLog : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Color logColor;
    [SerializeField] private Color errorColor;

    public static ErrorLog inst;

    private void Awake()
    {
        inst = this;
        Hide();
    }

    public void Log(string message)
    {
        text.color = logColor;
        Show(message);
    }

    public void Error(string message)
    {
        text.color = errorColor;
        Show(message);
    }

    private void Show(string message)
    {
        text.text = message;
        gameObject.SetActive(true);
        CancelInvoke();
        Invoke("Hide", 8);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
