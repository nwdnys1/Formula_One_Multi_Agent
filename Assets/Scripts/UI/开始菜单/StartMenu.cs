using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    private UIDocument _uiDocument;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();

        // 获取按钮引用
        var startButton = _uiDocument.rootVisualElement.Q<Button>("startButton");
        var settingsButton = _uiDocument.rootVisualElement.Q<Button>("settingsButton");
        var quitButton = _uiDocument.rootVisualElement.Q<Button>("quitButton");

        // 添加点击事件
        startButton.clicked += OnStartButtonClicked;
        settingsButton.clicked += OnSettingsButtonClicked;
        quitButton.clicked += OnQuitButtonClicked;
    }
    private void Update()
    {
        //按下ESC键回到开始菜单
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }
    }

    private void OnStartButtonClicked()
    {

        SceneManager.LoadScene("采访室");
    }

    private void OnSettingsButtonClicked()
    {
        Debug.Log("设置按钮被点击");
        // 在这里打开设置菜单
    }

    private void OnQuitButtonClicked()
    {
        Debug.Log("退出游戏按钮被点击");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}