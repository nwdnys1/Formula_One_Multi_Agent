using UnityEngine;
using UnityEngine.UIElements;

public class PracticeUI : MonoBehaviour
{
    private UIDocument _uiDocument;

    // Car Setup Elements
    private Label _frontWingAngleLabel;
    private Label _rearWingAngleLabel;
    private Label _antiRollDistributionLabel;
    private Label _tyreCamberLabel;
    private Label _toeOutLabel;

    // Driver Satisfaction Elements
    private Label _oversteerLabel;
    private Label _brakingStabilityLabel;
    private Label _corneringLabel;
    private Label _tractionLabel;
    private Label _straightsLabel;

    // Car Performance Elements
    private Label _topSpeedLabel;
    private Label _accelerationLabel;
    private Label _drsEffectivenessLabel;
    private Label _carCorneringLabel;

    // Team Discussion Elements
    private Label _mechanicMessageLabel;
    private Label _hamiltonMessageLabel;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();

        var root = _uiDocument.rootVisualElement;

        // Initialize all UI elements
        InitializeCarSetupElements(root);
        InitializeDriverSatisfactionElements(root);
        InitializeCarPerformanceElements(root);
        InitializeTeamDiscussionElements(root);
    }

    private void InitializeCarSetupElements(VisualElement root)
    {
        _frontWingAngleLabel = root.Q<Label>("Front-Wing-Angle");
        _rearWingAngleLabel = root.Q<Label>("Rear-Wing-Angle");
        _antiRollDistributionLabel = root.Q<Label>("Anti-Roll-Distribution");
        _tyreCamberLabel = root.Q<Label>("Tyre-Camber");
        _toeOutLabel = root.Q<Label>("Toe-Out");
    }

    private void InitializeDriverSatisfactionElements(VisualElement root)
    {
        _oversteerLabel = root.Q<Label>("Oversteer");
        _brakingStabilityLabel = root.Q<Label>("Braking-Stability");
        _corneringLabel = root.Q<Label>("Cornering");
        _tractionLabel = root.Q<Label>("Traction");
        _straightsLabel = root.Q<Label>("Straights");
    }

    private void InitializeCarPerformanceElements(VisualElement root)
    {
        _topSpeedLabel = root.Q<Label>("Top-Speed");
        _accelerationLabel = root.Q<Label>("Acceleration");
        _drsEffectivenessLabel = root.Q<Label>("DRS-Effectiveness");
        _carCorneringLabel = root.Q<Label>("Car-Cornering");
    }

    private void InitializeTeamDiscussionElements(VisualElement root)
    {
        _mechanicMessageLabel = root.Q<Label>("Has-everyone-reviewed-the-latest-setup-changes");
        _hamiltonMessageLabel = root.Q<Label>("Yes-the-new-wing-configuration-looks-promising");
    }

    // Method 1: Update Car Setup Parameters
    public void UpdateCarSetup(string frontWingAngle, string rearWingAngle,
                             string antiRollDistribution, string tyreCamber,
                             string toeOut)
    {
        _frontWingAngleLabel.text = frontWingAngle;
        _rearWingAngleLabel.text = rearWingAngle;
        _antiRollDistributionLabel.text = antiRollDistribution;
        _tyreCamberLabel.text = tyreCamber;
        _toeOutLabel.text = toeOut;
    }

    // Method 2: Update Driver Satisfaction
    public void UpdateDriverSatisfaction(string oversteer, string brakingStability,
                                        string cornering, string traction,
                                        string straights)
    {
        _oversteerLabel.text = oversteer;
        _brakingStabilityLabel.text = brakingStability;
        _corneringLabel.text = cornering;
        _tractionLabel.text = traction;
        _straightsLabel.text = straights;
    }

    // Method 3: Update Car Performance
    public void UpdateCarPerformance(string topSpeed, string acceleration,
                                    string drsEffectiveness, string carCornering)
    {
        _topSpeedLabel.text = topSpeed;
        _accelerationLabel.text = acceleration;
        _drsEffectivenessLabel.text = drsEffectiveness;
        _carCorneringLabel.text = carCornering;
    }

    // Method 4: Update Team Discussion
    public void UpdateTeamDiscussion(string mechanicMessage, string hamiltonMessage)
    {
        _mechanicMessageLabel.text = mechanicMessage;
        _hamiltonMessageLabel.text = hamiltonMessage;
    }
}