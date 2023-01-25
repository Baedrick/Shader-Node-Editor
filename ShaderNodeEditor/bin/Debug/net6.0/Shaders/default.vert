#version 450 core
in vec3 aPosition;
in vec2 aTexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoord;

void main()
{
	texCoord = aTexCoord;
	gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}