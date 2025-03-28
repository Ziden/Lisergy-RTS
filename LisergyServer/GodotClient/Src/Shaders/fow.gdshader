shader_type canvas_item;

uniform sampler2D visibility_texture;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, filter_nearest;
uniform vec4 fog_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform vec2 map_size;
uniform float transition_smoothness : hint_range(0.01, 1.0) = 0.3;
uniform vec2 map_offset = vec2(0.0, 0.0);

void fragment() {
    // Get screen color first
    vec4 screen_color = texture(SCREEN_TEXTURE, SCREEN_UV);

    // Simple fallback - just show a debug pattern to verify shader is working
    // COLOR = vec4(mod(SCREEN_UV.x * 10.0, 1.0), mod(SCREEN_UV.y * 10.0, 1.0), 0.0, 1.0);
    // return;

    // Get depth
    float depth = texture(DEPTH_TEXTURE, SCREEN_UV).r;

    // If depth is at max, this is background - don't apply fog
    if (depth >= 0.99) {
        COLOR = screen_color;
        return;
    }

    // Convert screen position to world position (simplified for debugging)
    // This is a simplified mapping just to get something working
    vec2 world_pos = SCREEN_UV * map_size;

    // Map to visibility texture coordinates
    vec2 vis_uv = world_pos / map_size;

    // Ensure we're within bounds
    if (vis_uv.x < 0.0 || vis_uv.x > 1.0 || vis_uv.y < 0.0 || vis_uv.y > 1.0) {
        COLOR = fog_color; // Out of bounds is fog
        return;
    }

    // Sample visibility
    float visibility = texture(visibility_texture, vis_uv).r;

    // Direct debug output - uncomment to see visibility texture
    // COLOR = vec4(visibility, visibility, visibility, 1.0);
    // return;

    // Apply fog factor with smooth transition
    float fog_factor = smoothstep(0.0, transition_smoothness, visibility);

    // Mix between fog and screen color based on visibility
    COLOR = mix(fog_color, screen_color, fog_factor);
}
