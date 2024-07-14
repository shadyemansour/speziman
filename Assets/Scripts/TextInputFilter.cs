using TMPro;
using UnityEngine;

public class TextInputFilter : MonoBehaviour
{
    private TMP_InputField inputField;
    private System.Action onEscape;

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            onEscape?.Invoke();
        }

        // Continuously filter the input text
        if (inputField.isFocused)
        {
            inputField.text = FilterInputText(inputField.text);
        }
    }

    // This method will filter out any unwanted characters
    string FilterInputText(string text)
    {
        // Example filter: Remove any character that is not alphanumeric or space
        string filteredText = "";
        foreach (char c in text)
        {
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
            {
                filteredText += c;
            }
        }
        return filteredText;
    }

    public void SetOnEscape(System.Action action)
    {
        onEscape = action;
    }

}
