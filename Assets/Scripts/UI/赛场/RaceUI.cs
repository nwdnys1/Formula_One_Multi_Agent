using System.Linq;
using Trackman;
using UnityEngine;
using UnityEngine.UIElements;

public class RaceUI : MonoBehaviour
{
    public UIDocument POVUI;
    public UIDocument StrategyUI;
    public UIDocument TrackUI;
    public PitStopWidget PitUI;

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

    private void UpdateParametersDisplay()
    {

        // 赛车参数
        tyreTypeLabel.text = targetCar.tyreType.ToString().ToUpper()[0].ToString(); // 轮胎类型 (S/M/H)
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
        row.Q<Label>("driver_name").text = data.driverName;
        row.Q<Label>($"+1.000").text = $"+{data.gap:F3}";
        row.Q<Label>("M").text = data.tireType;

        //// 动态加载车队图标（示例）
        //var teamIcon = row.Q<VisualElement>("Team").Q<VisualElement>();
        //teamIcon.style.backgroundImage = Resources.Load<Texture2D>($"Icons/Teams/{data.teamId}");
    }

}