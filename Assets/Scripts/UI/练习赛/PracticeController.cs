using UnityEngine;
using DTO;
using LitJson;
using UnityEngine.SceneManagement;

public class PracticeController : MonoBehaviour
{
    SocketClient client = SocketClient.Instance;
    ParaManager paraManager = ParaManager.Instance;
    public PracticeUI practiceUI;
    public string carId = "Hamilton";
    CarPara targetCar;

    private void Awake()
    {


        if (client == null)
        {
            Debug.LogError("SocketClient is not assigned in the inspector.");
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetCar = paraManager.getCarPara(carId);
        PracticeStart();
    }
    private void PracticeStart()
    {
        practiceUI.ShowWaitingUI(true);
        client.Send(JsonStr.practice_session_start, (response) =>
        {
            practiceUI.ShowWaitingUI(false);
            JsonData json = JsonMapper.ToObject(response);
            // 处理服务器返回的JSON数据
            switch (json["sender"].ToString())
            {
                case "Mechanic":
                    practiceUI.UpdateTeamDiscussion(json["content"].ToString(), null);

                    break;
                case "Hamilton":
                    practiceUI.UpdateTeamDiscussion(null, json["content"].ToString());
                    JsonData tuning = json["tuning_data"];
                    practiceUI.UpdateCarSetup(tuning["front_wing_angle"].ToString() + "°",
                        tuning["rear_wing_angle"].ToString() + "°",
                        tuning["anti_roll_distribution"].ToString(),
                        tuning["tyre_camber"].ToString() + "°",
                        tuning["toe_out"].ToString() + "°");
                    //修改赛车参数

                    targetCar.frontWingAngle = float.Parse(tuning["front_wing_angle"].ToString());
                    targetCar.rearWingAngle = float.Parse(tuning["rear_wing_angle"].ToString());
                    targetCar.antiRollDistribution = float.Parse(tuning["anti_roll_distribution"].ToString());
                    targetCar.tyreCamber = float.Parse(tuning["tyre_camber"].ToString());
                    targetCar.toeOut = float.Parse(tuning["toe_out"].ToString());
                    targetCar.CalculateTuningRatings();
                    //更新赛车性能
                    string[] satisfaction = new string[5]
                    {
                        "Disaster","Low","Medium","High","Perfect"
                    };

                    practiceUI.UpdateDriverSatisfaction(satisfaction[targetCar.oversteerRating - 1],
                        satisfaction[targetCar.brakingStabilityRating - 1],
                        satisfaction[targetCar.corneringRating - 1],
                        satisfaction[targetCar.tractionRating - 1],
                        satisfaction[targetCar.straightsRating - 1]);

                    practiceUI.UpdateCarPerformance(targetCar.topSpeed.ToString() + "km/h",
                        targetCar.acceleration.ToString() + "G",
                        targetCar.drsEffectiveness.ToString() + "%",
                        (targetCar.carCornering * 100).ToString() + "G");
                    break;
                default:
                    Debug.Log("未知场景类型");
                    break;
            }

        }, (r) => PracticeUpdate());
    }
    public void PracticeUpdate()
    {
        JsonData tuning_res = new JsonData();
        string[] satisfaction = new string[5]
            {
                "Disaster","Low","Medium","High","Perfect"
            };
        tuning_res["Oversteer"] = satisfaction[targetCar.oversteerRating - 1];
        tuning_res["BrakingStability"] = satisfaction[targetCar.brakingStabilityRating - 1];
        tuning_res["Cornering"] = satisfaction[targetCar.corneringRating - 1];
        tuning_res["Traction"] = satisfaction[targetCar.tractionRating - 1];
        tuning_res["Straights"] = satisfaction[targetCar.straightsRating - 1];
        string tuningJson = JsonMapper.ToJson(tuning_res);
        client.Send(JsonStr.practice_session_update(tuningJson), (response) =>
        {
            JsonData json = JsonMapper.ToObject(response);

        }, (r) => { SceneManager.LoadScene("会议室"); });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
