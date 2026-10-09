using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantItem : MonoBehaviour
{
    public PlantObject plant;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI priceTxt;
    public Image icon;
    public Image btnImage;
    public TextMeshProUGUI btnTxt;

    private FarmManager farm;

    private void Start()
    {
        farm = FindObjectOfType<FarmManager>();
        if (farm == null || plant == null || !plant.IsValid() || nameTxt == null ||
            priceTxt == null || icon == null || btnImage == null || btnTxt == null)
        {
            Debug.LogError("PlantItem requires a valid crop, farm manager and UI references.", this);
            enabled = false;
            return;
        }

        nameTxt.text = plant.plantName;
        priceTxt.text = "$" + plant.buyPrice;
        icon.sprite = plant.icon;
        SetSelected(false, farm.buyColor);
    }

    public void BuyPlant()
    {
        if (enabled && farm != null)
            farm.SelectPlant(this);
    }

    public void SetSelected(bool selected, Color color)
    {
        if (btnImage != null)
            btnImage.color = color;
        if (btnTxt != null)
            btnTxt.text = selected ? "Cancel" : "Buy";
    }
}
