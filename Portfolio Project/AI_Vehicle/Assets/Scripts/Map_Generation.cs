using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// // * * * * List Extension Code retrieved from https://pastebin.com/NwvLLu4J * * * *
// public static class ListExtensions  {
//     public static void Shuffle<T>(this IList<T> list) {
//         System.Random rnd = new System.Random();
//         for (var i = 0; i < list.Count; i++)
//             list.Swap(i, rnd.Next(i, list.Count));
//     }
 
//     public static void Swap<T>(this IList<T> list, int i, int j) {
//         var temp = list[i];
//         list[i] = list[j];
//         list[j] = temp;
//     }
// }

public class Map_Generation : MonoBehaviour
{
    /* * * * * * *
    * VARIABLES  *
    * * * * * * */
    public int MapHeight = 3;
    public int MapWidth = 3;
    public int PathSize = 3; // Number of tiles from start (exclusive) to goal (inclusive)

    public uint obstaclesPerTile;
    public CarAgent agent;
    private GameObject[] prefabs; //Array to store prefab objects
    private List<GameObject> TILES = new List<GameObject>();
    private List<GameObject> OBJECTS = new List<GameObject>();
    private bool[,] Board;
    private List<Position> path = new List<Position>();

    [HideInInspector]
    public bool generating;

    public float timescale = 1;

    public class Position {
        public int i;
        public int j;

        public Position(int i = 0, int j = 0) {
            this.i = i;
            this.j = j;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = this.timescale;
        
    }

    void Awake()
    {
        Load_Tiles();
        generating = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MapGen() {
        generating = true;
        //Generate_Map();
        if (!(GeneratePath())) {
            print("Error generating path");
        }
        else {
            print("Found path of length: " + path.Count);
        }

        print("Laying road sections");
        InstantiateTiles();
    }

    public void MapReGen() {

        generating = true;
        // Destroy all children
        print("Destroying current map");

        for (int i = 0; i < this.transform.childCount; i++) {
            Transform.Destroy(this.transform.GetChild(i).gameObject);
        }

        // Reset Path and Board
        path = new List<Position>();

        // Gen New Map
        MapGen();
    }

    void Load_Tiles() {
        print("Loading road prefab resources");
        TILES.Add(Resources.Load<GameObject>("Parking_Lot"));
        TILES.Add(Resources.Load<GameObject>("Straight_Road"));
        TILES.Add(Resources.Load<GameObject>("Curved_Road"));
        OBJECTS.Add(Resources.Load<GameObject>("Branch_01"));
        //OBJECTS.Add(Resources.Load<GameObject>("Bush_02"));
        OBJECTS.Add(Resources.Load<GameObject>("Rock_01"));
        OBJECTS.Add(Resources.Load<GameObject>("Rock_04"));
        OBJECTS.Add(Resources.Load<GameObject>("Stump_01"));
        OBJECTS.Add(Resources.Load<GameObject>("Tree_02"));
    }


    bool GeneratePath() {

        if((this.PathSize <= 0) || this.PathSize > ((this.MapHeight * this.MapWidth) - 2)) {
            print("Invalid pathsize of " + PathSize);
            print("Adjusting pathsize to default value of 3");
            this.PathSize = 3;
        }

        this.Board = new bool[this.MapHeight, this.MapWidth];

        //Ensure board is initialized properly
        for (int i = 0; i < this.MapHeight; i++) {
            for (int j = 0; j < this.MapWidth; j++) {
                this.Board[i,j] = false;
            }
        }

        // Generate starting position
        //Position startingPos = new Position(Random.Range(0, this.MapHeight), Random.Range(0, this.MapWidth));
        Position startingPos = new Position();

        // Add starting pos to path
        this.path.Add(startingPos);

        MarkVisited(startingPos, true);

        // Find path
        return FindPath(this.PathSize, startingPos);
    }

    bool FindPath(int remainingLength, Position pos) {
        List<Position> neighbors;
        int k;
        Position temp;


        //Found path of specified length
        if(remainingLength == 0) {
            return true;
        }

        // Get all viable neighbors
        neighbors = FindValidNeighbors(pos);

        //Shuffle the list
        for(int n = neighbors.Count - 1; n > 0; n--) {
            k = Random.Range(0, n);
            temp = neighbors[n];
            neighbors[n] = neighbors[k];
            neighbors[k] = temp;
        }
        //neighbors.Shuffle();

        for (int i = 0; i < neighbors.Count; i++) {
            MarkVisited(neighbors[i], true);
            this.path.Add(neighbors[i]);

            if(FindPath(remainingLength - 1, neighbors[i])) {
                return true; // Valid path found
            }

            // Not a valid subpath
            MarkVisited(neighbors[i], false);
            this.path.Remove(neighbors[i]);
        }

        // No valid neigbbors
        return false;
    }

    void MarkVisited(Position pos, bool val) {
        // Marks a position in private Board variable as T/F passed on passed val

        if((pos.i >= this.MapHeight) || (pos.j >= this.MapWidth)) {
            print("Warning: Provided pos of (" + pos.i + "," + pos.j + ") is invalid");
            return;
        }

        this.Board[pos.i, pos.j] = val;
    }

    List<Position> FindValidNeighbors(Position pos) {
        List<Position> neighbors = new List<Position>();

        if((pos.i >= this.MapHeight) || (pos.j >= this.MapWidth)) {
            print("Warning: Provided pos of (" + pos.i + "," + pos.j + ") is invalid.\nUnable to check neighbors");
        }
        else {
            // Check Northern neighbor
            if(pos.i - 1 >= 0 && !(this.Board[(pos.i -1), pos.j])) {
                neighbors.Add(new Position(pos.i -1, pos.j));
            }

            // Check Eastern neighbor
            if(pos.j + 1 < this.MapWidth && !(this.Board[pos.i, (pos.j + 1)])) {
                neighbors.Add(new Position(pos.i, pos.j + 1));
            }

            // Check Southern neighbor
            if(pos.i + 1 < this.MapHeight && !(this.Board[(pos.i + 1), pos.j])) {
                neighbors.Add(new Position(pos.i + 1, pos.j));
            }

            // Check Western neighbor
            if(pos.j - 1 >= 0 && !(this.Board[pos.i, (pos.j - 1)])) {
                neighbors.Add(new Position(pos.i, pos.j - 1));
            }
        }

        //print("Neighbors count: " + neighbors.Count + " for position [" + pos.i + "," + pos.j + "]");
        return neighbors;
    }

    void InstantiateTiles() {
        Vector3 rot;
        bool corner = false;
        GameObject nextTile;
        GameObject newTile;
        GameObject newObject;
        //int numObstacles;

        // Get the spawn orientation
        rot = GetStartPieceRotation(this.path[0], this.path[1]);
        newObject = Instantiate(TILES[0], new Vector3(10*(path[0].j), 0, -10*(path[0].i)), Quaternion.Euler(rot), this.transform);
        newObject.name = "Start";

        // Place agent on starting tile
        //this.agent.transform.rotation = newObject.transform.rotation;
        //this.agent.transform.Rotate(0, 180, 0);

        //Vector3 temp = newObject.transform.Find("Target").transform.position; // Get position of target
        //temp.y = temp.y + 1;
        //this.agent.transform.position = temp;


        print("Added spawn at pos [" + path[0].i + "," + path[0].j + "]");

        for(int ind = 1; ind < this.path.Count-1; ind++) {
            corner = IsCorner(path[ind-1], path[ind], path[ind+1]);

            if (corner) {
                rot = GetCornerRotation(path[ind-1], path[ind], path[ind+1]);
                nextTile = TILES[2];
                print("Added corner at pos [" + path[ind].i + "," + path[ind].j + "]");
            }
            else {
                rot = GetStraightRotation(path[ind], path[ind + 1]);
                nextTile = TILES[1];
                print("Added straight at pos [" + path[ind].i + "," + path[ind].j + "]");
            }

            newTile = Instantiate(nextTile, new Vector3(10*(path[ind].j), 0, -10*(path[ind].i)), Quaternion.Euler(rot), this.transform);

            // Add obstacles
            //numObstacles = Random.Range(2, 6);

            for(int j = 0; j < this.obstaclesPerTile; j++) {
                newObject = Instantiate(OBJECTS[Random.Range(0,5)], newTile.transform);
                newObject.transform.SetPositionAndRotation((new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f))) + newObject.transform.position, Quaternion.identity);
            }
        }

        rot = GetEndPieceRotation(this.path[this.path.Count - 2], this.path[this.path.Count - 1]);
        newObject = Instantiate(TILES[0], new Vector3(10*(this.path[this.path.Count - 1].j), 0, -10*(this.path[this.path.Count - 1].i)), Quaternion.Euler(rot), this.transform);
        print("Added End at pos [" + this.path[this.path.Count - 1].i + "," + this.path[this.path.Count - 1].j + "]");

        // Fill in agent goal field
        this.agent.goal = newObject.transform.Find("Target").transform;

        // Add particle effects to goal tile
        Instantiate(Resources.Load<GameObject>("Goal_Particles"), newObject.transform.Find("Target").position, Quaternion.identity, newObject.transform.Find("Target").transform);

        generating = false;
    }

    Vector3 GetStartPieceRotation(Position pos1, Position pos2) {
        Vector3 rotation = new Vector3();

        if (pos1.i == pos2.i) {
            rotation.y = pos1.j < pos2.j ? -90 : 90;
        }
        else if (pos1.j == pos2.j) {
            rotation.y = pos1.i < pos2.i ? 0 : 180;
        }

        return rotation;
    }

    Vector3 GetEndPieceRotation(Position pos1, Position pos2) {
        Vector3 rotation = new Vector3();

        if (pos1.i == pos2.i) {
            rotation.y = pos2.j > pos1.j ? 90 : -90;
        }
        else if (pos1.j == pos2.j) {
            rotation.y = pos2.i > pos1.i ? 180 : 0;
        }

        return rotation;
    }

    Vector3 GetStraightRotation(Position pos1, Position pos2) {
        Vector3 rotation = new Vector3();

        if (pos1.i == pos2.i) {
            rotation.y = 90;
        }
        else if (pos1.j == pos2.j) {
            rotation.y = 0;
        }

        return rotation;
    }

    Vector3 GetCornerRotation(Position pos1, Position pos2, Position pos3) {
        Vector3 rotation = new Vector3();

        if (((pos1.i == pos2.i) && (pos2.j == pos3.j) && ((pos1.i < pos3.i) && (pos1.j > pos3.j))) || 
            ((pos1.j == pos2.j) && (pos2.i == pos3.i) && ((pos1.i > pos3.i) && (pos1.j < pos3.j)))) {
                rotation.y = -90;
            }
        else if (((pos1.i == pos2.i) && (pos2.j == pos3.j) && ((pos1.i > pos3.i) && (pos1.j < pos3.j))) || 
                 ((pos1.j == pos2.j) && (pos2.i == pos3.i) && ((pos1.i < pos3.i) && (pos1.j > pos3.j)))) {
                rotation.y = 90;
            }
        else if (((pos1.j == pos2.j) && (pos2.i == pos3.i) && ((pos1.i < pos3.i) && (pos1.j < pos3.j))) || 
                 ((pos1.i == pos2.i) && (pos2.j == pos3.j) && ((pos1.i > pos3.i) && (pos1.j > pos3.j)))) {
                rotation.y = 180;
            }
        else {
            rotation.y = 0;
        }

        return rotation;

    }

    bool IsCorner(Position pos1, Position pos2, Position pos3) {

        if((pos1.i == pos2.i) && (pos2.i == pos3.i) && (pos1.i == pos3.i)) {
            return false;
        }

        if((pos1.j == pos2.j) && (pos2.j == pos3.j) && (pos1.j == pos3.j)) {
            return false;
        }

        return true;
    }

}
