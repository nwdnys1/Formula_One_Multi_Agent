using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class SocketManager : MonoBehaviour
{

    ParaManager paraManager = ParaManager.Instance;
    UIDocument _uiDocument;
    [Serializable]
    public class SceneData
    {
        public string sender;
        public string receiver;
        public string content;
    }

    [Serializable]
    public class RequestData
    {
        public string scene_type;
        public SceneData scene_data;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



        _uiDocument = GetComponent<UIDocument>();
        RequestData requestData = new RequestData
        {
            scene_type = "media_interview",
            scene_data = new SceneData
            {
                sender = "system",
                receiver = "记者",
                content = "现在是赛前的媒体采访时间，请向维斯塔潘进行简要的提问。"
            }
        };
        string request = JsonUtility.ToJson(requestData);
        SocketClient.Instance.Send(request, (response) =>
        {
            foreach (var item in response)
            {
                Debug.Log("Server response: " + item);
            }

            paraManager.getDriverPara("1号").overallRate = 1f;
        });

    }

    // Update is called once per frame
    void Update()
    {

    }


}
