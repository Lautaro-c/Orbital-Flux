using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] private GameObject toFollow;
    [SerializeField] private Vector3 offset;

    private void FixedUpdate()
    {
        transform.position = toFollow.transform.position + offset;
    }
}
