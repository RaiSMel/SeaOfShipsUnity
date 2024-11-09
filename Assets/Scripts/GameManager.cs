using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Ships")]
    public GameObject[] ships;
    public EnemyScript enemyScript;
    private ShipScript shipScript;
    private List<int[]> enemyShips;
    private int shipIndex = 0;
    public List<TileScript> allTileScripts;
    public bool playerTurn = true;

    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Button replayBtn;
    public Text topText;
    public Text playerShipText;
    public Text enemyShipText;
    public Button retornarBtn;
    public Button DoisTiros;
    public Button TiroAleatorio;
    public Button Escudo;

    [Header("Objects")]
    public GameObject missilePrefab;
    public GameObject enemyMissilePrefab;
    public GameObject firePrefab;
    public GameObject woodDock;

    private bool setupComplete = false;
    public bool Protect = false; // nova variavel que ira afetar o código a depender do escudo

    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires = new List<GameObject>();
    private List<GameObject> clickedTiles = new List<GameObject>();

    private int enemyShipCount = 5;
    public int playerShipCount = 5;
    private bool hasGainedExtraShot = false; // Controle do tiro extra

    private EventosAleatorios EvAle;

    private GameAudioManager audioManager;

    private Perks perksManager;


    // Start is called before the first frame update
    void Start()
    {
        EvAle = FindObjectOfType<EventosAleatorios>();

        if (EvAle == null)
        {
            Debug.LogError("RandomEventsScript não encontrado! Verifique se ele está na cena.");
        }

        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(NextShipClicked);
        rotateBtn.onClick.AddListener(RotateClicked);
        replayBtn.onClick.AddListener(ReplayClicked);
        enemyShips = enemyScript.PlaceEnemyShips();
        retornarBtn.onClick.AddListener(ReturnToMainMenu);
        audioManager = FindObjectOfType<GameAudioManager>();
        perksManager = FindObjectOfType<Perks>();
        DoisTiros.gameObject.SetActive(false);
        TiroAleatorio.gameObject.SetActive(false);
        Escudo.gameObject.SetActive(false);
        DoisTiros.onClick.AddListener(perksManager.doisTiros);
        TiroAleatorio.onClick.AddListener(perksManager.tiroAleatorio);
        Escudo.onClick.AddListener(perksManager.escudo);
    }


    private void NextShipClicked()
    {
        if (!shipScript.OnGameBoard())
        {
            shipScript.FlashColor(Color.red);
        }
        else
        {
            if (shipIndex <= ships.Length - 2)
            {
                shipIndex++;
                shipScript = ships[shipIndex].GetComponent<ShipScript>();
                shipScript.FlashColor(Color.yellow);
            }
            else
            {
                rotateBtn.gameObject.SetActive(false);
                nextBtn.gameObject.SetActive(false);
                DoisTiros.gameObject.SetActive(true);
                TiroAleatorio.gameObject.SetActive(true);
                Escudo.gameObject.SetActive(true);
                topText.text = "Selecione onde atirar";
                setupComplete = true;
                for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
            }
        }
    }

    public void TileClicked(GameObject tile)
    {
        if (setupComplete && playerTurn)
        {
            // Verifica se o tile já foi clicado
            if (!clickedTiles.Contains(tile))
            {
                Vector3 tilePos = tile.transform.position;
                tilePos.y += 15;
                playerTurn = false;
                Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
                //clickedTiles.Add(tile); Por razoes de praticidade esse metodo foi movido para as açoes pós acerto do missel
            }
            else
            {
                // Se o tile já foi clicado
                topText.text = "Você já atirou aqui!";
            }
        }
        else if (!setupComplete)
        {
            PlaceShip(tile);
            shipScript.SetClickedTile(tile);
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);
        ships[shipIndex].transform.localPosition = newVec;
    }

    void RotateClicked()
    {
        shipScript.RotateShip();
    }

    public void CheckHit(GameObject tile)
    {
        if (UnityEngine.Random.value <= 0.1f){//Chance do escudo ser ativado (10%)
            Protect = true;
        }
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitCount = 0;

        // Verifica se o tiro acerta um navio inimigo
        foreach (int[] tileNumArray in enemyShips)
        {
            if (tileNumArray.Contains(tileNum))
            {
                for (int i = 0; i < tileNumArray.Length; i++)
                {
                    if (tileNumArray[i] == tileNum)
                    {
                        if(Protect==true){// Quando escudo ta ativo ele nao contara como acertado
                            hitCount++;
                        }else{
                            tileNumArray[i] = -5;//Marca o tile como atingido
                            hitCount++;
                        }
                    }
                    else if (tileNumArray[i] == -5)
                    {
                        hitCount++; // Conta os hits em tiles já atingidos
                    }
                }
                if (hitCount == tileNumArray.Length)
                {
                    enemyShipCount--;
                    enemyShipText.text = enemyShipCount.ToString();
                    topText.text = "Navio Afundado";
                    if (enemyShipCount == 0)
                    {
                        GameOver(1);
                    }
                    enemyFires.Add(Instantiate(firePrefab, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                    clickedTiles.Add(tile);//Tile adicionado ao array de tiles atingidos apos a ação
                }
                else
                {
                    if(Protect==true){
                        EvAle.ProtecaoBarco();
                    }else{
                    topText.text = "Acertou";
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(255, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                    clickedTiles.Add(tile);//Tile adicionado ao array de tiles atingidos apos a ação
                    }
                }
                break;
            }
        }

        if (hitCount == 0)
        {
            tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
            tile.GetComponent<TileScript>().SwitchColors(1);
            topText.text = "Errou";
            clickedTiles.Add(tile);//Tile adicionado ao array de tiles atingidos apos a ação
            audioManager.PlayPobre();

            // 10% de chance - Segundo tiro
            if (UnityEngine.Random.value <= 0.1f)
            {
                EvAle.SegundoTiro();
            }
            else
            {
                // 10% de chance - Tiro aleatório
                if (!hasGainedExtraShot && UnityEngine.Random.value <= 0.1f)
                {
                    EvAle.TiroAleatorio(ref playerTurn, ref hasGainedExtraShot, clickedTiles);
                }
                else
                {
                    playerTurn = false;
                    Invoke("EndPlayerTurn", 1.0f);
                }
            }
        }
        else if (hitCount > 0)
        {
            playerTurn = true; // Se acertou, o jogador continua
        }
    }


    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)
    {
        enemyScript.MissileHit(tileNum);
        tile.y += 0.2f;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
        if (hitObj.GetComponent<ShipScript>().HitCheckSank())
        {
            enemyScript.SunkPlayer();
            playerShipCount--;
            playerShipText.text = playerShipCount.ToString();
            if (playerShipCount == 0)
            {
                GameOver(0);
            }
        }
    }

    private void EndPlayerTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(true);
        foreach (GameObject fire in playerFires) fire.SetActive(true);
        foreach (GameObject fire in enemyFires) fire.SetActive(false);
        enemyShipText.text = enemyShipCount.ToString();
        topText.text = "Turno do Inimigo";
        enemyScript.NPCTurn();
        ColorAllTiles(0);
        hasGainedExtraShot = false; // Reseta o tiro extra no final do turno
        playerTurn = false;
    }

    public void EndEnemyTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
        foreach (GameObject fire in playerFires) fire.SetActive(false);
        foreach (GameObject fire in enemyFires) fire.SetActive(true);
        playerShipText.text = playerShipCount.ToString();
        topText.text = "Selecione onde atirar";
        playerTurn = true;
        ColorAllTiles(1);
    }

    private void ColorAllTiles(int colorIndex)
    {
        foreach (TileScript tileScript in allTileScripts)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }

    public void GameOver(int winner)
    {
        if (winner == 1)
        {
            audioManager.PlayGanhamos();
            SceneManager.LoadScene("YouWin");
        }
        else
        {
            audioManager.PlayNaoAcredito();
            SceneManager.LoadScene("YouLose");
        }
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void ReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
