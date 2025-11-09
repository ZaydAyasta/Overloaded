using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_Tareas : MonoBehaviour
{
    [Header("References")]
    public GameObject tareaPrefab;
    public Transform contenedorTareas;

    [Header("Settings")]
    public int cantidadInicial = 6;
    [TextArea]
    public List<string> poolTareas = new List<string>()
    {
        "Barrer",
        "Limpiar ventanas",
        "Regar plantas",
        "Sacar la basura",
        "Encender luces",
        "Hacer la cena",
        "Hervir agua",
        "Lavar ropa",
        "Ordenar la cama"
    };

    private List<Toggle> toggles = new List<Toggle>();
    private System.Random rng = new System.Random();

    void Start()
    {
        GenerarListaAleatoria(cantidadInicial);
    }

    public void GenerarListaAleatoria(int cantidad)
    {
        //ClearLista();

        List<string> disponibles = new List<string>(poolTareas);

        //float offsetY = -210f;
        //float separacion = 30f;

        for (int i = 0; i < cantidad; i++)
        {
            int idx = rng.Next(disponibles.Count);
            string texto = disponibles[idx];
            disponibles.RemoveAt(idx);

            GameObject go = Instantiate(tareaPrefab, contenedorTareas);
            /*RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = new Vector2(-310, -offsetY);
            offsetY += separacion;*/

            Toggle tog = go.GetComponent<Toggle>();
            TextMeshProUGUI label = go.GetComponentInChildren<TextMeshProUGUI>();

            if (label != null)
            {
                label.text = texto;

                RectTransform rtLabel = label.GetComponent<RectTransform>();

                rtLabel.anchorMin = new Vector2(0, 0.5f);
                rtLabel.anchorMax = new Vector2(0, 0.5f);
                rtLabel.pivot = new Vector2(0, 0.5f);

                rtLabel.anchoredPosition = new Vector2(25f, 0f);

                rtLabel.sizeDelta = new Vector2(300f, rtLabel.sizeDelta.y);

                rtLabel.ForceUpdateRectTransforms();
            }


            if (tog != null)
            {
                tog.isOn = false;
                int index = toggles.Count;
                toggles.Add(tog);
                tog.onValueChanged.AddListener((val) => OnToggleChanged(val, index));
            }
        }
    }

    private void OnToggleChanged(bool marcado, int index)
    {
        TextMeshProUGUI label = toggles[index].GetComponentInChildren<TextMeshProUGUI>();
        if (label == null) return;

        if (marcado)
        {
            label.fontStyle = FontStyles.Strikethrough;
            label.color = Color.gray;
        }
        else
        {
            label.fontStyle = FontStyles.Normal;
            label.color = Color.white;
        }
    }

    public void CompletarTarea(int index)
    {
        if (index < 0 || index >= toggles.Count) return;
        toggles[index].isOn = true;
    }

    public void CompletarTareaPorTexto(string texto)
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            TextMeshProUGUI label = toggles[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null && label.text == texto)
            {
                toggles[i].isOn = true;
                return;
            }
        }
    }

    public void ClearLista()
    {
        for (int i = contenedorTareas.childCount - 1; i >= 0; i--)
        {
            Destroy(contenedorTareas.GetChild(i).gameObject);
        }

        toggles.Clear();
    }
}
