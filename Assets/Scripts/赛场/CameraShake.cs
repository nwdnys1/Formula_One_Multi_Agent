using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // 晃动的范围
    public Vector3 shakeRange = new Vector3(0.1f, 0.1f, 0.1f); // X, Y, Z 轴的晃动幅度

    // 晃动的速度
    public float shakeSpeed = 1.0f;

    // 相机的原始位置
    private Vector3 originalPosition;

    void Start()
    {
        // 保存相机的初始位置
        originalPosition = transform.position;
    }

    void Update()
    {
        if (shakeSpeed > 0)
        {
            // 使用正弦函数生成平滑的晃动效果
            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeRange.x * 0.5f;
            float offsetY = Mathf.Cos(Time.time * shakeSpeed) * shakeRange.y * 0.5f;
            float offsetZ = Mathf.Sin(Time.time * shakeSpeed * 1.5f) * shakeRange.z * 0.5f; // 不同频率的晃动

            // 计算新的相机位置
            Vector3 newPosition = originalPosition + new Vector3(offsetX, offsetY, offsetZ);

            // 平滑移动相机到新位置
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * shakeSpeed);
        }
    }
}