//https://github.com/mono/opentk/blob/main/Source/Examples/Shapes/Old/Cube.cs

#define CG_Debug
using CG_Biblioteca;
using System.Drawing;

namespace gcgcg
{
    internal class Cubo : Objeto
    {
        // private float[] vertices;
        // private int[] indices;
        // Vector3[] normals;
        // int[] colors;

        public Cubo(Objeto paiRef, ref char _rotulo) :
          this(paiRef, ref _rotulo, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5))
        { }

        public Cubo(Objeto paiRef, ref char _rotulo, Ponto4D ptoInfEsq, Ponto4D ptoSupDir) : base(paiRef, ref _rotulo)
        {
            var cubeVertices = new Ponto4D[]
            {
                new Ponto4D(-1.0f, -1.0f,  1.0f), // 0.0f, 0.0f,
                new Ponto4D(1.0f, -1.0f,  1.0f), // 1.0f, 0.0f,
                new Ponto4D(1.0f,  1.0f,  1.0f), // 1.0f, 1.0f,
                new Ponto4D(-1.0f,  1.0f,  1.0f), // 0.0f, 1.0f,
                new Ponto4D(-1.0f, -1.0f, -1.0f), // 0.0f, 0.0f,
                new Ponto4D(1.0f, -1.0f, -1.0f), // 1.0f, 0.0f,
                new Ponto4D(1.0f,  1.0f, -1.0f), // 1.0f, 1.0f,
                new Ponto4D(-1.0f,  1.0f, -1.0f), // 0.0f, 1.0f,
            };
            var cubeIndices = new uint[]
            {
                0, 1, 3, 1, 2, 3,
                1, 5, 2, 5, 6, 2,
                4, 5, 7, 5, 6, 7,
                0, 4, 3, 4, 7, 3,
                0, 1, 4, 1, 5, 4,
                3, 2, 7, 2, 6, 7,
            };
            foreach (var index in cubeIndices)
            {
                PontosAdicionar(cubeVertices[index]);
            }

            var textureVertices = new Ponto4D[]
            {
                new Ponto4D(0.0f, 0.0f),
                new Ponto4D(1.0f, 0.0f),
                new Ponto4D(0.0f, 1.0f),
                new Ponto4D(1.0f, 1.0f),
            };
            var textureIndices = new int[]
            {
                0, 1, 2, 1, 3, 2,
                0, 1, 2, 1, 3, 2,
                0, 1, 2, 1, 3, 2,
                0, 1, 2, 1, 3, 2,
                0, 1, 2, 1, 3, 2,
                0, 1, 2, 1, 3, 2,
            };
            foreach (var index in textureIndices)
            {
                AddTexturePoint(textureVertices[index]);
            }

            PrimitivaTipo = OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles;
            this.indices = cubeIndices;

            ObjetoAtualizar();
        }

        public static int ColorToRgba32(Color c)
        {
            return (int)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Cubo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
#endif

    }
}
