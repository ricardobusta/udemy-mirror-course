using System;
using UnityEngine;

public class ShakeAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 amplitude;
    [SerializeField] private Vector3 speed;

    private Vector3 _current;

    private void Start()
    {
        _current = transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        _current += speed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(
            amplitude.x * Mathf.Sin(_current.x),
            amplitude.y * Mathf.Sin(_current.y),
            amplitude.z * Mathf.Sin(_current.z)
        );
    }
}