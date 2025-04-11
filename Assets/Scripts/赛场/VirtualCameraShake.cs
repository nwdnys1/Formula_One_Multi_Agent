using UnityEngine;
using Cinemachine;

public class VirtualCameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // 引用Virtual Camera
    public float shakeSpeed = 1.0f; // 晃动速度
    public float shakeAmount = 0.5f; // 晃动幅度

    private Vector3 originalPosition; // 记录初始位置

    void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera is not assigned!");
            return;
        }

        // 记录Virtual Camera的初始位置
        originalPosition = virtualCamera.transform.position;
    }

    void Update()
    {
        if (virtualCamera == null)
            return;

        // 生成随机的晃动偏移量
        float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        float offsetY = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;

        // 应用晃动偏移量到Virtual Camera的位置
        Vector3 newPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
        virtualCamera.transform.position = newPosition;
    }
}