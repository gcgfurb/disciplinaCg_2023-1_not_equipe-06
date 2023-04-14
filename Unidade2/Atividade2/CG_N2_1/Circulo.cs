#define CG_Debug

using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Circulo : Objeto
  {
    private readonly List<Ponto> _circumferencePoints;

    public Circulo(Objeto paiRef, Ponto4D center, float radius, int precision) : base(paiRef)
    {
      PrimitivaTipo = PrimitiveType.LineLoop;
      PrimitivaTamanho = radius;
      
      _circumferencePoints = new List<Ponto>(precision);

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

        var circumferencePoint = new Ponto(null, point);
        circumferencePoint.PrimitivaTamanho = 5;
        circumferencePoint.shaderCor = circumferencePointShader;
        _circumferencePoints.Add(circumferencePoint);
      }
    }

    public void Atualizar()
    {
      base.ObjetoAtualizar();
      
      foreach (var point in _circumferencePoints)
      {
        point.ObjetoAtualizar();
      }
    }

    public override void Desenhar()
    {
      base.Desenhar();

      foreach (var point in _circumferencePoints)
      {
        point.Desenhar();
      }
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno  = "__ Objeto Círculo _ Tipo: " + PrimitivaTipo + " _ Raio: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
