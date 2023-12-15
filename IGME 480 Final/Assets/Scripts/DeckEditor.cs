using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum DrawColor
{
    Blue,
    Cyan,
    Green,
    Yellow,
    Orange,
    Red,
    Purple
}

public class DeckEditor : MonoBehaviour
{
    public DeckEditorGridCell gridCellPrefab;
    public Transform gridCellParent;
    [Space]
    public Button initColorButton;
    public TMP_InputField bgColorField;
    public Image bgColorDisp;
    [Space]
    public FilePicker filePicker;
    public string fileFormatKey;

    private List<DeckEditorGridCell> cells = new List<DeckEditorGridCell>();
    private DrawColor color;

    public static DeckEditor inst;

    private void Start()
    {
        inst = this;
    }

    public void Open()
    {
        // Initialize cells
        if(cells.Count == 0)
        {
            for (int i = 0; i < Manager.inst.defaultDeck.Length; i++)
            {
                DeckEditorGridCell cell = Instantiate(gridCellPrefab, gridCellParent);
                cell.Setup(Manager.inst.defaultDeck[i]);
                cells.Add(cell);
            }

            // Initialize color settings
            color = DrawColor.Blue;
            initColorButton.image.color = Color.blue;
            bgColorField.text = "#4D4D4D";
            ColorUtility.TryParseHtmlString(bgColorField.text, out Color bgCol);
            bgColorDisp.color = bgCol;
        }

        gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Close()
    {
        // Get card counts to send to manager
        List<CardCount> counts = new List<CardCount>();
        for(int i = 0; i < cells.Count; i++)
        {
            counts.Add(new CardCount(cells[i].card, cells[i].quantity));
        }

        Manager.inst.ClearDrawing();
        Manager.inst.SetCardCounts(counts.ToArray());

        gameObject.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResetCounts()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Setup(Manager.inst.defaultDeck[i]);
        }

        // Initialize color settings
        color = DrawColor.Blue;
        initColorButton.image.color = Color.blue;
        bgColorField.text = "#4D4D4D";
        ColorUtility.TryParseHtmlString(bgColorField.text, out Color bgCol);
        bgColorDisp.color = bgCol;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ZeroCounts()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].SetQuantity(0);
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ChangeInitColor()
    {
        color = (DrawColor)(((int)color + 1) % 7);
        initColorButton.image.color = Manager.GetColor(color);

        Manager.inst.SetColor(Manager.GetColor(color));

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ChangeBgColor(string text)
    {
        bool addCaret = false;
        if (!text.StartsWith("#"))
        {
            text = "#" + text;
            addCaret = true;
        }

        text = text.ToUpper();

        bgColorField.text = text;
        if (addCaret)
            bgColorField.stringPosition++;
        ColorUtility.TryParseHtmlString(text, out Color bgCol);
        bgColorDisp.color = bgCol;

        Manager.inst.SetBGColor(bgCol);
    }

    public void Save()
    {
        filePicker.Open("Enter file path to save to:", SaveTo);
    }

    public void Load()
    {
        filePicker.Open("Enter file path to load from:", LoadFrom);
    }

    private void SaveTo(string filepath)
    {
        if(!filepath.EndsWith(".deck"))
        {
            filepath += ".deck";
        }

        if(Path.GetFileName(filepath).StartsWith("."))
        {
            filepath = Path.GetDirectoryName(filepath) + "new_deck.deck";
        }

        if(!Directory.Exists(Path.GetDirectoryName(filepath)))
        {
            ErrorLog.inst.Error("Error: The directory does not exist.");
            return;
        }

        try
        {
            if(File.Exists(filepath))
            {
                BinaryReader reader = new BinaryReader(File.OpenRead(filepath));
                string formatKey = reader.ReadString();
                if(formatKey != fileFormatKey)
                {
                    reader.Close();
                    ErrorLog.inst.Error("Error: Trying to save over a file with the wrong format.");
                    return;
                }
                reader.Close();
            }

            BinaryWriter writer = new BinaryWriter(File.Create(filepath));

            // Format key
            writer.Write(fileFormatKey);

            // Colors
            writer.Write((int)color);

            string colorStr = ColorUtility.ToHtmlStringRGB(bgColorDisp.color);
            writer.Write(colorStr);

            // Get card counts to send to file
            List<CardCount> counts = new List<CardCount>();
            for (int i = 0; i < cells.Count; i++)
            {
                counts.Add(new CardCount(cells[i].card, cells[i].quantity));
            }

            writer.Write(counts.Count);
            foreach (CardCount cc in counts)
            {
                writer.Write(Manager.inst.IndexOfCard(cc.card));
                writer.Write(cc.count);
            }

            writer.Close();
            ErrorLog.inst.Log("Saved to " + Path.GetFileName(filepath));
        }
        catch(Exception e)
        {
            ErrorLog.inst.Error("Error: " + e.Message);
        }
    }

    private void LoadFrom(string filepath)
    {
        if (!filepath.EndsWith(".deck"))
        {
            filepath += ".deck";
        }

        if (!File.Exists(filepath))
        {
            ErrorLog.inst.Error("Error: The file does not exist.");
            return;
        }

        try
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(filepath));

            // format key
            string formatKey = reader.ReadString();
            if(formatKey != fileFormatKey)
            {
                reader.Close();
                ErrorLog.inst.Error("Error: Trying to load a file with the wrong format.");
                return;
            }

            // colors
            color = (DrawColor)reader.ReadInt32();

            initColorButton.image.color = Manager.GetColor(color);
            bgColorField.text = "#" + reader.ReadString();
            ColorUtility.TryParseHtmlString(bgColorField.text, out Color bgCol);
            bgColorDisp.color = bgCol;

            // card counts
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int cardIndex = reader.ReadInt32();
                int cardCount = reader.ReadInt32();

                for (int j = 0; j < cells.Count; j++)
                {
                    if (cells[j].card == Manager.inst.defaultDeck[cardIndex].card)
                    {
                        cells[j].Setup(new CardCount(cells[j].card, cardCount));
                        break;
                    }
                }
            }

            reader.Close();
            ErrorLog.inst.Log("Loaded deck from " + Path.GetFileName(filepath));
        }
        catch(Exception e)
        {
            ErrorLog.inst.Error("Error: " + e.Message);
        }
    }
}
