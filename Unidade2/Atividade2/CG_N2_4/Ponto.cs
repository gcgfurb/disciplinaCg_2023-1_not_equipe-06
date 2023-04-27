#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Ponto : Objeto
  {
    public Ponto4D Position { get; set; }

    public Ponto(Objeto paiRef, Ponto4D pto) : base(paiRef)
    {
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 20;

      base.PontosAdicionar(pto);

      Position = pto;

      Atualizar();
    }

    public void Atualizar()
    {
      base.ObjetoAtualizar();
    }

    public void Move(Ponto4D move)
    {
      pontosLista.Clear();
      Position += move;
      base.PontosAdicionar(Position);
      Atualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Ponto _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);

    }
#endif

  }
}
