using UnityEngine;

/// <summary>
/// Utility class for storing attack types and movement types as integers. 
/// Also contains a method for updating aim target's position to be at certain distance between aim target's old position and given origin.
/// </summary>
public class CombatUtilities
{
    public const int LightPunch = 0;
    public const int HeavyPunch = 1;
    public const int LightKick = 2;
    public const int HeavyKick = 3;  
    public const int Forward = 4;
    public const int Backward = 5;
    public const int Down = 6;

    /// <summary>
    /// Returns position in 3D space that is between origin and target at given distance.
    /// </summary>
    /// <param name="origin">Represents starting position for calculation, usually it's the character itself.</param>
    /// <param name="target">Represents position of the an object.</param>
    /// <param name="distance">How far from origin should new position be.</param>
    public static Vector3 MoveInfrontOf(Vector3 origin, Vector3 target, float distance) {
        Vector3 normalizedVector = (origin - target).normalized;
        return origin - normalizedVector * distance;
    }
}
