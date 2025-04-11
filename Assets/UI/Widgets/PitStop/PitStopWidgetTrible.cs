using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class PitStopWidgetTrible : MonoBehaviour
{
    public UIDocument uiDocument;

    [Header("Input Parameters")]
    public int[][] pitStopLaps = { new int[] { 15, 30 }, new int[] { 12, 32 }, new int[] { 10 } }; // 换胎的圈数
    public string[][] tyreStrategies = {
        new string[] { "hard", "medium", "soft" },
        new string[] { "hard", "medium", "soft" },
        new string[] { "hard", "soft" }
    }; // 轮胎策略
    public int[] fuelReleaseLaps = { 12, 10, 8 }; // 放油圈数
    public int[] ERSReleaseLaps = { 8, 6, 4 }; // ERS释放圈数
    public int totalLaps = 58; // 总圈数

    private static readonly Color HardColor = Color.white;
    private static readonly Color MediumColor = Color.yellow;
    private static readonly Color SoftColor = Color.red;

    private VisualElement root;
    private List<VisualElement> lapContainers; // 使用 List 存储 lapContainer

    private void OnEnable()
    {

        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned in the PitStopWidget script.");
            return;
        }

        root = uiDocument.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("Root Visual Element is null. Check if the UXML file is correctly assigned to UIDocument.");
            return;
        }

        GenerateLapDisplay();
    }

    public void UpdateStrategy(int[][] newPitStops, string[][] tyreTypes, int[] fuels, int[] ers)
    {
        pitStopLaps = newPitStops;
        tyreStrategies = tyreTypes;
        fuelReleaseLaps = fuels;
        ERSReleaseLaps = ers;
        foreach (var item in lapContainers)
        {
            item.Clear();
        }
        GenerateLapDisplay();
    }
   
    private void AddMarkers(VisualElement lapElement, int lapNumber, string markerType)
    {
        var marker = new Label
        {
            text = markerType == "pit" ? lapNumber.ToString() : markerType == "fuel" ? "F" : "E",
            name = $"{markerType}Marker"
        };

        marker.style.color = Color.white;
        marker.style.fontSize = markerType == "pit" ? 26 : 10;
        marker.style.unityTextAlign = TextAnchor.MiddleCenter;
        marker.style.marginTop = 40;

        var markerBackground = new VisualElement
        {
            style = {
                width = markerType == "pit" ? 30 : 15,
                height = markerType == "pit" ? 30 : 15,
                unityTextAlign = TextAnchor.MiddleCenter
            }
        };

        markerBackground.Add(marker);
        lapElement.Add(markerBackground);
    }

    private void GenerateLapDisplayForStrategy(int strategyIndex)
    {
        if (strategyIndex >= pitStopLaps.Length || strategyIndex >= tyreStrategies.Length || strategyIndex >= fuelReleaseLaps.Length || strategyIndex >= ERSReleaseLaps.Length)
        {
            Debug.LogError($"Invalid strategy index: {strategyIndex}");
            return;
        }

        // 获取对应的 lapContainer
        var lapContainer = root.Q<VisualElement>($"lapContainer{strategyIndex}");
        if (lapContainer == null)
        {
            Debug.LogError($"lapContainer{strategyIndex} not found in the UXML file.");
            return;
        }

        lapContainers[strategyIndex] = lapContainer; // 将找到的容器存储到 lapContainers 中

        var tyreColorMap = new System.Collections.Generic.Dictionary<string, Color>
        {
            { "hard", HardColor },
            { "medium", MediumColor },
            { "soft", SoftColor }
        };

        int currentTyreIndex = 0;

        for (int lap = 1; lap <= totalLaps; lap++)
        {
            var lapElement = new VisualElement
            {
                name = $"lap_{lap}",
                style = {
                    width = 8,
                    height = 20,
                    marginRight = 0,
                    position = Position.Relative
                }
            };

            if (currentTyreIndex < tyreStrategies[strategyIndex].Length)
            {
                lapElement.style.backgroundColor = tyreColorMap[tyreStrategies[strategyIndex][currentTyreIndex]];
            }

            if (lap == fuelReleaseLaps[strategyIndex])
            {
                lapElement.style.backgroundColor = new Color(1f, 0.5f, 0f); // 橙色
            }
            else if (lap == ERSReleaseLaps[strategyIndex])
            {
                lapElement.style.backgroundColor = Color.blue; // 蓝色
            }

            if (IsPitStopLap(lap, pitStopLaps[strategyIndex]))
            {
                AddMarkers(lapElement, lap, "pit");
                currentTyreIndex++;
            }
            else if (lap == ERSReleaseLaps[strategyIndex])
            {
                AddMarkers(lapElement, lap, "ERS");
            }
            else if (lap == fuelReleaseLaps[strategyIndex])
            {
                AddMarkers(lapElement, lap, "fuel");
            }

            lapContainer.Add(lapElement);

            if (currentTyreIndex >= tyreStrategies[strategyIndex].Length)
            {
                currentTyreIndex = 0;
            }
        }
    }

    private bool IsPitStopLap(int lap, int[] pitStopLaps)
    {
        return System.Array.Exists(pitStopLaps, element => element == lap);
    }

    private void GenerateLapDisplay()
    {

        lapContainers = new List<VisualElement>(new VisualElement[3]); // 初始化 List，大小为 3

        for (int i = 0; i < pitStopLaps.Length; i++)
        {
            GenerateLapDisplayForStrategy(i);
        }

    }
}