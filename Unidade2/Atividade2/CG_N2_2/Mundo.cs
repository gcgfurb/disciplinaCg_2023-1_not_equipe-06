#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
//#define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

//FIXME: padrão Singleton

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private int indiceSelecionado = 0;
    private List<Objeto> listaPoligonos = new List<Objeto>();
    private List<Objeto> objetosLista = new List<Objeto>();
    private Objeto objetoSelecionado = null;
    private char rotulo = '@';

    private readonly float[] _sruEixos =
    {
      -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f, /* Z+ */
    };

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderLaranja;

    private bool _firstMove = true;
    private Vector2 _lastPos;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
           : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    private void ObjetoNovo(Objeto objeto, Objeto objetoFilho = null)
    {
      if (objetoFilho == null)
      {
        objetosLista.Add(objeto);
        objeto.Rotulo = rotulo = Utilitario.charProximo(rotulo);
        objeto.ObjetoAtualizar();
        objetoSelecionado = objeto;
      }
      else
      {
        objeto.FilhoAdicionar(objetoFilho);
        objetoFilho.Rotulo = rotulo = Utilitario.charProximo(rotulo);
        objetoFilho.ObjetoAtualizar();
        objetoSelecionado = objetoFilho;
      }
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      // Eixos
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");

      Objeto objetoNovo = null;

      #region Objeto: points  
      objetoNovo = new Retangulo(null, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5));
      objetoNovo.PrimitivaTipo = PrimitiveType.Points;
      listaPoligonos.Add(objetoNovo);
      objetoNovo = null;
      #endregion

      #region Objeto: lines  
      objetoNovo = new Retangulo(null, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5));
      objetoNovo.PrimitivaTipo = PrimitiveType.Lines;
      listaPoligonos.Add(objetoNovo);
      objetoNovo = null;
      #endregion

      #region Objeto: line loop  
      objetoNovo = new Retangulo(null, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5));
      objetoNovo.PrimitivaTipo = PrimitiveType.LineLoop;
      listaPoligonos.Add(objetoNovo);
      objetoNovo = null;
      #endregion

      #region Objeto: line strip  
      objetoNovo = new Retangulo(null, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5));
      objetoNovo.PrimitivaTipo = PrimitiveType.LineStrip;
      listaPoligonos.Add(objetoNovo);
      objetoNovo = null;
      #endregion
      
      #region Objeto: triangles
      objetoNovo = new Retangulo(null, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5));
      objetoNovo.PrimitivaTipo = PrimitiveType.Triangles;
      listaPoligonos.Add(objetoNovo);
      objetoNovo = null;
      #endregion

      #region Objeto: triangle strip
      objetoNovo = new Retangulo(null, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5));
      objetoNovo.PrimitivaTipo = PrimitiveType.TriangleStrip;
      listaPoligonos.Add(objetoNovo);
      objetoNovo = null;
      #endregion


      #region Objeto: triangle fan   
      objetoNovo = new Retangulo(null, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5));
      objetoNovo.PrimitivaTipo = PrimitiveType.TriangleFan;
      listaPoligonos.Add(objetoNovo);
      objetoNovo = null;
      #endregion

      objetosLista.Add(listaPoligonos[indiceSelecionado]);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

#if CG_Gizmo      
      Sru3D();
#endif
      for (var i = 0; i < objetosLista.Count; i++)
        objetosLista[i].Desenhar();

      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      #region Teclado
      var input = KeyboardState;
      if (input.IsKeyDown(Keys.Escape))
      {
        Close();
      }
      else
      {
        if (input.IsKeyPressed(Keys.Space))
        {
          indiceSelecionado = (indiceSelecionado + 1) % listaPoligonos.Count;
          objetosLista.Clear();
          objetosLista.Add(listaPoligonos[indiceSelecionado]);
        }
      }
      #endregion
    }

#if CG_Gizmo
    private void Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

  }
}
