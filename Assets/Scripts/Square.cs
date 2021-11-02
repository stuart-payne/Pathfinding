using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour, INode
{
    [SerializeField] private Color m_HoverColor;
    [SerializeField] private Color m_SelectedColor;
    [SerializeField] private Color m_PathColor;
    [SerializeField] private Color m_ObstacleColor;

    public GridManager Manager;
    public SquareState State = SquareState.Empty;

    private SpriteRenderer m_Rend;
    private Color m_BaseColor;
    private Dictionary<SquareState, Color> m_StateColors;

    public Vector2Int GridPosition { get; set; }

    public void SetToBaseColor() => m_Rend.color = m_BaseColor;
    public void SetToSelectedColor() => m_Rend.color = m_SelectedColor;
    public void SetToPathColor() => m_Rend.color = m_PathColor;

    public INode[] GetNeighbours() => Manager.GetNeighbourSquares(GridPosition);

    public void SetColor(SquareState state)
    {
        m_Rend.color = m_StateColors[state];
    }
    
    private void Awake()
    {
        m_Rend = GetComponent<SpriteRenderer>();
        m_BaseColor = m_Rend.color;
        m_StateColors = new Dictionary<SquareState, Color>
        {
            [SquareState.Empty] = m_BaseColor,
            [SquareState.Obstacle] = m_ObstacleColor,
            [SquareState.Path] = m_PathColor,
            [SquareState.Selected] = m_SelectedColor
        };
    }

    private void OnMouseEnter()
    {
        if(State == SquareState.Selected || State == SquareState.Obstacle)
            return;
        m_Rend.color = m_HoverColor;
    }

    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (State == SquareState.Empty)
            {
                State = SquareState.Obstacle;
                SetColor(SquareState.Obstacle);
            }
        }
    }

    private void OnMouseExit()
    {
        if (State == SquareState.Selected || State == SquareState.Obstacle)
            return; 
        SetColor(State);
    }

    private void OnMouseDown()
    {
        Manager.SelectSquare(this);
        PrintNeighbours();
    }

    private void PrintNeighbours()
    {
        var neighbours = Manager.GetNeighbourSquares(GridPosition);
        Debug.Log($"I have {neighbours.Length} neighbours");
        foreach (var neighbour in neighbours)
        {
            Debug.Log($"{neighbour.GridPosition}");
        }
    }
}

public enum SquareState
{
    Empty, Path, Obstacle, Selected
}
