// InitializeCore.cs
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class InitializeCore
{
    private const string DEFAULT_SCENE = "开始菜单";
    private const string INITIALIZATION_TAG = "[Initialization]";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Debug.Log($"{INITIALIZATION_TAG} 开始系统初始化...");

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
        Cursor.lockState = CursorLockMode.None;// 解锁鼠标

        if (SceneManager.GetActiveScene().name == DEFAULT_SCENE)
            return;

        if (!Application.CanStreamedLevelBeLoaded(DEFAULT_SCENE))
        {
            Debug.LogError($"{INITIALIZATION_TAG} 错误：默认场景 {DEFAULT_SCENE} 未添加到Build Settings！");
            return;
        }

        Debug.Log($"{INITIALIZATION_TAG} 正在加载默认场景 {DEFAULT_SCENE}...");
        SceneManager.LoadScene(DEFAULT_SCENE);
    }

    private static void InitializeSystems()
    {
        // 在这里添加其他需要在游戏开始前初始化的系统
        // 例如：InputSystem、AudioSystem、存档系统等
    }
}