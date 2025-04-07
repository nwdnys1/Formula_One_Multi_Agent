using UnityEngine;

public class SocketInit : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    public int serverPort = 8888;
    private void Awake()
    {
        var dispatcher = UnityMainThreadDispatcher.Instance;
        var socketClient = SocketClient.Instance;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // 连接到服务器
        SocketClient.Instance.Connect("127.0.0.1", 8888, (success) =>
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
