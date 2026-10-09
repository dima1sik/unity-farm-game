using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public GameObject plantItem;

    private void Awake()
    {
        if (plantItem == null || plantItem.GetComponent<PlantItem>() == null)
        {
            Debug.LogError("StoreManager requires a prefab with a PlantItem component.", this);
            enabled = false;
            return;
        }

        var plants = new List<PlantObject>();
        foreach (PlantObject crop in Resources.LoadAll<PlantObject>("Plants"))
        {
            if (crop.IsValid())
                plants.Add(crop);
            else
                Debug.LogWarning("Skipping invalid crop asset: " + crop.name, crop);
        }

        plants.Sort((left, right) =>
        {
            int priceComparison = left.buyPrice.CompareTo(right.buyPrice);
            return priceComparison != 0
                ? priceComparison
                : string.Compare(left.plantName, right.plantName, StringComparison.Ordinal);
        });

        foreach (PlantObject crop in plants)
            Instantiate(plantItem, transform).GetComponent<PlantItem>().plant = crop;
    }
}
