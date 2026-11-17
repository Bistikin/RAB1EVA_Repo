using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraEvent : MonoBehaviour
{

    [Header("Commands")]
    public bool will180ºTurn;

    [Header("References")]
    public GameObject PlayerCam;
    public Transform CameraTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerCam.transform.Rotate(Vector3.up * 180);
            PlayerCam.transform.Rotate(Vector3.right * 60);
            gameObject.SetActive(false); //El propio switch se apaga y desaparece de escena
        }

    }
}
