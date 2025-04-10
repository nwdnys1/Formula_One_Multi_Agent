using UnityEngine;
using UnityEngine.UIElements;

public class PitStopWidget : MonoBehaviour
{
    public UIDocument uiDocument;

    [Header("Input Parameters")]
    public int[] pitStopLaps = { 15, 30 }; // 换胎的圈数
    public string[] tyreStrategy = { "hard", "medium", "soft" }; // 轮胎策略
    public int fuel_release_laps = 12;
    public int ERS_release_laps = 8;
    public int totalLaps = 58; // 总圈数

    private static readonly Color HardColor = Color.white;
    private static readonly Color MediumColor = Color.yellow;
    private static readonly Color SoftColor = Color.red;

    private VisualElement root;
    private VisualElement lapContainer;

    private void OnEnable()
    {
        Debug.Log("PitStopWidget OnEnable called.");

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

        Debug.Log("Root Visual Element found. Generating lap display.");
        GenerateLapDisplay();
    }

    public void UpdateStrategy(int[] newPitStops, string[] tyreTypes)
    {
        pitStopLaps = newPitStops;
        tyreStrategy = tyreTypes;
        lapContainer.Clear();
        GenerateLapDisplay();
    }

    private void AddPitStopMarker(VisualElement lapElement, int lapNumber)
    {
        // 创建一个 Label 用于显示数字
        var marker = new Label
        {
            text = lapNumber.ToString(), // 显示圈数
            name = "pitStopMarker"
        };

        // 设置数字的样式
        marker.style.color = Color.white; // 黑色文字
        marker.style.fontSize = 26; // 增大字体大小
        marker.style.unityTextAlign = TextAnchor.MiddleCenter; // 居中对齐
        marker.style.marginTop = 40;

        // 设置背景样式（可选）
        var markerBackground = new VisualElement();
        markerBackground.style.width = 30; // 背景宽度
        markerBackground.style.height = 30; // 背景高度
        markerBackground.style.unityTextAlign = TextAnchor.MiddleCenter;

        // 将数字添加到背景中
        markerBackground.Add(marker);

        // 将背景添加到 lapElement 中，并确保它在最上层
        lapElement.Add(markerBackground);
    }

    private void AddFuelMarker(VisualElement lapElement, int lapNumber){
        // 创建一个 Label 用于显示数字
        var marker = new Label
        {
            text = "F",
            name = "fuelMarker"
        };

        // 设置数字的样式
        marker.style.color = Color.white; // 黑色文字
        marker.style.fontSize = 10; // 增大字体大小
        marker.style.unityTextAlign = TextAnchor.MiddleCenter; // 居中对齐
        marker.style.marginTop = 40;

        // 设置背景样式（可选）
        var markerBackground = new VisualElement();
        markerBackground.style.width = 15; // 背景宽度
        markerBackground.style.height = 15; // 背景高度
        markerBackground.style.unityTextAlign = TextAnchor.MiddleCenter;

        // 将数字添加到背景中
        markerBackground.Add(marker);

        // 将背景添加到 lapElement 中，并确保它在最上层
        lapElement.Add(markerBackground);
    }

    private void AddERSMarker(VisualElement lapElement, int lapNumber){
        // 创建一个 Label 用于显示数字
        var marker = new Label
        {
            text = "E",
            name = "ERSMarker"
        };

        // 设置数字的样式
        marker.style.color = Color.white; // 黑色文字
        marker.style.fontSize = 10; // 增大字体大小
        marker.style.unityTextAlign = TextAnchor.MiddleCenter; // 居中对齐
        marker.style.marginTop = 40;

        // 设置背景样式（可选）
        var markerBackground = new VisualElement();
        markerBackground.style.width = 15; // 背景宽度
        markerBackground.style.height = 15; // 背景高度
        markerBackground.style.unityTextAlign = TextAnchor.MiddleCenter;

        // 将数字添加到背景中
        markerBackground.Add(marker);

        // 将背景添加到 lapElement 中，并确保它在最上层
        lapElement.Add(markerBackground);
    }

    private void GenerateLapDisplay()
    {
        if (root == null)
        {
            Debug.LogError("Root Visual Element is null.");
            return;
        }

        lapContainer = root.Q<VisualElement>("lapContainer");

        if (lapContainer == null)
        {
            Debug.LogError("lapContainer is null.");
            return;
        }

        

        var tyreColorMap = new System.Collections.Generic.Dictionary<string, Color>
        {
            { "hard", HardColor },
            { "medium", MediumColor },
            { "soft", SoftColor }
        };

        int currentTyreIndex = 0;

        for (int lap = 1; lap <= totalLaps; lap++)
        {
            var lapElement = new VisualElement();
            lapElement.name = $"lap_{lap}";
            lapElement.AddToClassList("lap");

            // 设置宽度为8，高度缩短为20，并移除右边距
            lapElement.style.width = 8;
            lapElement.style.height = 20; // 高度缩短为20
            lapElement.style.marginRight = 0; // 移除右边距
            lapElement.style.position = Position.Relative; // 设置相对定位

            if (currentTyreIndex < tyreStrategy.Length)
            {
                lapElement.style.backgroundColor = tyreColorMap[tyreStrategy[currentTyreIndex]];
            }

            if (lap == fuel_release_laps)
            {
                // 用十六进制颜色代码设置加油圈的颜色
                lapElement.style.backgroundColor = new Color(1f, 0.5f, 0f); // 设置加油圈的颜色为橙色
            }

            else if (lap == ERS_release_laps)
            {
                // 如果是ERS释放圈，添加数字标记
                lapElement.style.backgroundColor = Color.blue; // 设置ERS释放圈的颜色为蓝色
            }

            if (IsPitStopLap(lap))
            {
                // 如果是换胎圈，添加数字标记
                AddPitStopMarker(lapElement, lap);
                currentTyreIndex++;
            }
            else if (lap == ERS_release_laps)
            {
                // 如果是ERS释放圈，添加数字标记
                AddERSMarker(lapElement, lap);
            }
            else if (lap == fuel_release_laps)
            {
                // 如果是加油圈，添加数字标记
                AddFuelMarker(lapElement, lap);
            }

            lapContainer.Add(lapElement);

            if (currentTyreIndex >= tyreStrategy.Length)
            {
                currentTyreIndex = 0;
            }
        }

    }

    private bool IsPitStopLap(int lap)
    {
        foreach (var pitStopLap in pitStopLaps)
        {
            if (lap == pitStopLap)
            {
                return true;
            }
        }
        return false;
    }
}