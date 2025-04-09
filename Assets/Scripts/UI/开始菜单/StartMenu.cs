using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    private UIDocument _uiDocument;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();

        // ��ȡ��ť���� - �����µ�UXML�ṹ
        var startButton = _uiDocument.rootVisualElement.Q<Label>("Start");
        var quitButton = _uiDocument.rootVisualElement.Q<Label>("Quit");
        if (startButton == null || quitButton == null)
        {
            Debug.LogError("��ťδ�ҵ�������UXML�ļ��е������Ƿ���ȷ��");
            return;
        }
        // ��ӵ���¼� - ����ʹ��Label��RegisterCallback
        startButton.RegisterCallback<ClickEvent>(OnStartButtonClicked);
        quitButton.RegisterCallback<ClickEvent>(OnQuitButtonClicked);
    }

    private void Update()
    {
        
    }

    private void OnStartButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene("�ɷ���");
    }

    private void OnQuitButtonClicked(ClickEvent evt)
    {
        Debug.Log("�˳���Ϸ��ť�����");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}