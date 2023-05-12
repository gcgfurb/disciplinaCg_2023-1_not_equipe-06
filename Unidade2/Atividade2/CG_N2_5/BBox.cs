#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class BBox : Objeto
  {
    private readonly Ponto _point;
    private readonly Circulo _innerCircle;
    private readonly Retangulo _rectangle;
    private readonly Circulo _outerCircle;

    private bool _isForaRetangulo = false;

    public BBox(Objeto paiRef) : base(paiRef)
    {
      PrimitivaTipo = PrimitiveType.Points;

      var centerX = 0.5;
      var centerY = 0.5;
      var center = new Ponto4D(centerX, centerY);
      var rectangleSize = 0.2;
      var outerCircleRadius = 0.28284271F;
      // var outerCircleRadius = (float)Math.Sqrt(rectangleSize * rectangleSize * 2);

      _point = new Ponto(null, center);
      _point.PrimitivaTamanho = 10;

      _innerCircle = new Circulo(null, center, 0.1F, 50);

      var rectangleDownLeft = new Ponto4D(centerX - rectangleSize, centerY - rectangleSize);
      var rectangleUpRight = new Ponto4D(centerX + rectangleSize, centerY + rectangleSize);
      _rectangle = new Retangulo(null, rectangleDownLeft, rectangleUpRight);
      _rectangle.PrimitivaTipo = PrimitiveType.LineLoop;

      _outerCircle = new Circulo(null, center, outerCircleRadius, 50);

      FilhoAdicionar(_point);
      FilhoAdicionar(_innerCircle);
      FilhoAdicionar(_rectangle);
      FilhoAdicionar(_outerCircle);
      Atualizar();
    }

    public void Move(Ponto4D ponto)
    {

      var oldIsForaRetangulo = _isForaRetangulo;
      _isForaRetangulo = _rectangle.IsOutside(_point.Point);

      if (oldIsForaRetangulo != _isForaRetangulo)
      {
        _rectangle.ChangeForm();
      }

      _point.Move(ponto);
      if (_outerCircle.IsOutside(_point.Point))
      {
        ponto.X = -ponto.X;
        ponto.Y = -ponto.Y;
        _point.Move(ponto);
        return;
      }

      _innerCircle.Move(ponto);

      pontosLista.Clear();
      Atualizar();
    }

    public void Atualizar()
    {
      base.ObjetoAtualizar();
    }
  }
}
