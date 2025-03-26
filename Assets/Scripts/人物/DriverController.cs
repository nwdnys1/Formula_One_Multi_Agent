using UnityEngine;

public class DriverController : MonoBehaviour
{
    public string driverId;
    private DriverPara para;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        para = ParaManager.Instance.getDriverPara(driverId);
        print(para);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
