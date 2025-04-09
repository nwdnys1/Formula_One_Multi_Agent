using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    private UIDocument _uiDocument;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();

        // 获取按钮引用 - 根据新的UXML结构
        var startButton = _uiDocument.rootVisualElement.Q<Label>("Start");
        var quitButton = _uiDocument.rootVisualElement.Q<Label>("Quit");
        if (startButton == null || quitButton == null)
        {
            Debug.LogError("按钮未找到，请检查UXML文件中的名称是否正确。");
            return;
        }
        // 添加点击事件 - 现在使用Label的RegisterCallback
        startButton.RegisterCallback<ClickEvent>(OnStartButtonClicked);
        quitButton.RegisterCallback<ClickEvent>(OnQuitButtonClicked);
    }

    private void Update()
    {
        
    }

    private void OnStartButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene("采访室");
    }

    private void OnQuitButtonClicked(ClickEvent evt)
    {
        Debug.Log("退出游戏按钮被点击");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}