using UnityEngine;

public class IsometricZ : MonoBehaviour
{
    private void Start()
    {
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x, position.y, position.y / 100f);
    }
}
