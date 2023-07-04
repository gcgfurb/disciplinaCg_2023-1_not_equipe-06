﻿#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;

//FIXME: padrão Singleton

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        Objeto mundo;
        private char rotuloNovo = '?';
        private Objeto objetoSelecionado = null;

        private Cubo _cube = null;
        private float _cameraMovementSpeed = 5f;
        private float _cameraRotationSpeed = 500f;
        private float _cameraAngle = 90;

        private readonly float[] _sruEixos =
        {
            -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
            0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
            0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
        };

        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;

        private Shader _shaderBranca;
        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;
        private Shader _shaderCiano;
        private Shader _shaderMagenta;
        private Shader _shaderAmarela;

        private Camera _camera;

        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo = new Objeto(null, ref rotuloNovo);
        }

        private void Diretivas()
        {
#if DEBUG
            Console.WriteLine("Debug version");
#endif
#if RELEASE
    Console.WriteLine("Release version");
#endif
#if CG_Gizmo
            Console.WriteLine("#define CG_Gizmo  // debugar gráfico.");
#endif
#if CG_OpenGL
            Console.WriteLine("#define CG_OpenGL // render OpenGL.");
#endif
#if CG_DirectX
      Console.WriteLine("#define CG_DirectX // render DirectX.");
#endif
#if CG_Privado
      Console.WriteLine("#define CG_Privado // código do professor.");
#endif
            Console.WriteLine("__________________________________ \n");
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Diretivas();

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            GL.Enable(EnableCap.DepthTest);       // Ativar teste de profundidade
            // GL.Enable(EnableCap.CullFace);     // Desenha os dois lados da face
            // GL.FrontFace(FrontFaceDirection.Cw);
            // GL.CullFace(CullFaceMode.FrontAndBack);

            #region Cores
            _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
            _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            #endregion

            #region Eixos: SRU  
            _vertexBufferObject_sruEixos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
            GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
            _vertexArrayObject_sruEixos = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            #endregion

            _cube = new Cubo(mundo, ref rotuloNovo);
            _camera = new Camera(Vector3.UnitZ * 10, Size.X / (float)Size.Y);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            mundo.Desenhar(new Transformacao4D(), _camera);

#if CG_Gizmo
            Gizmo_Sru3D();
#endif
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
            #region Teclado
            var input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
                Close();
            if (input.IsKeyPressed(Keys.Space))
            {
                if (objetoSelecionado == null)
                    objetoSelecionado = mundo;
                objetoSelecionado.shaderCor = _shaderBranca;
                objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
                objetoSelecionado.shaderCor = _shaderAmarela;
            }
            if (input.IsKeyPressed(Keys.D0) || input.IsKeyPressed(Keys.KeyPad0))
            {
                _cube.SetLightMode(0);
            }
            if (input.IsKeyPressed(Keys.D1) || input.IsKeyPressed(Keys.KeyPad1))
            {
                _cube.SetLightMode(1);
            }
            if (input.IsKeyPressed(Keys.D2) || input.IsKeyPressed(Keys.KeyPad2))
            {
                _cube.SetLightMode(2);
            }
            if (input.IsKeyPressed(Keys.D3) || input.IsKeyPressed(Keys.KeyPad3))
            {
                _cube.SetLightMode(3);
            }
            if (input.IsKeyPressed(Keys.D4) || input.IsKeyPressed(Keys.KeyPad4))
            {
                _cube.SetLightMode(4);
            }
            if (input.IsKeyPressed(Keys.D5) || input.IsKeyPressed(Keys.KeyPad5))
            {
                _cube.SetLightMode(5);
            }
            if (input.IsKeyPressed(Keys.D6) || input.IsKeyPressed(Keys.KeyPad6))
            {
                _cube.SetLightMode(6);
            }

            if (input.IsKeyDown(Keys.Z))
                _camera.Position = Vector3.UnitZ;
            if (input.IsKeyDown(Keys.W))
                _camera.Position += _camera.Front * _cameraMovementSpeed * (float)e.Time; // Forward
            if (input.IsKeyDown(Keys.S))
                _camera.Position -= _camera.Front * _cameraMovementSpeed * (float)e.Time; // Backwards
            if (input.IsKeyDown(Keys.A))
            {
                _cameraAngle -= _cameraRotationSpeed * (float)e.Time;
                var cameraPosition = new Ponto4D(_camera.Position.X, _cube.Bbox().obterCentro.X, _camera.Position.Z);
                var radius = Matematica.distancia(_cube.Bbox().obterCentro, cameraPosition);
                var newCameraPosition = Matematica.GerarPtosCirculo(_cameraAngle, radius);
                _camera.Position = new Vector3((float)newCameraPosition.X, (float)_camera.Position.Y, (float)newCameraPosition.Y);

                _camera.Yaw -= _cameraRotationSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                _cameraAngle += _cameraRotationSpeed * (float)e.Time;
                var cameraPosition = new Ponto4D(_camera.Position.X, _cube.Bbox().obterCentro.X, _camera.Position.Z);
                var radius = Matematica.distancia(_cube.Bbox().obterCentro, cameraPosition);
                var newCameraPosition = Matematica.GerarPtosCirculo(_cameraAngle, radius);
                _camera.Position = new Vector3((float)newCameraPosition.X, (float)_camera.Position.Y, (float)newCameraPosition.Y);

                _camera.Yaw += _cameraRotationSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.RightShift))
                _camera.Position += _camera.Up * _cameraMovementSpeed * (float)e.Time; // Up
            if (input.IsKeyDown(Keys.LeftShift))
                _camera.Position -= _camera.Up * _cameraMovementSpeed * (float)e.Time; // Down

            #endregion

            #region  Mouse

            if (MouseState.IsButtonDown(MouseButton.Left) && MouseState.Delta.X < 0)
            {
                _cameraAngle -= _cameraRotationSpeed * (float)e.Time;
                var cameraPosition = new Ponto4D(_camera.Position.X, _cube.Bbox().obterCentro.X, _camera.Position.Z);
                var radius = Matematica.distancia(_cube.Bbox().obterCentro, cameraPosition);
                var newCameraPosition = Matematica.GerarPtosCirculo(_cameraAngle, radius);
                _camera.Position = new Vector3((float)newCameraPosition.X, (float)_camera.Position.Y, (float)newCameraPosition.Y);

                _camera.Yaw -= _cameraRotationSpeed * (float)e.Time;
            }
            if (MouseState.IsButtonDown(MouseButton.Left) && MouseState.Delta.X > 0)
            {
                _cameraAngle += _cameraRotationSpeed * (float)e.Time;
                var cameraPosition = new Ponto4D(_camera.Position.X, _cube.Bbox().obterCentro.X, _camera.Position.Z);
                var radius = Matematica.distancia(_cube.Bbox().obterCentro, cameraPosition);
                var newCameraPosition = Matematica.GerarPtosCirculo(_cameraAngle, radius);
                _camera.Position = new Vector3((float)newCameraPosition.X, (float)_camera.Position.Y, (float)newCameraPosition.Y);

                _camera.Yaw += _cameraRotationSpeed * (float)e.Time;
            }

            if (MouseState.IsButtonPressed(MouseButton.Left))
            {
                System.Console.WriteLine("MouseState.IsButtonPressed(MouseButton.Left)");
                System.Console.WriteLine("__ Valores do Espaço de Tela");
                System.Console.WriteLine("Vector2 mousePosition: " + MousePosition);
                System.Console.WriteLine("Vector2i windowSize: " + Size);
            }
            if (MouseState.IsButtonDown(MouseButton.Right) && objetoSelecionado != null)
            {
                System.Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");

                int janelaLargura = Size.X;
                int janelaAltura = Size.Y;
                Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
                Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

                objetoSelecionado.PontosAlterar(sruPonto, 0);
            }
            if (MouseState.IsButtonReleased(MouseButton.Right))
            {
                System.Console.WriteLine("MouseState.IsButtonReleased(MouseButton.Right)");
            }

            #endregion

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUnload()
        {
            mundo.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

            GL.DeleteProgram(_shaderBranca.Handle);
            GL.DeleteProgram(_shaderVermelha.Handle);
            GL.DeleteProgram(_shaderVerde.Handle);
            GL.DeleteProgram(_shaderAzul.Handle);
            GL.DeleteProgram(_shaderCiano.Handle);
            GL.DeleteProgram(_shaderMagenta.Handle);
            GL.DeleteProgram(_shaderAmarela.Handle);

            base.OnUnload();
        }

#if CG_Gizmo
        private void Gizmo_Sru3D()
        {
#if CG_OpenGL && !CG_DirectX
            var model = Matrix4.Identity;
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // EixoX
            _shaderVermelha.SetMatrix4("model", model);
            _shaderVermelha.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderVermelha.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.SetMatrix4("model", model);
            _shaderVerde.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderVerde.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
            _shaderAzul.SetMatrix4("model", model);
            _shaderAzul.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderAzul.SetMatrix4("projection", _camera.GetProjectionMatrix());
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