using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Membre
{
    public string Id { get; private set; }
    public string Type { get; private set; }
    public string IntTexId { get; private set; }
    public string ExtTexId { get; private set; }
    public List<Vector3> positionsExt { get; private set; }
    public List<Vector3> positionsInt { get; private set; }
    public Vector3[] textures { get; private set; }

    public Membre(string identifiant, string type)
    {
        Id = identifiant;
        Type = type;
        IntTexId = "";
        ExtTexId = "";
        positionsExt = new List<Vector3>();
        positionsInt = new List<Vector3>();
    }

    /// <summary>
    /// Internal buffer for ear clipping multicall optimisation. Do not touch.
    /// </summary>
    private int[] clipBuffer;
    /// <summary>
    /// Computes a list of triangular surfaces for a polygon. This method is buffered.
    /// </summary>
    /// <returns>An array of vertice index containing triangle data for this polygon. Multiple calls to the function will return a buffered result, not to be modified.</returns>
    public int[] EarClipping()
    {
        if (clipBuffer != null) return clipBuffer;
        Vector2[] workbuffer = new Vector2[positionsExt.Count];
        bool vertical = false; // FIXME : compute plane verticality
        for (int i = 0; i < workbuffer.Length; i++)
            workbuffer[i] = new Vector2((float)positionsExt[i].x, vertical ? (float)positionsExt[i].y : (float)positionsExt[i].z);
        int[] answer;
        string error;

        if (!TriangulatePolygon.Triangulate(workbuffer, out answer, out error))
            Debug.Log(error);

        clipBuffer = answer;
        return answer;
    }

    //Set the poslist of an exterior surface
    public void SetExt(string id, List<Vector3> positions)
    {
        //Adding the ExtId
        ExtTexId = id;
        //Adding the positions
        positionsExt = positions;


    }

    //Set the poslist of an interior surface
    public void SetInt(string id, List<Vector3> positions)
    {
        //Adding the ExtId
        IntTexId = id;
        //Adding the positions
        positionsInt = positions;
    }

    public override string ToString()
    {
        string toreturn = "Member " + Id + " : {";
        for (int i = 0; i < positionsExt.Count; i++)
            toreturn += (i==0?"":",")+positionsExt[i];
        return toreturn + "}";
    }
}
