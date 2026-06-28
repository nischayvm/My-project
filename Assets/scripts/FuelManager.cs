using UnityEngine;

public class FuelManager : MonoBehaviour
{
    public static FuelManager Instance;

    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;

    public float fuelBurnRate = 0.02f;
    public float refillSpeed = 25f;

    [HideInInspector]
    public bool isRefuelling = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentFuel = maxFuel;
        isRefuelling = false;
    }

    void Update()
    {
        // Refuelling
        if (isRefuelling)
        {
            currentFuel += refillSpeed * Time.deltaTime;

            currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

            if (PitUIManager.Instance != null)
                PitUIManager.Instance.UpdateRefuelMessage();

            if (currentFuel >= maxFuel)
            {
                currentFuel = maxFuel;
                isRefuelling = false;

                if (PitUIManager.Instance != null)
                    PitUIManager.Instance.ShowMessage("Fuel Full!");
            }

            return;
        }

        // Consume fuel while driving
        if (currentFuel > 0f)
        {
            currentFuel -= fuelBurnRate * Time.deltaTime;
            currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        }
    }
}