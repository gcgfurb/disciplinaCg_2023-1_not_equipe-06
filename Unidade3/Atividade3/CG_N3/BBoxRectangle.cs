#define CG_Debug

using System.Collections.Generic;
using CG_Biblioteca;

namespace gcgcg
{
  internal class BBoxRectangle
  {
    public Retangulo Retangulo { get; private set; }

    private BBox _internalBbox;

    public BBoxRectangle(Objeto paiRef, ref char rotulo, BBox bbox)
    {
      _internalBbox = bbox;

      Retangulo = new Retangulo(
        paiRef,
        ref rotulo,
        new Ponto4D(_internalBbox.obterMenorX, _internalBbox.obterMenorY),
        new Ponto4D(_internalBbox.obterMaiorX, _internalBbox.obterMaiorY)
      );
      Retangulo.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      Retangulo.PrimitivaTipo = OpenTK.Graphics.OpenGL4.PrimitiveType.LineLoop;
    }

    public void UpdatePoints(List<Ponto4D> points)
    {
      _internalBbox.Atualizar(points);

      Retangulo.LowerLeftPoint.X = _internalBbox.obterMenorX;
      Retangulo.LowerLeftPoint.Y = _internalBbox.obterMenorY;
      Retangulo.UpperRightPoint.X = _internalBbox.obterMaiorX;
      Retangulo.UpperRightPoint.Y = _internalBbox.obterMaiorY;
      Retangulo.AddPoints();
      Retangulo.Atualizar();
    }

    public bool IsOutside(Ponto4D point)
    {
      return point.X < _internalBbox.obterMenorX
        || point.Y < _internalBbox.obterMenorY
        || point.X > _internalBbox.obterMaiorX
        || point.Y > _internalBbox.obterMaiorY;
    }
  }
}
