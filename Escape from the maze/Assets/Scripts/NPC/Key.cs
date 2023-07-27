using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Key : MonoBehaviour
{
    public float rotationSpeed = 5f;    // 鑰匙原地轉動速度

    void Start()
    {
        GameObject lightPosition = new GameObject();       // 上方聚光燈位置
        lightPosition.name = name + "_spotLight";
        lightPosition.transform.parent = transform;
        lightPosition.transform.position = new Vector3 (transform.position.x, transform.position.y + 8f, transform.position.z);
        lightPosition.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        Light spotLight = lightPosition.AddComponent<Light>();  // 設置聚光燈
        spotLight.type = LightType.Spot;
        spotLight.color = new Color32(255, 244, 214, 255);
        //spotLight.lightmapBakeType = LightmapBakeType.Baked;
        spotLight.shadows = LightShadows.Soft;
        spotLight.range = 10f;
        spotLight.spotAngle = 60f;
        spotLight.intensity = 3f;
    }
    void Update()
    {
        transform.Rotate(0, 1, 0, Space.World); // 鑰匙順時針轉動
    }

    private void OnTriggerEnter(Collider other) // 玩家碰撞獲得鑰匙
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.instance.AddKey();
            Destroy(gameObject);
        }
    }
}
