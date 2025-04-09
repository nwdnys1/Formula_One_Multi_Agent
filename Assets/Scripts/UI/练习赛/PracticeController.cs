using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using DTO;
using LitJson;
using UnityEngine.UIElements;
using UnityEditor.Rendering;
public class PracticeController : MonoBehaviour
{
    InterviewUI dialogUI;
    SocketClient client = SocketClient.Instance;
    CameraManager cm = CameraManager.Instance;
    ParaManager paraManager = ParaManager.Instance;
    public UIDocument carParaUI;

    private void Awake()
    {
        dialogUI = GetComponent<InterviewUI>();
        // 确保DialogUI组件已正确设置
        if (dialogUI == null)
        {
            Debug.LogError("DialogUI is not assigned in the inspector.");
        }

        if (client == null)
        {
            Debug.LogError("SocketClient is not assigned in the inspector.");
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogUI.ShowOptions(new string[] { "开始练习赛" }, (index) =>
        {
            if (index == 0)
            {
                PracticeStart();
            }
        });
    }
    private void PracticeStart()
    {
        client.Send(JsonStr.practice_session_start, (response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // 处理服务器返回的JSON数据
            switch (json["sender"].ToString())
            {
                case "梅奔车队机械师":
                    foreach (JsonData tuning in json["tuning_data"])
                    {
                        Debug.Log(tuning.ToString());
                    }
                    break;
                case "汉密尔顿":
                    Debug.Log(json["tuning_data"].ToString());
                    break;
                default:
                    Debug.Log("未知场景类型");
                    break;
            }

        }, null);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
