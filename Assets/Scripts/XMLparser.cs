using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

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
        XDocument doc = XDocument.Load(filePath);
        XElement indoorFeatures = doc.Root;

        XNamespace XsGml = indoorFeatures.GetNamespaceOfPrefix("gml");
        XNamespace XBldg = indoorFeatures.GetNamespaceOfPrefix("bldg");
        XNamespace Xapp = indoorFeatures.GetNamespaceOfPrefix("app");
        XNamespace Xcore = indoorFeatures.GetNamespaceOfPrefix("core");

        var results = doc.Elements(Xcore + "cityObjectMember").Select(
            x=> new
            {
                boundaries = x.Descendants(XBldg + "Building").Select(
                    y=> new
                    {
                        date = y.Descendants(Xcore + "creationDate")
                    })
            }).FirstOrDefault();

        foreach(var item in results.boundaries)
        {
            Debug.Log(item.date);
        }
    }



}
