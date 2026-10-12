using HarvestGrove.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmManager : MonoBehaviour
{
    public PlantItem selectPlant;
    public bool isPlanting;
    public int money = 100;
    public TextMeshProUGUI moneyTxt;

    public Color buyColor = Color.green;
    public Color cancelColor = Color.red;

    public bool isSelecting;
    // Tool numbers match the existing UI button callbacks.
    public int selectedTool;
    public Image[] buttonsImg;
    public Sprite normalButton;
    public Sprite selectedButton;

    private void Start()
    {
        money = Mathf.Max(0, money);
        ClearSelection();
        UpdateMoneyText();
    }

    public void SelectPlant(PlantItem newPlant)
    {
        if (newPlant == null || newPlant.plant == null || !newPlant.plant.IsValid())
            return;

        bool wasSelected = isPlanting && selectPlant == newPlant;
        ClearSelection();
        if (wasSelected)
            return;

        selectPlant = newPlant;
        selectPlant.SetSelected(true, cancelColor);
        isPlanting = true;
    }

    public void SelectTool(int toolNumber)
    {
        if (toolNumber < 1 || toolNumber > 3 || buttonsImg == null ||
            toolNumber > buttonsImg.Length || buttonsImg[toolNumber - 1] == null)
            return;

        bool wasSelected = isSelecting && selectedTool == toolNumber;
        ClearSelection();
        if (wasSelected)
            return;

        isSelecting = true;
        selectedTool = toolNumber;
        buttonsImg[toolNumber - 1].sprite = selectedButton;
    }

    private void ClearSelection()
    {
        if (selectPlant != null)
            selectPlant.SetSelected(false, buyColor);
        selectPlant = null;
        isPlanting = false;

        if (buttonsImg != null && selectedTool > 0 && selectedTool <= buttonsImg.Length &&
            buttonsImg[selectedTool - 1] != null)
            buttonsImg[selectedTool - 1].sprite = normalButton;
        isSelecting = false;
        selectedTool = 0;
    }

    public bool CanAfford(int price)
    {
        return FarmEconomy.CanAfford(money, price);
    }

    public bool TrySpend(int price)
    {
        return CanAfford(price) && TryTransaction(-price);
    }

    public bool TryTransaction(int amount)
    {
        if (!FarmEconomy.TryTransaction(money, amount, out int newBalance))
            return false;

        money = newBalance;
        UpdateMoneyText();
        return true;
    }

    private void UpdateMoneyText()
    {
        if (moneyTxt != null)
            moneyTxt.text = "$" + money;
    }
}
