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
        DisplayDriver("1��");
    }

    private void InitializeUI()
    {
        DisplayDriver("1��");


    }

    public void DisplayDriver(string driverId)
    {
        var driverData = ParaManager.Instance.getDriverPara(driverId);

        // ��ȡUIԪ������
        var root = _uiDocument.rootVisualElement;
        var overallRateLabel = root.Q<Label>("overallRateLabel");
        var attitudeLabel = root.Q<Label>("attitudeLabel");
        var accidentRateLabel = root.Q<Label>("accidentRateLabel");

        // ������ʾֵ
        overallRateLabel.text = driverData.overallRate.ToString();

        // ת����ֵ̬Ϊ�ɶ��ı�
        string attitudeText = driverData.attitude switch
        {
            DriverPara.AttitudeType.Aggressive => "����",
            DriverPara.AttitudeType.Balanced => "ƽ��",
            DriverPara.AttitudeType.Conservative => "����",
            _ => "δ֪"
        };
        attitudeLabel.text = attitudeText;

        accidentRateLabel.text = $"{driverData.accidentRate*100:F1}%";
    }




}