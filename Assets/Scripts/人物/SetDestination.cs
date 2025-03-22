using UnityEngine;
using UnityEngine.AI;

public class MoveToMouseClick : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>(); // 获取 NavMeshAgent 组件
        _animator = GetComponent<Animator>();  // 获取 Animator 组件
    }

    void Update()
    {

        
        // 设置动画的 Speed 参数为 NavMeshAgent 的速度
        _animator.SetFloat(Animator.StringToHash("Speed"), _agent.velocity.magnitude);
        _animator.SetFloat(Animator.StringToHash("MotionSpeed"), 1f);

        print(_agent.velocity.magnitude);


        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
        {
            // 获取鼠标点击的屏幕位置
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 射线检测鼠标点击位置是否与地面碰撞
            if (Physics.Raycast(ray, out hit))
            {
                // 设置 NavMeshAgent 的目标位置为鼠标点击的世界坐标
                _agent.SetDestination(hit.point);
            }
        }
    }
    
}
