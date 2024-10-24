
using TMPro;
using UnityEngine;


public class MenuEntrada : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nomeDoJogador;
    [SerializeField] private TMP_InputField _nomeDaSala;

    public void CriaSala()
    {
        GestorDeRede.Instancia.MudaNick("teste");
        GestorDeRede.Instancia.CriaSala("teste");
    }

    public void EntraSala()
    {
        GestorDeRede.Instancia.MudaNick(_nomeDoJogador.text);
        GestorDeRede.Instancia.EntraSala(_nomeDaSala.text);
    }
}
