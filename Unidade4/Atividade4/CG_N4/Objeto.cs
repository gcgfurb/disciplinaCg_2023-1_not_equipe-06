#define CG_OpenGL
#define CG_Debug
// #define CG_DirectX

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace gcgcg
{
    internal class Objeto
    {
        private readonly char rotulo;
        protected Objeto paiRef;
        private List<Objeto> objetosLista = new List<Objeto>();
        private PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
        public PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
        private float primitivaTamanho = 1;
        public float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }

        private Texture _texture = Texture.LoadFromFile("Assets/grass.png");
        // private Shader _basicLightingShader = new Shader("Shaders/shader.vert", "Shaders/shaderBasicLighting.frag");
        private Shader _lightingMapShader = new Shader("Shaders/shader.vert", "Shaders/shaderLightingMap.frag");
        private Shader _directionalLightShader = new Shader("Shaders/shader.vert", "Shaders/shaderDirectionalLight.frag");
        private Shader _pointLightsShader = new Shader("Shaders/shader.vert", "Shaders/shaderPointLights.frag");
        private Shader _spotlightShader = new Shader("Shaders/shader.vert", "Shaders/shaderSpotlight.frag");
        private Shader _multipleLightsShader = new Shader("Shaders/shader.vert", "Shaders/shaderMultipleLights.frag");
        private Shader _shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderTexture.frag");
        public Shader shaderCor { set => _shaderObjeto = value; }

        protected int lightMode = 0;

        protected List<Ponto4D> pontosLista = new List<Ponto4D>();
        protected List<Ponto4D> texturePoints = new List<Ponto4D>();
        protected List<Ponto4D> normalPoints = new List<Ponto4D>();
        protected uint[] indices = new uint[0];

        private int _vertexBufferObject;
        private int _textureBufferObject;
        private int _normalBufferObject;
        private int _elementBufferObject;
        private int _vertexArrayObject;
        private int _lampArrayObject;

        private BBox bBox = new BBox();
        public BBox Bbox()
        {
            return bBox;
        }

        private Transformacao4D matriz = new Transformacao4D();
        private static Transformacao4D matrizTmpTranslacao = new Transformacao4D();
        private static Transformacao4D matrizTmpTranslacaoInversa = new Transformacao4D();
        private static Transformacao4D matrizTmpEscala = new Transformacao4D();
        private static Transformacao4D matrizTmpRotacao = new Transformacao4D();
        private static Transformacao4D matrizGlobal = new Transformacao4D();
        private char eixoRotacao = 'z';

        public Objeto(Objeto paiRef, ref char _rotulo, Objeto objetoFilho = null)
        {
            this.paiRef = paiRef;
            rotulo = _rotulo = Utilitario.CharProximo(_rotulo);
            if (paiRef != null)
            {
                ObjetoAdicionar(objetoFilho);
            }
        }

        private void ObjetoAdicionar(Objeto objetoFilho)
        {
            if (objetoFilho == null)
            {
                paiRef.objetosLista.Add(this);
            }
            else
            {
                paiRef.FilhoAdicionar(objetoFilho);
            }
        }

        public void ObjetoAtualizar()
        {
            float[] vertices = new float[pontosLista.Count * 3];
            int ptoLista = 0;
            for (int i = 0; i < vertices.Length; i += 3)
            {
                vertices[i] = (float)pontosLista[ptoLista].X;
                vertices[i + 1] = (float)pontosLista[ptoLista].Y;
                vertices[i + 2] = (float)pontosLista[ptoLista].Z;
                ptoLista++;
            }

            float[] textureVertices = new float[texturePoints.Count * 2];
            var texturePointIndex = 0;
            for (int i = 0; i < textureVertices.Length; i += 2)
            {
                textureVertices[i] = (float)texturePoints[texturePointIndex].X;
                textureVertices[i + 1] = (float)texturePoints[texturePointIndex].Y;
                texturePointIndex++;
            }

            float[] normalVertices = new float[normalPoints.Count * 3];
            var normalPointsIndex = 0;
            for (int i = 0; i < normalVertices.Length; i += 3)
            {
                normalVertices[i] = (float)normalPoints[normalPointsIndex].X;
                normalVertices[i + 1] = (float)normalPoints[normalPointsIndex].Y;
                normalVertices[i + 2] = (float)normalPoints[normalPointsIndex].Z;
                normalPointsIndex++;
            }

            GL.PointSize(primitivaTamanho);

            _shaderObjeto.Use();

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _textureBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _textureBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, textureVertices.Length * sizeof(float), textureVertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);

            _normalBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _normalBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, normalVertices.Length * sizeof(float), normalVertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(2);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            _texture.Use(TextureUnit.Texture0);
        }

        public void Desenhar(Transformacao4D matrizGrafo, Camera _camera)
        {
            GL.PointSize(primitivaTamanho);

            GL.BindVertexArray(_vertexArrayObject);

            if (paiRef != null)
            {
                var lightPos = new Vector3(2.0f, 2.0f, 2.0f);

                _texture.Use(TextureUnit.Texture0);

                if (lightMode == 1)
                {
                    matrizGrafo = BasicLighting(matrizGrafo, _camera, lightPos);
                }
                else if (lightMode == 2)
                {
                    matrizGrafo = LightingMaps(matrizGrafo, _camera, lightPos);
                }
                else if (lightMode == 3)
                {
                    matrizGrafo = DirectionalLight(matrizGrafo, _camera, lightPos);
                }
                else if (lightMode == 4)
                {
                    matrizGrafo = PointLights(matrizGrafo, _camera, lightPos);
                }
                else if (lightMode == 5)
                {
                    matrizGrafo = Spotlight(matrizGrafo, _camera, lightPos);
                }
                else if (lightMode == 6)
                {
                    matrizGrafo = MultipleLights(
                        matrizGrafo,
                        _camera,
                        new List<Vector3>
                        {
                            new Vector3(2.0f, 0.0f, 0.0f),
                            new Vector3(0.0f, 2.0f, 0.0f),
                            new Vector3(0.0f, 0.0f, -2.0f),
                            new Vector3(2.0f, 2.0f, 2.0f),
                        });
                }
                else
                {
                    matrizGrafo = NoLight(matrizGrafo, _camera, lightPos);
                }

            }
            for (var i = 0; i < objetosLista.Count; i++)
            {
                objetosLista[i].Desenhar(matrizGrafo, _camera);
            }
        }

        private Transformacao4D NoLight(Transformacao4D matrizGrafo, Camera _camera, Vector3 lightPos)
        {
            _shaderObjeto.Use();

            matrizGrafo = matrizGrafo.MultiplicarMatriz(matriz);

            _shaderObjeto.SetMatrix4("model", matrizGrafo.ObterDadosOpenTK());
            _shaderObjeto.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderObjeto.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.DrawArrays(primitivaTipo, 0, pontosLista.Count);

            GL.BindVertexArray(_lampArrayObject);
            return matrizGrafo;
        }

        private Transformacao4D BasicLighting(Transformacao4D matrizGrafo, Camera _camera, Vector3 lightPos)
        {
            // _basicLightingShader.Use();

            // matrizGrafo = matrizGrafo.MultiplicarMatriz(matriz);

            // _basicLightingShader.SetMatrix4("model", matrizGrafo.ObterDadosOpenTK());
            // _basicLightingShader.SetMatrix4("view", _camera.GetViewMatrix());
            // _basicLightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            // _basicLightingShader.SetVector3("lightColor", new Vector3(1.0f, 2.0f, 1.0f));
            // _basicLightingShader.SetVector3("lightPos", lightPos);
            // _basicLightingShader.SetVector3("viewPos", _camera.Position);

            // GL.DrawArrays(primitivaTipo, 0, pontosLista.Count);

            // GL.BindVertexArray(_lampArrayObject);
            return matrizGrafo;
        }

        private Transformacao4D LightingMaps(Transformacao4D matrizGrafo, Camera _camera, Vector3 lightPos)
        {
            _lightingMapShader.Use();

            matrizGrafo = matrizGrafo.MultiplicarMatriz(matriz);

            _lightingMapShader.SetMatrix4("model", matrizGrafo.ObterDadosOpenTK());
            _lightingMapShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightingMapShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _lightingMapShader.SetVector3("viewPos", _camera.Position);

            _lightingMapShader.SetInt("material.diffuse", 0);
            _lightingMapShader.SetInt("material.specular", 1);
            _lightingMapShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _lightingMapShader.SetFloat("material.shininess", 32.0f);

            _lightingMapShader.SetVector3("light.position", lightPos);
            _lightingMapShader.SetVector3("light.ambient", new Vector3(0.8f));
            _lightingMapShader.SetVector3("light.diffuse", new Vector3(0.5f));
            _lightingMapShader.SetVector3("light.specular", new Vector3(1.0f));

            GL.DrawArrays(primitivaTipo, 0, pontosLista.Count);

            GL.BindVertexArray(_lampArrayObject);
            return matrizGrafo;
        }

        private Transformacao4D DirectionalLight(Transformacao4D matrizGrafo, Camera _camera, Vector3 lightDirection)
        {
            _directionalLightShader.Use();

            matrizGrafo = matrizGrafo.MultiplicarMatriz(matriz);

            _directionalLightShader.SetMatrix4("model", matrizGrafo.ObterDadosOpenTK());
            _directionalLightShader.SetMatrix4("view", _camera.GetViewMatrix());
            _directionalLightShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _directionalLightShader.SetVector3("viewPos", _camera.Position);

            _directionalLightShader.SetInt("material.diffuse", 0);
            _directionalLightShader.SetInt("material.specular", 1);
            _directionalLightShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _directionalLightShader.SetFloat("material.shininess", 32.0f);

            _directionalLightShader.SetVector3("light.direction", lightDirection);
            _directionalLightShader.SetVector3("light.ambient", new Vector3(0.2f));
            _directionalLightShader.SetVector3("light.diffuse", new Vector3(3.0f));
            _directionalLightShader.SetVector3("light.specular", new Vector3(0.0f));

            GL.DrawArrays(primitivaTipo, 0, pontosLista.Count);

            GL.BindVertexArray(_lampArrayObject);
            return matrizGrafo;
        }

        private Transformacao4D PointLights(Transformacao4D matrizGrafo, Camera _camera, Vector3 lightPos)
        {
            _pointLightsShader.Use();

            matrizGrafo = matrizGrafo.MultiplicarMatriz(matriz);

            _pointLightsShader.SetMatrix4("model", matrizGrafo.ObterDadosOpenTK());
            _pointLightsShader.SetMatrix4("view", _camera.GetViewMatrix());
            _pointLightsShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _pointLightsShader.SetVector3("viewPos", _camera.Position);

            _pointLightsShader.SetInt("material.diffuse", 0);
            _pointLightsShader.SetInt("material.specular", 1);
            _pointLightsShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _pointLightsShader.SetFloat("material.shininess", 32.0f);

            _pointLightsShader.SetVector3("light.position", lightPos);
            _pointLightsShader.SetFloat("light.constant", 1.0f);
            _pointLightsShader.SetFloat("light.linear", 0.09f);
            _pointLightsShader.SetFloat("light.quadratic", 0.032f);
            _pointLightsShader.SetVector3("light.ambient", new Vector3(0.2f));
            _pointLightsShader.SetVector3("light.diffuse", new Vector3(0.5f));
            _pointLightsShader.SetVector3("light.specular", new Vector3(1.0f));

            GL.DrawArrays(primitivaTipo, 0, pontosLista.Count);

            GL.BindVertexArray(_lampArrayObject);
            return matrizGrafo;
        }

        private Transformacao4D Spotlight(Transformacao4D matrizGrafo, Camera _camera, Vector3 lightPos)
        {
            _spotlightShader.Use();

            matrizGrafo = matrizGrafo.MultiplicarMatriz(matriz);

            _spotlightShader.SetMatrix4("model", matrizGrafo.ObterDadosOpenTK());
            _spotlightShader.SetMatrix4("view", _camera.GetViewMatrix());
            _spotlightShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _spotlightShader.SetVector3("viewPos", _camera.Position);

            _spotlightShader.SetInt("material.diffuse", 0);
            _spotlightShader.SetInt("material.specular", 1);
            _spotlightShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _spotlightShader.SetFloat("material.shininess", 32.0f);

            _spotlightShader.SetVector3("light.position", _camera.Position);
            _spotlightShader.SetVector3("light.direction", _camera.Front);
            _spotlightShader.SetFloat("light.cutOff", MathF.Cos(MathHelper.DegreesToRadians(0)));
            _spotlightShader.SetFloat("light.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(10)));
            _spotlightShader.SetFloat("light.constant", 1.0f);
            _spotlightShader.SetFloat("light.linear", 0.09f);
            _spotlightShader.SetFloat("light.quadratic", 0.032f);
            _spotlightShader.SetVector3("light.ambient", new Vector3(0.2f));
            _spotlightShader.SetVector3("light.diffuse", new Vector3(5f));
            _spotlightShader.SetVector3("light.specular", new Vector3(1.0f));

            GL.DrawArrays(primitivaTipo, 0, pontosLista.Count);

            GL.BindVertexArray(_lampArrayObject);
            return matrizGrafo;
        }

        private Transformacao4D MultipleLights(Transformacao4D matrizGrafo, Camera _camera, List<Vector3> pointLightPositions)
        {
            _multipleLightsShader.Use();

            matrizGrafo = matrizGrafo.MultiplicarMatriz(matriz);

            _multipleLightsShader.SetMatrix4("model", matrizGrafo.ObterDadosOpenTK());
            _multipleLightsShader.SetMatrix4("view", _camera.GetViewMatrix());
            _multipleLightsShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _multipleLightsShader.SetVector3("viewPos", _camera.Position);

            _multipleLightsShader.SetInt("material.diffuse", 0);
            _multipleLightsShader.SetInt("material.specular", 1);
            _multipleLightsShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _multipleLightsShader.SetFloat("material.shininess", 32.0f);

            // Directional light
            _multipleLightsShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
            _multipleLightsShader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
            _multipleLightsShader.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
            _multipleLightsShader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

            // Point lights
            for (int i = 0; i < pointLightPositions.Count; i++)
            {
                _multipleLightsShader.SetVector3($"pointLights[{i}].position", pointLightPositions[i]);
                _multipleLightsShader.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                _multipleLightsShader.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                _multipleLightsShader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                _multipleLightsShader.SetFloat($"pointLights[{i}].constant", 1.0f);
                _multipleLightsShader.SetFloat($"pointLights[{i}].linear", 0.09f);
                _multipleLightsShader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
            }

            // Spot light
            _multipleLightsShader.SetVector3("spotLight.position", _camera.Position);
            _multipleLightsShader.SetVector3("spotLight.direction", _camera.Front);
            _multipleLightsShader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
            _multipleLightsShader.SetVector3("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            _multipleLightsShader.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
            _multipleLightsShader.SetFloat("spotLight.constant", 1.0f);
            _multipleLightsShader.SetFloat("spotLight.linear", 0.09f);
            _multipleLightsShader.SetFloat("spotLight.quadratic", 0.032f);
            _multipleLightsShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(0)));
            _multipleLightsShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(10f)));

            GL.DrawArrays(primitivaTipo, 0, pontosLista.Count);

            GL.BindVertexArray(_lampArrayObject);
            return matrizGrafo;
        }

        #region Objeto: CRUD

        public void FilhoAdicionar(Objeto filho)
        {
            this.objetosLista.Add(filho);
        }

        public Ponto4D PontosId(int id)
        {
            return pontosLista[id];
        }

        public void PontosAdicionar(Ponto4D pto)
        {
            pontosLista.Add(pto);
        }

        public void AddTexturePoint(Ponto4D point)
        {
            texturePoints.Add(point);
        }

        public void AddNormalPoint(Ponto4D point)
        {
            normalPoints.Add(point);
        }

        public void PontosAlterar(Ponto4D pto, int posicao)
        {
            pontosLista[posicao] = pto;
            ObjetoAtualizar();
        }

        #endregion

        #region Objeto: Grafo de Cena

        public Objeto GrafocenaBusca(char _rotulo)
        {
            if (rotulo == _rotulo)
            {
                return this;
            }
            foreach (var objeto in objetosLista)
            {
                var obj = objeto.GrafocenaBusca(_rotulo);
                if (obj != null)
                {
                    return obj;
                }
            }
            return null;
        }

        public Objeto GrafocenaBuscaProximo(Objeto objetoAtual)
        {
            objetoAtual = GrafocenaBusca(Utilitario.CharProximo(objetoAtual.rotulo));
            if (objetoAtual != null)
            {
                return objetoAtual;
            }
            else
            {
                return GrafocenaBusca(Utilitario.CharProximo('@'));
            }
        }

        public void GrafocenaImprimir(String idt)
        {
            System.Console.WriteLine(idt + rotulo);
            foreach (var objeto in objetosLista)
            {
                objeto.GrafocenaImprimir(idt + "  ");
            }
        }

        #endregion

        #region Objeto: Transformações Geométricas

        public void MatrizImprimir()
        {
            System.Console.WriteLine(matriz);
        }
        public void MatrizAtribuirIdentidade()
        {
            matriz.AtribuirIdentidade();
            ObjetoAtualizar();
        }
        public void MatrizTranslacaoXYZ(double tx, double ty, double tz)
        {
            Transformacao4D matrizTranslate = new Transformacao4D();
            matrizTranslate.AtribuirTranslacao(tx, ty, tz);
            matriz = matrizTranslate.MultiplicarMatriz(matriz);
            ObjetoAtualizar();
        }
        public void MatrizEscalaXYZ(double Sx, double Sy, double Sz)
        {
            Transformacao4D matrizScale = new Transformacao4D();
            matrizScale.AtribuirEscala(Sx, Sy, Sz);
            matriz = matrizScale.MultiplicarMatriz(matriz);
            ObjetoAtualizar();
        }

        public void MatrizEscalaXYZBBox(double Sx, double Sy, double Sz)
        {
            matrizGlobal.AtribuirIdentidade();
            Ponto4D pontoPivo = bBox.obterCentro;

            matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
            matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

            matrizTmpEscala.AtribuirEscala(Sx, Sy, Sz);
            matrizGlobal = matrizTmpEscala.MultiplicarMatriz(matrizGlobal);

            matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
            matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

            matriz = matriz.MultiplicarMatriz(matrizGlobal);

            ObjetoAtualizar();
        }
        public void MatrizRotacaoEixo(double angulo)
        {
            switch (eixoRotacao)  // TODO: ainda não uso no exemplo
            {
                case 'x':
                    matrizTmpRotacao.AtribuirRotacaoX(Transformacao4D.DEG_TO_RAD * angulo);
                    break;
                case 'y':
                    matrizTmpRotacao.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
                    break;
                case 'z':
                    matrizTmpRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
                    break;
                default:
                    Console.WriteLine("opção de eixoRotacao: ERRADA!");
                    break;
            }
            ObjetoAtualizar();
        }
        public void MatrizRotacao(double angulo)
        {
            MatrizRotacaoEixo(angulo);
            matriz = matrizTmpRotacao.MultiplicarMatriz(matriz);
            ObjetoAtualizar();
        }
        public void MatrizRotacaoZBBox(double angulo)
        {
            matrizGlobal.AtribuirIdentidade();
            Ponto4D pontoPivo = bBox.obterCentro;

            matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
            matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

            MatrizRotacaoEixo(angulo);
            matrizGlobal = matrizTmpRotacao.MultiplicarMatriz(matrizGlobal);

            matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
            matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

            matriz = matriz.MultiplicarMatriz(matrizGlobal);

            ObjetoAtualizar();
        }

        #endregion

        public void OnUnload()
        {
            foreach (var objeto in objetosLista)
            {
                objeto.OnUnload();
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shaderObjeto.Handle);
        }

#if CG_Debug
        protected string ImprimeToString()
        {
            string retorno;
            retorno = "__ Objeto: " + rotulo + "\n";
            for (var i = 0; i < pontosLista.Count; i++)
            {
                retorno += "P" + i + "[ " +
                string.Format("{0,10}", pontosLista[i].X) + " | " +
                string.Format("{0,10}", pontosLista[i].Y) + " | " +
                string.Format("{0,10}", pontosLista[i].Z) + " | " +
                string.Format("{0,10}", pontosLista[i].W) + " ]" + "\n";
            }
            retorno += bBox.ToString();
            return (retorno);
        }
#endif

    }
}