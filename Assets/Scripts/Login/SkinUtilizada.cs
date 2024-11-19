using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinUtilizad : MonoBehaviour
{
    public GameObject Marinha;
    public GameObject Padrao;

    public void MarinhaSelect()
    {
        Padrao.GetComponent<Button>().interactable = true;
        Marinha.GetComponent<Button>().interactable = false;
        JogadorLogado.jogadorLogado.TipoJogador = "Marinha";
    }

    public void PadraoSelect()
    {
        Padrao.GetComponent<Button>().interactable = false;
        Marinha.GetComponent<Button>().interactable = true;
        JogadorLogado.jogadorLogado.TipoJogador = "Padrao";
    }
}
