using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // �ƶ��ٶ�
    public float moveSpeed = 5f;

    // ���ڴ洢��ɫ�ĸ���
    private Rigidbody rb;

    // Start �� MonoBehaviour ��һ���������ں������ڽű���ʼʱ����
    void Start()
    {
        // ��ȡ������ĸ������
        rb = GetComponent<Rigidbody>();
    }

    // Update ��ÿһ֡�����õĺ���
    void Update()
    {
        // ��ȡ��������ˮƽ�ʹ�ֱ����WASD ���ͷ����
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // ������ҵ��ƶ�����
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // �����ƶ�����
        MovePlayer(moveDirection);
    }

    // �ƶ���ҵķ���
    void MovePlayer(Vector3 direction)
    {
        // �������Ϊ�㣬�����ƶ�
        if (direction.magnitude > 0)
        {
            // ͨ�������������ƶ�
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }
    }
}

