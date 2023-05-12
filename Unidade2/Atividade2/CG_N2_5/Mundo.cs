#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
//#define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System.Collections.Generic;

//FIXME: padrão Singleton

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private BBox _bbox;
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

      _bbox = new BBox(null);
      objetoNovo = _bbox;
      ObjetoNovo(objetoNovo); objetoNovo = null;
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

      var input = KeyboardState;
      if (input.IsKeyDown(Keys.Escape))
      {
        Close();
      }

      if (input.IsKeyDown(Keys.C) || input.IsKeyDown(Keys.W))
      {
        _bbox.Move(new Ponto4D(y: 0.01));
      }
      if (input.IsKeyDown(Keys.B) || input.IsKeyDown(Keys.S))
      {
        _bbox.Move(new Ponto4D(y: -0.01));
      }
      if (input.IsKeyDown(Keys.E) || input.IsKeyDown(Keys.A))
      {
        _bbox.Move(new Ponto4D(x: -0.01));
      }
      if (input.IsKeyDown(Keys.D) || input.IsKeyDown(Keys.D))
      {
        _bbox.Move(new Ponto4D(x: 0.01));
      }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload()
    {
      foreach (var objeto in objetosLista)
      {
        objeto.OnUnload();
      }

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);

      base.OnUnload();
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
