using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public delegate void FilePickerDelegate(string filepath);

public class FilePicker : MonoBehaviour
{
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_InputField inputField;

    private FilePickerDelegate callback;

    public void Open(string instructionText, FilePickerDelegate callback)
    {
        this.instructionText.text = instructionText;
        this.callback = callback;
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }

    public void Accept()
    {
        callback(inputField.text);
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        callback = null;
        gameObject.SetActive(false);
    }
}
