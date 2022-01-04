using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;

public class GMLParser : MonoBehaviour
{
    static XNamespace xsGml;
    static XNamespace xBldg;
    static XNamespace xsApp;
    static XNamespace xsCore;
    const int scaleConst = 1000;
    static List<Batiments> batimentsListe;
    [SerializeField] string filePath;
    void Start()
    {
        batimentsListe = new List<Batiments>();
        LoadData();
        Picking1Sufrace();
    }

    private void Picking1Sufrace()
    {
        Batiments testBuilding = batimentsListe[156];
        Membre surface1 = testBuilding.GetSurface(0);
        surface1.EarClipping();
    }

    public void LoadData()
    {
        //filePath = "C:\\Users\\celeouet\\Cours\\S1\\BRON_2018\\BRON_BATI_2018.gml";
        Console.WriteLine("Fichier cible");
        Console.WriteLine(filePath);
        XDocument doc = new XDocument();

        try
        {
            doc = XDocument.Load(filePath);
        }
        catch (Exception e) { Console.WriteLine("Erreur lors du chargement du fichier. Erreur : " + e); return; }


        XElement indoorFeatures = doc.Root;

        //Tentative d'extraction des attributs
        xsGml = indoorFeatures.GetNamespaceOfPrefix("gml");
        xBldg = indoorFeatures.GetNamespaceOfPrefix("bldg");
        xsApp = indoorFeatures.GetNamespaceOfPrefix("app");
        xsCore = indoorFeatures.GetNamespaceOfPrefix("core");

        Dictionary<string, XNamespace> namespaces = new Dictionary<string, XNamespace>();
        namespaces.Add("gml", xsGml);
        namespaces.Add("xBldg", xBldg);
        namespaces.Add("xsApp", xsApp);
        namespaces.Add("xsCore", xsCore);

        XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
        nsmgr.AddNamespace("gml", "http://www.opengis.net/gml");
        nsmgr.AddNamespace("bldg", "http://www.opengis.net/citygml/building/2.0");
        nsmgr.AddNamespace("app", "http://www.opengis.net/citygml/appearance/2.0");
        nsmgr.AddNamespace("core", "http://www.opengis.net/citygml/2.0");

        IEnumerable<XElement> batiments = doc.XPathSelectElements("//bldg:Building", nsmgr);
        Console.WriteLine("Il y a " + batiments.Count() + " batiments");
        //Affichages des identifiants

        Stopwatch stopwatch = new Stopwatch();

        //Get the max numbers of cores for the current maachine
        int nbProcessors = Environment.ProcessorCount;
        stopwatch.Start();


        //Parsing all the buildings and adding the surface to them
        Parallel.ForEach(batiments, new ParallelOptions { MaxDegreeOfParallelism = nbProcessors }, elem =>
        {
            string id = elem.Attribute(xsGml + "id").Value;
            string type;
            Batiments tmp = new Batiments(id);
            //Eventuellement créer une fonction pour y ranger les operations suivantes
            IEnumerable<XElement> roofs = elem.Descendants(xBldg + "RoofSurface");
            if (roofs.Any())
            {

                type = "RoofSurface";
                ProcessMember(roofs, id, type, tmp);
                //La surface est une "Roofsurface"
                UnityEngine.Debug.Log("===============================================RoofSurface===============================================");
            }
            else
            {
                UnityEngine.Debug.Log("_____No roof surfaces_____");
            }
            IEnumerable<XElement> walls = elem.Descendants(xBldg + "WallSurface");
            if (walls.Any())
            {
                type = "WallSurface";
                ProcessMember(walls, id, type, tmp);
                UnityEngine.Debug.Log("===============================================WallSurface===============================================");
            }
            else
            {
                UnityEngine.Debug.Log("_____No wall surfaces_____");
            }

            lock (batiments)
            {
                batimentsListe.Add(tmp);
            }
        });


        stopwatch.Stop();
        UnityEngine.Debug.Log("_____DONE_____(duree : " + stopwatch.Elapsed.TotalMinutes + ")");

       //Console.ReadKey();
    }
    static void AddMemberPositions(XElement elem, Membre surfMember, int surfaceType)
    {
        if (elem != null)
        {
            //Setup de la partie ext de la surface
            string extId = elem.Attribute(xsGml + "id").Value;
            List<string> positionsString = elem.Element(xsGml + "posList").Value.Split(' ').ToList();

            List<Vector> positions = new List<Vector>();
            //Setting up the Vector by converting the positions into floats
            for (int i = 0; i < positionsString.Count; i += 3)
            {
                Vector tmp = Vector.zero;
                tmp.x = (float)Convert.ToDouble(positionsString[i], CultureInfo.InvariantCulture) / scaleConst;
                tmp.y = (float)Convert.ToDouble(positionsString[i + 1], CultureInfo.InvariantCulture) / scaleConst;
                tmp.z = (float)Convert.ToDouble(positionsString[i + 2], CultureInfo.InvariantCulture) / scaleConst;
                positions.Add(tmp);
            }
            //Adding the poslist to the surfaceMember
            switch (surfaceType)
            {
                case 1:
                    surfMember.SetExt(extId, positions);
                    break;
                case 2:
                    surfMember.SetInt(extId, positions);
                    break;
                default:
                    //Console.WriteLine("AddMemberPosition : Invalid surface type.");
                    return;
            }

        }
    }
    static void ProcessMember(IEnumerable<XElement> surfaces, string id, string type, Batiments tmp)
    {
        foreach (var surface in surfaces)
        {
            //Ajout de la surface a la liste du batiment
            id = surface.Attribute(xsGml + "id").Value;
            //Recuperation de l'identifiant de la surface
            Membre surfMember = new Membre(id, type);
            //Recuperation de la LinearRing pour l'identifiant de l'lext et des positions
            XElement ext = surface.Descendants(xsGml + "exterior").Descendants(xsGml + "LinearRing").FirstOrDefault();
            if (ext != null)
            {
                AddMemberPositions(ext, surfMember, 1);
            }

            XElement inte = surface.Descendants(xsGml + "interior").Descendants(xsGml + "LinearRing").FirstOrDefault();
            if (inte != null)
            {
                AddMemberPositions(inte, surfMember, 2);
            }

            tmp.AddSurface(surfMember);
        }
    }
}
