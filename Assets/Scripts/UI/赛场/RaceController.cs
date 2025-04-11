using UnityEngine;
using Cinemachine;
using DTO;
using LitJson;
using Cursor = UnityEngine.Cursor;
using UnityEngine.SceneManagement;

public class RaceController : MonoBehaviour
{
    public RaceUI raceUI;
    public CarController targetCar;
    SocketClient client = SocketClient.Instance;
    CameraManager cm = CameraManager.Instance;
    ParaManager para = ParaManager.Instance;
    [Header("摄像机")]
    public CinemachineVirtualCamera[] cameras = new CinemachineVirtualCamera[6];
    public int currentCameraIndex = 0;
    public bool hasAccident = false;
    public GameObject safeCar;
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
        cm.SetCamera(cameras[0]);

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
    public void onAccident(string carId)
    {
        hasAccident = true;
        safeCar.SetActive(true);// 显示安全车
        client.Send(JsonStr.accident_occurred(carId), (response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            if (json.ContainsKey("strategy"))
            {
                int[] pits = new int[json["strategy"]["pit_stop_laps"].Count];
                for (int i = 0; i < pits.Length; i++)
                {
                    pits[i] = int.Parse(json["strategy"]["pit_stop_laps"][i].ToString());
                }
                string[] tyres = new string[json["strategy"]["tyre_strategy"].Count];
                for (int i = 0; i < tyres.Length; i++)
                {
                    tyres[i] = json["strategy"]["tyre_strategy"][i].ToString();
                }
                // tyres解析为枚举
                string[] tyreTypes = new string[tyres.Length];
                for (int i = 0; i < tyres.Length; i++)
                {
                    tyreTypes[i] = json["strategy"]["tyre_strategy"][i].ToString();
                }
                targetCar.pitStopLaps = pits;
                targetCar.tyreTypes = tyreTypes;
                targetCar.fuelLap = int.Parse(json["strategy"]["fuel_release_laps"].ToString());
                targetCar.ERSLap = int.Parse(json["strategy"]["ers_release_laps"].ToString());
            }
            else if (json.ContainsKey("attitude"))
            {
                targetCar.attitude = json["attitude"].ToString();
            }
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
        // 按下esc键退出比赛
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("赛后会议室");

        }
    }
}