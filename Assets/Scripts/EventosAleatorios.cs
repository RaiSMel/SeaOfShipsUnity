using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventosAleatorios : MonoBehaviour
{
    private GameManager gameManager; // Referência ao GameManager
    private GameAudioManager audioManager;

    void Start()
    {
        // Busca o GameManager na cena
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogWarning("GameManager não encontrado na cena. Verifique se ele está presente.");
        }

        audioManager = FindObjectOfType<GameAudioManager>();
    }

    public void SegundoTiro()
    {
        if (gameManager != null && gameManager.topText != null)
        {
            gameManager.topText.text = "Atire novamente!";
            gameManager.playerTurn = true; // Atualiza o turno
        }
        else
        {
            Debug.LogWarning("topText ou GameManager não está atribuído.");
        }
    }

    public void TiroAleatorio(ref bool playerTurn, ref bool hasGainedExtraShot, List<GameObject> clickedTiles)
    {
        playerTurn = true;
        hasGainedExtraShot = true; // Marca que o jogador ganhou um tiro extra

        List<GameObject> availableTiles = new List<GameObject>();

        foreach (TileScript tileScript in gameManager.allTileScripts)
        {
            if (!clickedTiles.Contains(tileScript.gameObject))
            {
                availableTiles.Add(tileScript.gameObject);
            }
        }

        if (availableTiles.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableTiles.Count);
            GameObject randomTile = availableTiles[randomIndex];
            clickedTiles.Add(randomTile);
            gameManager.CheckHit(randomTile); // Chama CheckHit do GameManager
            gameManager.topText.text = "Você ganhou um novo tiro aleatório!";
        }

    }
}
