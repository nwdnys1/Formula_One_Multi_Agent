using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DTO;
using LitJson;
using UnityEngine.UIElements;

public class InterviewController : MonoBehaviour
{
    DialogUI dialogUI;
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
        dialogUI = GetComponent<DialogUI>();
        // 确保DialogUI组件已正确设置
        if (dialogUI == null)
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
            dialogUI.ShowDialogue(json["sender"] + ":" + json["content"]);
            // 切换摄像机
            cm.SetCamera(cameras[json["sender"].ToString()]);
        }, (r) =>
        { InterviewEnd(); });

    }
    private void InterviewEnd()
    {
        client.Send(JsonStr.media_interview_end, (string response) =>
        {
            print("response:" + response);
            JsonData json = JsonMapper.ToObject(response);
            // 处理服务器返回的JSON数据
            dialogUI.ShowDialogue(json["news_article"].ToString());
            // 切换摄像机
            cm.SetCamera(quanjing);
            driverParaUI.enabled = true;
            dialogUI.HideAll();
        }, null);
    }
    private void Update()
    {

    }
}