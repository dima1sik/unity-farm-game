using HarvestGrove.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlotManager : MonoBehaviour
{
    private const int WaterTool = 1;
    private const int FertilizerTool = 2;
    private const int BuyPlotTool = 3;

    public Color availableColor = Color.green;
    public Color unavailableColor = Color.red;
    public Sprite drySprite;
    public Sprite normalSprite;
    public Sprite unavailableSprite;
    public bool isBought = true;

    private SpriteRenderer plant;
    private BoxCollider2D plantCollider;
    private SpriteRenderer plot;
    private PlantObject selectedPlant;
    private FarmManager farm;
    private CropGrowth growth;
    private bool isDry = true;

    private void Start()
    {
        plot = GetComponent<SpriteRenderer>();
        farm = GetComponentInParent<FarmManager>();
        if (transform.childCount > 0)
        {
            Transform crop = transform.GetChild(0);
            plant = crop.GetComponent<SpriteRenderer>();
            plantCollider = crop.GetComponent<BoxCollider2D>();
        }

        if (plot == null || farm == null || plant == null || plantCollider == null ||
            drySprite == null || normalSprite == null || unavailableSprite == null)
        {
            Debug.LogError("PlotManager requires a farm, plot sprites and a crop child with a renderer and collider.", this);
            enabled = false;
            return;
        }

        // Unity sends mouse callbacks to the clicked collider's object, not its parent.
        PlotClickTarget clickTarget = plantCollider.GetComponent<PlotClickTarget>();
        if (clickTarget == null)
            clickTarget = plantCollider.gameObject.AddComponent<PlotClickTarget>();
        clickTarget.Owner = this;

        plant.gameObject.SetActive(false);
        plot.sprite = isBought ? drySprite : unavailableSprite;
    }

    private void Update()
    {
        if (growth != null && growth.Advance(Time.deltaTime))
            UpdatePlant();
    }

    private void OnMouseDown()
    {
        HandleClick();
    }

    private void OnMouseOver()
    {
        HandleHover();
    }

    private void OnMouseExit()
    {
        ClearHover();
    }

    public void HandleClick()
    {
        if (!enabled || farm == null || IsPointerOverUI())
            return;

        if (farm.isSelecting)
        {
            UseTool();
            return;
        }

        if (farm.isPlanting)
        {
            if (CanPlant())
                Plant(farm.selectPlant.plant);
            return;
        }

        if (growth != null && growth.IsMature)
            Harvest();
    }

    public void HandleHover()
    {
        if (!enabled || farm == null || plot == null)
            return;

        if (IsPointerOverUI())
        {
            ClearHover();
            return;
        }

        bool hasSelection = farm.isPlanting || farm.isSelecting;
        bool available = farm.isSelecting ? CanUseTool() : CanPlant();
        plot.color = hasSelection ? (available ? availableColor : unavailableColor) : Color.white;
    }

    public void ClearHover()
    {
        if (plot != null)
            plot.color = Color.white;
    }

    private bool CanPlant()
    {
        return isBought && growth == null && farm.selectPlant != null &&
            farm.selectPlant.plant != null && farm.selectPlant.plant.IsValid() &&
            farm.CanAfford(farm.selectPlant.plant.buyPrice);
    }

    private bool CanUseTool()
    {
        switch (farm.selectedTool)
        {
            case WaterTool:
                return isBought && isDry;
            case FertilizerTool:
                return isBought && growth != null && growth.CanFertilize &&
                    farm.CanAfford(FarmEconomy.FertilizerPrice);
            case BuyPlotTool:
                return !isBought && farm.CanAfford(FarmEconomy.PlotPrice);
            default:
                return false;
        }
    }

    private void UseTool()
    {
        if (!CanUseTool())
            return;

        switch (farm.selectedTool)
        {
            case WaterTool:
                isDry = false;
                plot.sprite = normalSprite;
                if (growth != null)
                {
                    growth.Water();
                    UpdatePlant();
                }
                break;
            case FertilizerTool:
                if (farm.TrySpend(FarmEconomy.FertilizerPrice))
                    growth.ApplyFertilizer();
                break;
            case BuyPlotTool:
                if (farm.TrySpend(FarmEconomy.PlotPrice))
                {
                    isBought = true;
                    isDry = true;
                    plot.sprite = drySprite;
                }
                break;
        }
    }

    private void Harvest()
    {
        // Keep the crop if its payout would overflow the balance.
        if (!farm.TryTransaction(selectedPlant.sellPrice))
            return;

        growth = null;
        selectedPlant = null;
        plant.gameObject.SetActive(false);
        isDry = true;
        plot.sprite = drySprite;
    }

    private void Plant(PlantObject newPlant)
    {
        if (!farm.TrySpend(newPlant.buyPrice))
            return;

        selectedPlant = newPlant;
        growth = new CropGrowth(newPlant.plantStages.Length, newPlant.timeBtwStages);
        if (!isDry)
            growth.Water();

        UpdatePlant();
        plant.gameObject.SetActive(true);
    }

    private void UpdatePlant()
    {
        Sprite sprite = isDry ? selectedPlant.dryPlanted : selectedPlant.plantStages[growth.Stage];
        plant.sprite = sprite;
        // Collider dimensions are local to the sprite, independent of transform scale.
        plantCollider.size = sprite.bounds.size;
        plantCollider.offset = sprite.bounds.center;
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
