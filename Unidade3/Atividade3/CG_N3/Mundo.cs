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
    private Objeto selectedObject = null;
    private Poligono selectedPolygon;
    private List<Poligono> polygons;

    public Mundo(
      GameWindowSettings gameWindowSettings,
      NativeWindowSettings nativeWindowSettings
    ) : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo = new Objeto(null, ref rotuloAtual);
      polygons = new List<Poligono>();
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
      polygons.Add(new Poligono(mundo, ref rotuloAtual, pontosPoligono));
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
            System.Console.WriteLine(selectedObject.ToString());
          }
          else
          {
            if (input.IsKeyPressed(Keys.M))
              selectedObject.MatrizImprimir();
            else
            {
              //TODO: não está atualizando a BBox com as transformações geométricas
              if (input.IsKeyPressed(Keys.I))
              {
                Console.WriteLine("aloha 2");
                selectedObject.MatrizAtribuirIdentidade();
              }
              else
              {
                if (input.IsKeyPressed(Keys.Left))
                {
                  Console.WriteLine("aloha");
                  selectedObject.MatrizTranslacaoXYZ(-0.05, 0, 0);
                }
                else
                {
                  if (input.IsKeyPressed(Keys.Right))
                    selectedObject.MatrizTranslacaoXYZ(0.05, 0, 0);
                  else
                  {
                    if (input.IsKeyPressed(Keys.Up))
                      selectedObject.MatrizTranslacaoXYZ(0, 0.05, 0);
                    else
                    {
                      if (input.IsKeyPressed(Keys.Down))
                        selectedObject.MatrizTranslacaoXYZ(0, -0.05, 0);
                      else
                      {
                        if (input.IsKeyPressed(Keys.PageUp))
                          selectedObject.MatrizEscalaXYZ(2, 2, 2);
                        else
                        {
                          if (input.IsKeyPressed(Keys.PageDown))
                            selectedObject.MatrizEscalaXYZ(0.5, 0.5, 0.5);
                          else
                          {
                            if (input.IsKeyPressed(Keys.Home))
                              selectedObject.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
                            else
                            {
                              if (input.IsKeyPressed(Keys.End))
                                selectedObject.MatrizEscalaXYZBBox(2, 2, 2);
                              else
                              {
                                if (input.IsKeyPressed(Keys.D1))
                                  selectedObject.MatrizRotacao(10);
                                else
                                {
                                  if (input.IsKeyPressed(Keys.D2))
                                    selectedObject.MatrizRotacao(-10);
                                  else
                                  {
                                    if (input.IsKeyPressed(Keys.D3))
                                      selectedObject.MatrizRotacaoZBBox(10);
                                    else
                                    {
                                      if (input.IsKeyPressed(Keys.D4))
                                        selectedObject.MatrizRotacaoZBBox(-10);
                                      else
                                      {
                                        if (input.IsKeyPressed(Keys.F11))
                                          if (this.WindowState == WindowState.Fullscreen)
                                            this.WindowState = WindowState.Normal;
                                          else
                                            this.WindowState = WindowState.Fullscreen;
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
      }
      #endregion

      HandlePolygonSelection();
      HandlePolygonCreation();
      HandleMovingVerticesOfSelection();
    }

    private void HandlePolygonSelection()
    {
      if (MouseState.IsButtonPressed(MouseButton.Right))
      {
        foreach (var polygon in polygons)
        {
          if (polygon.TrySelect(Size, MousePosition))
          {
            selectedPolygon?.Unselect();
            selectedPolygon = polygon;
            return;
          }
        }
        selectedPolygon = null;
      }
    }

    private void HandlePolygonCreation()
    {
      if (selectedPolygon == null)
      {
        var existsPendingPolygon = polygons.Count > 0 && !polygons[^1].IsComplete;
        if (MouseState.IsButtonPressed(MouseButton.Left))
        {
          if (!existsPendingPolygon)
          {
            polygons.Add(Poligono.StartDrawing(mundo, ref rotuloAtual, Size, MousePosition));
          }
          var newPolygon = polygons[^1];
          newPolygon.AddLine(Size, MousePosition);
        }
        if (existsPendingPolygon)
        {
          if (KeyboardState.IsKeyPressed(Keys.Enter))
          {
            polygons[^1].FinishLine();
          }
          polygons[^1].DragLine(Size, MousePosition);
        }
      }
    }

    private void HandleMovingVerticesOfSelection()
    {
      if (selectedPolygon != null)
      {
        if (MouseState.IsButtonPressed(MouseButton.Left))
        {
          selectedPolygon.TrySelectVertex(Size, MousePosition);
        }
        if (MouseState.IsButtonDown(MouseButton.Left))
        {
          selectedPolygon.TryDragVertex(Size, MousePosition);
        }
        if (MouseState.IsButtonReleased(MouseButton.Left))
        {
          selectedPolygon.ReleaseVertex();
        }
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
