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

public class MeetingController : MonoBehaviour
{
    MeetingUI meetingUI;
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
        meetingUI = GetComponent<MeetingUI>();
        // 确保DialogUI组件已正确设置
        if (meetingUI == null)
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
            meetingUI.ShowCharacterUI(json["sender"].ToString());
            meetingUI.ShowDialogue(json["content"].ToString());
        }, (r) => MeetingChoose());


    }
    private void MeetingChoose()
    {
        cm.SetCamera(cameras["Wolff"]);
        meetingUI.ShowCharacterUI("Wolff");
        meetingUI.ShowInputFieldByButton("请输入对话内容",
                (input) =>
                {
                    string sendStr = JsonStr.meeting_chat(input, "Mechanic");
                    // 发送输入的内容到服务器
                    client.Send(sendStr, (response) =>
                    {
                        JsonData json = JsonMapper.ToObject(response);
                        // 处理服务器返回的JSON数据
                        meetingUI.ShowCharacterUI(json["sender"].ToString());
                        meetingUI.ShowDialogue(json["content"].ToString());
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
                        meetingUI.ShowCharacterUI(json["sender"].ToString());
                        meetingUI.ShowDialogue(json["content"].ToString());
                        // 切换摄像机
                        cm.SetCamera(cameras[json["sender"].ToString()]);
                        //处理策略
                        if (json.ContainsKey("strategy"))
                        {
                            switch (json["sender"].ToString())
                            {
                                case "Strategist":
                                    JsonData strategies = json["strategy"];
                                    int[][] pits = new int[strategies.Count][];
                                    string[][] tyres = new string[strategies.Count][];
                                    int[] fuels = new int[strategies.Count];
                                    int[] ers = new int[strategies.Count];
                                    for (int i = 0; i < strategies.Count; i++)
                                    {
                                        pits[i] = new int[strategies[i]["pit_stop_laps"].Count];
                                        tyres[i] = new string[strategies[i]["tyre_strategy"].Count];
                                        for (int j = 0; j < strategies[i]["pit_stop_laps"].Count; j++)
                                        {
                                            pits[i][j] = int.Parse(strategies[i]["pit_stop_laps"][j].ToString());
                                        }
                                        for (int j = 0; j < strategies[i]["tyre_strategy"].Count; j++)
                                        {
                                            tyres[i][j] = strategies[i]["tyre_strategy"][j].ToString();
                                        }
                                        fuels[i] = int.Parse(strategies[i]["fuel_release_laps"].ToString());
                                        ers[i] = int.Parse(strategies[i]["ers_release_laps"].ToString());
                                    }
                                    pitUI.UpdateStrategy(pits, tyres, fuels, ers);
                                    break;
                                case "Hamilton":
                                    JsonData strategy = json["strategy"];
                                    int[] pit = new int[strategy["pit_stop_laps"].Count];
                                    string[] tyre = new string[strategy["tyre_strategy"].Count];
                                    for (int j = 0; j < strategy["pit_stop_laps"].Count; j++)
                                    {
                                        pit[j] = int.Parse(strategy["pit_stop_laps"][j].ToString());
                                    }
                                    for (int j = 0; j < strategy["tyre_strategy"].Count; j++)
                                    {
                                        tyre[j] = strategy["tyre_strategy"][j].ToString().ToLower();
                                    }
                                    targetCar.pitStopLaps = pit;
                                    targetCar.tyreTypes = tyre;
                                    targetCar.fuelLap = int.Parse(strategy["fuel_release_laps"].ToString());
                                    targetCar.ERSLap = int.Parse(strategy["ers_release_laps"].ToString());
                                    break;
                                default:
                                    print("策略:无");
                                    break;
                            }
                        }
                        else
                        {
                            print("策略:无");
                        }
                    }, (r) => MeetingEnd());
                },
                (input) =>
                {
                    client.Send(JsonStr.meeting_attitude, (response) =>
                    {
                        JsonData json = JsonMapper.ToObject(response);
                        // 处理服务器返回的JSON数据
                        meetingUI.ShowCharacterUI(json["sender"].ToString());
                        meetingUI.ShowDialogue(json["content"].ToString());
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
            meetingUI.ShowCharacterUI("report");
            JsonData news = json["summary"];
            meetingUI._currentRoot.Q<Label>("Title").text = news["title"].ToString();
            meetingUI._currentRoot.Q<Label>("Contents").text = news["content"].ToString();

        }, (r) => { SceneManager.LoadScene("赛场"); });
    }

    private void Update()
    {

    }
}