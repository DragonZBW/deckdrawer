using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckEditorGridCell : MonoBehaviour
{
    public Image cardImage;
    public TMP_Text descText;
    public TMP_Text quantityText;
    public Button minusButton;
    public Button plusButton;

    [HideInInspector] public int quantity;
    [HideInInspector] public Card card;

    /// <summary>
    /// Set up the grid cell to display a particular card
    /// </summary>
    public void Setup(CardCount cc)
    {
        card = cc.card;

        cardImage.sprite = cc.card.cardArt;
        descText.text = "<b>" + cc.card.name + "</b>\n" + cc.card.description;
        quantity = cc.count;

        UpdateButtons();
    }

    /// <summary>
    /// Callback for clicking plus button, increases quantity by 1
    /// </summary>
    public void OnPlusButton()
    {
        quantity++;
        UpdateButtons();
    }

    /// <summary>
    /// Callback for clicking minus button, decreases quantity by 1
    /// </summary>
    public void OnMinusButton()
    {
        quantity--;
        UpdateButtons();
    }

    public void SetQuantity(int quantity)
    {
        this.quantity = quantity;
        UpdateButtons();
    }

    // Update plus + minus buttons enabled state based on quantity limits
    private void UpdateButtons()
    {
        minusButton.interactable = quantity > 0;
        plusButton.interactable = quantity < 20;
        quantityText.text = "x" + quantity;
    }
}
