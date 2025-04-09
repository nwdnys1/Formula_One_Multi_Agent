using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using DTO;
using LitJson;
using UnityEngine.UIElements;
using UnityEditor.Rendering;
public class PracticeController : MonoBehaviour
{
    InterviewUI dialogUI;
    SocketClient client = SocketClient.Instance;
    CameraManager cm = CameraManager.Instance;
    ParaManager paraManager = ParaManager.Instance;
    public UIDocument carParaUI;

    private void Awake()
    {
        dialogUI = GetComponent<InterviewUI>();
        // ȷ��DialogUI�������ȷ����
        if (dialogUI == null)
        {
            Debug.LogError("DialogUI is not assigned in the inspector.");
        }

        if (client == null)
        {
            Debug.LogError("SocketClient is not assigned in the inspector.");
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogUI.ShowOptions(new string[] { "��ʼ��ϰ��" }, (index) =>
        {
            if (index == 0)
            {
                PracticeStart();
            }
        });
    }
    private void PracticeStart()
    {
        client.Send(JsonStr.practice_session_start, (response) =>
        {
            JsonData json = JsonMapper.ToObject(response);
            // ������������ص�JSON����
            switch (json["sender"].ToString())
            {
                case "÷�����ӻ�еʦ":
                    foreach (JsonData tuning in json["tuning_data"])
                    {
                        Debug.Log(tuning.ToString());
                    }
                    break;
                case "���ܶ���":
                    Debug.Log(json["tuning_data"].ToString());
                    break;
                default:
                    Debug.Log("δ֪��������");
                    break;
            }

        }, null);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
