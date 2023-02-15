using OpenTK.Mathematics;
using ShaderNodeEditor.Utility;

namespace ShaderNodeEditor.Common
{
	public class ArcBallCamera
	{
		public Matrix4 ViewMatrix;
		
		private Vector3 position;
		private Vector3 forwardVector = -Vector3.UnitZ;
		private Vector3 upVector = Vector3.UnitY;
		private Vector3 rightVector = Vector3.UnitX;
		
		public ArcBallCamera(Vector3 position)
		{
			this.position = position;
			ViewMatrix = CameraUtils.CreateLookAtMatrix(position, new Vector3(0, 0, 0), Vector3.UnitY);
		}
		
		public void Rotate(Vector2 previousPosition, Vector2 currentPosition)
		{
			
		}
	}
}