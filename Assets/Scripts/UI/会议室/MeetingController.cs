using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DTO;
using LitJson;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.WSA;
using Cursor = UnityEngine.Cursor;

public class MeetingController : MonoBehaviour
{
    MeetingUI dialog;
    SocketClient client = SocketClient.Instance;
    CameraManager cm = CameraManager.Instance;
    public UIDocument driverParaUI;
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera strategistCamera;
    public CinemachineVirtualCamera mechanicCamera;
    public CinemachineVirtualCamera hamiltonCamera;
    public CinemachineVirtualCamera wolffCamera;
    public CinemachineVirtualCamera quanjing;
    public Dictionary<string, CinemachineVirtualCamera> cameras = new Dictionary<string, CinemachineVirtualCamera>();
    public ChairController chair;
    public GameObject StandWolff;
    public GameObject SitWolff;

    private void Awake()
    {
        dialog = GetComponent<MeetingUI>();
        // ȷ��DialogUI�������ȷ����
        if (dialog == null)
        {
            Debug.LogError("DialogUI is not assigned in the inspector.");
        }
        if (driverParaUI == null)
        {
            Debug.LogError("DriverParaUI is not assigned in the inspector.");
        }


    }

    private void Start()
    {
        cameras.Add("���ܶ���", hamiltonCamera);
        cameras.Add("Wolff", wolffCamera);
        cameras.Add("÷�����Ӳ���ʦ", strategistCamera);
        cameras.Add("÷�����ӻ�еʦ", mechanicCamera);
        SitWolff.SetActive(false);
        cm.SetCamera(playerCamera);


    }

    private void MeetingStart()
    {
        print("��ʼ����");
        chair.interactionText.SetActive(false);
        StandWolff.SetActive(false);
        SitWolff.SetActive(true);
        cm.SetCamera(quanjing);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        client.Send(JsonStr.before_meeting_start, (response) =>
        { }, (r) => MeetingReplay());

    }
    private void MeetingReplay()
    {

        client.Send(JsonStr.meeting_replay, (response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // �л������
            cm.SetCamera(cameras[json["sender"].ToString()]);
            // ������������ص�JSON����
            dialog.ShowCharacterUI(json["sender"].ToString());
            dialog.ShowDialogue(json["content"].ToString());
        }, (r) => MeetingChoose());


    }
    private void MeetingChoose()
    {
        cm.SetCamera(cameras["Wolff"]);
        dialog.ShowCharacterUI("Wolff");
        dialog.ShowInputFieldByButton("������Ի�����",
                (input) =>
                {
                    string sendStr = JsonStr.meeting_chat(input, "÷�����ӻ�еʦ");
                    // ������������ݵ�������
                    client.Send(sendStr, (response) =>
                    {
                        JsonData json = JsonMapper.ToObject(response);
                        // ������������ص�JSON����
                        dialog.ShowCharacterUI(json["sender"].ToString());
                        dialog.ShowDialogue(json["content"].ToString());
                        // �л������
                        cm.SetCamera(cameras[json["sender"].ToString()]);
                    }, (r) => MeetingEnd());
                },
                (input) =>
                {
                    client.Send(JsonStr.meeting_strategy, (response) =>
                    {
                        JsonData json = JsonMapper.ToObject(response);
                        // ������������ص�JSON����
                        dialog.ShowCharacterUI(json["sender"].ToString());
                        dialog.ShowDialogue(json["content"].ToString());
                        // �л������
                        cm.SetCamera(cameras[json["sender"].ToString()]);
                        //�������
                        if (json.ContainsKey("strategy"))
                            print("����:" + json["strategy"].ToString());
                        else
                            print("����:��");
                    }, (r) => MeetingEnd());
                },
                (input) =>
                {
                    client.Send(JsonStr.meeting_attitude, (response) =>
                    {
                        JsonData json = JsonMapper.ToObject(response);
                        // ������������ص�JSON����
                        dialog.ShowCharacterUI(json["sender"].ToString());
                        dialog.ShowDialogue(json["content"].ToString());
                        // �л������
                        cm.SetCamera(cameras[json["sender"].ToString()]);
                        //������̬
                        if (json.ContainsKey("attitude"))
                            print("��̬:" + json["attitude"].ToString());
                        else
                            print("��̬:��");
                    }, (r) => MeetingEnd());
                }
            );
    }
    private void MeetingEnd()
    {
        client.Send(JsonStr.before_meeting_end, (string response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // ������������ص�JSON����
            dialog.ShowCharacterUI("report");
            JsonData news = json["summary"];
            dialog._currentRoot.Q<Label>("Title").text = news["title"].ToString();
            dialog._currentRoot.Q<Label>("Contents").text = news["content"].ToString();

        }, null);
    }

    private void Update()
    {
        // �������Ƿ��ڷ�Χ�ڲ�����F��
        if (chair.isInRange && Input.GetKeyDown(KeyCode.F))
        {
            MeetingStart();
        }
    }
}