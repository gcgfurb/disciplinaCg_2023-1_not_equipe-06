//https://github.com/mono/opentk/blob/main/Source/Examples/Shapes/Old/Cube.cs

#define CG_Debug
using CG_Biblioteca;

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
                new Ponto4D(-1.0f, -1.0f,  1.0f),
                new Ponto4D(1.0f, -1.0f,  1.0f),
                new Ponto4D(1.0f,  1.0f,  1.0f),
                new Ponto4D(-1.0f,  1.0f,  1.0f),
                new Ponto4D(-1.0f, -1.0f, -1.0f),
                new Ponto4D(1.0f, -1.0f, -1.0f),
                new Ponto4D(1.0f,  1.0f, -1.0f),
                new Ponto4D(-1.0f,  1.0f, -1.0f),
            };
            var cubeIndices = new uint[]
            {
                0, 1, 3, 1, 2, 3, // Front face
                1, 5, 2, 5, 6, 2, // Right face
                4, 5, 7, 5, 6, 7, // Back face
                0, 4, 3, 4, 7, 3, // Left face
                0, 1, 4, 1, 5, 4, // Bottom face
                3, 2, 7, 2, 6, 7, // Top face
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

            var normalVertices = new Ponto4D[]
            {
                new Ponto4D(0, 0, 1),
                new Ponto4D(1, 0, 0),
                new Ponto4D(0, 0, -1),
                new Ponto4D(-1, 0, 0),
                new Ponto4D(0, -1, 0),
                new Ponto4D(0, 1, 0),
            };
            var normalIndices = new int[]
            {
                0, 0, 0, 0, 0, 0, // Front face
                1, 1, 1, 1, 1, 1, // Right face
                2, 2, 2, 2, 2, 2, // Back face
                3, 3, 3, 3, 3, 3, // Left face
                4, 4, 4, 4, 4, 4, // Bottom face
                5, 5, 5, 5, 5, 5, // Top face
            };
            foreach (var index in normalIndices)
            {
                AddNormalPoint(normalVertices[index]);
            }

            PrimitivaTipo = OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles;
            this.indices = cubeIndices;

            ObjetoAtualizar();
        }

        public void SetLightMode(int lightMode)
        {
            this.lightMode = lightMode;
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
