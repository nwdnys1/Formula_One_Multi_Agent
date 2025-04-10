using UnityEngine;

[CreateAssetMenu(fileName = "DriverPara", menuName = "Scriptable Objects/DriverPara")]
public class DriverPara : ScriptableObject
{
    [Header("Driver Attributes")]
    [Range(0, 100)] public float overallRate = 50f;

    public enum AttitudeType { Aggressive, Balanced, Conservative }
    public AttitudeType attitude = AttitudeType.Balanced;

    [Range(0f, 1f)] public float accidentRate = 0.1f;

    // 根据心态自动调整事故率
    private void OnValidate()
    {
        switch (attitude)
        {
            case AttitudeType.Aggressive:
                accidentRate = Mathf.Clamp(accidentRate, 0.15f, 1f);
                break;
            case AttitudeType.Balanced:
                accidentRate = Mathf.Clamp(accidentRate, 0.05f, 0.3f);
                break;
            case AttitudeType.Conservative:
                accidentRate = Mathf.Clamp(accidentRate, 0f, 0.1f);
                break;
        }
    }
}
