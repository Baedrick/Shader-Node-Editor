using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ShaderNodeEditor
{
    public class Shader : IDisposable
    {
        private readonly int handle;

        public Shader(string vertexPath, string fragmentPath)
        {
            var vertexShaderSource = File.ReadAllText(vertexPath);
            var fragmentShaderSource = File.ReadAllText(fragmentPath);

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            
            CreateShader(vertexShader);
            CreateShader(fragmentShader);

            handle = GL.CreateProgram();
            
            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);
            
            GL.LinkProgram(handle);
            
            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out var linkSuccess);
            if (linkSuccess == 0)
            {
                var infoLog = GL.GetProgramInfoLog(handle);
                Console.WriteLine(infoLog);
            }
            
            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
        }

        private static void CreateShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var vertSuccess);
            if (vertSuccess != 0) {
                return;
            }
            var infoLog = GL.GetShaderInfoLog(shader);
            Console.WriteLine(infoLog);
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                return;
            }
            GL.DeleteProgram(handle);
            disposedValue = true;
        }

        ~Shader()
        {
            GL.DeleteProgram(handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}