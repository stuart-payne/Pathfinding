using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject m_SquarePrefab;
    [SerializeField] private int m_Width;
    [SerializeField] private int m_Height;
    
    private Queue<Square> m_SelectedSquares;
    private Square[,] m_Grid;
    private Square[] m_PreviousPath;

    private readonly Vector2Int[] m_PossibleNeighbours =
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    private void Awake()
    {
        m_SelectedSquares = new Queue<Square>();
        m_Grid = new Square[m_Width, m_Height];
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (var x = 0; x < m_Width; x++)
        {
            for (var y = 0; y < m_Height; y++)
            {
                var square = Instantiate(
                    m_SquarePrefab,
                    new Vector3(x - (m_Width / 2), y - (m_Height / 2), 0),
                    Quaternion.identity).GetComponent<Square>();
                square.Manager = this;
                m_Grid[x, y] = square;
                square.GridPosition = new Vector2Int(x, y);
            }
        }
    }

    private bool CheckInBounds(Vector2Int position)
    {
        if (position.x < 0 || position.x >= m_Width)
            return false;
        if (position.y < 0 || position.y >= m_Height)
            return false;
        return true;
    }

    public Square[] GetNeighbourSquares(Vector2Int position)
    {
        var squareList = new List<Square>();
        foreach (var possibleNeighbour in m_PossibleNeighbours)
        {
            var newPosition = position + possibleNeighbour;
            if(!CheckInBounds(newPosition))
                continue;
            var square = m_Grid[newPosition.x, newPosition.y];
            if(square.State != SquareState.Obstacle)
                squareList.Add(square);
        }
        return squareList.ToArray();
    }

    public void SelectSquare(Square square)
    {
        square.State = SquareState.Selected;
        m_SelectedSquares.Enqueue(square);
        square.SetToSelectedColor();
       
        if (m_SelectedSquares.Count > 2)
        {
            var poppedSquare = m_SelectedSquares.Dequeue();
            poppedSquare.SetColor(SquareState.Empty);
            poppedSquare.State = SquareState.Empty;
        }
        
        if (m_SelectedSquares.Count == 2)
        {
            var squares = m_SelectedSquares.ToArray();
            var path = Pathfinding.BreadthFillSearch(squares[0], squares[1]);

            if (m_PreviousPath != null)
            {
                foreach (var node in m_PreviousPath)
                {
                    if (node.State != SquareState.Selected)
                    {
                        node.SetColor(SquareState.Empty);
                        node.State = SquareState.Empty;
                    }
                }
            }

            foreach (var node in path)
            {
                if (node.State != SquareState.Selected)
                {
                    node.SetColor(SquareState.Path);
                    node.State = SquareState.Path;
                }
            }

            m_PreviousPath = path;
        }
    }
}