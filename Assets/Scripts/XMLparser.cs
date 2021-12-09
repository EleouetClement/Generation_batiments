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

<<<<<<< Updated upstream
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
=======
        XNamespace xsGml = indoorFeatures.GetNamespaceOfPrefix("gml");
        XNamespace xBldg = indoorFeatures.GetNamespaceOfPrefix("bldg");
        XNamespace xsApp = indoorFeatures.GetNamespaceOfPrefix("app");
        XNamespace xsCore = indoorFeatures.GetNamespaceOfPrefix("core");

        XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
        nsmgr.AddNamespace("gml", "http://www.opengis.net/gml");
        nsmgr.AddNamespace("bldg", "http://www.opengis.net/citygml/building/2.0");
        nsmgr.AddNamespace("app", "http://www.opengis.net/citygml/appearance/2.0");
        nsmgr.AddNamespace("core", "http://www.opengis.net/citygml/2.0");

        string s = doc.XPathSelectElement("//bldg:Building", nsmgr).Value;

        Debug.Log(s);



        //Requete pour recuperation des batiments dans le fichier
        /* var results = doc.Elements(xsCore + "CityModel")
             .Select(x => new
             {
             boundaries = x.Descendants(xsGml + "Envelope")
                 .Select(y => new {
                     lowerbound = (string)y.Element(xsGml + "lowerCorner"),
                     upperbound = (string)y.Element(xsGml + "upperCorner")
                 }),

             buildings = x.Descendants(xBldg+ "Building").Select(y => new
             {

                 positions = x.Descendants(xsGml + "LinearRing")
                 .Select(z => new
                 {
                     coord = (string)z.Element(xsGml + "posList")
                 }).ToList()
             }),





             texture = x.Descendants(xsApp + "ParameterizedTexture")
                             .Descendants(xsApp + "target")
                                 .Descendants(xsApp + "TexCoordList")
                                 .Select(y => new
                                 {
                                     textCoord = y.Elements(xsApp + "textureCoordinates").FirstOrDefault()
                                 }).ToList()
         }).FirstOrDefault();*/

        /*var batiments = doc.Elements(xBldg + "Building").Select(
            bat => new
            {
                boundedby = bat.Descendants(xBldg + "boundedBy").Select(
                    surf => new
                    {
                        polygone = surf.Descendants(xsGml + "Polygon").Select(
                            poly => new
                            {
                                ext = poly.Descendants(xsGml + "exterior").Select(
                                    ex => new
                                    {
                                        linearRing = ex.Elements(xsGml + "posList").ToList()
                                    }).ToList(),
                                inte = poly.Descendants(xsGml + "interior").Select(
                                    ex => new
                                    {
                                        linearRing = ex.Element(xsGml + "posList")
                                    }).ToList(),
                            }).ToList()
                    }).ToList()
            }).FirstOrDefault();*/


        /*var buildingsOnly = doc.Elements(xsCore + "cityObjectMember").Select(
        x => new
        {
            boundaries = x.Descendants(xBldg + "Building").Select(
                y => new
                {
                    date = y.Descendants(xsCore + "creationDate")
                }).ToList()
        }).FirstOrDefault();*/





        /*if (buildingsOnly == null)
        {
            Debug.LogError("Query batiments ko");
            return;
        }*/
        /*if(buildingsOnly.b.Count ==0)
        {
            Debug.LogError("La query a retourne 0 elem");
            return;
        }*/

        /*foreach (var item in buildingsOnly.boundaries)
        {
            Debug.Log(item.date);
        }*/


        /*foreach(var elem in buildingsOnly.b)
        {
            Debug.Log("Batiment : ");
            Debug.Log(elem.Value);
        }*/









    }
>>>>>>> Stashed changes


        IEnumerable<XElement> building = from elem in source.Descendants(XBldg + "Building")
                                         where (string)elem.Attribute(XsGml + "id") == "690290B1027"
                                         select elem;


    }
}

