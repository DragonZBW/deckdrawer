using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        Card card = target as Card;
        //return new Texture2D(width, height);

        if(card.cardArt == null)
            return base.RenderStaticPreview(assetPath, subAssets, width, height);

        Texture2D cache = new Texture2D(width, height);
        EditorUtility.CopySerialized(card.cardArt.texture, cache);
        return cache;
    }
}
