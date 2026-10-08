using UnityEngine;
using System.Collections;

public static class EmptySprite
{
    static Sprite instance;

    ///<summary>
    /// Returns the instance of a (1 x 1) white Spprite
    /// </summary>	
    public static Sprite Get()
    {
        if (instance == null)
        {
            instance = Resources.Load<Sprite>("procedural_ui_image_default_sprite");
            if (instance == null)
            {
                Texture2D tex = Texture2D.whiteTexture;
                instance = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
        return instance;
    }

    public static bool IsEmptySprite(Sprite s)
    {
        if (Get() == s)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
