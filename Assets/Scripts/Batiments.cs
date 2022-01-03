using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Batiments
{
    private List<Membre> surfaces { get; set; }
    public string Id { get; private set; }

    public int NbSurfaces { get; private set; }

    public Batiments(string identifiant)
    {
        Id = identifiant;
        surfaces = new List<Membre>();
        NbSurfaces = 0;
    }

    public void AddSurface(Membre m)
    {
        surfaces.Add(m);
        NbSurfaces++;
    }

    public void BuildMesh()
    {
        //TO DO
        //throw new NotImplementedException();
    }

    public Membre GetSurface(int id)
    {
        return surfaces[id];
    }
}
