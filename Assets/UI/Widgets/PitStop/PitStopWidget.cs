using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    // 引用到UIDocument组件
    public UIDocument uiDocument;

    // 要显示的数字
    private int displayNumber = 100;

    // 在UI Toolkit中引用的Label元素
    private Label numberLabel;

    private void Start()
    {
        var root = uiDocument.rootVisualElement;

        // 找到Label元素
        numberLabel = root.Q<Label>("numberLabel");

        // 找到按钮并绑定点击事件
        var increaseButton = root.Q<Button>("increaseButton");
        var decreaseButton = root.Q<Button>("decreaseButton");

        if (increaseButton != null)
        {
            increaseButton.clicked += IncrementNumber;
        }

        if (decreaseButton != null)
        {
            decreaseButton.clicked += DecrementNumber;
        }

        // 初始化显示数字
        UpdateDisplay();
    }

    // 更新显示的数字
    public void UpdateDisplay()
    {
        // 更新Label的文本内容
        if (numberLabel != null)
        {
            numberLabel.text = displayNumber.ToString();
        }
    }

    // 增加数字并更新显示
    public void IncrementNumber()
    {
        displayNumber++;
        UpdateDisplay();
    }

    // 减少数字并更新显示
    public void DecrementNumber()
    {
        displayNumber--;
        UpdateDisplay();
    }
}