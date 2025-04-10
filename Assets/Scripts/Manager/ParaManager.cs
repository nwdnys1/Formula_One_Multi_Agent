using UnityEngine;
using System.Collections.Generic;


public class ParaManager : MonoBehaviour
{
    public static ParaManager Instance { get; private set; }

    [Header("Parameter Assets")]
    public Dictionary<string, DriverPara> driverParasPreset = new Dictionary<string, DriverPara>();
    public Dictionary<string, CarPara> carParasPreset = new Dictionary<string, CarPara>();
    public ParaList paraList;
    private Dictionary<string, DriverPara> driverParasCurrent = new Dictionary<string, DriverPara>();
    private Dictionary<string, CarPara> carParasCurrent = new Dictionary<string, CarPara>();
    [Header("Environment Parameters")]
    public EnvPara envParaPreset;
    public EnvPara envPara;

    public EnvPara GetEnvPara()
    {
        return envPara;
    }

    public DriverPara getDriverPara(string id)
    {
        if (driverParasCurrent.TryGetValue(id, out DriverPara config))
        {
            return config;
        }
        else
        {
            Debug.LogError($"未找到角色配置: {id}");
            return null;
        }
    }

    public CarPara getCarPara(string id)
    {
        if (carParasCurrent.TryGetValue(id, out CarPara config))
        {
            return config;
        }
        else
        {
            Debug.LogError($"未找到赛车配置: {id}");
            return null;
        }
    }

    public void setDriverPara(string id, DriverPara config)
    {
        if (!driverParasCurrent.ContainsKey(id))
        {
            Debug.LogError($"未找到角色配置: {id}");
            return;
        }
        driverParasCurrent[id] = config;
    }

    public void setCarPara(string id, CarPara config)
    {
        if (!carParasCurrent.ContainsKey(id))
        {
            Debug.LogError($"未找到赛车配置: {id}");
            return;
        }
        carParasCurrent[id] = config;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 从资产初始化字典
            foreach (DriverPara config in paraList.driverParas)
            {
                driverParasPreset.Add(config.name, config);

            }

            foreach (CarPara config in paraList.carParas)
            {
                carParasPreset.Add(config.name, config);
            }
            // 赋值
            driverParasCurrent = new Dictionary<string, DriverPara>(driverParasPreset);
            carParasCurrent = new Dictionary<string, CarPara>(carParasPreset);
            envPara = Instantiate(envParaPreset);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}