using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]
public class Card : ScriptableObject
{
    public Sprite cardArt;
    [TextArea(6, 10)]
    public string description;
    public DrawingAction drawingAction;
}

[System.Serializable]
public class CardCount
{
    public Card card;
    public int count = 1;

    public CardCount(Card card, int count)
    {
        this.card = card;
        this.count = count;
    }
}
