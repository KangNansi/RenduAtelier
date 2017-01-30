using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public struct MapKey
{
    public int x;
    public int y;
    public MapKey(int nx, int ny)
    {
        x = nx;
        y = ny;
    }
}

[System.Serializable]
public class MapDictionary : SerializableDictionary<MapKey, TileMap> { }

[System.Serializable]
public class WorldMap : MonoBehaviour {

    


    public Texture texture;
    private Material material;
    public int tileWidth;
    public int tileHeight;
    [SerializeField]
    int mapSizeX=30;
    [SerializeField]
    int mapSizeY=30;

    [SerializeField]
	MapDictionary worldMap;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector3 getNormal()
    {
        return transform.rotation * Vector3.forward;
    }

    public void SetTile(int x, int y, int tx, int ty){
		MapKey mapKey= new MapKey(Mathf.FloorToInt(x/(float)mapSizeX),Mathf.FloorToInt(y/(float)mapSizeY));
		if (worldMap == null)
			worldMap = new MapDictionary ();
		if(!worldMap.ContainsKey(mapKey)){
			GameObject newmap = new GameObject(); 
			newmap.transform.SetParent(transform,false);
			newmap.transform.localPosition = new Vector3(mapKey.x*mapSizeX, mapKey.y*mapSizeY, 0);
			newmap.hideFlags = HideFlags.HideInHierarchy;
			TileMap tmap = newmap.AddComponent<TileMap> ();
            tmap.tile_width = tileWidth;
            tmap.tile_height = tileHeight;
            tmap.GetComponent<MeshRenderer>().material = material;
            tmap.setTexture(texture);
            tmap.size_x = mapSizeX;
            tmap.size_y = mapSizeY;
			tmap.CreateMap ();
            
			worldMap.Add(mapKey,tmap);
		}
        
		worldMap [mapKey].setTile ((x%mapSizeX+mapSizeX)%mapSizeX, (y % mapSizeY + mapSizeY) % mapSizeY, tx, ty);
	}

    public void modifyHeight(Vector2 center, float range, float value)
    {
        int rangeX = (int)range / mapSizeX+1;
        int rangeY = (int)range / mapSizeY+1;
        
        for(int i=-rangeX;i<=rangeX;i++)
            for(int j = -rangeY; j <= rangeY; j++)
            {
                MapKey mapKey = new MapKey(Mathf.FloorToInt((int)center.x / (float)mapSizeX + i),
                                            Mathf.FloorToInt((int)center.y / (float)mapSizeY + j));
                if (worldMap == null)
                    worldMap = new MapDictionary();
                if (!worldMap.ContainsKey(mapKey))
                {
                    GameObject newmap = new GameObject();
                    newmap.transform.SetParent(transform, false);
                    newmap.transform.localPosition = new Vector3(mapKey.x * mapSizeX, mapKey.y * mapSizeY, 0);
                    newmap.hideFlags = HideFlags.HideInHierarchy;
                    TileMap tmap = newmap.AddComponent<TileMap>();
                    tmap.tile_width = tileWidth;
                    tmap.tile_height = tileHeight;
                    tmap.GetComponent<MeshRenderer>().material = material;
                    tmap.setTexture(texture);
                    tmap.size_x = mapSizeX;
                    tmap.size_y = mapSizeY;
                    tmap.CreateMap();

                    worldMap.Add(mapKey, tmap);
                }
                Vector2 mcenter = center - (Vector2)worldMap[mapKey].transform.localPosition;
                worldMap[mapKey].modifyHeight(mcenter, range, value);
            }
    }

    public void setMaterial(Material mat)
    {
        material = mat;
        foreach(KeyValuePair<MapKey, TileMap> entry in worldMap)
        {
            entry.Value.GetComponent<MeshRenderer>().material = mat;
        }
    }

}
