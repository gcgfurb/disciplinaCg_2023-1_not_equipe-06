#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class SegReta : Objeto
  {
    private Ponto4D _initialPos;
    private Ponto4D _endPos;

    public SegReta(Objeto paiRef, ref char _rotulo, Ponto4D ptoIni, Ponto4D ptoFim) : base(paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Lines;
      PrimitivaTamanho = 1;

      this._initialPos = ptoIni;
      this._endPos = ptoFim;

      AddPoints();
      Atualizar();
    }

    private void AddPoints()
    {
      pontosLista.Clear();
      base.PontosAdicionar(_initialPos);
      base.PontosAdicionar(_endPos);
    }

    private void Atualizar()
    {
      base.ObjetoAtualizar();
    }

    public void MoveEnd(Ponto4D newEnd)
    {
      _endPos = newEnd;
      AddPoints();
      Atualizar();
    }
  }
}
