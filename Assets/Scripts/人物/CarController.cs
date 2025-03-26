using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{

    public string carId;
    private CarPara para;

    [Header("��������")]
    public Transform checkpointsParent; // ����Checkpoints������
    public float arrivalDistance = 0.5f; // �����ж�����

    private Transform[] _checkpoints;
    private int _currentIndex = 0;
    private NavMeshAgent _agent;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        para = ParaManager.Instance.getCarPara(carId);


        // ��ȡNavMeshAgent���
        _agent = GetComponent<NavMeshAgent>();

        // ��ʼ����������
        if (checkpointsParent != null)
        {
            _checkpoints = new Transform[checkpointsParent.childCount];
            for (int i = 0; i < checkpointsParent.childCount; i++)
            {
                Transform ckpt = checkpointsParent.GetChild(i);
                // ��ckptͶӰ������
                RaycastHit hit;
                if (Physics.Raycast(ckpt.position, Vector3.down, out hit))
                {
                    ckpt.position = hit.point;
                    print(ckpt.position);
                }
                _checkpoints[i] = ckpt;
            }

            // ��ʼ��������һ������
            SetNextDestination();
        }
        else
        {
            Debug.LogError("δָ��Checkpoints������");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ����Ƿ񵽴ﵱǰĿ���
        if (_agent.pathPending || _checkpoints == null || _currentIndex >= _checkpoints.Length)
            return;

        float distanceToTarget = Vector3.Distance(transform.position, _checkpoints[_currentIndex].position);
        if (distanceToTarget <= arrivalDistance)
        {
            Debug.Log($"�������: {_currentIndex + 1}");
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
            Debug.Log("��������м��㣡");
            _agent.isStopped = true; // ֹͣ����
        }
    }
}
