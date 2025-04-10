using UnityEngine;


[CreateAssetMenu(fileName = "CarPara", menuName = "Scriptable Objects/CarPara")]
public class CarPara : ScriptableObject
{
    // 赛车性能
    [Header("Speed Attributes")]
    [Tooltip("Top speed in km/h")]
    public float topSpeed = 320f / 3.6f;

    [Tooltip("Acceleration in m/s^2")]
    public float acceleration = 50f;

    [Tooltip("DRS effectiveness (0-1)")]
    [Range(0f, 1f)] public float drsEffectiveness = 0.5f;

    [Header("Cornering Attributes")]
    [Tooltip("Cornering ability in G")]
    public float carCornering = 3.5f;

    // 赛车调校
    [Header("Setup Parameters")]
    [Range(0f, 10f)] public float frontWingAngle = 5f;
    [Range(8f, 18f)] public float rearWingAngle = 12.5f;
    [Range(0.1f, 0.9f)] public float antiRollDistribution = 0.3f; // 3:7 ratio
    [Range(-3.5f, -2f)] public float tyreCamber = -3.2f;
    [Range(0f, 1f)] public float toeOut = 0.7f;

    [Header("Best Setup (Abu Dhabi)")]
    public float bestFrontWingAngle = 5f;
    public float bestRearWingAngle = 12.5f;
    public float bestAntiRollDistribution = 0.3f;
    public float bestTyreCamber = -3.2f;
    public float bestToeOut = 0.7f;

    [Header("Setup Result")]
    public int oversteerRating = 1; //1-5
    public int brakingStabilityRating = 1; //1-5
    public int corneringRating = 1; //1-5
    public int tractionRating = 1; //1-5
    public int straightsRating = 1; //1-5

    public void CalculateTuningRatings()
    {

        // 计算各参数匹配度（0-1，1表示完全匹配）
        float frontWingMatch = 1 - Mathf.Abs(frontWingAngle - bestFrontWingAngle) / 10f;
        float rearWingMatch = 1 - Mathf.Abs(rearWingAngle - bestRearWingAngle) / 10f;
        float antiRollMatch = 1 - Mathf.Abs(antiRollDistribution - bestAntiRollDistribution) / 2f;
        float camberMatch = 1 - Mathf.Abs(tyreCamber - bestTyreCamber) / 1.5f;
        float toeOutMatch = 1 - Mathf.Abs(toeOut - bestToeOut);

        // 计算5个评分（基于影响参数加权）
        oversteerRating = Mathf.Clamp((int)((frontWingMatch * 0.5f + camberMatch * 0.5f) * 5f + 0.5f), 1, 5);
        brakingStabilityRating = Mathf.Clamp((int)((antiRollMatch * 0.6f + camberMatch * 0.4f) * 5f + 0.5f), 1, 5);
        corneringRating = Mathf.Clamp((int)((frontWingMatch * 0.7f + toeOutMatch * 0.3f) * 5f + 0.5f), 1, 5);
        tractionRating = Mathf.Clamp((int)((rearWingMatch * 0.6f + toeOutMatch * 0.4f) * 5f + 0.5f), 1, 5);
        straightsRating = Mathf.Clamp((int)(rearWingMatch * 5f + 0.5f), 1, 5);


    }


    // 轮胎参数

    [Header("Tyre Attributes")]
    public string tyreType = "medium";
    [Range(0f, 1f)] public float wearRate = 1f;
    [Range(0f, 100f)] public float currentWear = 100f;

    [Header("Tyre Life")]
    public int hardTyreLife = 40;
    public int mediumTyreLife = 32;
    public int softTyreLife = 25;

    public int GetMaxLaps()
    {
        switch (tyreType)
        {
            case "hard": return hardTyreLife;
            case "medium": return mediumTyreLife;
            case "soft": return softTyreLife;
            default: return mediumTyreLife;
        }
    }

    public void UpdateWear(float deltaTime, float speedFactor)
    {
        float wearPerSecond = (1f / GetMaxLaps()) * speedFactor;
        currentWear = Mathf.Clamp(currentWear - wearPerSecond * deltaTime, 0f, 100f);
    }

    // 赛中参数

    [Header("ERS System")]
    public bool ersAvailable = true;
    public float ersSpeedBoost = 15f; // km/h
    public float ersAccelerationBoost = 2f; // m/s^2
    public float ersDuration = 120f; // seconds

    [Header("Fuel Release")]
    public bool fuelReleaseAvailable = true;
    public float fuelSpeedBoost = 7.5f; // km/h
    public float fuelAccelerationBoost = 1f; // m/s^2
    public float fuelDuration = 120f; // seconds

    [Header("LOGO")]
    public Texture2D logoTexture;
}
