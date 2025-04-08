using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private CinemachineVirtualCamera _currentCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetCamera(CinemachineVirtualCamera newCamera)
    {
        if (_currentCamera != null)
        {
            _currentCamera.Priority = 10;
        }

        _currentCamera = newCamera;
        _currentCamera.Priority = 100;
    }

    public void FollowTarget(Transform target)
    {
        if (_currentCamera != null)
        {
            _currentCamera.Follow = target;
            _currentCamera.LookAt = target;
        }
    }
}