using UnityEngine;

public class DialogExample : MonoBehaviour
{
    DialogUI dialogUI;

    private void Awake()
    {
        dialogUI = GetComponent<DialogUI>();
        // ȷ��DialogUI�������ȷ����
        if (dialogUI == null)
        {
            Debug.LogError("DialogUI is not assigned in the inspector.");
        }
    }

    private void Start()
    {
        // ��ʾNPC�Ի�
        dialogUI.ShowDialogue("��ã����Ǵ���Ұ�ơ����������ô����");
        // ��ʾ��������
        dialogUI.ShowInputField("��������Ļش�...", OnPlayerResponse);
    }

    private void OnPlayerResponse(string playerText)
    {
        // �����������
        Debug.Log("���˵: " + playerText);

        // ������һ�ζԻ�
        dialogUI.ShowOptions(new string[] { "�Һܺã�лл��", "�е�æ..." }, (input) =>
        {
            Debug.Log("���ѡ����ѡ��" + input);
        });
    }


    private void Update()
    {

    }
}