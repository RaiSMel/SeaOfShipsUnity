using UnityEngine;
using UnityEngine.SceneManagement;

public class MoverCenas : MonoBehaviour
{
public void MoverEntreCenas(int scena)
    {
        SceneManager.LoadScene(scena);
    }
}
