using UnityEngine;
using UnityEngine.UIElements;

public class DriverParaSetting : MonoBehaviour
{
    private UIDocument _uiDocument;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();

        InitializeUI();

    }
    private void Update()
    {
        DisplayDriver("1号");
    }

    private void InitializeUI()
    {
        DisplayDriver("1号");


    }

    public void DisplayDriver(string driverId)
    {
        var driverData = ParaManager.Instance.getDriverPara(driverId);

        // 获取UI元素引用
        var root = _uiDocument.rootVisualElement;
        var overallRateLabel = root.Q<Label>("overallRateLabel");
        var attitudeLabel = root.Q<Label>("attitudeLabel");
        var accidentRateLabel = root.Q<Label>("accidentRateLabel");

        // 设置显示值
        overallRateLabel.text = driverData.overallRate.ToString();

        // 转换心态值为可读文本
        string attitudeText = driverData.attitude switch
        {
            DriverPara.AttitudeType.Aggressive => "激进",
            DriverPara.AttitudeType.Balanced => "平衡",
            DriverPara.AttitudeType.Conservative => "保守",
            _ => "未知"
        };
        attitudeLabel.text = attitudeText;

        accidentRateLabel.text = $"{driverData.accidentRate*100:F1}%";
    }




}