using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DTO;
using LitJson;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.WSA;
using Cursor = UnityEngine.Cursor;

public class MeetingController : MonoBehaviour
{
    MeetingUI dialog;
    SocketClient client = SocketClient.Instance;
    CameraManager cm = CameraManager.Instance;
    public UIDocument driverParaUI;
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera strategistCamera;
    public CinemachineVirtualCamera mechanicCamera;
    public CinemachineVirtualCamera hamiltonCamera;
    public CinemachineVirtualCamera wolffCamera;
    public CinemachineVirtualCamera quanjing;
    public Dictionary<string, CinemachineVirtualCamera> cameras = new Dictionary<string, CinemachineVirtualCamera>();
    public ChairController chair;
    public GameObject StandWolff;
    public GameObject SitWolff;

    private void Awake()
    {
        dialog = GetComponent<MeetingUI>();
        // 确保DialogUI组件已正确设置
        if (dialog == null)
        {
            Debug.LogError("DialogUI is not assigned in the inspector.");
        }
        if (driverParaUI == null)
        {
            Debug.LogError("DriverParaUI is not assigned in the inspector.");
        }


    }

    private void Start()
    {
        cameras.Add("Hamilton", hamiltonCamera);
        cameras.Add("Wolff", wolffCamera);
        cameras.Add("Strategist", strategistCamera);
        cameras.Add("Mechanic", mechanicCamera);
        SitWolff.SetActive(false);
        cm.SetCamera(playerCamera);


    }

    private void MeetingStart()
    {
        print("开始会议");
        chair.interactionText.SetActive(false);
        StandWolff.SetActive(false);
        SitWolff.SetActive(true);
        cm.SetCamera(quanjing);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        client.Send(JsonStr.before_meeting_start, (response) =>
        { }, (r) => MeetingReplay());

    }
    private void MeetingReplay()
    {

        client.Send(JsonStr.meeting_replay, (response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // 切换摄像机
            cm.SetCamera(cameras[json["sender"].ToString()]);
            // 处理服务器返回的JSON数据
            dialog.ShowCharacterUI(json["sender"].ToString());
            dialog.ShowDialogue(json["content"].ToString());
        }, (r) => MeetingChoose());


    }
    private void MeetingChoose()
    {
        cm.SetCamera(cameras["Wolff"]);
        dialog.ShowCharacterUI("Wolff");
        dialog.ShowInputFieldByButton("请输入对话内容",
                (input) =>
                {
                    string sendStr = JsonStr.meeting_chat(input, "Mechanic");
                    // 发送输入的内容到服务器
                    client.Send(sendStr, (response) =>
                    {
                        JsonData json = JsonMapper.ToObject(response);
                        // 处理服务器返回的JSON数据
                        dialog.ShowCharacterUI(json["sender"].ToString());
                        dialog.ShowDialogue(json["content"].ToString());
                        // 切换摄像机
                        cm.SetCamera(cameras[json["sender"].ToString()]);
                    }, (r) => MeetingEnd());
                },
                (input) =>
                {
                    client.Send(JsonStr.meeting_strategy, (response) =>
                    {
                        JsonData json = JsonMapper.ToObject(response);
                        // 处理服务器返回的JSON数据
                        dialog.ShowCharacterUI(json["sender"].ToString());
                        dialog.ShowDialogue(json["content"].ToString());
                        // 切换摄像机
                        cm.SetCamera(cameras[json["sender"].ToString()]);
                        //处理策略
                        if (json.ContainsKey("strategy"))
                            print("策略:" + json["strategy"].ToString());
                        else
                            print("策略:无");
                    }, (r) => MeetingEnd());
                },
                (input) =>
                {
                    client.Send(JsonStr.meeting_attitude, (response) =>
                    {
                        JsonData json = JsonMapper.ToObject(response);
                        // 处理服务器返回的JSON数据
                        dialog.ShowCharacterUI(json["sender"].ToString());
                        dialog.ShowDialogue(json["content"].ToString());
                        // 切换摄像机
                        cm.SetCamera(cameras[json["sender"].ToString()]);
                        //处理心态
                        if (json.ContainsKey("attitude"))
                            print("心态:" + json["attitude"].ToString());
                        else
                            print("心态:无");
                    }, (r) => MeetingEnd());
                }
            );
    }
    private void MeetingEnd()
    {
        client.Send(JsonStr.before_meeting_end, (string response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // 处理服务器返回的JSON数据
            dialog.ShowCharacterUI("report");
            JsonData news = json["summary"];
            dialog._currentRoot.Q<Label>("Title").text = news["title"].ToString();
            dialog._currentRoot.Q<Label>("Contents").text = news["content"].ToString();

        }, null);
    }

    private void Update()
    {
        // 检测玩家是否在范围内并按下F键
        if (chair.isInRange && Input.GetKeyDown(KeyCode.F))
        {
            MeetingStart();
        }
    }
}