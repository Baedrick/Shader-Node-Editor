using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ShaderNodeEditor.Common;
using ShaderNodeEditor.Utility;

namespace ShaderNodeEditor
{
	public class Window : GameWindow
	{
		private static readonly DebugProc DEBUG_MESSAGE_DELEGATE = OnDebugMessage;
		
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
		private int vertexBufferObject;
		private int elementBufferObject;
		
		private double time;
		
		private Matrix4 modelMatrix;
		private Matrix4 viewMatrix;
		private Matrix4 projectionMatrix;

		private ArcBallCamera camera;
		
		public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) { }
		
		protected override void OnLoad()
		{
			base.OnLoad();
			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			
			GL.Enable(EnableCap.DepthTest);

			camera = new ArcBallCamera();
			
			vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(vertexArrayObject);
			
			vertexBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, mesh.Length * sizeof(float), mesh, BufferUsageHint.StaticDraw);
			
			elementBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
			
			shader = new Shader("Shaders/default.vert", "Shaders/default.frag");
			shader.Use();
			
			//viewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
			projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float) Size.Y, 0.1f, 100.0f);
			
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
		
		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);
			
			time += 4.0 * args.Time;
			
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.BindVertexArray(vertexArrayObject);
			
			texture0?.Use(TextureUnit.Texture0);
			texture1?.Use(TextureUnit.Texture1);
			shader?.Use();
			
			modelMatrix = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(time));
			viewMatrix = CameraUtils.CreateFpsViewMatrix(new Vector3(0.0f, 0.0f, -3.0f), 0.0f, 180.0f);

			shader?.SetMatrix4("model", modelMatrix);
			shader?.SetMatrix4("view", viewMatrix);
			shader?.SetMatrix4("projection", projectionMatrix);
			
			GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
			
			// Debug Callback
			GL.DebugMessageCallback(DEBUG_MESSAGE_DELEGATE, IntPtr.Zero);
			GL.Enable(EnableCap.DebugOutput);
			
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
		
		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			base.OnMouseMove(e);
			var previousPosition = new Vector2(e.Position.X - e.DeltaX, e.Position.Y - e.DeltaY);
			var currentPosition = new Vector2(e.Position.X, e.Position.Y);

			camera.Rotate(previousPosition, currentPosition);
		}
		
		private static void OnDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr pMessage, IntPtr pUserParam)
		{
			var message = Marshal.PtrToStringUTF8(pMessage, length);
			Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);
			if (type == DebugType.DebugTypeError) {
				throw new Exception(message);
			}
		}
	}
}