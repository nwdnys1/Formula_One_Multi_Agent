using UnityEngine;

[CreateAssetMenu(fileName = "DriverPara", menuName = "Scriptable Objects/DriverPara")]
public class DriverPara : ScriptableObject
{
    [Header("Driver Attributes")]
    [Range(0, 100)] public float overallRate = 50f;

    public string attitude = "Balanced"; // 车手心态

    [Range(0f, 1f)] public float accidentRate = 0.1f;

    // 根据心态自动调整事故率
    private void OnValidate()
    {
        switch (attitude)
        {
            case "Optimistic":
                accidentRate = Mathf.Clamp(accidentRate, 0.15f, 1f);
                break;
            case "Balanced":
                accidentRate = Mathf.Clamp(accidentRate, 0.05f, 0.3f);
                break;
            case "Cautious":
                accidentRate = Mathf.Clamp(accidentRate, 0f, 0.1f);
                break;
        }
    }
}
