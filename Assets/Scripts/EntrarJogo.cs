using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntrarJogo : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField senha;

    public void ChamarEntrar()
    {
        StartCoroutine(EntrarNoJogo());
    }
    IEnumerator EntrarNoJogo()
    {
        WWWForm formularioEntrar = new();

        formularioEntrar.AddField("Email", email.text);
        formularioEntrar.AddField("Senha", senha.text);

        WWW www = new("http://localhost/BatalhaNaval/entrar.php", formularioEntrar);
        yield return www;
        if ( www.text == "-1")
        {
            Debug.Log("Algo deu errado!");
        }

        else
        {
            SceneManager.LoadScene(2);
        }
        
    }
}
