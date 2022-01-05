# Projet de mondes virtuels
# Unity project name : Generation_batiments
## Project Members
>- DERBEL Zeineb
>- CANDELA Nicolas
>- BELGACEM Fatma
>- FOMOSA Mathieu
>- ELEOUET Clément

## Description 

### Structure of the gml file 

In the first part of the file we find the boundries of the space positions. 
The second part conatins the Texture Data (Uvs, and name of the texture file).
Then there is the buildings informations. Each building is a **<cityObjectMember>**
between those tags, there is metadata about the building and all the surfaces of the 
building. Each Surface is between **<boundedBy>** tags. There is 3 types of surfaces : 
- RoofSurface
- WallSurface
- GroundSurface
Surfaces are represented by polygons and the vertices positions are given by the poslist tags.
The positions are 3d coordinates (x, z, y).

### Implementation

This project consisted of Generating buildings into Unity using an Open data gml file.
First we needed to parse correctly all the data. We used the C# Xml library and the **XDocuments**.
We First search all the *buildings* then, using multithreading we load each building with all of his
surfaces. We strutured the project with **Batiments** and **Membre** class. A building has an id and
a list of **Membre**. 
Each **Membre** contains his poslist and the texture coordinates attached to it. 
The positions are very long floats numbers so in order to avoid overflow during the parsing, we substract x and z by the minimum values given 
by the boundries.
Once the parsing is Over, we triangulate the polygons using the earclipping algorithm with all the surfaces to obtain the list of triangles and generate the meshs. To
complete the algogithm we need to translate the plan into a 2d plan using an orthogonal projection. Then we put back the surface in its position.

## Ressources

- [gml zip archive](https://download.data.grandlyon.com/files/grandlyon/imagerie/2018/maquette/BRON_2018.zip)
- [Ear clipping triangulation tutorial](https://www.youtube.com/watch?v=QAdfkylpYwc)

