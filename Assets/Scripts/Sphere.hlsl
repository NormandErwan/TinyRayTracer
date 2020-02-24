#ifndef __SPHERE__
#define __SPHERE__

#include "Assets/Scripts/Material.hlsl"

// Represents a 3D sphere.
struct Sphere
{
    // The position of the center of this sphere.
    float3 Center;

    // The material of this sphere.
    Material Material;

    // The radius of this sphere.
    float Radius;
};

#endif //__SPHERE__