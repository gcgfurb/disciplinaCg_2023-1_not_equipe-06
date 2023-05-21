#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace gcgcg
{
  internal class Poligono : Objeto
  {
    public bool IsComplete { get; private set; } = true;

    private const float VERTEX_THRESHOLD = 0.03f;
    private int _selectedVertex = -1;

    private BBoxRectangle _bboxRectangle;
    private bool _oldIsInside = false;

    public Poligono(Objeto paiRef, ref char _rotulo, List<Ponto4D> pontosPoligono) : base(paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.LineLoop;
      PrimitivaTamanho = 1;

      _bboxRectangle = new BBoxRectangle(null, ref _rotulo, Bbox());
      base.pontosLista = pontosPoligono;
      Atualizar();

      _bboxRectangle.UpdatePoints(pontosLista);
    }

    private void Atualizar()
    {
      base.ObjetoAtualizar();
    }

    public void TrySelectVertex(Vector2i windowSize, Vector2 mousePosition)
    {
      var vertexIndex = -1;
      var closestDistance = VERTEX_THRESHOLD * 2 + 1;
      for (var i = 0; i < pontosLista.Count; i++)
      {
        var mouse = Utils.MouseToPoint(windowSize, mousePosition);
        var diffX = (float)Math.Abs(pontosLista[i].X - mouse.X);
        var diffY = (float)Math.Abs(pontosLista[i].Y - mouse.Y);
        if (IsClickingVertex(i, mouse) && diffX + diffY < closestDistance)
        {
          closestDistance = diffX + diffY;
          vertexIndex = i;
        }
      }
      _selectedVertex = vertexIndex;
    }
    internal void TryDragVertex(Vector2i windowSize, Vector2 mousePosition)
    {
      if (_selectedVertex >= 0 && _selectedVertex < pontosLista.Count)
      {
        pontosLista[_selectedVertex] = Utils.MouseToPoint(windowSize, mousePosition);

        _bboxRectangle.UpdatePoints(pontosLista);
        Atualizar();
      }
    }
    internal void ReleaseVertex()
    {
      _selectedVertex = -1;
    }
    internal bool TrySelect(Vector2i windowSize, Vector2 mousePosition)
    {
      var click = Utils.MouseToPoint(windowSize, mousePosition);

      var newIsInside = IsInside(click);
      var changedIsInside = _oldIsInside != newIsInside;
      _oldIsInside = newIsInside;

      if (!changedIsInside)
      {
        return newIsInside;
      }

      if (IsInside(click))
      {
        Select();
        return true;
      }
      Unselect();
      return false;
    }
    internal void Select()
    {
      FilhoAdicionar(_bboxRectangle.Retangulo);
      Atualizar();
    }
    internal void Unselect()
    {
      FilhoRemover(_bboxRectangle.Retangulo);
      Atualizar();
    }
    private bool IsInside(Ponto4D point)
    {
      if (_bboxRectangle.IsOutside(point))
      {
        return false;
      }

      var nIntersections = 0;
      for (var i = 0; i < pontosLista.Count; i++)
      {
        var currPoint = pontosLista[i];
        var nextPoint = pontosLista[(i + 1) % pontosLista.Count];
        if (currPoint.Y == nextPoint.Y)
        {
          if (point.Y == currPoint.Y
            && point.X >= Math.Min(currPoint.X, nextPoint.X)
            && point.X <= Math.Min(currPoint.X, nextPoint.X))
          {
            break;
          }
        }
        else
        {
          var t = (point.Y - currPoint.Y) / (nextPoint.Y - currPoint.Y);
          var xIntersection = currPoint.X + (nextPoint.X - currPoint.X) * t;
          var yIntersection = currPoint.Y + (nextPoint.Y - currPoint.Y) * t;
          if (xIntersection == point.X)
          {
            break;
          }
          else if (xIntersection > point.X
            && yIntersection > Math.Min(currPoint.Y, nextPoint.Y)
            && yIntersection <= Math.Max(currPoint.Y, nextPoint.Y))
          {
            nIntersections += 1;
          }
        }
      }
      return nIntersections % 2 == 1;
    }

    internal static Poligono StartDrawing(
      Objeto paiRef, ref char _rotulo, Vector2i windowSize, Vector2 mousePosition)
    {
      var point = Utils.MouseToPoint(windowSize, mousePosition);
      var newPolygon = new Poligono(paiRef, ref _rotulo, new List<Ponto4D> { point });
      newPolygon.IsComplete = false;
      newPolygon.PrimitivaTipo = PrimitiveType.LineStrip;
      return newPolygon;
    }
    internal void AddLine(Vector2i windowSize, Vector2 mousePosition)
    {
      if (IsComplete)
      {
        return;
      }

      var newPoint = Utils.MouseToPoint(windowSize, mousePosition);
      if (pontosLista.Count > 2 && IsClickingVertex(0, newPoint))
      {
        PrimitivaTipo = PrimitiveType.LineLoop;
        FinishLine();
        return;
      }

      pontosLista.Add(newPoint);

      _bboxRectangle.UpdatePoints(pontosLista);
      Atualizar();
    }
    internal void DragLine(Vector2i windowSize, Vector2 mousePosition)
    {
      if (IsComplete)
      {
        return;
      }

      pontosLista[^1] = Utils.MouseToPoint(windowSize, mousePosition);

      _bboxRectangle.UpdatePoints(pontosLista);
      Atualizar();
    }
    internal void FinishLine()
    {
      pontosLista.RemoveAt(pontosLista.Count - 1);
      IsComplete = true;
    }

    private bool IsClickingVertex(int vertexIndex, Ponto4D point)
    {
      var diffX = (float)Math.Abs(pontosLista[vertexIndex].X - point.X);
      var diffY = (float)Math.Abs(pontosLista[vertexIndex].Y - point.Y);
      return diffX < VERTEX_THRESHOLD
        && diffY < VERTEX_THRESHOLD;
    }
  }
}
