using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Voxel {

    public byte TypeID { get; set; }

    public Voxel(byte type)
    {
        TypeID = type;
    }

}
