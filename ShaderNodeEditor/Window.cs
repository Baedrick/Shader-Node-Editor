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
		private readonly float[] meshVertices = Cube.MESH;
		private readonly Vector3[] meshPositions = Cube.MESH_POSITIONS;
		
		private Shader? shader;
		private Texture? texture0;
		private int vertexArrayObject;
		private int vertexBufferObject;
		
		private Matrix4 viewMatrix;
		private Matrix4 projectionMatrix;
		
		public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) { }
		
		protected override void OnLoad()
		{
			base.OnLoad();
			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			
			GL.Enable(EnableCap.DepthTest);
			
			vertexBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, meshVertices.Length * sizeof(float), meshVertices, BufferUsageHint.StaticDraw);
			
			shader = new Shader("Shaders/default.vert", "Shaders/default.frag");
			shader.Use();

			vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(vertexArrayObject);
			
			var vertexPosition = shader.GetAttribLocation("aPosition");
			GL.EnableVertexAttribArray(vertexPosition);
			GL.VertexAttribPointer(vertexPosition, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
			
			var texCoord = shader.GetAttribLocation("aTexCoord");
			GL.EnableVertexAttribArray(texCoord);
			GL.VertexAttribPointer(texCoord, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
			
			texture0 = Texture.LoadFromFile("Resources/wall.jpg");
			texture0.Use(TextureUnit.Texture0);
			shader.SetInt("texture0", 0);
		}
		
		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			texture0?.Use(TextureUnit.Texture0);
			shader?.Use();
			
			viewMatrix = CameraUtils.CreateFpsViewMatrix(new Vector3(0.0f, 0.0f, -3.0f), 0.0f, 180.0f);
			projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float) Size.Y, 0.1f, 100.0f);
			
			for (var i = 0; i < meshPositions.Length; ++i) {
				var modelMatrix = Matrix4.CreateTranslation(meshPositions[i]);
				var angle = 20.0f * i;
				modelMatrix = modelMatrix * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
				shader?.SetMatrix4("model", modelMatrix);
				
				GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
			}
			
			shader?.SetMatrix4("view", viewMatrix);
			shader?.SetMatrix4("projection", projectionMatrix);

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