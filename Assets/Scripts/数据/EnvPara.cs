using UnityEngine;

[CreateAssetMenu(fileName = "EnvPara", menuName = "Scriptable Objects/Environment Parameters")]
public class EnvPara : ScriptableObject
{
    [Range(0, 100)] public float trackTemperature = 25f; // �����¶�(��C)
    [Range(-20, 50)] public float airTemperature = 20f; // �����¶�(��C)
    [Range(0, 1)] public float trackGrip = 0.8f; // ����ץ����(0-1)
    [Range(0, 1)] public float rainfall = 0f; // ������(0-1)
    [Range(0, 1)] public float rubberAmount = 0.5f; // �𽺿�����(0-1)
    [Range(0, 50)] public float windSpeed = 10f; // ����(km/h)
    [Range(0, 360)] public float windDirection = 0f; // ����(��)
}
