using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class InMatchUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    [SerializeField] private string targetCarId = "playerCar"; // 默认监控的赛车ID

    private Label carIdLabel;
    private Label speedLabel;
    private Label accelerationLabel;
    private Label tyreTypeLabel;
    private ProgressBar tyreWearBar;
    private Label ersStatusLabel;
    private Label fuelStatusLabel;
    private Label checkpointLabel;
    private Label segmentTypeLabel;

    public CarController targetCar;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        // 获取UI元素
        carIdLabel = root.Q<Label>("carIdLabel");
        speedLabel = root.Q<Label>("speedLabel");
        accelerationLabel = root.Q<Label>("accelerationLabel");
        tyreTypeLabel = root.Q<Label>("tyreTypeLabel");
        tyreWearBar = root.Q<ProgressBar>("tyreWearBar");
        ersStatusLabel = root.Q<Label>("ersStatusLabel");
        fuelStatusLabel = root.Q<Label>("fuelStatusLabel");
        checkpointLabel = root.Q<Label>("checkpointLabel");
        segmentTypeLabel = root.Q<Label>("segmentTypeLabel");

    }

    private void Update()
    {
        // 更新UI显示
        UpdateParametersDisplay();
    }

    private void UpdateParametersDisplay()
    {
        // 基本信息
        speedLabel.text = $"{targetCar.GetComponent<NavMeshAgent>().velocity.magnitude * 3.6f:F1} km/h";
        accelerationLabel.text = $"{targetCar.acceleration:F1} m/s²";

        // 轮胎状态
        tyreTypeLabel.text = targetCar.tyreType.ToString();
        tyreWearBar.value = targetCar.currentTyreWear;

        // 根据磨损率改变颜色
        if (targetCar.currentTyreWear < 30f)
        {
            tyreWearBar.AddToClassList("tyre-wear-low");
        }
        else
        {
            tyreWearBar.RemoveFromClassList("tyre-wear-low");
        }

        // 性能增强状态
        ersStatusLabel.text = targetCar.ersAvailable ? "可用 (按1激活)" :
            targetCar.ersActive ? $"激活中 ({targetCar.ersDuration - targetCar.ersTimer:F1}s)" : "已用完";

        fuelStatusLabel.text = targetCar.fuelReleaseAvailable ? "可用 (按2激活)" :
            targetCar.fuelActive ? $"激活中 ({targetCar.fuelDuration - targetCar.fuelTimer:F1}s)" : "已用完";

        // 导航信息
        checkpointLabel.text = $"{targetCar._currentIndex + 1}/{targetCar._checkpoints.Length}";
        segmentTypeLabel.text = targetCar._currentSegmentType.ToString();
    }



}