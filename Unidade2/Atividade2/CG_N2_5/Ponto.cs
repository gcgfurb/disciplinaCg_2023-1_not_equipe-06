#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Ponto : Objeto
  {
    public Ponto4D Point { get; private set; }

    public Ponto(Objeto paiRef, Ponto4D pto) : base(paiRef)
    {
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 20;

      Point = pto;

      base.PontosAdicionar(pto);
      Atualizar();
    }

    public void Atualizar()
    {

      base.ObjetoAtualizar();
    }

    public void Move(Ponto4D ponto)
    {
      pontosLista.Clear();
      Point += ponto;
      base.PontosAdicionar(Point);
      Atualizar();
    }
  }
}
