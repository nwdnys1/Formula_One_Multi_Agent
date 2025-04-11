using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using DTO;
using LitJson;
using System.Collections.Generic;

public class RaceUI : MonoBehaviour
{
    public UIDocument POVUI;
    public UIDocument StrategyUI;
    public UIDocument TrackUI;
    public PitStopWidget PitUI;
    SocketClient client = SocketClient.Instance;

    private VisualElement POVRoot;
    private VisualElement StrategyRoot;
    private VisualElement TrackRoot;

    //POV
    private Label tyreTypeLabel;
    private Label tyreWearLabel;
    private Label ersStatusLabel;
    private Label fuelStatusLabel;
    private Label attitudeLabel;
    private Label lapLabel;
    private Label trackTempLabel;
    private Label airTempLabel;
    private VisualElement _listContainer;
    private CarRank[] _currentStandings = new CarRank[20];

    //Strategy
    private Label ersStrat;
    private Label fuelStrat;
    private Label moraleStrat;
    private Label lapStrat;
    private TextField _inputField; // 输入框
    private Label wolff;
    private Label hamilton;
    private Label strategist;

    //track
    private Label trackTemp;
    private Label airTemp;
    private Label rainfall;
    private Label windspeed;
    private Label winddir;
    private Label gripLevel;
    private Label rubber;
    private Label lapTrack;

    public CarController targetCar;
    private ParaManager para = ParaManager.Instance;

    [System.Serializable]
    public class CarRank
    {
        public string driverName; // 车手简称 (VER/HAM/LEC等)
        public string teamId;     // 车队标识 (Redbull/Ferrari等)
        public Texture2D logo;   // 车队logo
        public string tireType;   // 轮胎类型 (S/M/H)
        public float gap;         // 与前车差距 (秒)
    }

    private void OnEnable()
    {


        //root
        POVRoot = POVUI.rootVisualElement;
        StrategyRoot = StrategyUI.rootVisualElement;
        TrackRoot = TrackUI.rootVisualElement;
        //POV
        tyreWearLabel = POVRoot.Q<Label>("TyreWear");
        tyreTypeLabel = POVRoot.Q<Label>("TyreType");
        ersStatusLabel = POVRoot.Q<Label>("ERS");
        fuelStatusLabel = POVRoot.Q<Label>("FUEL");
        attitudeLabel = POVRoot.Q<Label>("Attitude");
        lapLabel = POVRoot.Q<Label>("Lap");
        trackTempLabel = POVRoot.Q<Label>("TrackTemp");
        airTempLabel = POVRoot.Q<Label>("AirTemp");

        _listContainer = POVRoot.Q<VisualElement>("Rank");

        //strategy
        ersStrat = StrategyRoot.Q<Label>("ERS");
        fuelStrat = StrategyRoot.Q<Label>("Fuel");
        moraleStrat = StrategyRoot.Q<Label>("Morale");
        lapStrat = StrategyRoot.Q<Label>("Lap");
        _inputField = StrategyRoot.Q<TextField>("Input");
        wolff = StrategyRoot.Q<Label>("Wolff");
        hamilton = StrategyRoot.Q<Label>("Hamilton");
        strategist = StrategyRoot.Q<Label>("Strategist");

        //track
        trackTemp = TrackRoot.Q<Label>("TrackTemp");
        airTemp = TrackRoot.Q<Label>("AirTemp");
        rainfall = TrackRoot.Q<Label>("Rainfall");
        windspeed = TrackRoot.Q<Label>("WindSpeed");
        winddir = TrackRoot.Q<Label>("WindDir");
        gripLevel = TrackRoot.Q<Label>("Grip");
        rubber = TrackRoot.Q<Label>("Rubber");
        lapTrack = TrackRoot.Q<Label>("Lap");

        // 初始化数据
        for (int i = 0; i < 20; i++)
        {
            _currentStandings[i] = new CarRank
            {
                driverName = "---",
                teamId = "Unknown",
                tireType = "M",
                gap = 0f,
            };
        }
    }
    private void Start()
    {
        // 隐藏所有UI
        POVRoot.style.display = DisplayStyle.None;
        StrategyRoot.style.display = DisplayStyle.None;
        TrackRoot.style.display = DisplayStyle.None;

        ShowInputField((input) =>
        {
            wolff.text = input;
            string strat = JsonMapper.ToJson(strat2Json());
            client.Send(JsonStr.strategy_update(input, strat), (response) =>
            {
                JsonData json = JsonMapper.ToObject(response);
                strategist.text = json["content"].ToString();
                int[] pits = new int[json["strategy"]["pit_stop_laps"].Count];
                for (int i = 0; i < pits.Length; i++)
                {
                    pits[i] = int.Parse(json["strategy"]["pit_stop_laps"][i].ToString());
                }
                string[] tyres = new string[json["strategy"]["tyre_strategy"].Count];
                for (int i = 0; i < tyres.Length; i++)
                {
                    tyres[i] = json["strategy"]["tyre_strategy"][i].ToString().ToLower();
                }
                // tyres解析为枚举
                string[] tyreTypes = new string[tyres.Length];
                for (int i = 0; i < tyres.Length; i++)
                {
                    tyreTypes[i] = json["strategy"]["tyre_strategy"][i].ToString().ToLower();
                }
                targetCar.pitStopLaps = pits;
                targetCar.tyreTypes = tyreTypes;
                targetCar.fuelLap = int.Parse(json["strategy"]["fuel_release_laps"].ToString());
                targetCar.ERSLap = int.Parse(json["strategy"]["ers_release_laps"].ToString());
            }, (r) =>
            {
                client.Send(JsonStr.attitude_update(input, targetCar.attitude), (response) =>
                {
                    JsonData json = JsonMapper.ToObject(response);
                    hamilton.text = json["content"].ToString();
                    targetCar.attitude = json["attitude"].ToString();
                }, null);
            });
        });
    }
    private void Update()
    {

        // 更新UI显示
        UpdateParametersDisplay();
        UpdateRanks(_currentStandings);
        // 按q展示或关闭POV 按w展示或关闭Strategy 按e展示或关闭Track
        if (Input.GetKeyDown(KeyCode.Q))
        {
            POVRoot.style.display = POVRoot.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
            StrategyRoot.style.display = DisplayStyle.None;
            TrackRoot.style.display = DisplayStyle.None;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            StrategyRoot.style.display = StrategyRoot.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
            POVRoot.style.display = DisplayStyle.None;
            TrackRoot.style.display = DisplayStyle.None;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            TrackRoot.style.display = TrackRoot.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
            POVRoot.style.display = DisplayStyle.None;
            StrategyRoot.style.display = DisplayStyle.None;
        }
    }
    private JsonData strat2Json()
    {
        JsonData json = new JsonData();
        JsonData pits = new JsonData();
        JsonData tyres = new JsonData();
        foreach (var item in targetCar.tyreTypes)
        {
            tyres.Add(item);
        }
        foreach (var item in targetCar.pitStopLaps)
        {
            pits.Add(item);
        }
        json["pit_stop_laps"] = pits;
        json["tyre_strategy"] = tyres;
        json["fuel_release_laps"] = targetCar.fuelLap;
        json["ERS_release_laps"] = targetCar.ERSLap;
        return json;
    }
    private void UpdateParametersDisplay()
    {

        // 赛车参数
        tyreTypeLabel.text = targetCar.tyreType.ToUpper().Substring(0, 1); // 轮胎类型 (S/M/H)
        tyreWearLabel.text = $"{targetCar.currentTyreWear:F1} %"; // 轮胎磨损百分比
        ersStatusLabel.text = targetCar.ersAvailable ? "1" : "0";
        fuelStatusLabel.text = targetCar.fuelReleaseAvailable ? "1" : "0";
        attitudeLabel.text = targetCar.attitude;
        lapLabel.text = (targetCar.lapCount + 1).ToString(); // 当前圈数  

        // 环境参数
        trackTempLabel.text = $"{para.GetEnvPara().trackTemperature} °C"; // 赛道温度
        airTempLabel.text = $"{para.GetEnvPara().airTemperature} °C"; // 空气温度

        // 策略
        string[] tyreStrategy = targetCar.tyreTypes.Select(x => x.ToString()).ToArray();
        PitUI.UpdateStrategy(targetCar.pitStopLaps, tyreStrategy);
        ersStrat.text = targetCar.ersAvailable ? "1" : "0";
        fuelStrat.text = targetCar.fuelReleaseAvailable ? "1" : "0";
        moraleStrat.text = targetCar.attitude;
        lapStrat.text = (targetCar.lapCount + 1).ToString(); // 当前圈数

        // track
        trackTemp.text = $"{para.GetEnvPara().trackTemperature} °C"; // 赛道温度
        airTemp.text = $"{para.GetEnvPara().airTemperature} °C"; // 空气温度
        rainfall.text = $"{para.GetEnvPara().rainfall} mm"; // 降雨量
        windspeed.text = $"{para.GetEnvPara().windSpeed} km/h"; // 风速
        winddir.text = para.GetEnvPara().windDirection; // 风向
        lapTrack.text = (targetCar.lapCount + 1).ToString(); // 当前圈数
    }

    public void UpdateRanks(CarRank[] newStandings)
    {
        int len = newStandings.Length;

        // 更新数据
        System.Array.Copy(newStandings, _currentStandings, len);

        // 刷新UI
        for (int i = 0; i < len; i++)
        {
            UpdateSingleRow(i + 1, _currentStandings[i]);
        }
    }

    private void UpdateSingleRow(int position, CarRank data)
    {
        // 获取对应行元素
        var row = _listContainer.Q<VisualElement>("Row" + position);

        // 更新子元素
        row.Q<Label>("driver_name").text = data.driverName.Substring(0, 3).ToUpper(); // 车手简称 (VER/HAM/LEC等)
        row.Q<Label>($"+1.000").text = $"+{data.gap:F3}";
        row.Q<Label>("M").text = data.tireType;
        if (row.Q<Image>("Logo") != null)
        {
            row.Q<Image>("Logo").image = data.logo;
        }

        //// 动态加载车队图标（示例）
        //var teamIcon = row.Q<VisualElement>("Team").Q<VisualElement>();
        //teamIcon.style.backgroundImage = Resources.Load<Texture2D>($"Icons/Teams/{data.teamId}");
    }

    // 显示输入框（仅WolffUI可用）
    public void ShowInputField(System.Action<string> onSubmit)
    {

        if (_inputField != null)
        {
            _inputField.value = "";
            _inputField.Q(TextField.textInputUssName).Focus();
            _inputField.style.display = DisplayStyle.Flex;

            _inputField.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode == KeyCode.Return)
                {
                    print("输入内容: " + _inputField.value);
                    onSubmit?.Invoke(_inputField.value);
                    _inputField.value = "";
                }
            });
        }
    }

}