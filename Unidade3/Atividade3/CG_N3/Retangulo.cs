#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Retangulo : Objeto
  {
    public Ponto4D LowerLeftPoint { get; set; }
    public Ponto4D UpperRightPoint { get; set; }

    public Retangulo(Objeto paiRef, ref char _rotulo, Ponto4D ptoInfEsq, Ponto4D ptoSupDir) : base(paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 10;

      LowerLeftPoint = ptoInfEsq;
      UpperRightPoint = ptoSupDir;

      AddPoints();
      Atualizar();
    }

    public void AddPoints()
    {
      pontosLista.Clear();
      base.PontosAdicionar(LowerLeftPoint);
      base.PontosAdicionar(new Ponto4D(UpperRightPoint.X, LowerLeftPoint.Y));
      base.PontosAdicionar(UpperRightPoint);
      base.PontosAdicionar(new Ponto4D(LowerLeftPoint.X, UpperRightPoint.Y));
    }

    public void Atualizar()
    {
      base.ObjetoAtualizar();
    }
  }
}