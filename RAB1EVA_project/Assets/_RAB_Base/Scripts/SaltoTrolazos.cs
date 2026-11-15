using UnityEngine;

public class SaltoSuave : MonoBehaviour
{
    public float altura = 0.5f;
    public float velocidad = 3f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        float salto = (1f - Mathf.Cos(Time.time * velocidad)) * altura / 2f;

        transform.localPosition = posicionInicial + Vector3.up * salto;
    }
}