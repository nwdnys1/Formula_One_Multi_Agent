using UnityEngine;

[CreateAssetMenu(fileName = "EnvPara", menuName = "Scriptable Objects/Environment Parameters")]
public class EnvPara : ScriptableObject
{
    [Range(0, 100)] public float trackTemperature = 25f; // 赛道温度(°C)
    [Range(-20, 50)] public float airTemperature = 20f; // 空气温度(°C)
    [Range(0, 1)] public float trackGrip = 0.8f; // 赛道抓地力(0-1)
    [Range(0, 1)] public float rainfall = 0f; // 降雨量(0-1)
    [Range(0, 1)] public float rubberAmount = 0.5f; // 橡胶颗粒量(0-1)
    [Range(0, 50)] public float windSpeed = 10f; // 风速(km/h)
    [Range(0, 360)] public float windDirection = 0f; // 风向(度)
}
