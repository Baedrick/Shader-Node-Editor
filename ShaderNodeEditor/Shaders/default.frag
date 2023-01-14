#version 450 core

out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main()
{
    FragColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), smoothstep(0.4, 0.6, texCoord.s));
}