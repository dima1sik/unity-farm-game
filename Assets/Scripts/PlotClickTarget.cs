using UnityEngine;

// Forward crop collider interaction to the plot's gameplay component.
public class PlotClickTarget : MonoBehaviour
{
    public PlotManager Owner { get; set; }

    private void OnMouseDown()
    {
        if (Owner != null)
            Owner.HandleClick();
    }

    private void OnMouseOver()
    {
        if (Owner != null)
            Owner.HandleHover();
    }

    private void OnMouseExit()
    {
        if (Owner != null)
            Owner.ClearHover();
    }
}
