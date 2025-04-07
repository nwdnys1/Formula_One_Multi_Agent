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

    private List<string> _pendingResponses = new List<string>(); // �洢δ��������ӦƬ��

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
                    // �����ѹر�
                    break;
                }

                string receivedString = Encoding.UTF8.GetString(buffer, 0, bytesRead).TrimEnd('\0');
                receivedData.Append(receivedString);

                // ����Ƿ��յ����� JSON��������������ض��� JSON ��Ϣ��
                string completeMessage = receivedData.ToString();
                receivedData.Clear();

                // ���� JSON �ж��Ƿ����
                bool isEnd = completeMessage.Contains("\"end\": \"true\""); // ���� JSON ����һ�� "end" �ֶ�����ʶ����

                // ���δ���������浱ǰƬ��
                if (!isEnd)
                {
                    _pendingResponses.Add(completeMessage);
                    continue;
                }

                // ����������ϲ�����Ƭ�β������ص�
                _pendingResponses.Add(completeMessage); // �������һ��Ƭ��
                string[] fullResponse = _pendingResponses.ToArray(); // ת��Ϊ����
                _pendingResponses.Clear(); // ��ջ���

                // �����߳�ִ�лص�
                if (_currentCallback != null)
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {   
                        print( fullResponse);
                        _currentCallback.Invoke(fullResponse);
                        _currentCallback = null; // ��ջص�����ֹ�ظ����ã�
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