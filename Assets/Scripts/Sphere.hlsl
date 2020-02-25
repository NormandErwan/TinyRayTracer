#ifndef __SPHERE__
#define __SPHERE__

#include "Assets/Scripts/Material.hlsl"
#include "Assets/Scripts/Ray.hlsl"

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

bool Intersect(in Ray ray, in Sphere sphere, out float hitDistance, out float3 hitPoint)
{
    bool isOnRay = Project(ray, sphere.Center, hitDistance);
    if (!isOnRay)
    {
        return false;
    }

    hitPoint = GetPoint(ray, hitDistance);

    float centerHitDistance = distance(sphere.Center, hitPoint);
    bool intersect = centerHitDistance <= sphere.Radius;
    return intersect;
}

#endif //__SPHERE__