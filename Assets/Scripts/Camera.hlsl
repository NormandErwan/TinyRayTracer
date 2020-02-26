#ifndef __CAMERA__
#define __CAMERA__

// Represents a camera.
struct Camera
{
    // The camera's background color.
    float4 BackgroundColor;

    // The forward vector of this camera.
    float3 Forward;

    // The camera's height when in orthographic mode.
    float OrthographicHeight;

    // The position of this camera.
    float3 Position;
};

#endif //__CAMERA__