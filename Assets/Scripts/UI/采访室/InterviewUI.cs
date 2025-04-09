using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class InterviewUI : MonoBehaviour
{
    [Header("��ɫUI�ĵ�")]
    public UIDocument WolffUI;
    public UIDocument HamUI;
    public UIDocument JournalistUI;  // ����UI
    public UIDocument HornerUI;  // ������ɫ...
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

    // ��ʾָ����ɫ��UI
    public void ShowCharacterUI(string characterName)
    {
        // ����������UI
        HideAll();

        // ���ݽ�ɫ����ʾ��Ӧ��UI
        switch (characterName)
        {
            case "Wolff":
                _currentRoot = WolffUI.rootVisualElement;
                _inputField = _currentRoot.Q<TextField>("Contents");
                break;
            case "���ܶ���":
                _currentRoot = HamUI.rootVisualElement;
                break;
            case "����":
                _currentRoot = JournalistUI.rootVisualElement;
                break;
            case "����":
                _currentRoot = HornerUI.rootVisualElement;
                break;
            case "ά˹����":
                _currentRoot = VerstappenUI.rootVisualElement;
                break;
            case "report":
                _currentRoot = ReportUI.rootVisualElement;
                break;
            default:
                Debug.LogWarning($"δ�ҵ���ɫUI: {characterName}");
                return;
        }

        _currentCharacter = characterName;
        _currentRoot.style.display = DisplayStyle.Flex;

        // ���ҹ���Ԫ��
        _dialogueText = _currentRoot.Q<Label>("Contents");
        _optionsContainer = _currentRoot.Q<VisualElement>("optionsContainer");
        _title = _currentRoot.Q<Label>("Title");
    }

    // ��ʾ�Ի��ı�������UIͨ�ã�
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

    // ���ضԻ��ı�
    public void HideDialogue()
    {
        if (_dialogueText != null)
        {
            _dialogueText.style.display = DisplayStyle.None;
        }
    }

    // ��ʾ����򣨽�WolffUI���ã�
    public void ShowInputField(string placeholder, System.Action<string> onSubmit)
    {
        if (_currentCharacter != "Wolff")
        {
            Debug.LogWarning("ֻ��Wolff��ɫ����ʹ�����빦��");
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
                    print("��������: " + _inputField.value);
                    onSubmit?.Invoke(_inputField.value);
                    //HideInputField();
                }
            });
        }
    }

    // ���������
    public void HideInputField()
    {
        if (_inputField != null)
        {
            _inputField.style.display = DisplayStyle.None;
        }
    }

    // ��ʾѡ�����UIͨ�ã�
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

    // ����ѡ��
    public void HideOptions()
    {
        if (_optionsContainer != null)
        {
            _optionsContainer.style.display = DisplayStyle.None;
        }
    }

    // ��������UIԪ��
    public void HideAll()
    {
        HideDialogue();
        HideInputField();
        HideOptions();

        // �������н�ɫUI
        if (WolffUI != null) WolffUI.rootVisualElement.style.display = DisplayStyle.None;
        if (HamUI != null) HamUI.rootVisualElement.style.display = DisplayStyle.None;
        if (JournalistUI != null) JournalistUI.rootVisualElement.style.display = DisplayStyle.None;
        if (HornerUI != null) HornerUI.rootVisualElement.style.display = DisplayStyle.None;
        if (VerstappenUI != null) VerstappenUI.rootVisualElement.style.display = DisplayStyle.None;
        if (ReportUI != null) ReportUI.rootVisualElement.style.display = DisplayStyle.None;
    }
}