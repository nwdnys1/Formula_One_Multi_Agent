using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class MeetingUI : MonoBehaviour
{
    [Header("��ɫUI�ĵ�")]
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
        // ��ʼ��ʱ��������UI
        HideAll();
    }
    private void Start()
    {
        // ���Ұ�ť
        toMechanic = WolffUI.rootVisualElement.Q<Button>("Mech");
        toStrategist = WolffUI.rootVisualElement.Q<Button>("Strat");
        toHamilton = WolffUI.rootVisualElement.Q<Button>("Ham");

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
            case "÷�����ӻ�еʦ":
                _currentRoot = MechanicUI.rootVisualElement;
                break;
            case "÷�����Ӳ���ʦ":
                _currentRoot = StrategistUI.rootVisualElement;
                break;
            case "report":
                _currentRoot = ReportUI.rootVisualElement;
                _titleText = _currentRoot.Q<Label>("Title");
                break;
            default:
                Debug.LogWarning($"δ�ҵ���ɫUI: {characterName}");
                return;
        }

        _currentRoot.style.display = DisplayStyle.Flex;

        // ���ҹ���Ԫ��
        _dialogueText = _currentRoot.Q<Label>("Contents");
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



    // ��������UIԪ��
    public void HideAll()
    {
        HideDialogue();


        // �������н�ɫUI
        if (WolffUI != null) WolffUI.rootVisualElement.style.display = DisplayStyle.None;
        if (HamUI != null) HamUI.rootVisualElement.style.display = DisplayStyle.None;
        if (StrategistUI != null) StrategistUI.rootVisualElement.style.display = DisplayStyle.None;
        if (MechanicUI != null) MechanicUI.rootVisualElement.style.display = DisplayStyle.None;
        if (ReportUI != null) ReportUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    // ��ʾ����򣨽�WolffUI���ã�
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
                    print("��������: " + _inputField.value);
                    onSubmit?.Invoke(_inputField.value);
                    //HideInputField();
                }
            });
        }
    }
    // ��ʾ����� ���ǲ��ǻس����� ���ǰ�ť����
    public void ShowInputFieldByButton(
        string placeholder,
        System.Action<string> onMechanic, // ��ӦtoMechanic
        System.Action<string> onStrategist, // ��ӦtoStrategist
        System.Action<string> onHamilton  // ��ӦtoHamilton
    )
    {
        if (_inputField == null) return;

        // ��ʼ�������
        _inputField.value = "";
        _inputField.label = placeholder;
        _inputField.style.display = DisplayStyle.Flex;
        _inputField.Focus();

        // �󶨰�ť�¼�
        toMechanic.clicked += () => OnButtonPressed(onMechanic);
        toStrategist.clicked += () => OnButtonPressed(onStrategist);
        toHamilton.clicked += () => OnButtonPressed(onHamilton);
    }

    private void OnButtonPressed(System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(_inputField.value))
        {
            Debug.LogWarning("�������ݲ���Ϊ��");
            return;
        }

        // ִ�лص������������
        callback?.Invoke(_inputField.value);
        HideInputField();
    }

    // ���������
    public void HideInputField()
    {
        if (_inputField != null)
        {
            _inputField.style.display = DisplayStyle.None;
        }
    }
}