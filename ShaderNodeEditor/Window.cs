using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ShaderNodeEditor.Common;

namespace ShaderNodeEditor
{
    public class Window : GameWindow
    {
        private readonly float[] mesh =
        {
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f,  // bottom right
            -0.5f, 0.5f, 0.0f, 0.0f, 1.0f,  // top left
            0.5f, 0.5f, 0.0f, 1.0f, 1.0f,   // top right
        };

        private readonly uint[] indices =
        {
            0, 1, 2,    // first triangle
            1, 2, 3     // second triangle
        };

        private Shader? shader;
        private Texture? texture0;
        private Texture? texture1;
        private int vertexArrayObject;
        private int elementBufferObject;

        public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) { }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);
            
            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Length * sizeof(float), mesh, BufferUsageHint.StaticDraw);

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            
            if (shader != null) {
                shader.Use();
                var vertexPosition = shader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexPosition);
                GL.VertexAttribPointer(vertexPosition, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                var texCoord = shader.GetAttribLocation("aTexCoord");
                GL.EnableVertexAttribArray(texCoord);
                GL.VertexAttribPointer(texCoord, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
                
                texture0 = Texture.LoadFromFile("Resources/wall.jpg");
                texture0.Use(TextureUnit.Texture0);
                shader.SetInt("texture0", 0);
                
                texture1 = Texture.LoadFromFile("Resources/leather.jpg");
                texture1.Use(TextureUnit.Texture1);
                shader.SetInt("texture1", 1);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindVertexArray(vertexArrayObject);
            
            texture0?.Use(TextureUnit.Texture0);
            texture1?.Use(TextureUnit.Texture1);
            shader?.Use();
            
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            
            Context.SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            shader?.Dispose();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}