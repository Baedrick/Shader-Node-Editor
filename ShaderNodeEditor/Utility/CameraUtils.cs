using OpenTK.Mathematics;

namespace ShaderNodeEditor.Utility
{
	public static class CameraUtils
	{
		/// <summary>
		/// Creates a world space to view space matrix.
		/// </summary>
		/// <param name="eyePosition">Eye Position in World Space.</param>
		/// <param name="targetPosition">Target Position in World Space.</param>
		/// <param name="upVector">Up Direction in World Space.</param>
		/// <returns>Returns a Matrix4</returns>
		public static Matrix4 CreateLookAtMatrix(Vector3 eyePosition, Vector3 targetPosition, Vector3 upVector)
		{
			var forwardVector = Vector3.Normalize(eyePosition - targetPosition);
			var rightVector = Vector3.Normalize(Vector3.Cross(upVector, forwardVector));
			var newUpVector = Vector3.Normalize(Vector3.Cross(forwardVector, rightVector));
			
			Matrix4 viewMatrix = new(
				new Vector4(rightVector.X, newUpVector.X, forwardVector.X, 0.0f),
				new Vector4(rightVector.Y, newUpVector.Y, forwardVector.Y, 0.0f),
				new Vector4(rightVector.Z, newUpVector.Z, forwardVector.Z, 0.0f),
				new Vector4(
					-Vector3.Dot(rightVector, eyePosition), 
					-Vector3.Dot(newUpVector, eyePosition), 
					-Vector3.Dot(forwardVector, eyePosition), 1.0f)
			);
			
			return viewMatrix;
		}
		
		/// <summary>
		/// Creates a world space to view space matrix for an FPS camera.
		/// </summary>
		/// <param name="eyePosition">Eye Position in World Space.</param>
		/// <param name="pitch">Pitch must be in the range of [-90 to 90] degrees.</param>
		/// <param name="yaw">Yaw must be in the range of [0 to 360] degrees.</param>
		/// <returns>Returns a Matrix4</returns>
		public static Matrix4 CreateFpsViewMatrix(Vector3 eyePosition, float pitch, float yaw)
		{
			var pitchRadians = MathHelper.DegreesToRadians(pitch);
			var yawRadians = MathHelper.DegreesToRadians(yaw);
			
			var cosPitch = MathF.Cos(pitchRadians);
			var sinPitch = MathF.Sin(pitchRadians);
			var cosYaw = MathF.Cos(yawRadians);
			var sinYaw = MathF.Sin(yawRadians);
			
			var forwardVector = new Vector3(sinYaw * cosPitch, -sinPitch, cosPitch * cosYaw);
			var rightVector = new Vector3(cosYaw, 0.0f, -sinYaw);
			var upVector = new Vector3(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch);
			
			Matrix4 viewMatrix = new(
				new Vector4(rightVector.X, upVector.X, forwardVector.X, 0.0f),
				new Vector4(rightVector.Y, upVector.Y, forwardVector.Y, 0.0f),
				new Vector4(rightVector.Z, upVector.Z, forwardVector.Z, 0.0f),
				new Vector4(
					-Vector3.Dot(rightVector, eyePosition), 
					-Vector3.Dot(upVector, eyePosition), 
					-Vector3.Dot(forwardVector, eyePosition), 1.0f)
			);
			
			return viewMatrix;
		}

		public static Matrix4 CreateArcBallViewMatrix(Vector3 translateAmount, Quaternion rotateAmount)
		{
			var translationMatrix = Matrix4.CreateTranslation(-translateAmount);
			var rotationMatrix = Matrix4.CreateFromQuaternion(Quaternion.Invert(rotateAmount));
			var viewMatrix = translationMatrix * rotationMatrix;
			return viewMatrix;
		}
	}
}