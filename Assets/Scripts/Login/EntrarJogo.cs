using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EntrarJogo : MonoBehaviour
{
    public TMP_Text emailStatus;
    public TMP_Text senhaStatus;
    public TMP_Text loginInvalido;
    bool verificarSenha = true, verificarEmail = true;
    public GameObject logarStatus;
    public TMP_InputField email;
    public TMP_InputField senha;

    public bool ValidarEmail(string email)
    {
        string padraoEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, padraoEmail);
    }

    // Valida senha: pelo menos 8 caracteres, uma maiúscula, uma minúscula, um número e um caractere especial
    public bool ValidarSenha(string senha)
    {
        if (senha.Length < 8) return false;
        else return true;
    }
    public void ChamarEntrar()
    {
        StartCoroutine(EntrarNoJogo());
    }

    public void FecharPainelLoginStatus()
    {
        LoginPaineis.FecharLoginStatus(logarStatus);
    }
    IEnumerator EntrarNoJogo()
    {
        if (!ValidarEmail(email.text))
        {
            verificarEmail = false;
        }
        else verificarEmail=true;

        if (!ValidarSenha(senha.text))
        {
            verificarSenha=false;
        }
        else verificarSenha=true;

        if(!ValidarSenha(senha.text) || !ValidarEmail(email.text))
        {
            LoginPaineis.AbrirLoginStatus(senhaStatus, verificarSenha, emailStatus, verificarEmail, logarStatus, loginInvalido); 
            StopCoroutine(EntrarNoJogo());
        }
        else
        {

            WWWForm formularioEntrar = new();

            formularioEntrar.AddField("Email", email.text);
            formularioEntrar.AddField("Senha", senha.text);

            WWW www = new("http://localhost/BatalhaNaval/entrar.php", formularioEntrar);
            yield return www;
            if ( www.text == "-1")
            {
                LoginPaineis.AbrirLoginStatus(senhaStatus, true, emailStatus, true, logarStatus, loginInvalido);
            }

            else
            {
                SceneManager.LoadScene(2);
            }
        }
    }
}
