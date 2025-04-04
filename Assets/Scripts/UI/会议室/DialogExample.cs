using UnityEngine;

public class DialogExample : MonoBehaviour
{
    private DialogUI dialogUI;

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
        dialogUI.ShowDialogue("����Ұ��", "��ã����Ǵ���Ұ�ơ����������ô����", () =>
        {
            // �Ի���ʾ��ɺ���ʾ�������
            dialogUI.ShowPlayerInput(OnPlayerResponse);
        });
    }

    private void OnPlayerResponse(string playerText)
    {
        // �����������
        Debug.Log("���˵: " + playerText);

        // ������һ�ζԻ�
        dialogUI.ShowDialogue("����Ұ��", "�������ˡ���ô����������Ϸ��ô����", () =>
        {
            dialogUI.ShowPlayerInput(OnSecondPlayerResponse);
        });
    }

    private void OnSecondPlayerResponse(string playerText)
    {
        Debug.Log("��ҵڶ��λ�Ӧ: " + playerText);
        // ���Լ����Ի���...
    }

    private void Update()
    {
        // ���������������ɵ�ǰ�ı���ʾ
        if (Input.GetMouseButtonDown(0))
        {
            dialogUI.CompleteCurrentText();
        }
    }
}