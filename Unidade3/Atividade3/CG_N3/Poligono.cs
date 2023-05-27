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

    private char _randomLabel;

    private Retangulo _rectangle;
    private bool _oldIsInside = false;
    private int _selectedVertex = -1;

    public bool IsComplete { get; private set; } = true;

    public Poligono(Objeto paiRef, ref char _rotulo, List<Ponto4D> pontosPoligono) : base(paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.LineLoop;
      PrimitivaTamanho = 1;

      pontosLista = pontosPoligono;
      Atualizar();

      _rectangle = new Retangulo(
        this,
        ref _rotulo,
        new Ponto4D(Bbox().obterMenorX, Bbox().obterMenorY),
        new Ponto4D(Bbox().obterMaiorX, Bbox().obterMaiorY));
      _rectangle.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      _rectangle.PrimitivaTipo = PrimitiveType.LineLoop;

      FilhoRemover(_rectangle);
    }

    private void Atualizar()
    {
      base.ObjetoAtualizar();
    }

    public void TrySelectVertex(Vector2i windowSize, Vector2 mousePosition)
    {
      var vertexIndex = -1;
      var closestDistance = VERTEX_THRESHOLD * 2 + 1;
      var mouse = Utils.MouseToPoint(windowSize, mousePosition);
      mouse = ToOrigin(mouse);
      for (var i = 0; i < pontosLista.Count; i++)
      {
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
        pontosLista[_selectedVertex] = ToOrigin(Utils.MouseToPoint(windowSize, mousePosition));

        Atualizar();
        UpdateBboxRectangle();
      }
    }
    internal void ReleaseVertex()
    {
      _selectedVertex = -1;
    }
    internal void TryRemoveVertex(Vector2i windowSize, Vector2 mousePosition)
    {
      var vertexIndex = -1;
      var closestDistance = VERTEX_THRESHOLD * 2 + 1;
      var mouse = Utils.MouseToPoint(windowSize, mousePosition);
      mouse = ToOrigin(mouse);
      for (var i = 0; i < pontosLista.Count; i++)
      {
        var diffX = (float)Math.Abs(pontosLista[i].X - mouse.X);
        var diffY = (float)Math.Abs(pontosLista[i].Y - mouse.Y);
        if (IsClickingVertex(i, mouse) && diffX + diffY < closestDistance)
        {
          closestDistance = diffX + diffY;
          vertexIndex = i;
        }
      }
      if (vertexIndex >= 0 && vertexIndex < pontosLista.Count)
      {
        pontosLista.RemoveAt(vertexIndex);
        Atualizar();
        UpdateBboxRectangle();
      }
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
      FilhoAdicionar(_rectangle);
      Atualizar();
    }
    internal void Unselect()
    {
      FilhoRemover(_rectangle);
      Atualizar();
    }
    private bool IsInside(Ponto4D point)
    {
      if (_rectangle.IsOutside(point))
      {
        return false;
      }

      point = ToOrigin(point);

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

    internal static Poligono StartDrawing(Objeto paiRef, ref char _rotulo, Vector2i windowSize, Vector2 mousePosition)
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
      pontosLista.Add(newPoint);
      Atualizar();
    }
    internal void DragLine(Vector2i windowSize, Vector2 mousePosition)
    {
      if (IsComplete)
      {
        return;
      }

      pontosLista[^1] = Utils.MouseToPoint(windowSize, mousePosition);
      Atualizar();
    }
    internal void FinishLine()
    {
      pontosLista.RemoveAt(pontosLista.Count - 1);
      UpdateBboxRectangle();
      IsComplete = true;
    }
    internal void OpenClosePolygon()
    {
      if (PrimitivaTipo == PrimitiveType.LineLoop)
      {
        PrimitivaTipo = PrimitiveType.LineStrip;
      }
      else
      {
        PrimitivaTipo = PrimitiveType.LineLoop;
      }
    }

    internal void RemovePoints()
    {
      pontosLista.Clear();
      FilhoRemover(_rectangle);
      Atualizar();
    }

    internal void Translate(double tx = 0, double ty = 0, double tz = 0)
    {
      if (tx == 0 && ty == 0 && tz == 0)
      {
        return;
      }

      MatrizTranslacaoXYZ(tx, ty, tz);
      UpdateBboxRectangle();
    }

    internal void Scale(double sx = 0, double sy = 0, double sz = 0)
    {
      if (sx == 0 && sy == 0 && sz == 0)
      {
        return;
      }

      MatrizEscalaXYZ(sx, sy, sz);

      UpdateBboxRectangle();
    }

    internal void Rotate(double rx = 0, double ry = 0, double rz = 0)
    {
      if (rx == 0 && ry == 0 && rz == 0)
      {
        return;
      }

      MatrizRotacao(rz);

      UpdateBboxRectangle();
    }

    private bool IsClickingVertex(int vertexIndex, Ponto4D point)
    {
      var diffX = (float)Math.Abs(pontosLista[vertexIndex].X - point.X);
      var diffY = (float)Math.Abs(pontosLista[vertexIndex].Y - point.Y);
      return diffX < VERTEX_THRESHOLD
        && diffY < VERTEX_THRESHOLD;
    }
    private Ponto4D ToOrigin(Ponto4D point)
    {
      return matriz.InverterMatriz().MultiplicarPonto(point);
    }
    private void UpdateBboxRectangle()
    {
      var points = new List<Ponto4D>
      {
        new Ponto4D(Bbox().obterMenorX, Bbox().obterMenorY),
        new Ponto4D(Bbox().obterMenorX, Bbox().obterMaiorY),
        new Ponto4D(Bbox().obterMaiorX, Bbox().obterMaiorY),
        new Ponto4D(Bbox().obterMaiorX, Bbox().obterMenorY)
      };

      double? minX = null;
      double? minY = null;
      double? maxX = null;
      double? maxY = null;

      foreach (var point in points)
      {
        var transformedPoint = matriz.MultiplicarPonto(point);
        if (minX is null || transformedPoint.X < minX)
        {
          minX = transformedPoint.X;
        }
        if (minY is null || transformedPoint.Y < minY)
        {
          minY = transformedPoint.Y;
        }
        if (maxX is null || transformedPoint.X > maxX)
        {
          maxX = transformedPoint.X;
        }
        if (maxY is null || transformedPoint.Y > maxY)
        {
          maxY = transformedPoint.Y;
        }
      }

      _rectangle.UpdatePoints(
        minX.Value, minY.Value, maxX.Value, maxY.Value
      );
    }
  }
}
