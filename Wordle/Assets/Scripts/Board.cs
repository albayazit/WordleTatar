using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[] 
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z,
    };

    private Row[] rows;

    private string[] solutions;
    private string[] validWords;
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
    public Button newWordButton;
    public Button tryAgainButton;

    private Button letterButtons;

    private void Awake() 
    {
        rows = GetComponentsInChildren<Row>();
    }

    private void Start() 
    {
        LoadData();
        NewGame();
    }

    public void NewGame() 
    {
        ClearBoard();
        SetRandomWord();

        enabled = true;
    }

    public void TryAgain() 
    {
        ClearBoard();

        enabled = true;
    }

    private void SetRandomWord()
    {
        word = solutions[Random.Range(0, solutions.Length)];
        word = word.ToLower().Trim();
    }

    private void LoadData() 
    {
        TextAsset textFile = Resources.Load("TatarWords/common") as TextAsset;
        validWords = textFile.text.Split('\n');

        textFile = Resources.Load("TatarWords/common") as TextAsset;
        solutions = textFile.text.Split('\n');
    }

    private void Update()
    {
        Row currentRow = rows[rowIndex];

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);

            invalidWordText.gameObject.SetActive(false);
        }
        else if (columnIndex >= currentRow.tiles.Length) 
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(currentRow);
            }
        }
        else
        {
            for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
            {
                if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                    currentRow.tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                    currentRow.tiles[columnIndex].SetState(occupiedState);
                    columnIndex++;
                    break;
                }
            }
        }
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

        if (HasWon(row)) {
            enabled = false; 
        }

        rowIndex++;
        columnIndex = 0;

        if (rowIndex >= rows.Length)
        {
            enabled = false;
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

        rowIndex = 0;
        columnIndex = 0;
    }

    private bool IsValidWord(string word)
    {
        for (int i = 0; i < validWords.Length; i++)
        {
            if (word.ToLower() == validWords[i])
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

    private void OnEnable() 
    {
        tryAgainButton.gameObject.SetActive(false);
        newWordButton.gameObject.SetActive(false);
    }

    private void OnDisable() 
    {
        tryAgainButton.gameObject.SetActive(true);
        newWordButton.gameObject.SetActive(true);
    }

    public void GetButtonClick(Button letterButtons)
    {
        Row currentRow = rows[rowIndex];
        string buttonText = letterButtons.GetComponentInChildren<TextMeshProUGUI>().text;

        if (buttonText == "<<<") 
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);

            invalidWordText.gameObject.SetActive(false);
        }
        else if (columnIndex >= currentRow.tiles.Length) 
        {
            if (buttonText == "===")
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
