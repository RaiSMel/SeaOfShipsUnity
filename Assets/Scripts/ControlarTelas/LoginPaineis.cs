using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginPaineis : MonoBehaviour
{
    public static void AbrirLoginStatus(TMP_Text Senha, bool SenhaInvalida, TMP_Text Email, bool EmailInvalido, GameObject LogarStatus)
    {
        if (!SenhaInvalida)
        {
            Senha.gameObject.SetActive(true);
        }
        else
        {
            Senha.gameObject.SetActive(false);
        }
        if (!EmailInvalido)
        {
            Email.gameObject.SetActive(true);
        }
        else
        {
            Email.gameObject.SetActive(false);
        }
        LogarStatus.SetActive(true);
    }
    public static void FecharLoginStatus(GameObject LogarStatus)
    {
        LogarStatus.SetActive(false);
    }
}
