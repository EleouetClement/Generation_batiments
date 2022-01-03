using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Membre
{
    public string Id { get; private set; }
    public string Type { get; private set; }
    public string IntTexId { get; private set; }
    public string ExtTexId { get; private set; }
    public List<Vector> positionsExt { get; private set; }
    public List<Vector> positionsInt { get; private set; }
    public Vector[] textures { get; private set; }

    public Membre(string identifiant, string type)
    {
        Id = identifiant;
        Type = type;
        IntTexId = "";
        ExtTexId = "";
        positionsExt = new List<Vector>();
        positionsInt = new List<Vector>();
    }

    public void EarClipping()
    {

    }

    //Set the poslist of an exterior surface
    public void SetExt(string id, List<Vector> positions)
    {
        //Adding the ExtId
        ExtTexId = id;
        //Adding the positions
        positionsExt = positions;


    }

    //Set the poslist of an interior surface
    public void SetInt(string id, List<Vector> positions)
    {
        //Adding the ExtId
        IntTexId = id;
        //Adding the positions
        positionsInt = positions;
    }
}
