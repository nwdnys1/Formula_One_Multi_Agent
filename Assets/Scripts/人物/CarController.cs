using UnityEngine;
using UnityEngine.AI;
using Cinemachine;



public class CarController : MonoBehaviour
{
    [System.Serializable]
    public class TrackSegment
    {
        public enum SegmentType { Straight, Corner }
        public SegmentType type;
        public int startCheckpointIndex;
        public int endCheckpointIndex;
    }

    public string carId;
    private CarPara para;

    [Header("导航设置")]
    public Transform checkpointsParent;
    public float arrivalDistance = 1f;
    public TrackSegment[] trackSegments; // 在Inspector中设置每个赛道段的类型

    [Header("性能参数")]
    public float straightTopSpeed = 320f / 3.6f;
    public float cornerTopSpeed = 100f / 3.6f;
    public float acceleration = 50f;
    public float brakingDeceleration = 50f;

    public Transform[] _checkpoints;
    public int _currentIndex = 0;
    private NavMeshAgent _agent;
    public TrackSegment.SegmentType _currentSegmentType;
    private bool _isBraking = false;

    [Header("轮胎系统")]
    public CarPara.TyreType tyreType = CarPara.TyreType.Hard; // 轮胎类型
    public float currentTyreWear = 100f; // 当前轮胎磨损率(0-100%)
    public float tyreWearEffectThreshold = 30f; // 磨损影响性能的阈值
    public float maxSpeedReduction = 0.3f; // 最大速度减少比例(30%)

    [Header("ERS系统")]
    public bool ersAvailable = true;
    public float ersSpeedBoost = 15f; // km/h
    public float ersAccelerationBoost = 2f; // m/s²
    public float ersDuration = 10f; // 秒
    public float ersTimer = 0f;
    public bool ersActive = false;

    [Header("油量释放")]
    public bool fuelReleaseAvailable = true;
    public float fuelSpeedBoost = 7.5f; // km/h
    public float fuelAccelerationBoost = 1f; // m/s²
    public float fuelDuration = 10f; // 秒
    public float fuelTimer = 0f;
    public bool fuelActive = false;

    [Header("车手事故参数")]
    public string driverId; // 车手ID
    public float accidentRate; // 事故率(0-1)
    public float accidentCheckInterval = 10f; // 事故检测间隔(秒)
    private float accidentCheckTimer = 0f;
    private DriverPara driverPara;

    [Header("虚拟相机")]
    public CinemachineVirtualCamera car3rd;

    // 在CarController类中添加
    private float _raceStartTime;
    private float _lastCheckpointTime;

    void Start()
    {   CameraManager.Instance.SetCamera(car3rd);
        // 从管理器获取赛车参数
        para = ParaManager.Instance.getCarPara(carId);

        // 应用初始性能参数
        straightTopSpeed = para.topSpeed;
        acceleration = para.acceleration;
        tyreType = para.tyreType;
        // 弯道速度基于过弯能力计算
        //cornerTopSpeed = para.topSpeed * (para.carCornering / 5f); // 假设5G为基准

        // 获取车手参数
        driverPara = ParaManager.Instance.getDriverPara(driverId);
        if (driverPara != null)
        {
            accidentRate = driverPara.accidentRate;
            Debug.Log($"车手{driverId}事故率: {accidentRate * 100}%");
        }
        else
        {
            Debug.LogError($"未找到车手ID: {driverId}");
        }

        // 初始化NavMeshAgent
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = 0; // 初始速度为0
        //_agent.acceleration = acceleration;

        _raceStartTime = Time.time;
        _lastCheckpointTime = _raceStartTime;

        // 初始化检查点
        if (checkpointsParent != null)
        {
            _checkpoints = new Transform[checkpointsParent.childCount];
            for (int i = 0; i < checkpointsParent.childCount; i++)
            {
                Transform ckpt = checkpointsParent.GetChild(i);
                if (Physics.Raycast(ckpt.position, Vector3.down, out RaycastHit hit))
                {
                    ckpt.position = hit.point;
                }
                _checkpoints[i] = ckpt;
            }

            // 确定初始赛道段类型
            UpdateCurrentSegmentType();
            SetNextDestination();
        }
        else
        {
            Debug.LogError("未指定Checkpoints父对象！");
        }


    }

    void Update()
    {
        if (_agent.pathPending || _checkpoints == null || _currentIndex >= _checkpoints.Length)
            return;

        if (currentTyreWear <= 0)
        {
            TriggerTyreBlowout();
            return;
        }

        // 检查是否到达当前目标点
        float distanceToTarget = Vector3.Distance(transform.position, _checkpoints[_currentIndex].position);

        // 根据距离决定是否需要刹车
        _isBraking = distanceToTarget < CalculateBrakingDistance();

        // 应用当前速度限制
        UpdateTyreWear();
        UpdateSpeedLimit();

        // 定时事故检测
        UpdateAccidentCheck();

        // 检查是否到达检查点
        if (distanceToTarget <= arrivalDistance)
        {
            float currentTime = Time.time;
            float checkpointTime = currentTime - _lastCheckpointTime;
            RaceTimeManager.Instance.UpdateCarCheckpoint(carId, _currentIndex, checkpointTime);
            _lastCheckpointTime = currentTime;

            _currentIndex++;
            if (_currentIndex < _checkpoints.Length)
            {
                UpdateCurrentSegmentType();
                SetNextDestination();
            }
            else
            {
                Debug.Log("已完成所有检查点！");
                _agent.isStopped = true;
            }
        }

        // 检测按键输入
        if (Input.GetKeyDown(KeyCode.Alpha1) && ersAvailable)
        {
            ActivateERS();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && fuelReleaseAvailable)
        {
            ActivateFuelRelease();
        }

        // 更新ERS计时器
        if (ersActive)
        {
            ersTimer += Time.deltaTime;
            print($"ERS计时器: {ersTimer:F1}/{ersDuration:F1}秒");
            if (ersTimer >= ersDuration)
            {
                DeactivateERS();
            }
        }

        // 更新油量释放计时器
        if (fuelActive)
        {
            fuelTimer += Time.deltaTime;
            if (fuelTimer >= fuelDuration)
            {
                DeactivateFuelRelease();
            }
        }
    }


    private void UpdateCurrentSegmentType()
    {
        foreach (var segment in trackSegments)
        {
            if (_currentIndex >= segment.startCheckpointIndex && _currentIndex < segment.endCheckpointIndex)
            {
                _currentSegmentType = segment.type;
                break;
            }
        }
    }
    // 更新轮胎磨损
    private void UpdateTyreWear()
    {

        // 根据轮胎类型计算磨损速度
        float wearRate = 0f;
        switch (tyreType)
        {
            case CarPara.TyreType.Hard:
                wearRate = 40f; // Hard轮胎40圈磨损完
                break;
            case CarPara.TyreType.Medium:
                wearRate = 32f; // Medium轮胎32圈磨损完
                break;
            case CarPara.TyreType.Soft:
                wearRate = 25f; // Soft轮胎25圈磨损完
                break;
        }

        // 更新当前磨损
        currentTyreWear -= wearRate * 0.01f * Time.deltaTime;
        currentTyreWear = Mathf.Clamp(currentTyreWear, 0f, 100f);


    }

    private void UpdateSpeedLimit()
    {
        float targetSpeed = _currentSegmentType == TrackSegment.SegmentType.Straight ? straightTopSpeed : cornerTopSpeed;
        float realAcceleration = acceleration;
        if (_isBraking)
        {
            // 刹车逻辑
            _agent.speed = Mathf.Max(0, _agent.speed - brakingDeceleration * Time.deltaTime);
        }
        else
        {
            // 加速逻辑
            // 计算当前轮胎磨损对速度的影响 线性
            float wearEffect = Mathf.Clamp01((100f - currentTyreWear) / (100f - tyreWearEffectThreshold));
            float speedReduction = wearEffect * maxSpeedReduction;
            targetSpeed *= (1 - speedReduction);

            // 应用ERS加成
            targetSpeed += ersActive ? ersSpeedBoost / 3.6f : 0; // 转换为m/s
            realAcceleration += ersActive ? ersAccelerationBoost : 0;

            // 应用油量释放加成
            targetSpeed += fuelActive ? fuelSpeedBoost / 3.6f : 0; // 转换为m/s
            realAcceleration += fuelActive ? fuelAccelerationBoost : 0;

            _agent.speed = Mathf.Min(targetSpeed, _agent.speed + acceleration * Time.deltaTime);
        }
    }

    private float CalculateBrakingDistance()
    {
        // 根据当前速度和减速度计算刹车距离
        float currentSpeed = _agent.velocity.magnitude;
        return (currentSpeed * currentSpeed) / (2 * brakingDeceleration) * 0.5f; // 1.2为安全系数
    }

    private void SetNextDestination()
    {
        if (_currentIndex < _checkpoints.Length)
        {
            _agent.SetDestination(_checkpoints[_currentIndex].position);
        }
    }

    private void ActivateERS()
    {
        if (!ersAvailable) return;

        ersActive = true;
        ersAvailable = false;
        ersTimer = 0f;

        Debug.Log("ERS已激活！");
    }

    private void DeactivateERS()
    {
        ersActive = false;
        Debug.Log("ERS效果结束");
    }

    private void ActivateFuelRelease()
    {
        if (!fuelReleaseAvailable) return;

        fuelActive = true;
        fuelReleaseAvailable = false;
        fuelTimer = 0f;

        Debug.Log("油量释放已激活！");
    }

    private void DeactivateFuelRelease()
    {
        fuelActive = false;
        Debug.Log("油量释放效果结束");
    }

    // 触发爆胎
    private void TriggerTyreBlowout()
    {
        Debug.Log("轮胎爆胎！发生事故！");

        // 立即停车
        _agent.isStopped = true;

        // 将这个对象从赛道上移除
        RemoveCarFromRace();
    }

    // 定时事故检测
    private void UpdateAccidentCheck()
    {
        accidentCheckTimer += Time.deltaTime;

        if (accidentCheckTimer >= accidentCheckInterval)
        {
            accidentCheckTimer = 0f;
            print($"车手{driverId}事故检测");
            CheckForAccident();
            //// 只在弯道检测事故
            //if (_currentSegmentType == TrackSegment.SegmentType.Corner)
            //{
            //    CheckForAccident();
            //}
        }
    }

    // 基础事故检测
    public void CheckForAccident()
    {
        if (Random.value < accidentRate)
        {
            TriggerAccident();
        }
    }

    //// 弯道专用事故检测(备用)
    //public void CheckForCornerAccident()
    //{
    //    if (_currentSegmentType != TrackSegment.SegmentType.Corner) return;

    //    // 根据车手心态调整事故率
    //    float adjustedRate = accidentRate;
    //    switch (driverPara.attitude)
    //    {
    //        case DriverPara.Attitude.Aggressive:
    //            adjustedRate *= 1.5f; // 积极心态增加50%事故率
    //            break;
    //        case DriverPara.Attitude.Conservative:
    //            adjustedRate *= 0.7f; // 保守心态减少30%事故率
    //            break;
    //    }

    //    if (Random.value < adjustedRate)
    //    {
    //        TriggerAccident();
    //    }
    //}

    // 触发事故
    private void TriggerAccident()
    {
        Debug.Log($"车手{driverId}发生事故！");

        // 1. 赛车原地旋转180度
        SpinCar();

        // 2. 撞墙效果(这里只打印日志，实际可以添加粒子效果等)
        Debug.Log("赛车撞墙！");

        // 3. 触发安全车(这里只打印日志)
        Debug.Log("安全车出动！");

        // 4. 赛车从赛道上消失
        //RemoveCarFromRace();
    }

    private void SpinCar()
    {
        float spinDuration = 2f;
        float spinSpeed = 180f / spinDuration; // 每秒旋转90度

        _agent.isStopped = true;

        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
            elapsed += Time.deltaTime;

        }
    }

    private void RemoveCarFromRace()
    {
        gameObject.SetActive(false);
        Debug.Log($"车手{driverId}已退出比赛");
    }
}