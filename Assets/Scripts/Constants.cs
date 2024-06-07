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
        public static int Attack = 5;
        public static int HeldAttack = 6;
    }

    public static class Layers
    {
        public static int Player = 3;
        public static int PlayerIntangible = 6;
        public static int PlayerHitbox = 7;
        public static int Enemy = 8;
        public static int EnemyIntangible = 9;
        public static int Weapon = 10;
        public static int Hazard = 11;
        public static int FixedCamera = 12;
    }

    public static class AnimationPrams
    {
        public static string StartAttack = "StartAttack";
        public static string StartHeldAttack = "StartHeldAttack";
        public static string StartSpinAttack = "StartSpinAttack";
        public static string AttackSwing = "AttackSwing";
    }
}
