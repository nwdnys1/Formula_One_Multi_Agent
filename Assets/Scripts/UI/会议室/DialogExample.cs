using UnityEngine;

public class DialogExample : MonoBehaviour
{
    private DialogUI dialogUI;

    private void Awake()
    {
        dialogUI = GetComponent<DialogUI>();
        // 确保DialogUI组件已正确设置
        if (dialogUI == null)
        {
            Debug.LogError("DialogUI is not assigned in the inspector.");
        }
    }

    private void Start()
    {
        // 显示NPC对话
        dialogUI.ShowDialogue("春日野悠", "你好，我是春日野悠。最近过得怎么样？", () =>
        {
            // 对话显示完成后显示玩家输入
            dialogUI.ShowPlayerInput(OnPlayerResponse);
        });
    }

    private void OnPlayerResponse(string playerText)
    {
        // 处理玩家输入
        Debug.Log("玩家说: " + playerText);

        // 继续下一段对话
        dialogUI.ShowDialogue("春日野悠", "我明白了。那么你觉得这个游戏怎么样？", () =>
        {
            dialogUI.ShowPlayerInput(OnSecondPlayerResponse);
        });
    }

    private void OnSecondPlayerResponse(string playerText)
    {
        Debug.Log("玩家第二次回应: " + playerText);
        // 可以继续对话链...
    }

    private void Update()
    {
        // 点击鼠标左键快速完成当前文本显示
        if (Input.GetMouseButtonDown(0))
        {
            dialogUI.CompleteCurrentText();
        }
    }
}