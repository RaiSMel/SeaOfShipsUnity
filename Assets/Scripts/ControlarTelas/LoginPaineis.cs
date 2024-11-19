using TMPro;
using UnityEngine;

public class LoginPaineis : MonoBehaviour
{
    public static void AbrirLoginStatus(TMP_Text Senha, bool SenhaInvalida, TMP_Text Email, bool EmailInvalido, GameObject LogarStatus, TMP_Text LoginInvalido)
    {
        if (!SenhaInvalida)
        {
            LoginInvalido.gameObject.SetActive(false);
            Senha.gameObject.SetActive(true);
        }
        else
        {
            Senha.gameObject.SetActive(false);
        }
        if (!EmailInvalido)
        {
            LoginInvalido.gameObject.SetActive(false);
            Email.gameObject.SetActive(true);
        }
        else
        {
            Email.gameObject.SetActive(false);
        }
        LogarStatus.SetActive(true);

        if(SenhaInvalida && EmailInvalido) 
        {
            LoginInvalido.gameObject.SetActive(true);
        }

    }
    public static void FecharLoginStatus(GameObject LogarStatus)
    {
        LogarStatus.SetActive(false);
    }
}
