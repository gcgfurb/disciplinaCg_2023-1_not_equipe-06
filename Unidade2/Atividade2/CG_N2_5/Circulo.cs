#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Circulo : Objeto
  {
    private Ponto4D _center;
    private readonly float _radius;
    private readonly int _precision;

    public Circulo(Objeto paiRef, Ponto4D center, float radius, int precision) : base(paiRef)
    {
      PrimitivaTipo = PrimitiveType.LineLoop;
      PrimitivaTamanho = radius;

      _center = center;
      _radius = radius;
      _precision = precision;

      AddPoints(center, radius, precision);

      Atualizar();
    }

    private void AddPoints(Ponto4D center, float radius, int precision)
    {
      var angle_step = 360 / precision;
      var circumferencePointShader = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      for (double angle = 0; angle < 360; angle += angle_step)
      {
        var point = Matematica.GerarPtosCirculo(angle, radius);
        point.X += center.X;
        point.Y += center.Y;
        point.Z += center.Z;
        base.PontosAdicionar(point);
      }
    }

    public void Atualizar()
    {
      base.ObjetoAtualizar();
    }

    public void Move(Ponto4D ponto)
    {
      pontosLista.Clear();
      _center += ponto;
      AddPoints(_center, _radius, _precision);
      Atualizar();
    }

    public bool IsOutside(Ponto4D ponto)
    {
      var x = (ponto.X - _center.X) * (ponto.X - _center.X);
      var y = (ponto.Y - _center.Y) * (ponto.Y - _center.Y);
      var centerDistance = x + y;
      return centerDistance > _radius * _radius;
    }
  }
}
