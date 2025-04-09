using UnityEngine;
using UnityEngine.UI;

public class ChairController : MonoBehaviour
{
    public GameObject interactionText; // 显示"按F开始会议"的UI文本
    public bool isInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            interactionText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            interactionText.SetActive(false);
        }
    }

}