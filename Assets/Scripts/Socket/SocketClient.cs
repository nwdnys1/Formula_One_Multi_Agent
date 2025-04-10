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
    private Action<string> _currentCallback;
    private Action<string> _nextCallback;

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

    public void Send(object jsonData, Action<string> callback, Action<string> nextCallback)
    {
        Send(JsonUtility.ToJson(jsonData), callback, nextCallback);
    }

    public void Send(string jsonData, Action<string> callback, Action<string> nextCallback)
    {
        if (!_isConnected)
        {
            Debug.LogError("Not connected to server");
            return;
        }

        _currentCallback = callback;
        _nextCallback = nextCallback;

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



    private void ReceiveData()
    {
        byte[] buffer = new byte[4096];

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
                print("received:" + receivedString); // 打印接收到的字符串

                bool isEnd = receivedString.Contains("finally_end"); // 判断是否为结束标志

                if (isEnd)
                {
                    _currentCallback = _nextCallback;
                    _nextCallback = null;

                }
                // 在主线程执行回调
                if (_currentCallback != null)
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {

                        _currentCallback.Invoke(receivedString);

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