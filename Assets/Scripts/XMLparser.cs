using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

//690290B1027

public class XMLparser : MonoBehaviour
{
    [SerializeField] private string filePath;
    // Start is called before the first frame update
    void Start()
    {
        LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Load Data from a GML document
    public void LoadData()
    {
        filePath = "C:\\Users\\celeouet\\Cours\\S1\\BRON_2018\\BRON_BATI_2018.gml";
        Debug.Log("Fichier cible");
        Debug.Log(filePath);

        XDocument source;

        try
        {
            //Chargement du document
            source = XDocument.Load(filePath);
        }
        catch (Exception e) { Debug.LogError("Erreur lors du chargement du fichier le chemin peut être érroné. Erreur : " + e); return; }


        //Recuperation de la racine
        XElement contenu = source.Root;

        //Renseignement des namespaces
        XNamespace XsGml = contenu.GetNamespaceOfPrefix("gml");
        XNamespace XBldg = contenu.GetNamespaceOfPrefix("bldg");
        XNamespace Xapp = contenu.GetNamespaceOfPrefix("app");
        XNamespace Xcore = contenu.GetNamespaceOfPrefix("core");

        var results = contenu.Elements(Xcore + "cityObjectMember").Select(
        x => new
        {
            boundaries = x.Descendants(XBldg + "Building").Select(
                y => new
                {
                    date = y.Descendants(Xcore + "creationDate")
                })
        }).FirstOrDefault();

        if (results == null)
        {
            Debug.Log("results null la requête à échouée");
            return;
        }

        foreach (var elem in results.boundaries)
        {
            List<XElement> dates = elem.date.ToList();
            Debug.Log("Il y a " + dates.Count + " element(s)");
            Debug.Log(dates[0].Value);
        }

        //Tentative d'extraction des attributs


        IEnumerable<XElement> building = from elem in source.Descendants(XBldg + "Building")
                                         where (string)elem.Attribute(XsGml + "id") == "690290B1027"
                                         select elem;


    }
}

