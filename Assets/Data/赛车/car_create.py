import os

# 定义车手名称和编号
drivers = {
    "Hamilton": 1,
    "Verstappen": 2,
    "Bottas": 3,
    "Gasly": 4,
    "Leclerc": 5,
    "Sainz": 6,
    "Alonso": 7,
    "Ocon": 8,
    "Norris": 9,
    "Ricciardo": 10,
    "Stroll": 11,
    "Vettel": 12,
    "Schumacher": 13,
    "Mazepin": 14,
    "Latifi": 15,
    "Albon": 16,
    "Tsunoda": 17,
    "Perez": 18,
    "Raikkonen": 19,
    "Russell": 20
}

# 定义 Unity 的 YAML 模板（严格按照你的格式，不包含任何需要替换的占位符，除了 m_Name）
template = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 556f726f73e6e7042b5d2fb448b7acfc, type: 3}
  m_Name: "driver_name"
  m_EditorClassIdentifier: 
  topSpeed: topSpeed_value
  acceleration: acceleration_value
  drsEffectiveness: drsEffectiveness_value
  carCornering: carCornering_value
  frontWingAngle: frontWingAngle_value
  rearWingAngle: rearWingAngle_value
  antiRollDistribution: antiRollDistribution_value
  tyreCamber: tyreCamber_value
  toeOut: toeOut_value
  bestFrontWingAngle: 5
  bestRearWingAngle: 12.5
  bestAntiRollDistribution: 0.3
  bestTyreCamber: -3.2
  bestToeOut: 0.7
  oversteerRating: rating_1
  brakingStabilityRating: rating_2
  corneringRating: rating_3
  tractionRating: rating_4
  straightsRating: rating_5
  tyreType: 0
  wearRate: 1
  currentWear: 100
  hardTyreLife: 40
  mediumTyreLife: 32
  softTyreLife: 25
  ersAvailable: 1
  ersSpeedBoost: 15
  ersAccelerationBoost: 2
  ersDuration: 120
  fuelReleaseAvailable: 1
  fuelSpeedBoost: 7.5
  fuelAccelerationBoost: 1
  fuelDuration: 120
"""

# Unity 的 YAML 文件保存路径
output_folder = "./"  # Unity 项目中的路径

# 确保输出文件夹存在
os.makedirs(output_folder, exist_ok=True)

# 遍历车手列表并生成 YAML 文件
for driver_name in drivers.keys():
    # 将模板中的车手名称替换为实际的车手名称
    driver_data = template.replace("driver_name", driver_name)
    
    # 替换rating_1到rating_5的值，分别为 1到5的随机整数
    import random
    for i in range(1, 6):
        driver_data = driver_data.replace(f"rating_{i}", str(random.randint(1, 5)))

    # 替换 topSpeed_value 为 80 + 一个随机浮点数（-3到3）
    top_speed_value = 80 + random.uniform(-3, 3)
    driver_data = driver_data.replace("topSpeed_value", str(round(top_speed_value, 2)))

    # 替换 acceleration_value 为 50 + 一个随机浮点数（-3到3）
    acceleration_value = 50 + random.uniform(-3, 3)
    driver_data = driver_data.replace("acceleration_value", str(round(acceleration_value, 2)))

    # 替换 drsEffectiveness_value 为 0.5 + 一个随机浮点数（-0.1到0.1）
    drs_effectiveness_value = 0.5 + random.uniform(-0.1, 0.1)
    driver_data = driver_data.replace("drsEffectiveness_value", str(round(drs_effectiveness_value, 2)))

    # 替换 carCornering_value 为 3.5 + 一个随机浮点数（-0.3到0.3）
    car_cornering_value = 3.5 + random.uniform(-0.3, 0.3)
    driver_data = driver_data.replace("carCornering_value", str(round(car_cornering_value, 2)))

    # 替换 frontWingAngle_value 为 5 + 一个随机浮点数（-3到3）
    front_wing_angle_value = 5 + random.uniform(-3, 3)
    driver_data = driver_data.replace("frontWingAngle_value", str(round(front_wing_angle_value, 2)))

    # 替换 rearWingAngle_value 为 12.5 + 一个随机浮点数（-3到3）
    rear_wing_angle_value = 12.5 + random.uniform(-3, 3)
    driver_data = driver_data.replace("rearWingAngle_value", str(round(rear_wing_angle_value, 2)))

    # 替换 antiRollDistribution_value 为 0.3 + 一个随机浮点数（-0.1到0.1）
    anti_roll_distribution_value = 0.3 + random.uniform(-0.1, 0.1)
    driver_data = driver_data.replace("antiRollDistribution_value", str(round(anti_roll_distribution_value, 2)))

    # 替换 tyreCamber_value 为 -2.9 + 一个随机浮点数（-0.5到0.5）
    tyre_camber_value = -2.9 + random.uniform(-0.5, 0.5)
    driver_data = driver_data.replace("tyreCamber_value", str(round(tyre_camber_value, 2)))

    # 替换 toeOut_value 为 0.6 + 一个随机浮点数（-0.2到0.2）
    toe_out_value = 0.6 + random.uniform(-0.2, 0.2)
    driver_data = driver_data.replace("toeOut_value", str(round(toe_out_value, 2)))

    # 构造文件路径
    file_name = f"{driver_name}.asset"
    file_path = os.path.join(output_folder, file_name)
    
    # 将数据写入文件
    with open(file_path, "w", encoding="utf-8") as file:
        file.write(driver_data)
    
    print(f"Generated: {file_path}")

print("All driver assets have been created successfully!")