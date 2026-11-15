using System.Collections;
using UnityEngine;

public class VagonRespawn : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Transform puntoFinal;
    [SerializeField, Min(0.1f)] private float velocidad = 3f;

    [Header("Respawn")]
    [SerializeField, Min(0f)] private float tiempoRespawn = 2f;

    [Header("Audio")]
    [SerializeField, Min(0f)] private float duracionFadeOut = 0.8f;

    private Vector3 posicionInicial;

    private Renderer[] renderizadores;
    private Collider[] colliders;
    private AudioSource[] audios;

    private bool[] renderizadoresActivos;
    private bool[] collidersActivos;
    private float[] volumenesOriginales;

    private void Awake()
    {
        posicionInicial = transform.position;

        renderizadores = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        audios = GetComponentsInChildren<AudioSource>(true);

        renderizadoresActivos = new bool[renderizadores.Length];
        collidersActivos = new bool[colliders.Length];
        volumenesOriginales = new float[audios.Length];

        for (int i = 0; i < renderizadores.Length; i++)
            renderizadoresActivos[i] = renderizadores[i].enabled;

        for (int i = 0; i < colliders.Length; i++)
            collidersActivos[i] = colliders[i].enabled;

        for (int i = 0; i < audios.Length; i++)
            volumenesOriginales[i] = audios[i].volume;
    }

    private IEnumerator Start()
    {
        if (puntoFinal == null)
        {
            Debug.LogWarning(
                "Asigna el punto final en VagonRespawn.", this);

            yield break;
        }

        ReproducirAudio();

        while (true)
        {
            // Avanzar hasta el final del recorrido.
            while (Vector3.Distance(
                transform.position, puntoFinal.position) > 0.001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    puntoFinal.position,
                    velocidad * Time.deltaTime
                );

                yield return null;
            }

            transform.position = puntoFinal.position;

            // Ocultar el vagón, manteniendo activo el audio.
            MostrarVagon(false);

            // El fade está incluido dentro del tiempo de respawn.
            float tiempoEspera = Mathf.Max(0f, tiempoRespawn);
            float duracion = Mathf.Clamp(
                duracionFadeOut, 0f, tiempoEspera);

            float transcurrido = 0f;

            while (transcurrido < duracion)
            {
                transcurrido += Time.deltaTime;

                float progreso = Mathf.Clamp01(
                    transcurrido / duracion);

                for (int i = 0; i < audios.Length; i++)
                {
                    audios[i].volume = Mathf.Lerp(
                        volumenesOriginales[i],
                        0f,
                        progreso
                    );
                }

                yield return null;
            }

            // Detener el audio después del desvanecimiento.
            foreach (AudioSource audio in audios)
            {
                audio.volume = 0f;
                audio.Stop();
            }

            float esperaRestante = Mathf.Max(
                0f, tiempoEspera - transcurrido);

            if (esperaRestante > 0f)
                yield return new WaitForSeconds(esperaRestante);

            // Reaparecer en el origen.
            transform.position = posicionInicial;

            MostrarVagon(true);
            ReproducirAudio();
        }
    }

    private void MostrarVagon(bool mostrar)
    {
        for (int i = 0; i < renderizadores.Length; i++)
        {
            renderizadores[i].enabled =
                mostrar && renderizadoresActivos[i];
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled =
                mostrar && collidersActivos[i];
        }
    }

    private void ReproducirAudio()
    {
        for (int i = 0; i < audios.Length; i++)
        {
            AudioSource audio = audios[i];

            audio.volume = volumenesOriginales[i];

            if (audio.enabled
                && audio.gameObject.activeInHierarchy
                && audio.clip != null)
            {
                audio.loop = true;

                if (!audio.isPlaying)
                    audio.Play();
            }
        }
    }
}