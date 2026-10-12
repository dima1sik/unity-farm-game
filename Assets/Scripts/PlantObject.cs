using UnityEngine;

[CreateAssetMenu(fileName = "New Plant", menuName = "Plant")]
public class PlantObject : ScriptableObject
{
    public string plantName;
    public Sprite[] plantStages;
    public float timeBtwStages;
    public int buyPrice;
    public int sellPrice;
    public Sprite icon;
    public Sprite dryPlanted;

    public bool IsValid()
    {
        if (plantStages == null || plantStages.Length == 0 || dryPlanted == null ||
            buyPrice < 0 || sellPrice < 0 || timeBtwStages <= 0f ||
            float.IsNaN(timeBtwStages) || float.IsInfinity(timeBtwStages))
            return false;

        foreach (Sprite stage in plantStages)
        {
            if (stage == null)
                return false;
        }

        return true;
    }
}
