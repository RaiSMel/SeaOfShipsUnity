using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Loja : MonoBehaviour
{
    public TextMeshProUGUI Moedas;
    string[] Perks = JogadorLogado.jogadorLogado.perks.Split(";");
    public GameObject BtnFileira;
    public GameObject BtnDuasVezes;
    public GameObject BtnEscudo;

    private void Start()
    {
        AtualizarMoedas();
        verificarPerks();
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

    public void verificarPerks()
    {
        foreach  (string Perk in Perks)
        {
            if(Perk == "1")
            {
                BtnDuasVezes.GetComponent<Button>().interactable = false;
            }
            else if(Perk == "2")
            {
                BtnFileira.GetComponent<Button>().interactable = false;
            }
            else if(Perk == "3")
            {
                BtnEscudo.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void ComprarPerk( string ID_Perk)
    {
        Array.Resize(ref Perks, Perks.Length + 1);
        Perks[Perks.Length - 1] = ID_Perk;
        JogadorLogado.jogadorLogado.perks = string.Join(";", Perks);
        verificarPerks();
    }


}
