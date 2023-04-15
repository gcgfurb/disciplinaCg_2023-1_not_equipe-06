#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class SrPalito : Objeto
  {
    private Ponto4D origin;
    private float size;
    private float angle;

    public SrPalito(Objeto paiRef, Ponto4D origin, float size, float angle) : base(paiRef)
    {
      PrimitivaTipo = PrimitiveType.Lines;
      PrimitivaTamanho = size;
      
      this.origin = origin;
      this.size = size;
      this.angle = angle;

      AddPoints();

      Atualizar();
    }

    private void AddPoints()
    {
      base.PontosAdicionar(origin);
      
      var head = Matematica.GerarPtosCirculo(angle, size) + origin;
      base.PontosAdicionar(head);
    }

    public void Atualizar()
    {
      base.ObjetoAtualizar();
    }

    public void Move(float stepSize)
    {
      pontosLista.Clear();
      origin.X += stepSize;
      AddPoints();
      Atualizar();
    }

    public void Rotate(float degrees)
    {
      pontosLista.Clear();
      angle += degrees;
      AddPoints();
      Atualizar();
    }
    
    public void Scale(float size)
    {
      pontosLista.Clear();
      this.size += size;
      AddPoints();
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
