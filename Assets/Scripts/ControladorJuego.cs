using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    [SerializeField] private float tiempoMaximo;
    private float tiempoActual;
    private bool tiempoActivado = false;

    private void Start()
    {
        CambiarTemporizador(true);
        tiempoActual = tiempoMaximo;
    }

    private void Update()
    {
        if (tiempoActivado)
        {
            CambiarContador();
        }
    }

    private void CambiarContador()
    {
        tiempoActual -= Time.deltaTime;

        if (tiempoActual <= 0)
        {
            Debug.Log("Derrota");
            CambiarTemporizador(false);
        }
    }

    private void CambiarTemporizador(bool estado)
    {
        tiempoActivado = estado;
    }
}
