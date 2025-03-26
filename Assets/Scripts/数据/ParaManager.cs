using UnityEngine;
using System.Collections.Generic;


public class ParaManager : MonoBehaviour
{
    public static ParaManager Instance { get; private set; }

    [Header("Parameter Assets")]
    public Dictionary<string, DriverPara> driverParas = new Dictionary<string, DriverPara>();
    public Dictionary<string, CarPara> carParas = new Dictionary<string, CarPara>();
    public ParaList paraList;

    public DriverPara getDriverPara(string id)
    {
        if (driverParas.TryGetValue(id, out DriverPara config))
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
        if (carParas.TryGetValue(id, out CarPara config))
        {
            return config;
        }
        else
        {
            Debug.LogError($"未找到赛车配置: {id}");
            return null;
        }
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
                driverParas.Add(config.name, config);
                print(config.name);
            }

            foreach (CarPara config in paraList.carParas)
            {
                carParas.Add(config.name, config);
                print(config.name);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}