using UnityEngine;
using UnityEngine.UIElements;

public class CarParametersUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private string carId = "1号";

    private CarPara currentCarPara;

    // 参数显示标签
    private Label topSpeedValue;
    private Label accelerationValue;
    private Label drsEffectivenessValue;
    private Label carCorneringValue;
    private Label tyreTypeValue;
    private Label wearRateValue;
    private Label currentWearValue;
    private Label maxLapsValue;

    // 调校参数显示标签
    private Label frontWingValue;
    private Label rearWingValue;
    private Label antiRollValue;
    private Label camberValue;
    private Label toeOutValue;

    // 调校结果标签
    private Label oversteerRating;
    private Label brakingStabilityRating;

    private void OnEnable()
    {
        // 获取赛车参数
        currentCarPara = ParaManager.Instance.getCarPara(carId);
        if (currentCarPara == null)
        {
            Debug.LogError($"无法找到赛车ID: {carId}");
            return;
        }

        // 获取UI元素
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        // 获取显示标签
        topSpeedValue = root.Q<Label>("topSpeedValue");
        accelerationValue = root.Q<Label>("accelerationValue");
        drsEffectivenessValue = root.Q<Label>("drsEffectivenessValue");
        carCorneringValue = root.Q<Label>("carCorneringValue");
        tyreTypeValue = root.Q<Label>("tyreTypeValue");
        wearRateValue = root.Q<Label>("wearRateValue");
        currentWearValue = root.Q<Label>("currentWearValue");
        maxLapsValue = root.Q<Label>("maxLapsValue");

        // 获取调校参数显示标签
        frontWingValue = root.Q<Label>("frontWingValue");
        rearWingValue = root.Q<Label>("rearWingValue");
        antiRollValue = root.Q<Label>("antiRollValue");
        camberValue = root.Q<Label>("camberValue");
        toeOutValue = root.Q<Label>("toeOutValue");

        // 获取调校结果标签
        oversteerRating = root.Q<Label>("oversteerRating");
        brakingStabilityRating = root.Q<Label>("brakingStabilityRating");

        // 初始化UI
        UpdateAllParametersDisplay();
    }

    private void Update()
    {
        // 这里可以添加实时更新逻辑，例如根据赛车状态更新参数
        UpdateAllParametersDisplay();
    }

    /// <summary>
    /// 更新所有参数显示
    /// </summary>
    public void UpdateAllParametersDisplay()
    {
        UpdateCarParametersDisplay();
        UpdateSetupParametersDisplay();
        UpdateSetupRatingsDisplay();
    }

    /// <summary>
    /// 更新赛车基本参数显示
    /// </summary>
    private void UpdateCarParametersDisplay()
    {
        topSpeedValue.text = currentCarPara.topSpeed.ToString("F1");
        accelerationValue.text = currentCarPara.acceleration.ToString("F1");
        drsEffectivenessValue.text = currentCarPara.drsEffectiveness.ToString("P0");
        carCorneringValue.text = currentCarPara.carCornering.ToString("F1");

        tyreTypeValue.text = currentCarPara.tyreType.ToString();
        wearRateValue.text = currentCarPara.wearRate.ToString("P0");
        currentWearValue.text = currentCarPara.currentWear.ToString("F1");
        maxLapsValue.text = currentCarPara.GetMaxLaps().ToString();
    }

    /// <summary>
    /// 更新调校参数显示
    /// </summary>
    private void UpdateSetupParametersDisplay()
    {
        frontWingValue.text = currentCarPara.frontWingAngle.ToString("F1");
        rearWingValue.text = currentCarPara.rearWingAngle.ToString("F1");
        antiRollValue.text = currentCarPara.antiRollDistribution.ToString("F2");
        camberValue.text = currentCarPara.tyreCamber.ToString("F1");
        toeOutValue.text = currentCarPara.toeOut.ToString("F2");
    }

    /// <summary>
    /// 更新调校结果评价显示
    /// </summary>
    private void UpdateSetupRatingsDisplay()
    {
        oversteerRating.text = currentCarPara.oversteerRating.ToString();
        brakingStabilityRating.text = currentCarPara.brakingStabilityRating.ToString();
    }

    /// <summary>
    /// 修改赛车调校参数
    /// </summary>
    /// <param name="frontWing">前翼角度 (0-10)</param>
    /// <param name="rearWing">后翼角度 (8-18)</param>
    /// <param name="antiRoll">防倾杆分布 (0.1-0.9)</param>
    /// <param name="camber">轮胎外倾角 (-3.5 - -2)</param>
    /// <param name="toeOut">前束角 (0-1)</param>
    public void ModifySetupParameters(float frontWing, float rearWing, float antiRoll, float camber, float toeOut)
    {
        // 应用新的调校参数
        currentCarPara.frontWingAngle = Mathf.Clamp(frontWing, 0f, 10f);
        currentCarPara.rearWingAngle = Mathf.Clamp(rearWing, 8f, 18f);
        currentCarPara.antiRollDistribution = Mathf.Clamp(antiRoll, 0.1f, 0.9f);
        currentCarPara.tyreCamber = Mathf.Clamp(camber, -3.5f, -2f);
        currentCarPara.toeOut = Mathf.Clamp(toeOut, 0f, 1f);

        // 更新管理器中的参数
        ParaManager.Instance.setCarPara(carId, currentCarPara);

        // 更新UI显示
        UpdateSetupParametersDisplay();
        UpdateSetupRatingsDisplay();
    }

    /// <summary>
    /// 修改单个调校参数
    /// </summary>
    /// <param name="parameterType">参数类型枚举</param>
    /// <param name="value">参数值</param>
    public void ModifySingleSetupParameter(SetupParameterType parameterType, float value)
    {
        switch (parameterType)
        {
            case SetupParameterType.FrontWing:
                currentCarPara.frontWingAngle = Mathf.Clamp(value, 0f, 10f);
                break;
            case SetupParameterType.RearWing:
                currentCarPara.rearWingAngle = Mathf.Clamp(value, 8f, 18f);
                break;
            case SetupParameterType.AntiRoll:
                currentCarPara.antiRollDistribution = Mathf.Clamp(value, 0.1f, 0.9f);
                break;
            case SetupParameterType.Camber:
                currentCarPara.tyreCamber = Mathf.Clamp(value, -3.5f, -2f);
                break;
            case SetupParameterType.ToeOut:
                currentCarPara.toeOut = Mathf.Clamp(value, 0f, 1f);
                break;
        }

        ParaManager.Instance.setCarPara(carId, currentCarPara);
        UpdateSetupParametersDisplay();
        UpdateSetupRatingsDisplay();
    }

    /// <summary>
    /// 获取当前赛车参数
    /// </summary>
    public CarPara GetCurrentCarParameters()
    {
        return currentCarPara;
    }

    /// <summary>
    /// 设置显示的赛车ID
    /// </summary>
    public void SetCarId(string newCarId)
    {
        carId = newCarId;
        currentCarPara = ParaManager.Instance.getCarPara(carId);
        UpdateAllParametersDisplay();
    }
}

/// <summary>
/// 调校参数类型枚举
/// </summary>
public enum SetupParameterType
{
    FrontWing,
    RearWing,
    AntiRoll,
    Camber,
    ToeOut
}