﻿#version 330

in vec2 pass_uv;
//texture_mode: 0-don't use texture, use fixed_color; 1-use texture; 2-use only alpha channel from texture and fixed_color for RGB
uniform int texture_mode;
uniform vec4 fixed_color;
uniform sampler2D textureDiffuse;
out vec4 fragColor;

void main(void){
	if (texture_mode == 1){
		vec4 tex = texture(textureDiffuse, pass_uv);
		fragColor = tex;
	}
	else if(texture_mode == 0){
		fragColor = fixed_color;
	}
	else{
		vec4 tex = texture(textureDiffuse, pass_uv);
		fragColor = vec4(fixed_color.x, fixed_color.y, fixed_color.z, tex.w);
	}
}
