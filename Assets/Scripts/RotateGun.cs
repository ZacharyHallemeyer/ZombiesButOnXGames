using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGun : MonoBehaviour
{
    public PlayerShooting playerShooting;

    private Quaternion desiredRotation;
    private float rotationSpeed = 5f;

    private void Update()
    {
        if (!playerShooting.IsGrappling)
        {
            desiredRotation = transform.parent.rotation;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(playerShooting.GrapplePoint - transform.position);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }
}
