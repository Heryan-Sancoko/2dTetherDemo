using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : Singleton<BulletManager>
{
    public List<BulletScript> bulletList = new List<BulletScript>();
}
