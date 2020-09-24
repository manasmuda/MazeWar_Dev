using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SerializableColor
{
    public float _r;
    public float _g;
    public float _b;
    public float _a;

    public Color Color
    {
        get
        {
            return new Color(_r, _g, _b, _a);
        }
        set
        {
            _r = value.r;
            _g = value.g;
            _b = value.b;
            _a = value.a;
        }
    }

    public SerializableColor(Color color)
    {
        _r = color.r;
        _g = color.g;
        _b = color.b;
        _a = color.a;
    }

    public static SerializableColor[] serializeColorArr(Color[] colors)
    {
        //
        SerializableColor[] colors1 = new SerializableColor[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors1[i] = new SerializableColor(colors[i]);
        }
        return colors1;
    }

    public static Color[] deserializeColorArr(SerializableColor[] colors)
    {
        //
        Color[] colors1 = new Color[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors1[i] = new Color(colors[i]._r,colors[i]._g,colors[i]._b,colors[i]._a);
        }
        return colors1;
    }
}
