using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 playerPosition;
    public Vector3 offset;  // çRî^Î»ÖÃÆ«ÒÆÁ¿

    void Update()
    {
        playerPosition = PlayerMovement.instance.GetPlayerTransform().position;
        transform.position = new Vector3(playerPosition.x + offset.x, playerPosition.y + offset.y, playerPosition.z + offset.z);
    }

}
