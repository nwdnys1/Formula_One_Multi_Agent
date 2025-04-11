using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class EndUI : MonoBehaviour
{
    [Header("角色UI文档")]
    public UIDocument WolffUI;
    public UIDocument HamUI;
    public UIDocument StrategistUI;
    public UIDocument MechanicUI;
    public UIDocument ReportUI;

    public VisualElement _currentRoot;
    private Label _dialogueText;
    public Label _titleText;
    private TextField _inputField;
    private Button toMechanic;
    private Button toStrategist;
    private Button toHamilton;

    private void Awake()
    {
        // 初始化时隐藏所有UI
        HideAll();
    }
    private void Start()
    {
        // 查找按钮
        toMechanic = WolffUI.rootVisualElement.Q<Button>("Mech");
        toStrategist = WolffUI.rootVisualElement.Q<Button>("Strat");
        toHamilton = WolffUI.rootVisualElement.Q<Button>("Ham");

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
            case "Mechanic":
                _currentRoot = MechanicUI.rootVisualElement;
                break;
            case "Strategist":
                _currentRoot = StrategistUI.rootVisualElement;
                break;
            case "report":
                _currentRoot = ReportUI.rootVisualElement;
                _titleText = _currentRoot.Q<Label>("Title");
                break;
            default:
                Debug.LogWarning($"未找到角色UI: {characterName}");
                return;
        }

        _currentRoot.style.display = DisplayStyle.Flex;

        // 查找公共元素
        _dialogueText = _currentRoot.Q<Label>("Contents");
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



    // 隐藏所有UI元素
    public void HideAll()
    {
        HideDialogue();


        // 隐藏所有角色UI
        if (WolffUI != null) WolffUI.rootVisualElement.style.display = DisplayStyle.None;
        if (HamUI != null) HamUI.rootVisualElement.style.display = DisplayStyle.None;
        if (StrategistUI != null) StrategistUI.rootVisualElement.style.display = DisplayStyle.None;
        if (MechanicUI != null) MechanicUI.rootVisualElement.style.display = DisplayStyle.None;
        if (ReportUI != null) ReportUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    // 显示输入框（仅WolffUI可用）
    public void ShowInputField(string placeholder, System.Action<string> onSubmit)
    {

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
    // 显示输入框 但是不是回车触发 而是按钮触发
    public void ShowInputFieldByButton(
        string placeholder,
        System.Action<string> onMechanic, // 对应toMechanic
        System.Action<string> onStrategist, // 对应toStrategist
        System.Action<string> onHamilton  // 对应toHamilton
    )
    {
        if (_inputField == null) return;

        // 初始化输入框
        _inputField.value = "";
        _inputField.label = placeholder;
        _inputField.style.display = DisplayStyle.Flex;
        _inputField.Focus();

        // 绑定按钮事件
        toMechanic.clicked += () => OnButtonPressed(onMechanic);
        toStrategist.clicked += () => OnButtonPressed(onStrategist);
        toHamilton.clicked += () => OnButtonPressed(onHamilton);
    }

    private void OnButtonPressed(System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(_inputField.value))
        {
            Debug.LogWarning("输入内容不能为空");
            return;
        }

        // 执行回调并隐藏输入框
        callback?.Invoke(_inputField.value);
        HideInputField();
    }

    // 隐藏输入框
    public void HideInputField()
    {
        if (_inputField != null)
        {
            _inputField.style.display = DisplayStyle.None;
        }
    }
}