using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    public InterstitialAd ad;
    public RewardedAd rewardedAd;
    public GameObject noAds;

    private Row[] rows;

    private string[] solutions;
    private string[] validWords;
    private string[] completed;
    private string word;

    private int rowIndex;
    private int columnIndex;

    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongState;
    public Tile.State incorrectState;

    [Header("UI")]
    public TextMeshProUGUI invalidWordText;

    public GameObject gameOver;
    public GameObject gameWin;


    private Button letterButtons;

    public Color defaultKeyColor;
    public Color wrongKeyColor;
    public Color incorrectKeyColor;
    public Color correctKeyColor;

    public Button[] allButtons;

    public TextMeshProUGUI translate;
    public TextMeshProUGUI levelText;

    public GameObject winner;

    private int complete;
    private bool tryAgain;

    public string level;

    private void Awake() 
    {
        rows = GetComponentsInChildren<Row>();
    }

    private void Start() 
    {
        level = SceneManager.GetActiveScene().name;
        LoadData();
        NewGame();
    }

    public void NewGame() 
    {
        noAds.SetActive(false);
        winner.SetActive(false);
        tryAgain = false;
        ClearBoard();
        SetRandomWord();
        complete = 0;
        levelText.text = PlayerPrefs.GetInt("Completed", 0).ToString();
        gameOver.gameObject.SetActive(false);
        gameWin.gameObject.SetActive(false);
    }

    public void TryAgain() 
    {
        ad.ShowAd();
        noAds.SetActive(false);
        winner.SetActive(false);
        tryAgain = true;
        ClearBoard();
        gameOver.gameObject.SetActive(false);
        gameWin.gameObject.SetActive(false);
    }

    private void SetRandomWord()
    {
        if (PlayerPrefs.GetInt("Completed", 0) < 300)
            word = solutions[PlayerPrefs.GetInt("Completed", 0)];
        else
            word = solutions[Random.Range(0, 300)];
        translate.text = word;
        word = word.ToLower().Trim().Substring(0, 5);
    }

    private void LoadData() 
    {
        TextAsset textFile = Resources.Load("TatarWords/all_words") as TextAsset;
        validWords = textFile.text.Split('\n');
        textFile = Resources.Load("TatarWords/solutions") as TextAsset;
        solutions = textFile.text.Split('\n');
    }

    private void Update()
    {
        if (columnIndex < 7)
        {
            Row currentRow = rows[rowIndex];

            if (columnIndex >= currentRow.tiles.Length) 
            {
                SubmitRow(currentRow);
            }
        }
        if(Input.GetKeyDown(KeyCode.D))
            PlayerPrefs.DeleteAll();
    }

    private void SubmitRow(Row row)
    {
        if (!IsValidWord(row.word))
        {
            invalidWordText.gameObject.SetActive(true);
            return;
        }

        string remaining = word;

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (char.ToLower(tile.letter) == word[i])
            {
                tile.SetState(correctState);

                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
            }
            else if (!word.Contains(char.ToLower(tile.letter)))
            {
                tile.SetState(incorrectState);
            }
        }

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.state != correctState && tile.state != incorrectState)
            {
                if (remaining.Contains(char.ToLower(tile.letter)))
                {
                    tile.SetState(wrongState);

                    int index = remaining.IndexOf(char.ToLower(tile.letter));
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else 
                {
                    tile.SetState(incorrectState);
                }
            }
        }

        SetButtonColor(row);
        
        if (HasWon(row)) {
            gameWin.gameObject.SetActive(true);
            complete += 1;
            if (complete == 1) {
                PlayerPrefs.SetInt("Completed", PlayerPrefs.GetInt("Completed", 0) + 1);
                levelText.text = PlayerPrefs.GetInt("Completed", 0).ToString();
                if (PlayerPrefs.GetInt("Completed", 0) == 300)
                    winner.SetActive(true);
            }
        }

        rowIndex++;
        columnIndex = 0;

        if (rowIndex >= rows.Length)
        {
            gameOver.gameObject.SetActive(true);
            rowIndex = 0;
        }
    }

    public void WinGame() {
        gameWin.gameObject.SetActive(true);
        complete += 1;
        if (complete == 1) {
            PlayerPrefs.SetInt("Completed", PlayerPrefs.GetInt("Completed", 0) + 1);
            levelText.text = PlayerPrefs.GetInt("Completed", 0).ToString();
        }
    }

    private void SetButtonColor(Row row)
    {
        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];
            for (int j = 0; j < allButtons.Length; j++)
            {
                if (allButtons[j].GetComponentInChildren<TextMeshProUGUI>().text[0] == tile.letter)
                {
                    if (tile.state == wrongState && allButtons[j].image.color != correctKeyColor)
                    {
                        allButtons[j].image.color = wrongKeyColor;
                    }
                    else if (tile.state == incorrectState && allButtons[j].image.color != correctKeyColor && allButtons[j].image.color != wrongKeyColor)
                    {
                        allButtons[j].image.color = incorrectKeyColor;
                    }
                    else if (tile.state == correctState)
                    {
                        allButtons[j].image.color = correctKeyColor;
                    }
                }
            }
        }
    }

    private void ClearBoard()
    {
        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].tiles.Length; col++)
            {
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetState(emptyState);
            }
        }
        for (int j = 0; j < allButtons.Length; j++)
        {
            if (tryAgain && PlayerPrefs.GetInt("ADS") == 1) {
                if (allButtons[j].image.color == correctKeyColor) {
                    allButtons[j].image.color = wrongKeyColor;
                }
            } else {
                allButtons[j].image.color = defaultKeyColor;
            }
        }

        rowIndex = 0;
        columnIndex = 0;
    }

    private bool IsValidWord(string word)
    {
        for (int i = 0; i < validWords.Length; i++)
        {
            if (word.ToLower() == validWords[i].Substring(0, 5))
            {
                return true;
            }
        }
        return false;
    }

    private bool HasWon(Row row)
    {
        for (int i = 0; i < row.tiles.Length; i++)
        {
            if (row.tiles[i].state != correctState) {
                return false;
            }
        }
        return true;
    }

    public void GetButtonClick(Button letterButtons)
    {
        Row currentRow = rows[rowIndex];
        string buttonText = letterButtons.GetComponentInChildren<TextMeshProUGUI>().text;

        if (buttonText == "x") 
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);

            invalidWordText.gameObject.SetActive(false);
        }
        else if (columnIndex >= currentRow.tiles.Length) 
        {
            if (buttonText == "=")
            {
                SubmitRow(currentRow);
            }
        }
        else
        {
            currentRow.tiles[columnIndex].SetLetter((char)buttonText[0]);
            currentRow.tiles[columnIndex].SetState(occupiedState);
            columnIndex++;
        }
    }
}
