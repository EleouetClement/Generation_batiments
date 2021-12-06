using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batiment : ScriptableObject
{
    private Membre[] Surfaces;
    private Vector3 [] Vertices;
    private int[] Triangles;
    private Vector2 [] TexCoord;
    public Mesh Msh { get; private set; }


    public Batiment()
    {
        
    }

    //Genere le mesh à partir des vertices, des triangles et des texCoord
    public void GetMesh()
    {
        //TO DO
    }


    //Triangule les coordonnées en utilisant le ear cutting
    public void Triangulate()
    {
        //TO DO
    }









}
