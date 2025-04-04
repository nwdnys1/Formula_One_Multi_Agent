using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System;

public class DialogUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    [SerializeField] private float textSpeed = 0.05f; // 每个字符显示的速度(秒)

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

        // 获取UI元素
        characterNameLabel = root.Q<Label>("characterName");
        dialogueTextLabel = root.Q<Label>("dialogueText");
        playerInput = root.Q<TextField>("playerInput");
        submitButton = root.Q<Button>("submitButton");
        continueHint = root.Q<VisualElement>("continueHint");

        // 初始隐藏输入区域和继续提示
        SetInputVisible(false);
        SetContinueHintVisible(false);

        // 注册按钮点击事件
        submitButton.clicked += OnSubmitClicked;

        // 注册回车键提交
        playerInput.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
    }

    private void OnDisable()
    {
        submitButton.clicked -= OnSubmitClicked;
        playerInput.UnregisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
    }

    /// <summary>
    /// 显示对话
    /// </summary>
    /// <param name="characterName">角色名称</param>
    /// <param name="dialogueText">对话文本</param>
    /// <param name="onComplete">显示完成回调</param>
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
    /// 立即完成当前文本显示
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
    /// 显示玩家输入区域
    /// </summary>
    /// <param name="onSubmit">玩家提交文本时的回调</param>
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