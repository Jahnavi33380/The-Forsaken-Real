
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player, cameraTrans;

    void Update()
    {
        cameraTrans.LookAt(player);
    }
}