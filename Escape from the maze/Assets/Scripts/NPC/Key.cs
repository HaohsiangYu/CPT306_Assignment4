using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Key : MonoBehaviour
{
    public float rotationSpeed = 5f;    // 耳�ԭ���D���ٶ�

    void Start()
    {
        GameObject lightPosition = new GameObject();       // �Ϸ��۹��λ��
        lightPosition.name = name + "_spotLight";
        lightPosition.transform.parent = transform;
        lightPosition.transform.position = new Vector3 (transform.position.x, transform.position.y + 8f, transform.position.z);
        lightPosition.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        Light spotLight = lightPosition.AddComponent<Light>();  // �O�þ۹��
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
        transform.Rotate(0, 1, 0, Space.World); // 耳�형r��D��
    }

    private void OnTriggerEnter(Collider other) // �����ײ�@��耳�
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.instance.AddKey();
            Destroy(gameObject);
        }
    }
}
