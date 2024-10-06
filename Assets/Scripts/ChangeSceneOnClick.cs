using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeSceneOnClick : MonoBehaviour
{
    public string Cena; // Nome da cena para carregar (configure no Inspetor Unity)

    public Button button;

    private void Start()
    {
        button.onClick.AddListener(LoadScene); // Adiciona um ouvinte para o evento de clique do botão
    }

    public void LoadScene()
    {

        SceneManager.LoadScene(Cena); // Carrega a cena especificada

    }
}