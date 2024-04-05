﻿#version 330

in vec4 in_position;
in vec2 in_uv;
uniform mat4 model_matrix;
uniform mat4 vp_matrix;
out vec2 pass_uv;

void main(void){
    gl_Position = vp_matrix * model_matrix * in_position;
    pass_uv = in_uv;
}
