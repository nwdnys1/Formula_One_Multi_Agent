using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class InterviewUI : MonoBehaviour
{
    [Header("角色UI文档")]
    public UIDocument WolffUI;
    public UIDocument HamUI;
    public UIDocument JournalistUI;  // journalistUI
    public UIDocument HornerUI;  // 其他角色...
    public UIDocument VerstappenUI;
    public UIDocument ReportUI;

    public VisualElement _currentRoot;
    private Label _dialogueText;
    private TextField _inputField;
    private Label _title;
    private VisualElement _optionsContainer;

    private System.Action<int> _optionCallback;
    private string _currentCharacter;

    private void Awake()
    {
        HideAll();
    }

    // 显示指定角色的UI
    public void ShowCharacterUI(string characterName)
    {
        // 先隐藏所有UI
        HideAll();

        // 根据角色名显示对应的UI
        switch (characterName)
        {
            case "Wolff":
                _currentRoot = WolffUI.rootVisualElement;
                _inputField = _currentRoot.Q<TextField>("Contents");
                break;
            case "Hamilton":
                _currentRoot = HamUI.rootVisualElement;
                break;
            case "Journalist":
                _currentRoot = JournalistUI.rootVisualElement;
                break;
            case "Horner":
                _currentRoot = HornerUI.rootVisualElement;
                break;
            case "Verstappen":
                _currentRoot = VerstappenUI.rootVisualElement;
                break;
            case "report":
                _currentRoot = ReportUI.rootVisualElement;
                break;
            default:
                Debug.LogWarning($"未找到角色UI: {characterName}");
                return;
        }

        _currentCharacter = characterName;
        _currentRoot.style.display = DisplayStyle.Flex;

        // 查找公共元素
        _dialogueText = _currentRoot.Q<Label>("Contents");
        _optionsContainer = _currentRoot.Q<VisualElement>("optionsContainer");
        _title = _currentRoot.Q<Label>("Title");
    }

    // 显示对话文本（所有UI通用）
    public void ShowDialogue(string text)
    {
        //print(_dialogueText);
        if (_dialogueText != null)
        {
            _dialogueText.text = text;
            _dialogueText.style.display = DisplayStyle.Flex;
        }
        else
        {
            _inputField.value = text;
            _inputField.style.display = DisplayStyle.Flex;
        }
    }

    // 隐藏对话文本
    public void HideDialogue()
    {
        if (_dialogueText != null)
        {
            _dialogueText.style.display = DisplayStyle.None;
        }
    }

    // 显示输入框（仅WolffUI可用）
    public void ShowInputField(string placeholder, System.Action<string> onSubmit)
    {
        if (_currentCharacter != "Wolff")
        {
            Debug.LogWarning("只有Wolff角色可以使用输入功能");
            return;
        }
        print(_inputField);
        if (_inputField != null)
        {
            _inputField.value = "";
            _inputField.Q(TextField.textInputUssName).Focus();
            _inputField.style.display = DisplayStyle.Flex;

            _inputField.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode == KeyCode.Return)
                {
                    print("输入内容: " + _inputField.value);
                    onSubmit?.Invoke(_inputField.value);
                    //HideInputField();
                }
            });
        }
    }

    // 隐藏输入框
    public void HideInputField()
    {
        if (_inputField != null)
        {
            _inputField.style.display = DisplayStyle.None;
        }
    }

    // 显示选项（所有UI通用）
    public void ShowOptions(string[] options, System.Action<int> callback)
    {
        if (_optionsContainer != null)
        {
            _optionsContainer.Clear();
            _optionCallback = callback;

            for (int i = 0; i < options.Length; i++)
            {
                int index = i;
                var button = new Button(() => OnOptionSelected(index))
                {
                    text = options[i]
                };
                _optionsContainer.Add(button);
            }

            _optionsContainer.style.display = DisplayStyle.Flex;
        }
    }

    private void OnOptionSelected(int index)
    {
        _optionCallback?.Invoke(index);
        HideOptions();
    }

    // 隐藏选项
    public void HideOptions()
    {
        if (_optionsContainer != null)
        {
            _optionsContainer.style.display = DisplayStyle.None;
        }
    }

    // 隐藏所有UI元素
    public void HideAll()
    {
        HideDialogue();
        HideInputField();
        HideOptions();

        // 隐藏所有角色UI
        if (WolffUI != null) WolffUI.rootVisualElement.style.display = DisplayStyle.None;
        if (HamUI != null) HamUI.rootVisualElement.style.display = DisplayStyle.None;
        if (JournalistUI != null) JournalistUI.rootVisualElement.style.display = DisplayStyle.None;
        if (HornerUI != null) HornerUI.rootVisualElement.style.display = DisplayStyle.None;
        if (VerstappenUI != null) VerstappenUI.rootVisualElement.style.display = DisplayStyle.None;
        if (ReportUI != null) ReportUI.rootVisualElement.style.display = DisplayStyle.None;
    }
}