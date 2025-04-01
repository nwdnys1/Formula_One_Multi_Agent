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

        // ��ȡ��ť����
        var startButton = _uiDocument.rootVisualElement.Q<Button>("startButton");
        var settingsButton = _uiDocument.rootVisualElement.Q<Button>("settingsButton");
        var quitButton = _uiDocument.rootVisualElement.Q<Button>("quitButton");

        // ��ӵ���¼�
        startButton.clicked += OnStartButtonClicked;
        settingsButton.clicked += OnSettingsButtonClicked;
        quitButton.clicked += OnQuitButtonClicked;
    }
    private void Update()
    {
        //����ESC���ص���ʼ�˵�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }
    }

    private void OnStartButtonClicked()
    {

        SceneManager.LoadScene("�ɷ���");
    }

    private void OnSettingsButtonClicked()
    {
        Debug.Log("���ð�ť�����");
        // ����������ò˵�
    }

    private void OnQuitButtonClicked()
    {
        Debug.Log("�˳���Ϸ��ť�����");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}