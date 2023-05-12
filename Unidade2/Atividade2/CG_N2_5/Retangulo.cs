#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Retangulo : Objeto
  {
    private readonly Ponto4D _ptoInfEsq;
    private readonly Ponto4D _ptoSupDir;
    private bool _isRectangle;

    public Retangulo(Objeto paiRef, Ponto4D ptoInfEsq, Ponto4D ptoSupDir) : base(paiRef)
    {
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 10;

      _ptoInfEsq = ptoInfEsq;
      _ptoSupDir = ptoSupDir;
      _isRectangle = true;

      AddRectanglePoints();
      Atualizar();
    }

    private void AddRectanglePoints()
    {
      base.PontosAdicionar(_ptoInfEsq);
      base.PontosAdicionar(new Ponto4D(_ptoSupDir.X, _ptoInfEsq.Y));
      base.PontosAdicionar(_ptoSupDir);
      base.PontosAdicionar(new Ponto4D(_ptoInfEsq.X, _ptoSupDir.Y));
    }

    private void AddCornerPoints()
    {
      var point4D = new Ponto4D(_ptoSupDir);
      var point = new Ponto(null, point4D);
      point.PrimitivaTamanho = 15;
      FilhoAdicionar(point);

      point4D = new Ponto4D(_ptoInfEsq);
      point = new Ponto(null, point4D);
      point.PrimitivaTamanho = 15;
      FilhoAdicionar(point);

      point4D = new Ponto4D(_ptoSupDir);
      point4D.Y = _ptoInfEsq.Y;
      point = new Ponto(null, point4D);
      point.PrimitivaTamanho = 15;
      FilhoAdicionar(point);

      point4D = new Ponto4D(_ptoInfEsq);
      point4D.Y = _ptoSupDir.Y;
      point = new Ponto(null, point4D);
      point.PrimitivaTamanho = 15;
      FilhoAdicionar(point);
    }

    public void Atualizar()
    {

      base.ObjetoAtualizar();
    }

    public bool IsOutside(Ponto4D point)
    {
      return point.X < _ptoInfEsq.X
        || point.Y < _ptoInfEsq.Y
        || point.X > _ptoSupDir.X
        || point.Y > _ptoSupDir.Y;
    }

    public bool IsInside(Ponto4D point)
    {
      return !IsOutside(point);
    }

    public void ChangeForm()
    {
      pontosLista.Clear();
      LimparFilhos();

      _isRectangle = !_isRectangle;
      if (_isRectangle)
      {
        AddRectanglePoints();
      }
      else
      {
        AddCornerPoints();
      }
      Atualizar();
    }
  }
}
