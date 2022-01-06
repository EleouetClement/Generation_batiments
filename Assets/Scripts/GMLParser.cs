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
using System.IO;

public class GMLParser : MonoBehaviour
{
    static XNamespace xsGml;
    static XNamespace xBldg;
    static XNamespace xsApp;
    static XNamespace xsCore;
    public const int scaleConst = 1000;
    static List<Batiments> batimentsListe;
    [SerializeField] string filePath;
    private Dictionary<string, List<Vector2>> texturesDic;
    UnityEngine.Vector3[] gizmos;
    public Material mat;
    public List<GameObject> objectsToCombine;
    void Start()
    {
        objectsToCombine = new List<GameObject>();
        batimentsListe = new List<Batiments>();
        texturesDic = new Dictionary<string, List<Vector2>>();
        LoadData();
        DisplayOneBuilding();
        Combine();
    }
    private int GetTextureSize(GameObject[] o)
    {
        List<Texture> textures = new List<Texture>();
        // Find unique textures
        for (int i = 0; i < o.Length; i++)
        {
            if (o[i].TryGetComponent(out MeshRenderer renderer) && !textures.Contains(renderer.material.mainTexture))
            {
                textures.Add(renderer.material.mainTexture);
            }
        }
        if (textures.Count == 1) return 1;
        if (textures.Count < 5) return 2;
        if (textures.Count < 17) return 4;
        if (textures.Count < 65) return 8;
        if (textures.Count < 129) return 16;
        if (textures.Count < 257) return 32;
        if (textures.Count < 513) return 64;
        if (textures.Count < 1025) return 128;
        if (textures.Count < 2049) return 256;
        if (textures.Count < 4097) return 512;
        // Doesn't handle more than 64 different textures but I think you can see how to extend
        return 0;
    }

    private void Combine()
    {

        int size;
        int originalSize;
        int pow2;
        Texture2D combinedTexture;
        Material material;
        Texture2D texture;
        Mesh mesh;
        System.Collections.Hashtable textureAtlas = new System.Collections.Hashtable();

        if (objectsToCombine.Count > 1 && objectsToCombine[0].TryGetComponent(out MeshRenderer renderer))
        {
            originalSize = renderer.material.mainTexture.width;
            pow2 = GetTextureSize(objectsToCombine.ToArray());
            size = pow2 * originalSize;
            combinedTexture = new Texture2D(size, size, TextureFormat.RGB24, true);

            // Create the combined texture (remember to ensure the total size of the texture isn't
            // larger than the platform supports)
            for (int i = 0; i < objectsToCombine.Count; i++)
            {
                if (objectsToCombine[i].TryGetComponent(out MeshRenderer rdr))
                {
                    texture = (Texture2D)rdr.material.mainTexture;
                    if (!textureAtlas.ContainsKey(texture))
                    {
                        combinedTexture.SetPixels((i % pow2) * originalSize, (i / pow2) * originalSize, originalSize, originalSize, texture.GetPixels());
                        textureAtlas.Add(texture, new Vector2(i % pow2, i / pow2));
                    }
                }
            }
            combinedTexture.Apply();
            material = new Material(renderer.material);
            material.mainTexture = combinedTexture;

            // Update texture co-ords for each mesh (this will only work for meshes with coords betwen 0 and 1).
            for (int i = 0; i < objectsToCombine.Count; i++)
            {
                if (objectsToCombine[i].TryGetComponent(out MeshFilter filter) && objectsToCombine[i].TryGetComponent(out MeshRenderer rdr))
                {
                    mesh = filter.mesh;
                    Vector2[] uv = new Vector2[mesh.uv.Length];
                    Vector2 offset;
                    if (textureAtlas.ContainsKey(rdr.material.mainTexture))
                    {
                        offset = (Vector2)textureAtlas[rdr.material.mainTexture];
                        for (int u = 0; u < mesh.uv.Length; u++)
                        {
                            uv[u] = mesh.uv[u] / (float)pow2;
                            uv[u].x += ((float)offset.x) / (float)pow2;
                            uv[u].y += ((float)offset.y) / (float)pow2;
                        }
                    }
                    else
                    {
                        // This happens if you use the same object more than once, don't do it :)
                    }

                    mesh.uv = uv;
                    rdr.material = material;
                }
            }

            // Combine each mesh marked as static
            int staticCount = 0;
            CombineInstance[] combine = new CombineInstance[objectsToCombine.Count];
            for (int i = 0; i < objectsToCombine.Count; i++)
            {
                if (objectsToCombine[i].isStatic)
                {
                    staticCount++;
                    combine[i].mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                    combine[i].transform = objectsToCombine[i].transform.localToWorldMatrix;
                }
            }

            // Create a mesh filter and renderer
            if (staticCount > 1)
            {
                MeshFilter nFilter = gameObject.AddComponent<MeshFilter>();
                MeshRenderer nRenderer = gameObject.AddComponent<MeshRenderer>();
                nFilter.mesh = new Mesh();
                nFilter.mesh.CombineMeshes(combine);
                renderer.material = material;

                // Disable all the static object renderers
                for (int i = 0; i < objectsToCombine.Count; i++)
                {
                    if (objectsToCombine[i].isStatic && objectsToCombine[i].TryGetComponent(out MeshFilter filter) && objectsToCombine[i].TryGetComponent(out MeshRenderer rdr))
                    {
                        filter.mesh = null;
                        rdr.material = null;
                        rdr.enabled = false;
                    }
                }
            }

            Resources.UnloadUnusedAssets();
        }
    }




    public void DisplayOneBuilding()
    {
        int nbProcessors = Environment.ProcessorCount;
        //List<MeshFilter> meshs = new List<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>();
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        int i = 0;
        foreach(var bat in batimentsListe.Take(100))
        {
            Texture2D tex = Resources.Load(bat.Id) as Texture2D;


            List<Membre> surfaces = bat.surfaces;
            foreach (var surface in surfaces)
            {
                var go = new GameObject();
                go.AddComponent<MeshRenderer>();
                go.AddComponent<MeshFilter>();
                Mesh msh = new Mesh();
                msh.vertices = surface.positionsExt.ToArray();
                msh.triangles = surface.EarClipping();
                msh.uv = surface.textures.ToArray();
                msh.RecalculateNormals();
                //combine.Add(new CombineInstance
                //{
                //    mesh = msh,
                //    transform = Matrix4x4.identity,
                //    subMeshIndex = i
                //}) ;
                //i++;
                go.GetComponent<MeshFilter>().mesh = msh;
                go.GetComponent<MeshRenderer>().material = mat;
                go.GetComponent<MeshRenderer>().material.mainTexture = tex;
                objectsToCombine.Add(Instantiate(go));
                //gizmos = new Vector3[surface.positionsExt.Count];

                //surface.positionsExt.ToArray().CopyTo(gizmos, 0);
            }  
        }
        mf.mesh = new Mesh();
        mf.mesh.subMeshCount = i;
        mf.mesh.CombineMeshes(combine.ToArray());
    }
    //get vertices from a surface of a "batiment"
    private Vector3[] GetRandomShape(int buildingID, int surfaceID)
    {
        Membre surface = batimentsListe[buildingID].GetSurface(surfaceID);
        int nbPositions = surface.positionsExt.Count;
        Vector3[] vertices = new Vector3[3];
        vertices[0] = new Vector3((float)surface.positionsExt[0].x, (float)surface.positionsExt[0].y, (float)surface.positionsExt[0].z);
        vertices[1] = new Vector3((float)surface.positionsExt[nbPositions / 2].x, (float)surface.positionsExt[nbPositions / 2].y, (float)surface.positionsExt[nbPositions / 2].z);
        vertices[2] = new Vector3((float)surface.positionsExt[nbPositions - 1].x, (float)surface.positionsExt[nbPositions - 1].y, (float)surface.positionsExt[nbPositions - 1].z);
        return vertices;
    }

    //Display a building using vertices and triangles array 
    private void DisplayBuilding(UnityEngine.Vector3[] vertices, int[] triangles)
    {
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>();
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = triangles;
        msh.RecalculateNormals();
        gameObject.GetComponent<MeshFilter>().mesh = msh;
    }

    private void Picking1Sufrace()
    {
        Batiments testBuilding = batimentsListe[156];
        Membre surface1 = testBuilding.GetSurface(0);
        surface1.EarClipping();
    }

    /// <summary>
    /// DEPRECATED
    /// This function takes a string and returns a float number
    /// it cuts the number before and after the dot in order to avoid overflow
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    static double ParseLongFloat(string number)
    {
        string[] numbers = number.Split('.');
        if (numbers.Length != 2)
        {
            return -1.0;
        }
        string left = "";
        string right = "";

        if (numbers[0].Length < 3)
        {
            left = numbers[0];
        }
        else
        {
            left = numbers[0][0].ToString() + numbers[0][1].ToString() + numbers[0][2].ToString();
        }
        if (numbers[1].Length < 3)
        {
            right = numbers[1];
        }
        else
        {
            right = numbers[1][0].ToString() + numbers[1][1].ToString() + numbers[1][2].ToString();
        }
        left = left + "." + right;
        double value = Convert.ToDouble(left, CultureInfo.InvariantCulture);
        return value;
    }

    /// <summary>
    /// This function open the file at the path given by 
    /// the filepath. It search for all the buildings
    /// and load all of their surface in objects
    /// </summary>
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

        int nbProcessors = Environment.ProcessorCount;

        IEnumerable<XElement> batiments = doc.XPathSelectElements("//bldg:Building", nsmgr);
        IEnumerable<XElement> textures = doc.XPathSelectElements("//app:textureCoordinates", nsmgr);

        //Get textures
        Parallel.ForEach(textures, new ParallelOptions { MaxDegreeOfParallelism = nbProcessors }, elem => {
            string id = elem.Attribute("ring").Value.Remove(0, 1);
            lock (texturesDic)
            {
                if (!texturesDic.ContainsKey(id))
                {
                    texturesDic.Add(id, ProcessMemberTexture(elem.Value));
                }
            }
        });
        Console.WriteLine("Il y a " + batiments.Count() + " batiments");
        //Affichages des identifiants

        Stopwatch stopwatch = new Stopwatch();

        //Get the max numbers of cores for the current maachine
        stopwatch.Start();


        //Parsing all the buildings and adding the surface to them
        Parallel.ForEach(batiments, new ParallelOptions { MaxDegreeOfParallelism = nbProcessors }, elem =>
        {
            string id = elem.Attribute(xsGml + "id").Value;
            string type;
            Batiments tmp = new Batiments(id);
            IEnumerable<XElement> roofs = elem.Descendants(xBldg + "RoofSurface");
            if (roofs.Any())
            {

                type = "RoofSurface";
                ProcessMember(roofs, id, type, tmp);
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

    }

    /// <summary>
    /// Takes a string containing all the textures coordinates and parse them into Vector2
    /// 
    /// </summary>
    /// <param name="textureContent"></param>
    ///
    /// <returns type="Vector2"></returns>
    private List<Vector2> ProcessMemberTexture(string textureContent)
    {
        List<string> texturesString = textureContent.Split(' ').ToList();

        List<Vector2> positions = new List<Vector2>();
        //Setting up the Vector by converting the positions into floats
        for (int i = 0; i < texturesString.Count-2; i += 2)
        {
            Vector2 tmp = Vector2.zero;
            tmp.x = (float)Convert.ToDouble(texturesString[i], CultureInfo.InvariantCulture);
            tmp.y = (float)Convert.ToDouble(texturesString[i + 1], CultureInfo.InvariantCulture);
            positions.Add(tmp);
        }
        return positions;
    }
    /// <summary>
    /// take a surface and an XElement containing the positions of the surface
    /// to load it, parse it into float then it save into the surface positions list
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="surfMember"></param>
    /// <param name="surfaceType"></param>
    static void AddMemberPositions(XElement elem, Membre surfMember, int surfaceType)
    {
        if (elem != null)
        {
            //Setup de la partie ext de la surface
            string extId = elem.Attribute(xsGml + "id").Value;
            List<string> positionsString = elem.Element(xsGml + "posList").Value.Split(' ').ToList();

            List<Vector3> positions = new List<Vector3>(positionsString.Count-3);
            //Setting up the Vector by converting the positions into floats
            for (int i = 0; i < positionsString.Count-3; i += 3)
            {
                Vector3 tmp = Vector3.zero;
                tmp.x = (float)((Convert.ToDouble(positionsString[i], CultureInfo.InvariantCulture) - 1848779d) / scaleConst);
                tmp.y = (float)(Convert.ToDouble(positionsString[i + 2], CultureInfo.InvariantCulture) / scaleConst);
                tmp.z = (float)((Convert.ToDouble(positionsString[i + 1], CultureInfo.InvariantCulture) - 5170460d) / scaleConst);
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

    /// <summary>
    /// taked a group of surfaces attached to a building to parse them and load the positions and textures coordinates
    /// </summary>
    /// <param name="surfaces">liste of "Membre"</param>
    /// <param name="id">Building's ID</param>
    /// <param name="type">Roof or wall surfaces</param>
    /// <param name="tmp">target building</param>
    void ProcessMember(IEnumerable<XElement> surfaces, string id, string type, Batiments tmp)
    {
        foreach (var surface in surfaces)
        {
            
            id = surface.Attribute(xsGml + "id").Value;
            //getting surface id
            Membre surfMember = new Membre(id, type);
            //Getting "linearRing" id for the positions and the textures coordinates later
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
            //Add texture coordinates
            var textureId = ext != null ? ext.Attribute(xsGml + "id").Value : inte.Attribute(xsGml + "id").Value;
            if (texturesDic.TryGetValue(textureId, out List<Vector2> texturesPositions))
                surfMember.textures = texturesPositions;
            else
                UnityEngine.Debug.Log("Unable to find texture for surface : " + id);

            //Adding the surface into the building's surfaces list
            tmp.AddSurface(surfMember);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (gizmos == null)
    //    {
    //        return;
    //    }
    //    Gizmos.color = Color.red;
    //    for (int i = 0; i < gizmos.Length; i++)
    //    {
    //        Gizmos.DrawSphere(gizmos[i], 0.1f);
    //    }
    //}
}