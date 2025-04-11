using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DTO;
using LitJson;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.WSA;
using Cursor = UnityEngine.Cursor;
using UnityEngine.SceneManagement;

public class EndController : MonoBehaviour
{
    EndUI UI;
    public PitStopWidgetTrible pitUI;
    SocketClient client = SocketClient.Instance;
    CameraManager cm = CameraManager.Instance;
    public CinemachineVirtualCamera strategistCamera;
    public CinemachineVirtualCamera mechanicCamera;
    public CinemachineVirtualCamera hamiltonCamera;
    public CinemachineVirtualCamera wolffCamera;
    public CinemachineVirtualCamera quanjing;
    public Dictionary<string, CinemachineVirtualCamera> cameras = new Dictionary<string, CinemachineVirtualCamera>();
    public string carId = "Hamilton";
    ParaManager paraManager = ParaManager.Instance;
    CarPara targetCar;

    private void Awake()
    {
        UI = GetComponent<EndUI>();
        // 确保DialogUI组件已正确设置
        if (UI == null)
        {
            Debug.LogError("DialogUI is not assigned in the inspector.");
        }


    }

    private void Start()
    {
        targetCar = paraManager.getCarPara(carId);
        cameras.Add("Hamilton", hamiltonCamera);
        cameras.Add("Wolff", wolffCamera);
        cameras.Add("Strategist", strategistCamera);
        cameras.Add("Mechanic", mechanicCamera);
        cm.SetCamera(quanjing);
        MeetingStart();

    }

    private void MeetingStart()
    {
        print("开始会议");
        cm.SetCamera(quanjing);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        client.Send(JsonStr.after_meeting_start, (response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // 切换摄像机
            cm.SetCamera(cameras[json["sender"].ToString()]);
            // 处理服务器返回的JSON数据
            UI.ShowCharacterUI(json["sender"].ToString());
            UI.ShowDialogue(json["content"].ToString());
        }, (r) => MeetingEnd());

    }


    private void MeetingEnd()
    {
        client.Send(JsonStr.after_meeting_end, (string response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // 处理服务器返回的JSON数据
            UI.ShowCharacterUI("report");
            JsonData news = json["summary"];
            UI._currentRoot.Q<Label>("Title").text = news["title"].ToString();
            UI._currentRoot.Q<Label>("Contents").text = news["content"].ToString();

        }, (r) => { SceneManager.LoadScene("开始菜单"); });
    }

    private void Update()
    {

    }
}