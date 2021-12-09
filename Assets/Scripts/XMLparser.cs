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

    void TestSomething()
    { 
}
    //Load Data from a GML document
    public void LoadData()
    {
        XDocument doc = XDocument.Load(filePath);
        XElement indoorFeatures = doc.Root;

        XNamespace xsGml = indoorFeatures.GetNamespaceOfPrefix("gml");
        XNamespace xBldg = indoorFeatures.GetNamespaceOfPrefix("bldg");
        XNamespace xsApp = indoorFeatures.GetNamespaceOfPrefix("app");
        XNamespace xsCore = indoorFeatures.GetNamespaceOfPrefix("core");
        
        var results = doc.Elements(xsCore + "CityModel")
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
        }).FirstOrDefault();


        //foreach(var item in results.boundaries)
        //{
        //    Debug.Log(item.date);
        //}
    }



}
