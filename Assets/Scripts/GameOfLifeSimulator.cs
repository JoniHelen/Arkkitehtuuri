using UnityEngine;

public class GameOfLifeSimulator : MonoBehaviour
{
    [SerializeField] private GameObject TileObject;

    private const int Width = 100;
    private const int Height = 100;

    private bool[][] SimulationBuffer1 = new bool[Width][];
    private bool[][] SimulationBuffer2 = new bool[Width][];
    private Material[][] Tiles = new Material[Width][];

    private bool SimulationStep;

    private float NextUpdate = 2;

    private const float UpdateInterval = 0.1f;
    
    void Start()
    {
        GenerateCells();
        SeedSimulation();
        UpdateVisual(SimulationBuffer1);
    }
    
    void Update()
    {
        if (Time.time >= NextUpdate)
        {
            SimulationStep = !SimulationStep;
            
            if (SimulationStep)
            {
                UpdateSimulation(SimulationBuffer1, SimulationBuffer2);
                UpdateVisual(SimulationBuffer2);
            }
            else
            {
                UpdateSimulation(SimulationBuffer2, SimulationBuffer1);
                UpdateVisual(SimulationBuffer1);
            }
            
            NextUpdate = Time.time + UpdateInterval;
        }
    }

    // Seeds R-Pentomino
    private void SeedSimulation()
    {
        int middleX = Width / 2;
        int middleY = Height / 2;
        
        SimulationBuffer1[middleX][middleY] = true;
        Tiles[middleX][middleY].color = Color.green;
        SimulationBuffer1[middleX][middleY + 1] = true;
        Tiles[middleX][middleY + 1].color = Color.green;
        SimulationBuffer1[middleX + 1][middleY + 1] = true;
        Tiles[middleX + 1][middleY + 1].color = Color.green;
        SimulationBuffer1[middleX - 1][middleY] = true;
        Tiles[middleX - 1][middleY].color = Color.green;
        SimulationBuffer1[middleX][middleY - 1] = true;
        Tiles[middleX][middleY - 1].color = Color.green;
    }
    
    private void GenerateCells()
    {
        for (int x = 0; x < Width; ++x)
        {
            SimulationBuffer1[x] = new bool[Height];
            SimulationBuffer2[x] = new bool[Height];

            Tiles[x] = new Material[Height];
            
            for (int y = 0; y < Height; ++y)
            {
                SimulationBuffer1[x][y] = false;
                SimulationBuffer2[x][y] = false;

                Tiles[x][y] = Instantiate(TileObject, new Vector3(x, 0, y), Quaternion.identity)
                                .GetComponent<MeshRenderer>().material;
                Tiles[x][y].color = Color.black;
            }
        }
    }
    
    private int GetLiveNeighbours(bool[][] buffer, int x, int y)
    {
        int neighbours = 0;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i == x & j == y || i < 0 || j < 0 || i >= Width || j >= Height) continue;
                
                if (buffer[i][j])
                {
                    neighbours++;
                }
            }
        }
        return neighbours;
    }

    private void UpdateSimulation(bool[][] sourceBuffer, bool[][] destinationBuffer)
    {
        for (int x = 0; x < Width; ++x)
        {
            for (int y = 0; y < Height; ++y)
            {
                int countNeighbours = GetLiveNeighbours(sourceBuffer, x, y);

                destinationBuffer[x][y] = 
                    sourceBuffer[x][y] ? countNeighbours is > 1 and < 4 && sourceBuffer[x][y] : countNeighbours == 3;
            }
        }
    }

    private void UpdateVisual(bool[][] buffer)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tiles[x][y].color = buffer[x][y] ? Color.green : Color.black;
            }
        }
    }
}