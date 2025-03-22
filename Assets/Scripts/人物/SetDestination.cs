using UnityEngine;
using UnityEngine.AI;

public class MoveToMouseClick : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>(); // ��ȡ NavMeshAgent ���
        _animator = GetComponent<Animator>();  // ��ȡ Animator ���
    }

    void Update()
    {

        
        // ���ö����� Speed ����Ϊ NavMeshAgent ���ٶ�
        _animator.SetFloat(Animator.StringToHash("Speed"), _agent.velocity.magnitude);
        _animator.SetFloat(Animator.StringToHash("MotionSpeed"), 1f);

        print(_agent.velocity.magnitude);


        if (Input.GetMouseButtonDown(0)) // ������������
        {
            // ��ȡ���������Ļλ��
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ���߼�������λ���Ƿ��������ײ
            if (Physics.Raycast(ray, out hit))
            {
                // ���� NavMeshAgent ��Ŀ��λ��Ϊ���������������
                _agent.SetDestination(hit.point);
            }
        }
    }
    
}
