using System.Collections;
using TMPro;
using UnityEngine;

public class PitUIManager : MonoBehaviour
{
    public static PitUIManager Instance;

    public TMP_Text fuelText;
    public TMP_Text pitMessage;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pitMessage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (FuelManager.Instance == null)
            return;

        float fuel = FuelManager.Instance.currentFuel;

        fuelText.text = "Fuel: " + Mathf.RoundToInt(fuel) + "%";

        if (fuel <= 20)
            fuelText.color = Color.red;
        else if (fuel <= 50)
            fuelText.color = Color.yellow;
        else
            fuelText.color = Color.green;
    }

    public void ShowMessage(string message)
    {
        StopAllCoroutines();

        pitMessage.gameObject.SetActive(true);
        pitMessage.text = message;

        if (message == "Fuel Full!")
            StartCoroutine(HideAfterDelay());
    }

    public void HideMessage()
    {
        StopAllCoroutines();
        pitMessage.gameObject.SetActive(false);
    }

    public void UpdateRefuelMessage()
    {
        pitMessage.gameObject.SetActive(true);

        pitMessage.text =
            "REFUELLING...\n" +
            Mathf.RoundToInt(FuelManager.Instance.currentFuel) + "%";
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        pitMessage.gameObject.SetActive(false);
    }
}