using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float destroyAfterSeconds;
    [SerializeField] private float launchForce;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
