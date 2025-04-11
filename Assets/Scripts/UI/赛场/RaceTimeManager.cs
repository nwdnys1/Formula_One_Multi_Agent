using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DTO;
using LitJson;
using UnityEngine.SceneManagement;
public class RaceTimeManager : MonoBehaviour
{
    public static RaceTimeManager Instance { get; private set; }
    SocketClient client = SocketClient.Instance;

    public struct CarRaceData
    {
        public string carId;        // 车辆ID
        public int lapIdx;      // 当前是第几圈
        public int ckptIdx;     // 当前圈的第几个检查点
        public float ckptTime;  // 到达当前检查点的时间
        public Texture2D logo;   // 车队logo
        public int pitCount; // 进站次数
        public string tyreType; // 轮胎类型
        public float tyreWear; // 轮胎磨损
        public bool finished; // 是否完成比赛
        public CarRaceData(string id, int lap, int ckpt, float time, Texture2D teamlogo, int pitCnt, string tyreType, float tyreWear, bool finished)
        {
            carId = id;
            lapIdx = lap;
            ckptIdx = ckpt;
            ckptTime = time;
            logo = teamlogo;
            pitCount = pitCnt;
            this.tyreType = tyreType;
            this.tyreWear = tyreWear;
            this.finished = finished;
        }
    }
    [SerializeField]
    private List<CarRaceData> _rankingData = new List<CarRaceData>();
    [SerializeField]
    private List<float> fastestTime = new List<float>();// 记录每个检查点的最快时间 如果是第二圈 就是16+i
    public RaceUI raceUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        if (raceUI == null)
        {
            Debug.LogError("RaceUI is not assigned in the inspector.");
        }
    }
    private void Start()
    {
        InvokeRepeating("update2LLM", 5f, 60f);
    }

    public void UpdateCarCheckpoint(string carId, int lapIdx, int ckptIdx, float time, int pitCnt, string tyreType, float tyreWear, Texture2D logo, bool finished)
    {

        // 查找现有数据
        int existingIndex = _rankingData.FindIndex(c => c.carId == carId);

        var newData = new CarRaceData(carId, lapIdx, ckptIdx, time, logo, pitCnt, tyreType, tyreWear, finished);

        if (existingIndex >= 0)
        {
            _rankingData.RemoveAt(existingIndex);
        }

        // 插入到正确位置以保持排序
        int insertPos = FindInsertPosition(newData);
        _rankingData.Insert(insertPos, newData);

        // 更新UI
        RaceUI.CarRank[] carRanks = new RaceUI.CarRank[_rankingData.Count];
        for (int i = 0; i < _rankingData.Count; i++)
        {
            var data = _rankingData[i];
            float gap = (i == 0) ? 0 : data.ckptTime - _rankingData[i - 1].ckptTime;
            if (gap > 0)
            {
                carRanks[i] = new RaceUI.CarRank
                {
                    driverName = data.carId, // 取车手ID的前3个字符作为车手名字
                    teamId = "Unknown", // 这里可以根据实际情况设置车队ID
                    tireType = "M",     // 这里可以根据实际情况设置轮胎类型
                    gap = gap,
                    logo = data.logo,
                };
            }
            else
            {
                carRanks[i] = new RaceUI.CarRank
                {
                    driverName = data.carId,
                    teamId = "Unknown", // 这里可以根据实际情况设置车队ID
                    tireType = "M",     // 这里可以根据实际情况设置轮胎类型
                    gap = 0,
                    logo = data.logo,
                };
            }
        }
        raceUI.UpdateRanks(carRanks);

        // 检查所有车辆是否完成比赛
        bool allFinished = true;
        foreach (var carData in _rankingData)
        {
            if (!carData.finished)
            {
                allFinished = false;
                break;
            }
        }
        if (allFinished)
        {
            SceneManager.LoadScene("赛后会议室");
        }
    }

    public JsonData Ranks2Json(List<CarRaceData> raceData)
    {
        JsonData ranks = new JsonData();
        for (int i = 0; i < _rankingData.Count; i++)
        {
            var data = _rankingData[i];
            ranks[data.carId] = i + 1; // 1-based ranking
        }
        return ranks;
    }
    public JsonData Time2Json(List<CarRaceData> raceData)
    {
        JsonData time = new JsonData();
        for (int i = 0; i < _rankingData.Count; i++)
        {
            var data = _rankingData[i];
            time[data.carId] = data.ckptTime.ToString("00") + ":" + (data.ckptTime % 1).ToString("F3").Substring(2); // 1:00.000
        }
        return time;
    }
    public JsonData pitcnt2Json(List<CarRaceData> raceData)
    {
        JsonData pitcnt = new JsonData();
        for (int i = 0; i < _rankingData.Count; i++)
        {
            var data = _rankingData[i];
            pitcnt[data.carId] = data.pitCount;
        }
        return pitcnt;
    }
    public JsonData tyre2Json(List<CarRaceData> raceData)
    {
        JsonData tyre = new JsonData();
        for (int i = 0; i < _rankingData.Count; i++)
        {
            var data = _rankingData[i];
            JsonData json = new JsonData();
            json.Add(data.tyreType);
            json.Add((data.tyreWear / 100).ToString("F1"));
            tyre[data.carId] = json;
        }
        return tyre;
    }

    //更新参数给agent
    public void update2LLM()
    {
        string ranks = JsonMapper.ToJson(Ranks2Json(_rankingData));
        string times = JsonMapper.ToJson(Time2Json(_rankingData));
        string pitcnt = JsonMapper.ToJson(pitcnt2Json(_rankingData));
        string tyre = JsonMapper.ToJson(tyre2Json(_rankingData));
        client.Send(JsonStr.Grid_Position_upadate(ranks), (response) => { }, (r) =>
        {
            client.Send(JsonStr.Lap_Time_upadate(times), (response) => { }, (r) =>
            {
                client.Send(JsonStr.Pit_Stop_Count_upadate(pitcnt), (response) => { }, (r) =>
                {
                    client.Send(JsonStr.Tyre_Condition_upadate(tyre), (response) => { }, null);
                });
            }
            );
        }
        );
    }

    // 核心排序逻辑
    private int FindInsertPosition(CarRaceData newData)
    {
        for (int i = 0; i < _rankingData.Count; i++)
        {
            var current = _rankingData[i];

            // 比较圈数
            if (newData.lapIdx > current.lapIdx)
                return i;

            if (newData.lapIdx == current.lapIdx)
            {
                // 比较检查点
                if (newData.ckptIdx > current.ckptIdx)
                    return i;

                if (newData.ckptIdx == current.ckptIdx)
                {
                    // 比较时间
                    if (newData.ckptTime < current.ckptTime)
                        return i;
                }
            }
        }
        return _rankingData.Count;
    }

    // 获取当前排名（从第一名到最后一名的顺序）
    public List<CarRaceData> GetRaceRanking()
    {
        return new List<CarRaceData>(_rankingData); // 返回副本
    }

    // 获取指定车辆的排名（1-based）
    public int GetCarRanking(string carId)
    {
        int index = _rankingData.FindIndex(c => c.carId == carId);
        return index >= 0 ? index + 1 : -1;
    }
}