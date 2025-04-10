using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DTO;
using LitJson;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class RaceController : MonoBehaviour
{
    public RaceUI raceUI;
    SocketClient client = SocketClient.Instance;
    CameraManager cm = CameraManager.Instance;
    ParaManager para = ParaManager.Instance;
    [Header("摄像机")]
    public CinemachineVirtualCamera[] cameras = new CinemachineVirtualCamera[6];
    public int currentCameraIndex = 0;
    private void Awake()
    {

        raceUI = GetComponent<RaceUI>();
        if (raceUI == null)
        {
            Debug.LogError("RaceUI is not assigned in the inspector.");
        }

    }

    private void Start()
    {
        RaceStart();
    }

    private void RaceStart()
    {
        print("开始比赛");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        client.Send(JsonStr.race_start, (response) =>
        { }, (r) => EnvUpdate());

    }
    private void EnvUpdate()
    {

        client.Send(JsonStr.weather_update, (response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            float track_temp = float.Parse(json["track_conditions"]["circuit_temperature"].ToString());
            float air_temp = float.Parse(json["track_conditions"]["air_temperature"].ToString());
            float rainfall = float.Parse(json["track_conditions"]["rainfall"].ToString());
            float wind_speed = float.Parse(json["track_conditions"]["wind_speed"].ToString());
            string wind_direction = json["track_conditions"]["wind_direction"].ToString();
            para.GetEnvPara().trackTemperature = track_temp;
            para.GetEnvPara().airTemperature = air_temp;
            para.GetEnvPara().rainfall = rainfall;
            para.GetEnvPara().windSpeed = wind_speed;
            para.GetEnvPara().windDirection = wind_direction;


        }, (r) => { });


    }
    private void MeetingChoose()
    {
        
        
    }
    private void MeetingEnd()
    {
        client.Send(JsonStr.before_meeting_end, (string response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // 处理服务器返回的JSON数据
            

        }, null);
    }

    private void Update()
    {
        // 按左右方向键切换摄像机 顺序遍历cameras
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentCameraIndex--;
            if (currentCameraIndex < 0)
            {
                currentCameraIndex = cameras.Length - 1;
            }
            cm.SetCamera(cameras[currentCameraIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentCameraIndex++;
            if (currentCameraIndex >= cameras.Length)
            {
                currentCameraIndex = 0;
            }
            cm.SetCamera(cameras[currentCameraIndex]);
        }
    }
}