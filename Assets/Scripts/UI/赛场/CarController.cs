using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using DTO;
using LitJson;



public class CarController : MonoBehaviour
{
    public string carId;
    private CarPara para;
    SocketClient client = SocketClient.Instance;
    public RaceController controller;

    [Header("导航设置")]
    public Transform checkpointsParent;
    public float arrivalDistance = 1f;

    [Header("性能参数")]
    public float straightTopSpeed = 320f / 3.6f;
    public float cornerTopSpeed = 100f / 3.6f;
    public float acceleration = 50f;
    public float brakingDeceleration = 50f;

    public Transform[] _checkpoints;
    public int _currentIndex = 0;
    private NavMeshAgent _agent;

    [Header("轮胎系统")]
    public string tyreType = "medium"; // 轮胎类型
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
    public string attitude; // 车手心态
    public float accidentRate; // 事故率(0-1)
    public float accidentCheckInterval = 3f; // 事故检测间隔(秒)
    private float accidentCheckTimer = 0f;
    private DriverPara driverPara;

    [Header("虚拟相机")]
    public CinemachineVirtualCamera car3rd;

    [Header("比赛设置")]
    public int totalLaps = 2; // 总圈数
    public int lapCount = 0; // 当前圈数(0=第1圈)
    public bool raceFinished = false; // 比赛是否完成
    public float _raceStartTime; // 比赛开始时间
    public float ckptTime; // 当前检查点时间

    [Header("比赛策略")]
    public int[] pitStopLaps = { 5, 10 }; // 进站圈数
    public string[] tyreTypes = { "hard", "medium" }; // 轮胎策略
    public int fuelLap = 1; // 进站加油圈数
    public int ERSLap = 1; // 进站充电圈数

    [Header("进站设置")]
    public Transform pitStop; // 单独的进站点
    private bool _isPitting = false; // 是否正在进站
    private int _nextPitStopIndex = 0; // 下一个进站策略的索引
    public int pitCnt = 0; // 进站次数

    [Header("安全车")]
    public bool isSafeCar = false; // 是否安全车
    public GameObject safeCar;



    void Start()
    {
        if (isSafeCar)
        {
            _currentIndex = 2;
        }
        // 从管理器获取赛车参数
        para = ParaManager.Instance.getCarPara(carId);

        // 应用初始性能参数
        straightTopSpeed = para.topSpeed;
        acceleration = para.acceleration;
        tyreType = para.tyreType;
        // 弯道速度基于过弯能力计算
        cornerTopSpeed = para.topSpeed * (para.carCornering / 5f); // 假设5G为基准
        pitStopLaps = para.pitStopLaps;
        tyreTypes = para.tyreTypes;
        fuelLap = para.fuelLap;
        ERSLap = para.ERSLap;


        // 获取车手参数
        driverPara = ParaManager.Instance.getDriverPara(driverId);
        if (driverPara != null)
        {
            accidentRate = driverPara.accidentRate;
            attitude = driverPara.attitude.ToString();
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
        ckptTime = _raceStartTime;

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
            SetNextDestination();
        }
        else
        {
            Debug.LogError("未指定Checkpoints父对象！");
        }


    }

    void Update()
    {
        if (raceFinished || _agent.pathPending || _checkpoints == null)
            return;

        if (currentTyreWear <= 0)
        {
            TriggerTyreBlowout();
            return;
        }

        // 检查是否到达当前目标点
        float distanceToTarget = _isPitting
             ? Vector3.Distance(transform.position, pitStop.position)
             : Vector3.Distance(transform.position, _checkpoints[_currentIndex].position);


        // 应用当前速度限制
        UpdateTyreWear();
        UpdateSpeedLimit();

        // 定时事故检测
        UpdateAccidentCheck();

        // 检查是否到达检查点
        if (distanceToTarget <= arrivalDistance)
        {
            if (isSafeCar)
            {
                if (lapCount == 1 && _currentIndex == 1)
                {
                    gameObject.SetActive(false);
                    controller.hasAccident = false;
                    Debug.Log($"安全车已退出赛道");
                }
                _currentIndex++;
                // 检查是否完成一圈
                if (_currentIndex >= _checkpoints.Length)
                {
                    _currentIndex = 0; // 重置检查点索引
                    lapCount++; // 增加圈数

                }
                SetNextDestination();
                return;
            }
            if (!_isPitting)
            {
                float currentTime = Time.time;
                ckptTime = currentTime - _raceStartTime;
                RaceTimeManager.Instance.UpdateCarCheckpoint(carId, lapCount, _currentIndex, ckptTime, pitCnt, tyreType.ToString(), currentTyreWear, para.logoTexture,raceFinished);
                ckptTime = currentTime;

                // 检查是否需要释放fuel或ERS
                if (fuelLap == lapCount && _currentIndex == 4) // 假设检查点5释放fuel
                {
                    ActivateFuelRelease();

                }

                if (ERSLap == lapCount && _currentIndex == 4) // 假设检查点5激活ERS
                {
                    ActivateERS();

                }

                _currentIndex++;
                // 检查是否完成一圈
                if (_currentIndex >= _checkpoints.Length)
                {
                    _currentIndex = 0; // 重置检查点索引
                    lapCount++; // 增加圈数

                    Debug.Log($"车辆{carId}完成第{lapCount}圈！");

                    // 检查比赛是否结束
                    if (lapCount >= totalLaps)
                    {
                        raceFinished = true;
                        _agent.isStopped = true;
                        RaceTimeManager.Instance.UpdateCarCheckpoint(carId, lapCount, _currentIndex, ckptTime, pitCnt, tyreType.ToString(), currentTyreWear, para.logoTexture, true);
                        Debug.Log($"车辆{carId}已完成比赛！");
                        return;
                    }
                }
            }
            // 设置下一个目标点
            SetNextDestination();
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

    // 更新轮胎磨损
    private void UpdateTyreWear()
    {

        // 根据轮胎类型计算磨损速度
        float wearRate = 0f;
        switch (tyreType)
        {
            case "hard":
                wearRate = 5f; // Hard轮胎40圈磨损完
                break;
            case "medium":
                wearRate = 7f; // Medium轮胎32圈磨损完
                break;
            case "soft":
                wearRate = 10f; // Soft轮胎25圈磨损完
                break;
        }

        // 更新当前磨损
        currentTyreWear -= wearRate * 0.01f * Time.deltaTime;
        currentTyreWear = Mathf.Clamp(currentTyreWear, 0f, 100f);


    }

    private void UpdateSpeedLimit()
    {
        // 1. 计算到下一个检查点的距离
        float distanceToNextCheckpoint = Vector3.Distance(transform.position, _checkpoints[_currentIndex].position);

        // 2. 判断当前是直道还是弯道（距离检查点10m内为弯道）
        bool isCorner = distanceToNextCheckpoint <= 10f;

        // 3. 设置基础目标速度（直道/弯道/进站）
        float targetSpeed = isCorner ? cornerTopSpeed : straightTopSpeed;
        targetSpeed = _isPitting ? 80f / 3.6f : targetSpeed; // 进站时80km/h → 转换为m/s
        targetSpeed = isSafeCar ? 80f / 3.6f : targetSpeed; // 安全车时80km/h → 转换为m/s

        // 如果非安全车距离安全车距离小于10m 减速为80km/h
        if (!isSafeCar && Vector3.Distance(transform.position, safeCar.transform.position) <= 100f)
        {
            targetSpeed = 80f / 3.6f; // 转换为m/s
        }

        // 4. 动态调整速度：加速或减速到目标速度
        if (targetSpeed > _agent.speed)
        {
            // 加速阶段：应用轮胎磨损、ERS、油量加成
            float wearEffect = Mathf.Clamp01((100f - currentTyreWear) / (100f - tyreWearEffectThreshold));
            float speedReduction = wearEffect * maxSpeedReduction;
            targetSpeed *= (1 - speedReduction);

            // ERS和油量加成（仅加速时生效）
            targetSpeed += ersActive ? ersSpeedBoost / 3.6f : 0f; // 转换为m/s
            float effectiveAcceleration = acceleration;
            effectiveAcceleration += ersActive ? ersAccelerationBoost : 0f;
            effectiveAcceleration += fuelActive ? fuelAccelerationBoost : 0f;

            // 加速到目标速度
            _agent.speed = Mathf.Min(targetSpeed, _agent.speed + effectiveAcceleration * Time.deltaTime);
        }
        else
        {
            // 减速阶段：以固定减速度（brakingDeceleration）平滑减速到目标速度
            _agent.speed = Mathf.Max(targetSpeed, _agent.speed - brakingDeceleration * Time.deltaTime);
        }
    }

    private float CalculateBrakingDistance()
    {
        // 根据当前速度和减速度计算刹车距离
        float currentSpeed = _agent.velocity.magnitude;
        return (currentSpeed * currentSpeed) / (2 * brakingDeceleration) * 1.0f; // 1.2为安全系数
    }

    private void SetNextDestination()
    {
        if (_currentIndex < _checkpoints.Length)
        {
            // 检查是否需要进站
            bool shouldPitThisLap = _nextPitStopIndex < pitStopLaps.Length && lapCount == pitStopLaps[_nextPitStopIndex];

            // 如果是最后一个检查点且需要进站，则设置目标为进站点
            if (shouldPitThisLap && _currentIndex == 0 && !_isPitting)
            {
                _isPitting = true;
                _agent.SetDestination(pitStop.position);
            }
            // 如果是进站完成，则设置目标为下一圈的第三个检查点
            else if (_isPitting && Vector3.Distance(transform.position, pitStop.position) <= arrivalDistance)
            {
                _isPitting = false;
                pitCnt++;
                _currentIndex = 2;
                _nextPitStopIndex++;

                // 更换轮胎
                if (_nextPitStopIndex <= tyreTypes.Length )
                {
                    tyreType = tyreTypes[_nextPitStopIndex-1];
                    currentTyreWear = 100f; // 重置轮胎磨损
                    Debug.Log($"进站换胎，新轮胎类型: {tyreType}");
                }

                _agent.SetDestination(_checkpoints[2].position);

            }
            else
            {
                // 正常设置下一个检查点
                _agent.SetDestination(_checkpoints[_currentIndex].position);
            }
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
        Invoke("RemoveCarFromRace", 3);

        // 发送给llm
        controller.onAccident("Latifi");
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