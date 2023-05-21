#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Axis : Objeto
  {
    public Axis(Objeto paiRef, ref char _rotulo) : base(paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Lines;
      PrimitivaTamanho = 1;

      var xAxis = new SegReta(this, ref _rotulo, new Ponto4D(-0.5, 0.0, 0.0), new Ponto4D(0.5, 0.0, 0.0));
      xAxis.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      FilhoAdicionar(xAxis);

      var yAxis = new SegReta(null, ref _rotulo, new Ponto4D(0.0, -0.5, 0.0), new Ponto4D(0.0, 0.5, 0.0));
      yAxis.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      FilhoAdicionar(yAxis);

      var zAxis = new SegReta(null, ref _rotulo, new Ponto4D(0.0, 0.0, -0.5), new Ponto4D(0.0, 0.0, 0.5));
      zAxis.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      FilhoAdicionar(zAxis);

      Atualizar();
    }

    private void Atualizar()
    {
      base.ObjetoAtualizar();
    }
  }
}
