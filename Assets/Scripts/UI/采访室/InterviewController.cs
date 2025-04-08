using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DTO;
using LitJson;
using UnityEngine.UIElements;

public class InterviewController : MonoBehaviour
{
    DialogUI dialogUI;
    SocketClient client = SocketClient.Instance;
    CameraManager cm = CameraManager.Instance;
    public UIDocument driverParaUI;
    public CinemachineVirtualCamera reporterCamera;
    public CinemachineVirtualCamera hornerCamera;
    public CinemachineVirtualCamera verstappenCamera;
    public CinemachineVirtualCamera hamiltonCamera;
    public CinemachineVirtualCamera wolffCamera;
    public CinemachineVirtualCamera quanjing;
    public Dictionary<string, CinemachineVirtualCamera> cameras = new Dictionary<string, CinemachineVirtualCamera>();

    private void Awake()
    {
        dialogUI = GetComponent<DialogUI>();
        // ȷ��DialogUI�������ȷ����
        if (dialogUI == null)
        {
            Debug.LogError("DialogUI is not assigned in the inspector.");
        }
        if (driverParaUI == null)
        {
            Debug.LogError("DriverParaUI is not assigned in the inspector.");
        }
        if (client == null)
        {
            Debug.LogError("SocketClient is not assigned in the inspector.");
        }
        cameras.Add("����", reporterCamera);
        cameras.Add("����", hornerCamera);
        cameras.Add("ά˹����", verstappenCamera);
        cameras.Add("���ܶ���", hamiltonCamera);
        cameras.Add("Wolff", wolffCamera);
    }

    private void Start()
    {
        // ȫ���ӽ�
        cm.SetCamera(quanjing);

        // ��ʼ�ɷ�
        InterviewStart();

    }

    private void InterviewStart()
    {
        client.Send(JsonStr.media_interview_start, (response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // ������������ص�JSON����
            dialogUI.ShowDialogue(json["sender"] + ":" + json["content"]);
            // �л������
            cm.SetCamera(cameras[json["sender"].ToString()]);
        }, (r) =>
        { InterviewEnd(); });

    }
    private void InterviewEnd()
    {
        client.Send(JsonStr.media_interview_end, (string response) =>
        {
            print("response:" + response);
            JsonData json = JsonMapper.ToObject(response);
            // ������������ص�JSON����
            dialogUI.ShowDialogue(json["news_article"].ToString());
            // �л������
            cm.SetCamera(quanjing);
            driverParaUI.enabled = true;
            dialogUI.HideAll();
        }, null);
    }
    private void Update()
    {

    }
}