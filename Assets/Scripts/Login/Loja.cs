using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Loja : MonoBehaviour
{
    public TextMeshProUGUI Moedas;

    private void Start()
    {
        AtualizarMoedas();
    }

    public void AtualizarMoedas()
    {
        Moedas.text = JogadorLogado.jogadorLogado.Moeda.ToString();
    }
    public void ComprarMoeda(int QtdMoeda)
    {
        JogadorLogado.jogadorLogado.Moeda = JogadorLogado.jogadorLogado.Moeda + QtdMoeda;
        AtualizarMoedas();
    }
}
