using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System;

public class DialogUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    [SerializeField] private float textSpeed = 0.05f; // ÿ���ַ���ʾ���ٶ�(��)

    private VisualElement root;
    private Label characterNameLabel;
    private Label dialogueTextLabel;
    private TextField playerInput;
    private Button submitButton;
    private VisualElement continueHint;

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private Action<string> onPlayerSubmit;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        root = _uiDocument.rootVisualElement;

        // ��ȡUIԪ��
        characterNameLabel = root.Q<Label>("characterName");
        dialogueTextLabel = root.Q<Label>("dialogueText");
        playerInput = root.Q<TextField>("playerInput");
        submitButton = root.Q<Button>("submitButton");
        continueHint = root.Q<VisualElement>("continueHint");

        // ��ʼ������������ͼ�����ʾ
        SetInputVisible(false);
        SetContinueHintVisible(false);

        // ע�ᰴť����¼�
        submitButton.clicked += OnSubmitClicked;

        // ע��س����ύ
        playerInput.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
    }

    private void OnDisable()
    {
        submitButton.clicked -= OnSubmitClicked;
        playerInput.UnregisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
    }

    /// <summary>
    /// ��ʾ�Ի�
    /// </summary>
    /// <param name="characterName">��ɫ����</param>
    /// <param name="dialogueText">�Ի��ı�</param>
    /// <param name="onComplete">��ʾ��ɻص�</param>
    public void ShowDialogue(string characterName, string dialogueText, Action onComplete = null)
    {
        characterNameLabel.text = characterName;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(dialogueText, onComplete));
    }

    /// <summary>
    /// ������ɵ�ǰ�ı���ʾ
    /// </summary>
    public void CompleteCurrentText()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueTextLabel.text = characterNameLabel.text + ": " + dialogueTextLabel.text;
            isTyping = false;
            SetContinueHintVisible(true);
        }
    }

    /// <summary>
    /// ��ʾ�����������
    /// </summary>
    /// <param name="onSubmit">����ύ�ı�ʱ�Ļص�</param>
    public void ShowPlayerInput(Action<string> onSubmit)
    {
        onPlayerSubmit = onSubmit;
        playerInput.value = "";
        SetInputVisible(true);
        playerInput.Focus();
    }

    private IEnumerator TypeText(string text, Action onComplete)
    {
        isTyping = true;
        SetContinueHintVisible(false);
        dialogueTextLabel.text = "";

        foreach (char c in text)
        {
            dialogueTextLabel.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
        SetContinueHintVisible(true);
        onComplete?.Invoke();
    }

    private void OnSubmitClicked()
    {
        SubmitPlayerInput();
    }

    private void OnInputKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
        {
            SubmitPlayerInput();
        }
    }

    private void SubmitPlayerInput()
    {
        if (!string.IsNullOrEmpty(playerInput.value))
        {
            onPlayerSubmit?.Invoke(playerInput.value);
            SetInputVisible(false);
        }
    }

    private void SetInputVisible(bool visible)
    {
        playerInput.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        submitButton.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void SetContinueHintVisible(bool visible)
    {
        continueHint.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}