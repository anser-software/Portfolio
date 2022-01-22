using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntersectionPointChoke
{

    public int id;

    public float chokeLatticeLocalY;

    public bool enabled;

}

[System.Serializable]
public struct IntersectionRelease
{

    public int chokePointID;

    public float releaseLatticeLocalY;

    public Tape[] callOnRelease;

}
