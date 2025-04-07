using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class SocketClient : MonoBehaviour
{
    private static SocketClient _instance;
    public static SocketClient Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SocketClient");
                _instance = go.AddComponent<SocketClient>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _receiveThread;
    private bool _isConnected = false;
    private Action<string[]> _currentCallback;

    public void Connect(string serverIP, int port, Action<bool> onConnected = null)
    {
        try
        {
            _client = new TcpClient();
            _client.BeginConnect(serverIP, port, (ar) =>
            {
                try
                {
                    _client.EndConnect(ar);
                    _stream = _client.GetStream();
                    _isConnected = true;

                    // Start receive thread
                    _receiveThread = new Thread(new ThreadStart(ReceiveData));
                    _receiveThread.IsBackground = true;
                    _receiveThread.Start();

                    // Callback on main thread
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        onConnected?.Invoke(true);
                    });
                }
                catch (Exception e)
                {
                    Debug.LogError("Connection failed: " + e.Message);
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        onConnected?.Invoke(false);
                    });
                }
            }, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                onConnected?.Invoke(false);
            });
        }
    }

    public void Send(string jsonData, Action<string[]> callback)
    {
        if (!_isConnected)
        {
            Debug.LogError("Not connected to server");
            return;
        }

        _currentCallback = callback;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(jsonData);
            _stream.BeginWrite(data, 0, data.Length, (ar) =>
            {
                try
                {
                    _stream.EndWrite(ar);
                }
                catch (Exception e)
                {
                    Debug.LogError("Send error: " + e.Message);
                }
            }, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Send error: " + e.Message);
        }
    }

    private List<string> _pendingResponses = new List<string>(); // 存储未结束的响应片段

    private void ReceiveData()
    {
        byte[] buffer = new byte[4096];
        StringBuilder receivedData = new StringBuilder();

        try
        {
            while (_isConnected)
            {
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    // 连接已关闭
                    break;
                }

                string receivedString = Encoding.UTF8.GetString(buffer, 0, bytesRead).TrimEnd('\0');
                receivedData.Append(receivedString);

                // 检查是否收到完整 JSON（假设服务器返回独立 JSON 消息）
                string completeMessage = receivedData.ToString();
                receivedData.Clear();

                // 解析 JSON 判断是否结束
                bool isEnd = completeMessage.Contains("\"end\": \"true\""); // 假设 JSON 中有一个 "end" 字段来标识结束

                // 如果未结束，缓存当前片段
                if (!isEnd)
                {
                    _pendingResponses.Add(completeMessage);
                    continue;
                }

                // 如果结束，合并所有片段并触发回调
                _pendingResponses.Add(completeMessage); // 加入最后一个片段
                string[] fullResponse = _pendingResponses.ToArray(); // 转换为数组
                _pendingResponses.Clear(); // 清空缓存

                // 在主线程执行回调
                if (_currentCallback != null)
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {   
                        print( fullResponse);
                        _currentCallback.Invoke(fullResponse);
                        _currentCallback = null; // 清空回调（防止重复调用）
                    });
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Receive error: " + e.Message);
        }
        finally
        {
            Disconnect();
        }
    }




    public void Disconnect()
    {
        _isConnected = false;

        if (_receiveThread != null && _receiveThread.IsAlive)
        {
            _receiveThread.Abort();
        }

        if (_stream != null)
        {
            _stream.Close();
            _stream = null;
        }

        if (_client != null)
        {
            _client.Close();
            _client = null;
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}