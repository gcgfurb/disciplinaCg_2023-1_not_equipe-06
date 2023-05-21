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
    private const float VERTEX_THRESHOLD = 0.03f;
    private int _selectedVertex = -1;

    private List<Ponto4D> _points;
    private BBoxRectangle _bboxRectangle;
    private bool _oldIsInside = false;

    public Poligono(Objeto paiRef, ref char _rotulo, List<Ponto4D> pontosPoligono) : base(paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.LineLoop;
      PrimitivaTamanho = 1;

      _points = pontosPoligono;
      _bboxRectangle = new BBoxRectangle(null, ref _rotulo, Bbox());
      base.pontosLista = _points;
      Atualizar();

      _bboxRectangle.UpdatePoints(_points);
    }

    private void Atualizar()
    {
      base.ObjetoAtualizar();
    }

    public void OnClick(Vector2i windowSize, Vector2 mousePosition)
    {
      var vertexIndex = -1;
      var closestDistance = VERTEX_THRESHOLD * 2 + 1;
      for (var i = 0; i < _points.Count; i++)
      {
        var mouse = Utils.MouseToPoint(windowSize, mousePosition);
        var diffX = (float)Math.Abs(_points[i].X - mouse.X);
        var diffY = (float)Math.Abs(_points[i].Y - mouse.Y);
        if (diffX < VERTEX_THRESHOLD && diffY < VERTEX_THRESHOLD && diffX + diffY < closestDistance)
        {
          closestDistance = diffX + diffY;
          vertexIndex = i;
        }
      }
      _selectedVertex = vertexIndex;
    }
    internal void OnDrag(Vector2i windowSize, Vector2 mousePosition)
    {
      if (_selectedVertex >= 0 && _selectedVertex < _points.Count)
      {
        _points[_selectedVertex] = Utils.MouseToPoint(windowSize, mousePosition);

        _bboxRectangle.UpdatePoints(_points);
        Atualizar();
      }
    }
    internal void OnUnclick()
    {
      _selectedVertex = -1;
    }
    internal void OnRightClick(Vector2i windowSize, Vector2 mousePosition)
    {
      var click = Utils.MouseToPoint(windowSize, mousePosition);

      var newIsInside = IsInside(click);
      var changedIsInside = _oldIsInside != newIsInside;
      _oldIsInside = newIsInside;

      if (!changedIsInside)
      {
        return;
      }


      if (IsInside(click))
      {
        FilhoAdicionar(_bboxRectangle.Retangulo);
      }
      else
      {
        FilhoRemover(_bboxRectangle.Retangulo);
      }
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
  }
}
