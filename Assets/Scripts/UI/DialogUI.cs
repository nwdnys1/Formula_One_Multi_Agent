using UnityEngine;
using UnityEngine.UIElements;

public class DialogUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _root;
    private Label _dialogueText;
    private TextField _inputField;
    private VisualElement _optionsContainer;

    private System.Action<int> _optionCallback;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _root = _uiDocument.rootVisualElement;

        _dialogueText = _root.Q<Label>("dialogueText");
        _inputField = _root.Q<TextField>("inputField");
        _optionsContainer = _root.Q<VisualElement>("optionsContainer");

        HideAll();
    }

    public void ShowDialogue(string text)
    {
        _dialogueText.text = text;
        _dialogueText.style.display = DisplayStyle.Flex;
    }

    public void HideDialogue()
    {
        _dialogueText.style.display = DisplayStyle.None;
    }

    public void ShowInputField(string placeholder, System.Action<string> onSubmit)
    {
        _inputField.value = "";
        _inputField.Q(TextField.textInputUssName).Focus();
        //_inputField.placeholder = placeholder;
        _inputField.style.display = DisplayStyle.Flex;

        _inputField.RegisterCallback<KeyDownEvent>(e =>
        {
            if (e.keyCode == KeyCode.Return)
            {
                onSubmit?.Invoke(_inputField.value);
                HideInputField();
            }
        });
    }

    public void HideInputField()
    {
        _inputField.style.display = DisplayStyle.None;
    }

    public void ShowOptions(string[] options, System.Action<int> callback)
    {
        _optionsContainer.Clear();
        _optionCallback = callback;

        for (int i = 0; i < options.Length; i++)
        {
            int index = i; // ±Õ°ü²¶»ñ
            var button = new Button(() => OnOptionSelected(index))
            {
                text = options[i]
            };
            _optionsContainer.Add(button);
        }

        _optionsContainer.style.display = DisplayStyle.Flex;
    }

    private void OnOptionSelected(int index)
    {
        _optionCallback?.Invoke(index);
        HideOptions();
    }

    public void HideOptions()
    {
        _optionsContainer.style.display = DisplayStyle.None;
    }

    public void HideAll()
    {
        HideDialogue();
        HideInputField();
        HideOptions();
    }
}