using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // 移动速度
    public float moveSpeed = 5f;

    // 用于存储角色的刚体
    private Rigidbody rb;

    // Start 是 MonoBehaviour 的一个生命周期函数，在脚本开始时调用
    void Start()
    {
        // 获取该物体的刚体组件
        rb = GetComponent<Rigidbody>();
    }

    // Update 是每一帧都调用的函数
    void Update()
    {
        // 获取玩家输入的水平和垂直方向（WASD 或箭头键）
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 计算玩家的移动方向
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // 调用移动函数
        MovePlayer(moveDirection);
    }

    // 移动玩家的方法
    void MovePlayer(Vector3 direction)
    {
        // 如果方向不为零，进行移动
        if (direction.magnitude > 0)
        {
            // 通过刚体控制玩家移动
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }
    }
}

