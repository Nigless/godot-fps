shader_type canvas_item;

uniform vec4 dot_color: source_color;
uniform float dot_radius : hint_range(0.0, 1.0);
uniform float dot_blur_radius : hint_range(0.0, 1.0);
uniform float shadow_spread_radius : hint_range(0.0, 1.0);
uniform float shadow_blur_radius : hint_range(0.0, 1.0);
uniform float hole_radius : hint_range(0.0, 1.0);

void fragment() {
    // Преобразуем координаты UV в диапазон от -1 до 1
    float dist = distance(UV, vec2(0.5,0.5));
    // Расчет радиуса дыры с учетом размытия
    float hole_radius_blurred = hole_radius - dot_blur_radius;



    // Расчет круга для центральной точки
    float circle = smoothstep(dot_radius, dot_radius - dot_blur_radius, dist)
        - smoothstep(hole_radius_blurred + dot_blur_radius, hole_radius_blurred, dist);

    // Расчет тени
    float shadow = smoothstep(dot_radius + shadow_spread_radius + shadow_blur_radius, dot_radius + shadow_spread_radius, dist)
        - smoothstep(hole_radius_blurred - shadow_spread_radius, hole_radius_blurred - shadow_spread_radius - shadow_blur_radius, dist);

    // Итоговый цвет пикселя с учетом круга и тени
    COLOR = vec4(dot_color.rgb * circle, dot_color.a * shadow);
}
