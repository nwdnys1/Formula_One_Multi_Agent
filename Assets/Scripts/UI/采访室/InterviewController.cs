using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DTO;
using LitJson;
using UnityEngine.UIElements;

public class InterviewController : MonoBehaviour
{
    InterviewDialog dialog;
    SocketClient client = SocketClient.Instance;
    CameraManager cm = CameraManager.Instance;
    public UIDocument driverParaUI;
    public CinemachineVirtualCamera reporterCamera;
    public CinemachineVirtualCamera hornerCamera;
    public CinemachineVirtualCamera verstappenCamera;
    public CinemachineVirtualCamera hamiltonCamera;
    public CinemachineVirtualCamera wolffCamera;
    public CinemachineVirtualCamera quanjing;
    public Dictionary<string, CinemachineVirtualCamera> cameras = new Dictionary<string, CinemachineVirtualCamera>();

    private void Awake()
    {
        dialog = GetComponent<InterviewDialog>();
        // 确保DialogUI组件已正确设置
        if (dialog == null)
        {
            Debug.LogError("DialogUI is not assigned in the inspector.");
        }
        if (driverParaUI == null)
        {
            Debug.LogError("DriverParaUI is not assigned in the inspector.");
        }
        if (client == null)
        {
            Debug.LogError("SocketClient is not assigned in the inspector.");
        }
        cameras.Add("记者", reporterCamera);
        cameras.Add("霍纳", hornerCamera);
        cameras.Add("维斯塔潘", verstappenCamera);
        cameras.Add("汉密尔顿", hamiltonCamera);
        cameras.Add("Wolff", wolffCamera);
    }

    private void Start()
    {
        // 全景视角
        cm.SetCamera(quanjing);

        // 开始采访
        InterviewStart();

    }

    private void InterviewStart()
    {
        client.Send(JsonStr.media_interview_start, (response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // 处理服务器返回的JSON数据
            dialog.ShowCharacterUI(json["sender"].ToString());
            dialog.ShowDialogue(json["content"].ToString());
            // 切换摄像机
            cm.SetCamera(cameras[json["sender"].ToString()]);
        }, (r) =>
        { InterviewChat(); });

    }
    private void InterviewChat()
    {
        cm.SetCamera(cameras["Wolff"]);
        dialog.ShowCharacterUI("Wolff");
        dialog.ShowInputField("请输入采访内容", (input) =>
        {
            string sendStr = JsonStr.media_interview_chat(input);
            // 发送输入的内容到服务器
            client.Send(sendStr, (response) =>
            {
                JsonData json = JsonMapper.ToObject(response);
                // 处理服务器返回的JSON数据
                dialog.ShowCharacterUI(json["sender"].ToString());
                dialog.ShowDialogue(json["content"].ToString());
                // 切换摄像机
                cm.SetCamera(cameras[json["sender"].ToString()]);
            }, (r) =>
            { InterviewEnd(); });
        });

    }
    private void InterviewEnd()
    {
        client.Send(JsonStr.media_interview_end, (string response) =>
        {
            print("response:" + response);
            JsonData json = JsonMapper.ToObject(response);
            // 处理服务器返回的JSON数据
            dialog.ShowCharacterUI("report");
            JsonData news = json["news_article"];
            dialog._currentRoot.Q<Label>("Title").text = news["title"].ToString();
            dialog._currentRoot.Q<Label>("Contents").text = news["content"].ToString();

        }, null);
    }
    private void Update()
    {

    }
}