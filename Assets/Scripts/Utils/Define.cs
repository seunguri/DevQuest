﻿using UnityEngine;
using System.Collections;

public class Define
{

    public enum State
    {
        None,
        Die,
        Idle,
        Moving,
        Attack,
    }

    public enum WorldObject
    {
        Unknown,
        Player,
        Enemy,
    }

    public enum Scene
    {
        Unknown,
        Game,
        End,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
    }

    public enum Layer
    {
        Ground = 3,
        Player = 6,
        Enemy = 7,
    }
}
