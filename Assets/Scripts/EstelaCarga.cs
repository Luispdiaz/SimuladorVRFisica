using UnityEngine;
using System.Collections.Generic;

public class EstelaCarga : MonoBehaviour
{
    public GameObject miniSpherePrefab;
    public float intervalo = 0.005f; // Tiempo entre cada esfera
    public float duracionEsfera = 2f; // Tiempo antes de destruir cada esfera

    private List<GameObject> estela = new List<GameObject>();
    private bool enMovimiento = false;
    private float tiempoUltimaEsfera;

    void Update()
    {
        if (enMovimiento && Time.time - tiempoUltimaEsfera >= intervalo)
        {
            GameObject esfera = Instantiate(miniSpherePrefab, transform.position, Quaternion.identity);
            estela.Add(esfera);
            //Destroy(esfera, duracionEsfera);
            tiempoUltimaEsfera = Time.time;
        }
    }

    public void IniciarEstela()
    {
        enMovimiento = true;
        tiempoUltimaEsfera = Time.time;
    }

    public void DetenerEstela()
    {
        enMovimiento = false;
        foreach (GameObject esfera in estela)
        {
            if (esfera != null) Destroy(esfera);
        }
        estela.Clear();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlanoFisico"))
        {
            DetenerEstela();
        }
    }
}