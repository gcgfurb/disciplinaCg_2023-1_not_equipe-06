using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using CG_Biblioteca;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private Objeto mundo;
    private char rotuloAtual = '?';
    private Objeto objetoSelecionado = null;
    private Poligono polygon;

    public Mundo(
      GameWindowSettings gameWindowSettings,
      NativeWindowSettings nativeWindowSettings
    ) : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo = new Objeto(null, ref rotuloAtual);
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      new Axis(mundo, ref rotuloAtual);

      List<Ponto4D> pontosPoligono = new List<Ponto4D>();
      pontosPoligono.Add(new Ponto4D(0.25, 0.25));
      pontosPoligono.Add(new Ponto4D(0.75, 0.25));
      pontosPoligono.Add(new Ponto4D(0.75, 0.75));
      pontosPoligono.Add(new Ponto4D(0.50, 0.50));
      pontosPoligono.Add(new Ponto4D(0.25, 0.75));
      polygon = new Poligono(mundo, ref rotuloAtual, pontosPoligono);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      mundo.Desenhar();
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
        if (input.IsKeyPressed(Keys.G))
        {
          mundo.GrafocenaImprimir("ssss");
        }
        else
        {
          if (input.IsKeyPressed(Keys.P))
          {
            System.Console.WriteLine(objetoSelecionado.ToString());
          }
          else
          {
            if (input.IsKeyPressed(Keys.M))
              objetoSelecionado.MatrizImprimir();
            else
            {
              //TODO: não está atualizando a BBox com as transformações geométricas
              if (input.IsKeyPressed(Keys.I))
              {
                Console.WriteLine("aloha 2");
                objetoSelecionado.MatrizAtribuirIdentidade();
              }
              else
              {
                if (input.IsKeyPressed(Keys.Left))
                {
                  Console.WriteLine("aloha");
                  objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
                }
                else
                {
                  if (input.IsKeyPressed(Keys.Right))
                    objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
                  else
                  {
                    if (input.IsKeyPressed(Keys.Up))
                      objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
                    else
                    {
                      if (input.IsKeyPressed(Keys.Down))
                        objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
                      else
                      {
                        if (input.IsKeyPressed(Keys.PageUp))
                          objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
                        else
                        {
                          if (input.IsKeyPressed(Keys.PageDown))
                            objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
                          else
                          {
                            if (input.IsKeyPressed(Keys.Home))
                              objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
                            else
                            {
                              if (input.IsKeyPressed(Keys.End))
                                objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);
                              else
                              {
                                if (input.IsKeyPressed(Keys.D1))
                                  objetoSelecionado.MatrizRotacao(10);
                                else
                                {
                                  if (input.IsKeyPressed(Keys.D2))
                                    objetoSelecionado.MatrizRotacao(-10);
                                  else
                                  {
                                    if (input.IsKeyPressed(Keys.D3))
                                      objetoSelecionado.MatrizRotacaoZBBox(10);
                                    else
                                    {
                                      if (input.IsKeyPressed(Keys.D4))
                                        objetoSelecionado.MatrizRotacaoZBBox(-10);
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      #endregion

      if (MouseState.IsButtonPressed(MouseButton.Left))
      {
        polygon.OnClick(Size, MousePosition);
      }
      if (MouseState.IsButtonDown(MouseButton.Left))
      {
        polygon.OnDrag(Size, MousePosition);
      }
      if (MouseState.IsButtonReleased(MouseButton.Left))
      {
        polygon.OnUnclick();
      }

      if (MouseState.IsButtonPressed(MouseButton.Right))
      {
        polygon.OnRightClick(Size, MousePosition);
      }
    }


    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      base.OnUnload();
    }
  }
}
