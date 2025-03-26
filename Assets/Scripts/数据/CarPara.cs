using UnityEngine;

[CreateAssetMenu(fileName = "CarPara", menuName = "Scriptable Objects/CarPara")]
public class CarPara : ScriptableObject
{
    // 赛车性能
    [Header("Speed Attributes")]
    [Tooltip("Top speed in km/h")]
    public float topSpeed = 320f;

    [Tooltip("Acceleration in m/s^2")]
    public float acceleration = 15f;

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

    public enum SetupRating { Disaster, Low, Medium, High, Perfect }

    public SetupRating GetOversteerRating()
    {
        float frontDiff = Mathf.Abs(frontWingAngle - bestFrontWingAngle);
        float camberDiff = Mathf.Abs(tyreCamber - bestTyreCamber);
        return GetRatingFromDifference(frontDiff + camberDiff);
    }

    public SetupRating GetBrakingStabilityRating()
    {
        float antiRollDiff = Mathf.Abs(antiRollDistribution - bestAntiRollDistribution);
        float camberDiff = Mathf.Abs(tyreCamber - bestTyreCamber);
        return GetRatingFromDifference(antiRollDiff + camberDiff);
    }

    private SetupRating GetRatingFromDifference(float difference)
    {
        if (difference < 0.5f) return SetupRating.Perfect;
        if (difference < 1f) return SetupRating.High;
        if (difference < 2f) return SetupRating.Medium;
        if (difference < 3f) return SetupRating.Low;
        return SetupRating.Disaster;
    }

    // 轮胎参数
    public enum TyreType { Hard, Medium, Soft }

    [Header("Tyre Attributes")]
    public TyreType tyreType = TyreType.Medium;
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
            case TyreType.Hard: return hardTyreLife;
            case TyreType.Medium: return mediumTyreLife;
            case TyreType.Soft: return softTyreLife;
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
}
