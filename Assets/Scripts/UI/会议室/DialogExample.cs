using UnityEngine;

public class DialogExample : MonoBehaviour
{
    DialogUI dialogUI;

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
        dialogUI.ShowDialogue("你好，我是春日野悠。最近过得怎么样？");
        // 显示玩家输入框
        dialogUI.ShowInputField("请输入你的回答...", OnPlayerResponse);
    }

    private void OnPlayerResponse(string playerText)
    {
        // 处理玩家输入
        Debug.Log("玩家说: " + playerText);

        // 继续下一段对话
        dialogUI.ShowOptions(new string[] { "我很好，谢谢！", "有点忙..." }, (input) =>
        {
            Debug.Log("玩家选择了选项" + input);
        });
    }


    private void Update()
    {

    }
}