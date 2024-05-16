using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{

    public static class ActionMapActions
    {
        public static int Move = 0;
        public static int Jump = 1;
        public static int Tether = 2;
        public static int Aim = 3;
        public static int Dash = 4;
    }

    public static class Layers
    {
        public static int Player = 3;
        public static int PlayerIntangible = 6;
        public static int PlayerHitbox = 7;
        public static int Enemy = 8;
        public static int EnemyIntangible = 9;
    }
}