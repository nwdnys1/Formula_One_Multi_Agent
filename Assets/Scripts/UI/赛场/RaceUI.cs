using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class RaceUI : MonoBehaviour
{
    public UIDocument POVUI;
    public UIDocument StrategyUI;
    public UIDocument TrackUI;

    private VisualElement POVRoot;
    private VisualElement StrategyRoot;
    private VisualElement TrackRoot;

    //POV
    private Label carIdLabel;
    private Label speedLabel;
    private Label accelerationLabel;
    private Label tyreTypeLabel;
    private Label tyreWearLabel;
    private Label ersStatusLabel;
    private Label fuelStatusLabel;
    private Label checkpointLabel;
    private Label segmentTypeLabel;
    private VisualElement _listContainer;
    private RaceCarData[] _currentStandings = new RaceCarData[20];

    public CarController targetCar;

    [System.Serializable]
    public class RaceCarData
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

        _listContainer = POVRoot.Q<VisualElement>("Rank");

        // 初始化数据
        for (int i = 0; i < 20; i++)
        {
            _currentStandings[i] = new RaceCarData
            {
                driverName = "---",
                teamId = "Unknown",
                tireType = "M",
                gap = 0f,
            };
        }
    }

    private void Update()
    {
        // 更新UI显示
        UpdateParametersDisplay();
        UpdateRanks(_currentStandings);
    }

    private void UpdateParametersDisplay()
    {
        // 基本信息
        //speedLabel.text = $"{targetCar.GetComponent<NavMeshAgent>().velocity.magnitude * 3.6f:F1} km/h";
        //accelerationLabel.text = $"{targetCar.acceleration:F1} m/s²";

        // 轮胎状态
        tyreTypeLabel.text = targetCar.tyreType.ToString();
        tyreWearLabel.text = $"{targetCar.currentTyreWear:F1} %"; // 轮胎磨损百分比

        // 性能增强状态
        ersStatusLabel.text = targetCar.ersAvailable ? "1" : "0";
        fuelStatusLabel.text = targetCar.fuelReleaseAvailable ? "1" : "0";

        // 导航信息
        //checkpointLabel.text = $"{targetCar._currentIndex + 1}/{targetCar._checkpoints.Length}";
        //segmentTypeLabel.text = targetCar._currentSegmentType.ToString();
    }

    public void UpdateRanks(RaceCarData[] newStandings)
    {
        // 数据校验
        if (newStandings.Length != 20)
        {
            Debug.LogError("必须提供20个赛车数据！");
            return;
        }

        // 更新数据
        System.Array.Copy(newStandings, _currentStandings, 20);

        // 刷新UI
        for (int i = 0; i < 20; i++)
        {
            UpdateSingleRow(i + 1, _currentStandings[i]);
        }
    }
    private void UpdateSingleRow(int position, RaceCarData data)
    {
        // 获取对应行元素
        var row = _listContainer.Q<VisualElement>("Row" + position);
        print(row);

        // 更新子元素
        row.Q<Label>("driver_name").text = data.driverName;
        row.Q<Label>($"+1.000").text = $"+{data.gap:F3}";
        row.Q<Label>("M").text = data.tireType;

        //// 动态加载车队图标（示例）
        //var teamIcon = row.Q<VisualElement>("Team").Q<VisualElement>();
        //teamIcon.style.backgroundImage = Resources.Load<Texture2D>($"Icons/Teams/{data.teamId}");
    }

}