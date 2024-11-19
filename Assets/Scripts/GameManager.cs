using System;
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
    public bool tiroFileira = false;
    public bool tiroDuplo = false;

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
        DoisTiros.onClick.AddListener(perkTiroDuplo);
        TiroAleatorio.onClick.AddListener(perkTiroFileira);
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
                if (tiroFileira == true)
                {
                    TiroEmFileira(tile);
                    playerTurn = false;
                    Invoke("EndPlayerTurn", 1.0f);
                    tiroFileira = false;

                }
                else
                {
                    Vector3 tilePos = tile.transform.position;
                    tilePos.y += 15;
                    playerTurn = false;
                    Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
                    // clickedTiles.Add(tile);
                }
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
        if (UnityEngine.Random.value <= 0.1f)
        {//Chance do escudo ser ativado (10%)
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
                        if (Protect == true)
                        {// Quando escudo ta ativo ele nao contara como acertado
                            hitCount++;
                        }
                        else
                        {
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
                    if (Protect == true)
                    {
                        EvAle.ProtecaoBarco();
                    }
                    else
                    {
                        topText.text = "Acertou";

                        List<AudioClip> audiosSelecionados = new List<AudioClip>
                    {
                        audioManager.podeNaoParecer,
                        audioManager.acertoMizeravi,
                        audioManager.eNoisVelho

                    };

                        audioManager.PlayRandomAudio(audiosSelecionados);

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
            if (tiroDuplo == true)
            {
                tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
                tile.GetComponent<TileScript>().SwitchColors(1);
                topText.text = "Errou";
                audioManager.PlayPobre();
                EvAle.SegundoTiro();
                tiroDuplo = false;
            }
            else
            {
                tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
                tile.GetComponent<TileScript>().SwitchColors(1);
                topText.text = "Errou";

                List<AudioClip> audiosSelecionados = new List<AudioClip>
                {
                    audioManager.pobre,
                    audioManager.taTriste,
                    audioManager.gp,
                    audioManager.areYou,
                    audioManager.errou
                };
                audioManager.PlayRandomAudio(audiosSelecionados);
                clickedTiles.Add(tile);

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
        }
        else if (hitCount > 0)
        {
            playerTurn = true; // Se acertou, o jogador continua
        }
    }

        private void TiroEmFileira(GameObject tile)
    {
        topText.text = "Atirando em fileira...";
        // Pega o número do tile clicado
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);

        // Vê qual a linha do tile com base no número (1 a 100, com 10 tiles por linha)
        int rowNum = (tileNum - 1) / 10;

        // Pega cada tile de cada linha
        foreach (TileScript tileScript in allTileScripts)
        {
            // Verifica se o tile já foi clicado
            if (clickedTiles.Contains(tileScript.gameObject))
            {
                continue; // Pula para o próximo tile
            }

            // Pega o tile atual do loop
            int currentTileNum = Int32.Parse(Regex.Match(tileScript.gameObject.name, @"\d+").Value);
            int currentRowNum = (currentTileNum - 1) / 10; // Calcula a linha do tile atual

            // Se o tile estiver na mesma linha
            if (currentRowNum == rowNum)
            {
                bool isHit = false;
                int hitCount = 0;

                // Verifica se o tile atual faz parte de um navio inimigo
                foreach (int[] tileNumArray in enemyShips)
                {
                    if (tileNumArray.Contains(currentTileNum))
                    {
                        // Atualiza o estado do navio
                        for (int i = 0; i < tileNumArray.Length; i++)
                        {
                            if (tileNumArray[i] == currentTileNum)
                            {
                                tileNumArray[i] = -5; // Marca o tile como atingido
                                hitCount++;
                            }
                            else if (tileNumArray[i] == -5)
                            {
                                hitCount++; // Conta os hits em tiles já atingidos
                            }
                        }

                        // Verifica se o navio foi completamente afundado
                        if (hitCount == tileNumArray.Length)
                        {
                            enemyShipCount--;
                            enemyShipText.text = enemyShipCount.ToString();
                            if (enemyShipCount == 0)
                            {
                                GameOver(1);
                            }

                            // Marca o tile como afundado
                            tileScript.SetTileColor(1, new Color32(68, 0, 0, 255));
                            tileScript.SwitchColors(1);
                            isHit = true;
                        }
                        else
                        {
                            // Marca o tile como atingido
                            tileScript.SetTileColor(1, new Color32(255, 0, 0, 255));
                            tileScript.SwitchColors(1);
                            isHit = true;
                        }
                        break;
                    }
                }
                if (!isHit)
                {
                    tileScript.SetTileColor(1, new Color32(38, 57, 76, 255));
                    tileScript.SwitchColors(1);                }

                // Adiciona o tile clicado à lista
                clickedTiles.Add(tileScript.gameObject);
            }
        }
    }

    public void perkTiroFileira()
    {
        tiroFileira = true;
        Button botao = TiroAleatorio.GetComponent<Button>();
        botao.interactable = false;
    }

    public void perkTiroDuplo()
    {
        tiroDuplo = true;
        Button botao = DoisTiros.GetComponent<Button>();
        botao.interactable = false;

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
