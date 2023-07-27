using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private GameObject spotLightPosition, pointLightPosition;
    private Light spotLight, pointLight;
    public float spotLightHeight, spotLightAngle;

    private void Start()
    {
        spotLightPosition = new GameObject();   // 玩家頭頂聚光燈
        spotLightPosition.name = name + "_SpotLight";
        spotLightPosition.transform.parent = transform;
        spotLightPosition.transform.position = new Vector3(transform.position.x, transform.position.y + spotLightHeight, transform.position.z);
        spotLightPosition.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        spotLight = spotLightPosition.AddComponent<Light>();
        spotLight.type = LightType.Spot;
        spotLight.color = new Color32(255, 244, 214, 255);
        //spotLight.lightmapBakeType = LightmapBakeType.Baked;
        spotLight.shadows = LightShadows.Soft;
        spotLight.range = 10f;
        spotLight.spotAngle = spotLightAngle;
        spotLight.intensity = 2f;

        pointLightPosition = new GameObject();  // 玩家面前點光源（照亮臉部）
        pointLightPosition.name = name + "_PointLight";
        pointLightPosition.transform.parent = transform;
        pointLightPosition.transform.position = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z + 0.5f);
        pointLight = pointLightPosition.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.color = new Color32(255, 244, 214, 255);
        //pointLight.lightmapBakeType = LightmapBakeType.Baked;
        pointLight.shadows = LightShadows.Soft;
        pointLight.range = 2f;
        pointLight.intensity = 1f;
    }
}
