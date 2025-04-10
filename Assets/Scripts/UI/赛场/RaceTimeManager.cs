using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DTO;
using LitJson;

public class RaceTimeManager : MonoBehaviour
{
    public static RaceTimeManager Instance { get; private set; }
    SocketClient client = SocketClient.Instance;

    public struct CarRaceData
    {
        public string carId;        // ����ID
        public int lapIdx;      // ��ǰ�ǵڼ�Ȧ
        public int ckptIdx;     // ��ǰȦ�ĵڼ�������
        public float ckptTime;  // ���ﵱǰ�����ʱ��
        public Texture2D logo;   // ����logo
        public CarRaceData(string id, int lap, int ckpt, float time, Texture2D teamlogo)
        {
            carId = id;
            lapIdx = lap;
            ckptIdx = ckpt;
            ckptTime = time;
            logo = teamlogo;
        }
    }
    [SerializeField]
    private List<CarRaceData> _rankingData = new List<CarRaceData>();
    [SerializeField]
    private List<float> fastestTime = new List<float>();// ��¼ÿ����������ʱ�� ����ǵڶ�Ȧ ����16+i
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

    public void UpdateCarCheckpoint(string carId, int lapIdx, int ckptIdx, float time)
    {
        // ������������
        int existingIndex = _rankingData.FindIndex(c => c.carId == carId);

        var newData = new CarRaceData(carId, lapIdx, ckptIdx, time, null);

        if (existingIndex >= 0)
        {
            _rankingData.RemoveAt(existingIndex);
        }

        // ���뵽��ȷλ���Ա�������
        int insertPos = FindInsertPosition(newData);
        _rankingData.Insert(insertPos, newData);

        // ����UI
        RaceUI.CarRank[] carRanks = new RaceUI.CarRank[_rankingData.Count];
        for (int i = 0; i < _rankingData.Count; i++)
        {
            var data = _rankingData[i];
            float gap = (i == 0) ? 0 : data.ckptTime - _rankingData[i - 1].ckptTime;
            if (gap > 0)
            {
                carRanks[i] = new RaceUI.CarRank
                {
                    driverName = data.carId,
                    teamId = "Unknown", // ������Ը���ʵ��������ó���ID
                    tireType = "M",     // ������Ը���ʵ�����������̥����
                    gap = gap,
                    logo = data.logo,
                };
            }
            else
            {
                carRanks[i] = new RaceUI.CarRank
                {
                    driverName = data.carId,
                    teamId = "Unknown", // ������Ը���ʵ��������ó���ID
                    tireType = "M",     // ������Ը���ʵ�����������̥����
                    gap = 0,
                    logo = data.logo,
                };
            }
        }
        raceUI.UpdateRanks(carRanks);
        string ranks = Ranks2Json(_rankingData).ToString();
        // ���͸�llm���и���
        client.Send(JsonStr.Grid_Position_upadate(ranks), (response) => { }, null);
        string times = Time2Json(_rankingData).ToString();
        // ���͸�llm���и���
        client.Send(JsonStr.Lap_Time_upadate(times), (response) => { }, null);
    }

    // ��rankingsת��Ϊjsondata
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
    // ��Ȧ��ת��Ϊjsondata ʱ���ʽ����1��00.000
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



    // ���������߼�
    private int FindInsertPosition(CarRaceData newData)
    {
        for (int i = 0; i < _rankingData.Count; i++)
        {
            var current = _rankingData[i];

            // �Ƚ�Ȧ��
            if (newData.lapIdx > current.lapIdx)
                return i;

            if (newData.lapIdx == current.lapIdx)
            {
                // �Ƚϼ���
                if (newData.ckptIdx > current.ckptIdx)
                    return i;

                if (newData.ckptIdx == current.ckptIdx)
                {
                    // �Ƚ�ʱ��
                    if (newData.ckptTime < current.ckptTime)
                        return i;
                }
            }
        }
        return _rankingData.Count;
    }

    // ��ȡ��ǰ�������ӵ�һ�������һ����˳��
    public List<CarRaceData> GetRaceRanking()
    {
        return new List<CarRaceData>(_rankingData); // ���ظ���
    }

    // ��ȡָ��������������1-based��
    public int GetCarRanking(string carId)
    {
        int index = _rankingData.FindIndex(c => c.carId == carId);
        return index >= 0 ? index + 1 : -1;
    }
}