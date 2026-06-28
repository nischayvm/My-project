using UnityEngine;

public class PitStop : MonoBehaviour
{
    private bool playerInside = false;

    void Update()
    {
        // Added the check to ensure fuel isn't already full, as you suggested!
        if (playerInside &&
            Input.GetKeyDown(KeyCode.E) &&
            !FuelManager.Instance.isRefuelling &&
            FuelManager.Instance.currentFuel < FuelManager.Instance.maxFuel)
        {
            FuelManager.Instance.isRefuelling = true;
            PitUIManager.Instance.ShowMessage("Refuelling...");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            PitUIManager.Instance.ShowMessage("Press E to Refuel");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            FuelManager.Instance.isRefuelling = false;
            PitUIManager.Instance.HideMessage();
        }
    }
}