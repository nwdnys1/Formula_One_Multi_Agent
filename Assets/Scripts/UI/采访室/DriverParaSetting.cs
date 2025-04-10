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


        attitudeLabel.text = driverData.attitude;

        accidentRateLabel.text = $"{driverData.accidentRate * 100:F1}%";
    }




}