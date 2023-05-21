#define CG_Debug


using CG_Biblioteca;
using OpenTK.Mathematics;

namespace gcgcg
{
  internal static class Utils
  {
    public static Ponto4D MouseToPoint(Vector2i windowSize, Vector2 mousePosition)
    {
      var x = 2 * (mousePosition.X / windowSize.X) - 1;
      var y = 2 * (-mousePosition.Y / windowSize.Y) + 1;
      return new Ponto4D(x, y);
    }
  }
}
