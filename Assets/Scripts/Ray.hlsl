#ifndef __RAY__
#define __RAY__

// Represents a ray.
struct Ray
{
    // The origin of this ray.
    float3 Origin;

    // The direction of this ray.
    float3 Direction;
};

// Project a position into a Ray.
bool Project(in Ray ray, in float3 position, out float distance)
{
    distance = dot(ray.Direction, position);

    bool isOnRay = distance >= 0;
    return isOnRay;
}

// Get a position along a Ray.
float3 GetPoint(in Ray ray, float distance)
{
    return ray.Origin + distance * ray.Direction;
}

#endif //__RAY__