using System.Collections.Generic;
using UnityEngine;

public class RaceTimeManager : MonoBehaviour
{
    public static RaceTimeManager Instance { get; private set; }

    public struct CarLapData
    {
        public string carId;
        public int checkpointIndex;
        public float checkpointTime;
        public float lapTime;
    }

    private Dictionary<string, CarLapData> _carTimings = new Dictionary<string, CarLapData>();
    private List<string> _sortedCarIds = new List<string>();

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
    }

    public void UpdateCarCheckpoint(string carId, int checkpointIndex, float time)
    {
        if (!_carTimings.ContainsKey(carId))
        {
            _carTimings[carId] = new CarLapData
            {
                carId = carId,
                checkpointIndex = checkpointIndex,
                checkpointTime = time,
                lapTime = time
            };
            _sortedCarIds.Add(carId);
        }
        else
        {
            var data = _carTimings[carId];
            data.checkpointIndex = checkpointIndex;
            data.checkpointTime = time;
            data.lapTime = time;
            _carTimings[carId] = data;
        }

        // ÖØÐÂÅÅÐò
        _sortedCarIds.Sort((a, b) => _carTimings[a].lapTime.CompareTo(_carTimings[b].lapTime));
    }

    public List<CarLapData> GetRankingsAroundCar(string carId, int countBefore, int countAfter)
    {
        List<CarLapData> result = new List<CarLapData>();

        int index = _sortedCarIds.IndexOf(carId);
        if (index == -1) return result;

        int start = Mathf.Max(0, index - countBefore);
        int end = Mathf.Min(_sortedCarIds.Count - 1, index + countAfter);

        for (int i = start; i <= end; i++)
        {
            result.Add(_carTimings[_sortedCarIds[i]]);
        }

        return result;
    }
}