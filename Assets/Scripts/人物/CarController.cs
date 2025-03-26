using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{

    public string carId;
    private CarPara para;

    [Header("导航设置")]
    public Transform checkpointsParent; // 拖入Checkpoints父对象
    public float arrivalDistance = 0.5f; // 到达判定距离

    private Transform[] _checkpoints;
    private int _currentIndex = 0;
    private NavMeshAgent _agent;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        para = ParaManager.Instance.getCarPara(carId);


        // 获取NavMeshAgent组件
        _agent = GetComponent<NavMeshAgent>();

        // 初始化检查点数组
        if (checkpointsParent != null)
        {
            _checkpoints = new Transform[checkpointsParent.childCount];
            for (int i = 0; i < checkpointsParent.childCount; i++)
            {
                Transform ckpt = checkpointsParent.GetChild(i);
                // 将ckpt投影到地面
                RaycastHit hit;
                if (Physics.Raycast(ckpt.position, Vector3.down, out hit))
                {
                    ckpt.position = hit.point;
                    print(ckpt.position);
                }
                _checkpoints[i] = ckpt;
            }

            // 开始导航到第一个检查点
            SetNextDestination();
        }
        else
        {
            Debug.LogError("未指定Checkpoints父对象！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 检查是否到达当前目标点
        if (_agent.pathPending || _checkpoints == null || _currentIndex >= _checkpoints.Length)
            return;

        float distanceToTarget = Vector3.Distance(transform.position, _checkpoints[_currentIndex].position);
        if (distanceToTarget <= arrivalDistance)
        {
            Debug.Log($"到达检查点: {_currentIndex + 1}");
            _currentIndex++;
            SetNextDestination();
        }



    }

    private void SetNextDestination()
    {
        if (_currentIndex < _checkpoints.Length)
        {
            _agent.SetDestination(_checkpoints[_currentIndex].position);
        }
        else
        {
            Debug.Log("已完成所有检查点！");
            _agent.isStopped = true; // 停止导航
        }
    }
}
