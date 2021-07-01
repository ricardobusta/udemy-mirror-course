using UnityEngine;
using UnityEngine.InputSystem;

public static class CameraExtensionMethods
{
    public static bool RayCast(this Camera camera, out RaycastHit hit, float maxDist, int layerMask)
    {
        var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        return Physics.Raycast(ray, out hit, maxDist, layerMask);
    }
}