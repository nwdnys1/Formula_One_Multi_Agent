// InitializeCore.cs
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class InitializeCore
{
    private const string DEFAULT_SCENE = "��ʼ�˵�";
    private const string INITIALIZATION_TAG = "[Initialization]";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Debug.Log($"{INITIALIZATION_TAG} ��ʼϵͳ��ʼ��...");

#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            return;
#endif

        EnsureDefaultSceneLoaded();
        InitializeSystems();
    }

    private static void EnsureDefaultSceneLoaded()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;// �������

        if (SceneManager.GetActiveScene().name == DEFAULT_SCENE)
            return;

        if (!Application.CanStreamedLevelBeLoaded(DEFAULT_SCENE))
        {
            Debug.LogError($"{INITIALIZATION_TAG} ����Ĭ�ϳ��� {DEFAULT_SCENE} δ��ӵ�Build Settings��");
            return;
        }

        Debug.Log($"{INITIALIZATION_TAG} ���ڼ���Ĭ�ϳ��� {DEFAULT_SCENE}...");
        SceneManager.LoadScene(DEFAULT_SCENE);
    }

    private static void InitializeSystems()
    {
        // ���������������Ҫ����Ϸ��ʼǰ��ʼ����ϵͳ
        // ���磺InputSystem��AudioSystem���浵ϵͳ��
    }
}