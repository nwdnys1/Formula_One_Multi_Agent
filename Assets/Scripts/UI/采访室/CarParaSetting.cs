using UnityEngine;
using UnityEngine.UIElements;

public class CarParametersUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private string carId = "1��";

    private CarPara currentCarPara;

    // ������ʾ��ǩ
    private Label topSpeedValue;
    private Label accelerationValue;
    private Label drsEffectivenessValue;
    private Label carCorneringValue;
    private Label tyreTypeValue;
    private Label wearRateValue;
    private Label currentWearValue;
    private Label maxLapsValue;

    // ��У������ʾ��ǩ
    private Label frontWingValue;
    private Label rearWingValue;
    private Label antiRollValue;
    private Label camberValue;
    private Label toeOutValue;

    // ��У�����ǩ
    private Label oversteerRating;
    private Label brakingStabilityRating;

    private void OnEnable()
    {
        // ��ȡ��������
        currentCarPara = ParaManager.Instance.getCarPara(carId);
        if (currentCarPara == null)
        {
            Debug.LogError($"�޷��ҵ�����ID: {carId}");
            return;
        }

        // ��ȡUIԪ��
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        // ��ȡ��ʾ��ǩ
        topSpeedValue = root.Q<Label>("topSpeedValue");
        accelerationValue = root.Q<Label>("accelerationValue");
        drsEffectivenessValue = root.Q<Label>("drsEffectivenessValue");
        carCorneringValue = root.Q<Label>("carCorneringValue");
        tyreTypeValue = root.Q<Label>("tyreTypeValue");
        wearRateValue = root.Q<Label>("wearRateValue");
        currentWearValue = root.Q<Label>("currentWearValue");
        maxLapsValue = root.Q<Label>("maxLapsValue");

        // ��ȡ��У������ʾ��ǩ
        frontWingValue = root.Q<Label>("frontWingValue");
        rearWingValue = root.Q<Label>("rearWingValue");
        antiRollValue = root.Q<Label>("antiRollValue");
        camberValue = root.Q<Label>("camberValue");
        toeOutValue = root.Q<Label>("toeOutValue");

        // ��ȡ��У�����ǩ
        oversteerRating = root.Q<Label>("oversteerRating");
        brakingStabilityRating = root.Q<Label>("brakingStabilityRating");

        // ��ʼ��UI
        UpdateAllParametersDisplay();
    }

    /// <summary>
    /// �������в�����ʾ
    /// </summary>
    public void UpdateAllParametersDisplay()
    {
        UpdateCarParametersDisplay();
        UpdateSetupParametersDisplay();
        UpdateSetupRatingsDisplay();
    }

    /// <summary>
    /// ������������������ʾ
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
    /// ���µ�У������ʾ
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
    /// ���µ�У���������ʾ
    /// </summary>
    private void UpdateSetupRatingsDisplay()
    {
        oversteerRating.text = currentCarPara.GetOversteerRating().ToString();
        brakingStabilityRating.text = currentCarPara.GetBrakingStabilityRating().ToString();
    }

    /// <summary>
    /// �޸�������У����
    /// </summary>
    /// <param name="frontWing">ǰ��Ƕ� (0-10)</param>
    /// <param name="rearWing">����Ƕ� (8-18)</param>
    /// <param name="antiRoll">����˷ֲ� (0.1-0.9)</param>
    /// <param name="camber">��̥����� (-3.5 - -2)</param>
    /// <param name="toeOut">ǰ���� (0-1)</param>
    public void ModifySetupParameters(float frontWing, float rearWing, float antiRoll, float camber, float toeOut)
    {
        // Ӧ���µĵ�У����
        currentCarPara.frontWingAngle = Mathf.Clamp(frontWing, 0f, 10f);
        currentCarPara.rearWingAngle = Mathf.Clamp(rearWing, 8f, 18f);
        currentCarPara.antiRollDistribution = Mathf.Clamp(antiRoll, 0.1f, 0.9f);
        currentCarPara.tyreCamber = Mathf.Clamp(camber, -3.5f, -2f);
        currentCarPara.toeOut = Mathf.Clamp(toeOut, 0f, 1f);

        // ���¹������еĲ���
        ParaManager.Instance.setCarPara(carId, currentCarPara);

        // ����UI��ʾ
        UpdateSetupParametersDisplay();
        UpdateSetupRatingsDisplay();
    }

    /// <summary>
    /// �޸ĵ�����У����
    /// </summary>
    /// <param name="parameterType">��������ö��</param>
    /// <param name="value">����ֵ</param>
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
    /// ��ȡ��ǰ��������
    /// </summary>
    public CarPara GetCurrentCarParameters()
    {
        return currentCarPara;
    }

    /// <summary>
    /// ������ʾ������ID
    /// </summary>
    public void SetCarId(string newCarId)
    {
        carId = newCarId;
        currentCarPara = ParaManager.Instance.getCarPara(carId);
        UpdateAllParametersDisplay();
    }
}

/// <summary>
/// ��У��������ö��
/// </summary>
public enum SetupParameterType
{
    FrontWing,
    RearWing,
    AntiRoll,
    Camber,
    ToeOut
}