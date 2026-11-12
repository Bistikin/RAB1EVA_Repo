using UnityEngine;

public class GiroObstaculo : MonoBehaviour
{
    [SerializeField] private float velocidad = 90f;

    private void Update()
    {
        transform.Rotate(
            Vector3.up,
            velocidad * Time.deltaTime,
            Space.World
        );
    }
}