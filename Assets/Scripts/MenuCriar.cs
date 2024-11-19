using TMPro;
using UnityEngine;

public class MenuCriar : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nomeDaSala;
    [SerializeField] private string _nomeDoJogador = "Rai";

    public void CriaSala()
    {
        GestorDeRede.Instancia.CriaSala(_nomeDaSala.text);
    }

}
