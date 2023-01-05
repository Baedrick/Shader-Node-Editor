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
            
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var vertSuccess);
            if (vertSuccess == 0)
            {
                var infoLog = GL.GetShaderInfoLog(vertexShader);
                Console.WriteLine(infoLog);
            }
            
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out var fragSuccess);
            if (fragSuccess == 0)
            {
                var infoLog = GL.GetShaderInfoLog(fragmentShader);
                Console.WriteLine(infoLog);
            }

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