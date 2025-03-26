using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParaList", menuName = "Scriptable Objects/ParaList")]
public class ParaList : ScriptableObject
{
    public List<CarPara> carParas;
    public List<DriverPara> driverParas;
}
