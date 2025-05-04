using UnityEngine;

public class SocketInit : MonoBehaviour
{
    private void Awake()
    {
        var dispatcher = UnityMainThreadDispatcher.Instance;
        var socketClient = SocketClient.Instance;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // 连接到服务器
        SocketClient.Instance.Connect("47.101.153.28", 8887, (success) =>
        {
            if (success)
            {
                Debug.Log("Connected to server!");


            }
            else
            {
                Debug.LogError("Failed to connect to server");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
