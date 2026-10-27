using UnityEngine;

public class DoorSensorTrigger : MonoBehaviour
{
    public AutomaticSlidingDoor slidingDoor;

    void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            if (slidingDoor != null) slidingDoor.OnSensorTriggerEnter();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (IsPlayer(other))
        {
            if (slidingDoor != null) slidingDoor.OnSensorTriggerEnter();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            if (slidingDoor != null) slidingDoor.OnSensorTriggerExit();
        }
    }

    private bool IsPlayer(Collider col)
    {
        if (col.CompareTag("Player")) return true;
        if (col.GetComponent<FPSController>() != null) return true;
        if (col.GetComponent<CharacterController>() != null) return true;
        if (col.GetComponentInParent<FPSController>() != null) return true;
        if (col.name.ToLower().Contains("player") || col.name.ToLower().Contains("camera")) return true;
        return false;
    }
}
