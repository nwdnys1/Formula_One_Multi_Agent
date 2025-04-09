using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DTO;
using LitJson;
using UnityEngine.UIElements;

public class InterviewController : MonoBehaviour
{
    InterviewDialog dialog;
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
        dialog = GetComponent<InterviewDialog>();
        // ȷ��DialogUI�������ȷ����
        if (dialog == null)
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
            dialog.ShowCharacterUI(json["sender"].ToString());
            dialog.ShowDialogue(json["content"].ToString());
            // �л������
            cm.SetCamera(cameras[json["sender"].ToString()]);
        }, (r) =>
        { InterviewChat(); });

    }
    private void InterviewChat()
    {
        cm.SetCamera(cameras["Wolff"]);
        dialog.ShowCharacterUI("Wolff");
        dialog.ShowInputField("������ɷ�����", (input) =>
        {
            string sendStr = JsonStr.media_interview_chat(input);
            // ������������ݵ�������
            client.Send(sendStr, (response) =>
            {
                JsonData json = JsonMapper.ToObject(response);
                // ������������ص�JSON����
                dialog.ShowCharacterUI(json["sender"].ToString());
                dialog.ShowDialogue(json["content"].ToString());
                // �л������
                cm.SetCamera(cameras[json["sender"].ToString()]);
            }, (r) =>
            { InterviewEnd(); });
        });

    }
    private void InterviewEnd()
    {
        client.Send(JsonStr.media_interview_end, (string response) =>
        {
            print("response:" + response);
            JsonData json = JsonMapper.ToObject(response);
            // ������������ص�JSON����
            dialog.ShowCharacterUI("report");
            JsonData news = json["news_article"];
            dialog._currentRoot.Q<Label>("Title").text = news["title"].ToString();
            dialog._currentRoot.Q<Label>("Contents").text = news["content"].ToString();

        }, null);
    }
    private void Update()
    {

    }
}