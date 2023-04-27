#define CG_Debug

using System;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Spline : Objeto
  {
    private readonly double _bezierStep;

    private readonly List<Ponto> _splinePoints;
    private readonly Shader _splintPointShader;
    private readonly Shader _splintPointSelectedShader;

    private readonly List<SegReta> _splineLines;
    private readonly Shader _splineLineShader;
    private readonly Shader _bezierLineShader;

    private int _selectedPointIndex;

    public Spline(Objeto paiRef, double bezierStep) : base(paiRef)
    {
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 10;

      _bezierStep = bezierStep;

      _splinePoints = new List<Ponto>();
      _splintPointShader = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _splintPointSelectedShader = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");

      _splineLines = new List<SegReta>();
      _splineLineShader = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _bezierLineShader = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");

      Reset();
    }

    public void Reset()
    {
      pontosLista.Clear();
      _splineLines.Clear();
      _splinePoints.Clear();

      AddPoints();
      Atualizar();
    }

    private void AddPoints()
    {
      var xKeyPoints = new double[] { -0.5, -0.5, 0.5, 0.5 };
      var yKeyPoints = new double[] { -0.5, 0.5, 0.5, -0.5 };
      for (var i = 0; i < xKeyPoints.Length; i++)
      {
        _splinePoints.Add(GetPoint(xKeyPoints[i], yKeyPoints[i]));
      }

      UpdateSelectedPoint(0);
    }

    private Ponto GetPoint(double x = 0.0, double y = 0.0)
    {
      var point = new Ponto(null, new Ponto4D(x: x, y: y));
      point.PrimitivaTamanho = 20;
      point.shaderCor = _splintPointShader;
      return point;
    }

    private void AddLines()
    {
      for (var i = 0; i < _splinePoints.Count - 1; i++)
      {
        var line = new SegReta(null, _splinePoints[i].Position, _splinePoints[i + 1].Position);
        line.shaderCor = _splineLineShader;
        _splineLines.Add(line);
      }
    }

    private void AddBezier()
    {
      var bezierPoints = new List<Ponto4D>();
      for (var t = 0.0; t <= 1; t += _bezierStep)
      {
        bezierPoints.Add(GetBezierValue(t));
      }

      for (var i = 0; i < bezierPoints.Count - 1; i++)
      {
        var line = new SegReta(null, bezierPoints[i], bezierPoints[i + 1]);
        line.shaderCor = _bezierLineShader;
        _splineLines.Add(line);
      }
    }

    private Ponto4D GetBezierValue(double t)
    {
      var n = _splinePoints.Count - 1;
      var x = 0.0;
      var y = 0.0;
      for (var i = 0; i < _splinePoints.Count; i++)
      {
        var basis = GetBernstein(n, i, t);
        x += basis * _splinePoints[i].Position.X;
        y += basis * _splinePoints[i].Position.Y;
      }
      return new Ponto4D(x: x, y: y);
    }

    private double GetBernstein(int n, int i, double t)
    {
      var coefficient = GetFactorial(n) / (GetFactorial(i) * GetFactorial(n - i));
      return coefficient * Math.Pow(t, i) * Math.Pow(1 - t, n - i);
    }

    private double GetFactorial(int n)
    {
      var result = 1.0;
      for (var i = 2; i <= n; i++)
      {
        result *= i;
      }
      return result;
    }

    public void Atualizar()
    {
      AddLines();
      AddBezier();

      base.ObjetoAtualizar();

      foreach (var point in _splinePoints)
      {
        point.ObjetoAtualizar();
      }

      foreach (var line in _splineLines)
      {
        line.ObjetoAtualizar();
      }
    }

    public override void Desenhar()
    {
      base.Desenhar();

      foreach (var point in _splinePoints)
      {
        point.Desenhar();
      }

      foreach (var line in _splineLines)
      {
        line.Desenhar();
      }
    }

    public void SelectNextPoint()
    {
      UpdateSelectedPoint((_selectedPointIndex + 1) % _splinePoints.Count);
    }

    private void UpdateSelectedPoint(int newSelectedPointIndex)
    {
      _selectedPointIndex = newSelectedPointIndex;

      var oldIndex = _selectedPointIndex == 0 ? _splinePoints.Count - 1 : _selectedPointIndex - 1;
      _splinePoints[oldIndex].shaderCor = _splintPointShader;
      _splinePoints[_selectedPointIndex].shaderCor = _splintPointSelectedShader;
    }

    public void MoveSelectedPoint(Ponto4D move)
    {
      pontosLista.Clear();
      _splineLines.Clear();
      _splinePoints[_selectedPointIndex].Move(move);
      Atualizar();
    }

    public void AddPoint()
    {
      pontosLista.Clear();
      _splineLines.Clear();

      var newPointIndex = _selectedPointIndex + 1;
      _splinePoints.Insert(newPointIndex, GetPoint(0, 0));
      UpdateSelectedPoint(newPointIndex);
      Atualizar();
    }

    public void RemovePoint()
    {
      pontosLista.Clear();
      _splineLines.Clear();
      _splinePoints.Remove(_splinePoints[_selectedPointIndex]);
      UpdateSelectedPoint(_selectedPointIndex);
      Atualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Spline _ Tipo: " + PrimitivaTipo + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
