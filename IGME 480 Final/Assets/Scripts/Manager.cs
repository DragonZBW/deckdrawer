using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum DrawingAction
{
    LiftToRandom,
    LiftToTopRightFront,
    LiftToTopRightBack,
    LiftToTopLeftFront,
    LiftToTopLeftBack,
    LiftToBotRightFront,
    LiftToBotRightBack,
    LiftToBotLeftFront,
    LiftToBotLeftBack,
    LineToRandom,
    LineToLeft,
    LineToBack,
    LineToRight,
    LineToFront,
    LineToTop,
    LineToBottom,
    SmallCircle,
    LargeCircle,
    FlipCoinToEnd,
    FlipCoinToReshuffle,
    SetColorPurple,
    SetColorRed,
    SetColorOrange,
    SetColorYellow,
    SetColorGreen,
    SetColorCyan,
    SetColorBlue
}

public enum MinMax
{
    Min,
    Center,
    Max
}

public class Manager : MonoBehaviour
{
    public Image deckImage;
    public TMP_Text deckCardCountText;
    public Image drawnCardImage;
    public TMP_Text drawnCardDescText;
    [Space]
    public Vector3 spaceMin;
    public Vector3 spaceMax;
    public GameObject dotPrefab;
    public GameObject linePrefab;
    public GameObject circlePrefab;
    [Space]
    public Sprite cardBackSprite;
    public CardCount[] defaultDeck;

    private List<Card> deckCards = new List<Card>();
    private List<Card> drawnCards = new List<Card>();

    private float holdSpaceTimer;

    private Vector3 currPos;
    private Color color = Color.blue;

    private List<Transform> paths = new List<Transform>();
    private List<Transform> dots = new List<Transform>();

    public static Manager inst;

    private void Start()
    {
        SetCardCounts(defaultDeck);

        inst = this;
    }

    private void Update()
    {
        // Can't draw cards?
        if (!AreCardsRemaining())
        {
            holdSpaceTimer = 0;
            return;
        }

        // Draw card when pressing the key
        if(Input.GetKeyDown(KeyCode.Space))
        {
            DrawCard();
        }

        // Reset hold timer when releasing the key
        if(Input.GetKeyUp(KeyCode.Space))
        {
            holdSpaceTimer = 0;
        }

        // Draw cards continually when holding the key
        if(Input.GetKey(KeyCode.Space))
        {
            holdSpaceTimer += Time.deltaTime;
            if(holdSpaceTimer > 1)
            {
                DrawCard();
                holdSpaceTimer = .9f;
            }
        }
    }

    public void SetColor(Color col)
    {
        color = col;
    }

    public void SetBGColor(Color col)
    {
        Camera.main.backgroundColor = col;
    }

    public void SetCardCounts(CardCount[] counts)
    {
        // Initialize deck
        deckCards.Clear();
        drawnCards.Clear();
        foreach (CardCount cc in counts)
        {
            for (int i = 0; i < cc.count; i++)
                deckCards.Add(cc.card);
        }

        // Shuffle deck
        deckCards.Shuffle();

        // Display the deck
        deckImage.sprite = cardBackSprite;
        deckCardCountText.text = deckCards.Count.ToString();
        drawnCardImage.sprite = null;
        drawnCardDescText.text = string.Empty;

        // Draw initial point
        LiftTo(SpaceExtent(MinMax.Center, MinMax.Center, MinMax.Center));
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void DrawCard()
    {
        // Move drawn card
        Card drawnCard = deckCards[deckCards.Count - 1];
        drawnCards.Add(deckCards[deckCards.Count - 1]);
        deckCards.RemoveAt(deckCards.Count - 1);

        // Display the drawn card
        deckCardCountText.text = deckCards.Count.ToString();
        drawnCardImage.sprite = drawnCard.cardArt;
        drawnCardDescText.text = "<b>" + drawnCard.name + "</b>\n" + drawnCard.description;
        if (deckCards.Count == 0)
            deckImage.sprite = null;

        // Perform the draw action
        DrawAction(drawnCard);
    }

    private void DrawAction(Card drawnCard)
    {
        switch(drawnCard.drawingAction)
        {
            case DrawingAction.LiftToRandom:
                LiftTo(RandomPos());
                break;
            case DrawingAction.LiftToTopRightFront:
                LiftTo(SpaceExtent(MinMax.Max, MinMax.Max, MinMax.Min));
                break;
            case DrawingAction.LiftToTopRightBack:
                LiftTo(SpaceExtent(MinMax.Max, MinMax.Max, MinMax.Max));
                break;
            case DrawingAction.LiftToTopLeftFront:
                LiftTo(SpaceExtent(MinMax.Min, MinMax.Max, MinMax.Min));
                break;
            case DrawingAction.LiftToTopLeftBack:
                LiftTo(SpaceExtent(MinMax.Min, MinMax.Max, MinMax.Max));
                break;
            case DrawingAction.LiftToBotRightFront:
                LiftTo(SpaceExtent(MinMax.Max, MinMax.Min, MinMax.Min));
                break;
            case DrawingAction.LiftToBotRightBack:
                LiftTo(SpaceExtent(MinMax.Max, MinMax.Min, MinMax.Max));
                break;
            case DrawingAction.LiftToBotLeftFront:
                LiftTo(SpaceExtent(MinMax.Min, MinMax.Min, MinMax.Min));
                break;
            case DrawingAction.LiftToBotLeftBack:
                LiftTo(SpaceExtent(MinMax.Min, MinMax.Min, MinMax.Max));
                break;
            case DrawingAction.LineToRandom:
                LineTo(RandomPos());
                break;
            case DrawingAction.LineToLeft:
                LineTo(SpaceExtent(MinMax.Min, MinMax.Center, MinMax.Center) * .5f);
                break;
            case DrawingAction.LineToBack:
                LineTo(SpaceExtent(MinMax.Center, MinMax.Center, MinMax.Max) * .5f);
                break;
            case DrawingAction.LineToRight:
                LineTo(SpaceExtent(MinMax.Max, MinMax.Center, MinMax.Center) * .5f);
                break;
            case DrawingAction.LineToFront:
                LineTo(SpaceExtent(MinMax.Center, MinMax.Center, MinMax.Min) * .5f);
                break;
            case DrawingAction.LineToTop:
                LineTo(SpaceExtent(MinMax.Center, MinMax.Max, MinMax.Center) * .5f);
                break;
            case DrawingAction.LineToBottom:
                LineTo(SpaceExtent(MinMax.Center, MinMax.Min, MinMax.Center) * .5f);
                break;
            case DrawingAction.SmallCircle:
                Circle(0.08f);
                break;
            case DrawingAction.LargeCircle:
                Circle(0.16f);
                break;
            case DrawingAction.FlipCoinToEnd:
                if(Random.Range(0,2) == 0)
                {
                    for(int i = deckCards.Count - 1; i >= 0; i--)
                    {
                        drawnCards.Add(deckCards[deckCards.Count - 1]);
                        deckCards.RemoveAt(deckCards.Count - 1);

                        // Display the deck
                        deckImage.sprite = null;
                        deckCardCountText.text = deckCards.Count.ToString();
                    }
                }    
                break;
            case DrawingAction.FlipCoinToReshuffle:
                if(Random.Range(0,2) == 0)
                {
                    deckCards.AddRange(drawnCards);
                    drawnCards.Clear();
                    deckCards.Shuffle();

                    // Display the deck
                    deckImage.sprite = cardBackSprite;
                    deckCardCountText.text = deckCards.Count.ToString();
                    drawnCardImage.sprite = null;
                    drawnCardDescText.text = string.Empty;
                }
                break;
            case DrawingAction.SetColorBlue:
                color = Color.blue;
                break;
            case DrawingAction.SetColorCyan:
                color = Color.cyan;
                break;
            case DrawingAction.SetColorGreen:
                color = Color.green;
                break;
            case DrawingAction.SetColorYellow:
                color = Color.yellow;
                break;
            case DrawingAction.SetColorOrange:
                color = Color.Lerp(Color.yellow, Color.red, .5f);
                break;
            case DrawingAction.SetColorRed:
                color = Color.red;
                break;
            case DrawingAction.SetColorPurple:
                color = Color.Lerp(Color.red, Color.blue, .5f);
                break;
        }
    }

    public void ResetCards()
    {
        ClearDrawing();

        foreach(Card c in drawnCards)
        {
            deckCards.Add(c);
        }
        deckCards.Shuffle();

        // Display the deck
        deckImage.sprite = cardBackSprite;
        deckCardCountText.text = deckCards.Count.ToString();
        drawnCardImage.sprite = null;
        drawnCardDescText.text = string.Empty;

        // Draw initial point
        LiftTo(SpaceExtent(MinMax.Center, MinMax.Center, MinMax.Center));

        EventSystem.current.SetSelectedGameObject(null);
    }

    // Clears the drawing
    public void ClearDrawing()
    {
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        paths.Clear();
        dots.Clear();
    }

    // Lifts the "pen" and places a dot in a new position
    private void LiftTo(Vector3 pos)
    {
        currPos = pos;
        Renderer dot = Instantiate(dotPrefab, currPos, Quaternion.identity, transform).GetComponentInChildren<Renderer>();
        dot.material.color = color;

        paths.Add(dot.transform.parent);

        DotCheck(dot.transform.parent);
    }

    // Draws a line to a new position
    private void LineTo(Vector3 pos)
    {
        Vector3 oldPos = currPos;
        currPos = pos;
        Vector3 fromToVec = currPos - oldPos;
        float dist = fromToVec.magnitude;
        Transform line = Instantiate(linePrefab, (oldPos + currPos) * .5f, Quaternion.LookRotation(fromToVec) * Quaternion.Euler(90, 0, 0), transform).transform;
        line.localScale = new Vector3(1, dist, 1);
        line.GetComponentInChildren<Renderer>().material.color = color;
        Renderer dot = Instantiate(dotPrefab, currPos, Quaternion.identity, transform).GetComponentInChildren<Renderer>();
        dot.material.color = color;

        line.parent = paths[^1];
        dot.transform.parent.parent = paths[^1];
        paths.Add(dot.transform.parent);

        DotCheck(dot.transform.parent);
    }

    private void DotCheck(Transform dot)
    {
        foreach (Transform d in dots)
        {
            if (Vector3.Distance(d.position, dot.transform.position) < .001f)
            {
                float scale = d.GetChild(0).localScale.x - .01f;
                d.GetChild(0).localScale = new Vector3(scale, scale, scale);
            }
        }

        dots.Add(dot.transform);
    }

    // Draws a circle around the current position
    private void Circle(float size)
    {
        Renderer circle = Instantiate(circlePrefab, currPos, Quaternion.identity, transform).GetComponentInChildren<Renderer>();
        circle.material.SetFloat("_Radius", size);
        circle.material.color = color;

        circle.transform.parent = paths[^1];
    }

    // Whether or not there are cards in the deck
    private bool AreCardsRemaining() => deckCards.Count > 0;

    // Get an extent of the drawing space based on a min/max value in all 3 dimensions
    private Vector3 SpaceExtent(MinMax x, MinMax y, MinMax z) => new Vector3(
        x == MinMax.Min ? Mathf.Min(spaceMin.x, spaceMax.x) : x == MinMax.Center ? (spaceMin.x + spaceMax.x) * .5f : Mathf.Max(spaceMin.x, spaceMax.x),
        y == MinMax.Min ? Mathf.Min(spaceMin.y, spaceMax.y) : y == MinMax.Center ? (spaceMin.y + spaceMax.y) * .5f : Mathf.Max(spaceMin.y, spaceMax.y),
        z == MinMax.Min ? Mathf.Min(spaceMin.z, spaceMax.z) : z == MinMax.Center ? (spaceMin.z + spaceMax.z) * .5f : Mathf.Max(spaceMin.z, spaceMax.z));

    // Get a random position in the drawing space
    private Vector3 RandomPos() => new Vector3(
        Random.Range(spaceMin.x, spaceMax.x),
        Random.Range(spaceMin.y, spaceMax.y),
        Random.Range(spaceMin.z, spaceMax.z));

    public static Color GetColor(DrawColor col)
    {
        switch(col)
        {
            case DrawColor.Blue:
                return Color.blue;
            case DrawColor.Cyan:
                return Color.cyan;
            case DrawColor.Green:
                return Color.green;
            case DrawColor.Yellow:
                return Color.yellow;
            case DrawColor.Orange:
                return Color.Lerp(Color.red, Color.yellow, .5f);
            case DrawColor.Red:
                return Color.red;
            case DrawColor.Purple:
                return Color.Lerp(Color.red, Color.blue, .5f);
        }
        return Color.black;
    }

    public int IndexOfCard(Card c)
    {
        for(int i = 0; i < defaultDeck.Length; i++)
        {
            if (defaultDeck[i].card == c)
                return i;
        }
        return -1;
    }
}
