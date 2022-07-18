// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using UnityEngine;

public class CombatUtilities
{
    public const int LightPunch = 0;
    public const int HeavyPunch = 1;
    public const int LightKick = 2;
    public const int HeavyKick = 3;  
    public const int Forward = 4;
    public const int Backward = 5;
    public const int Down = 6;

    //Get new position in front of "origin", on line between "target" and "origin" and "distance" away from "origin". 
    public static Vector3 MoveInfrontOf(Vector3 origin, Vector3 target, float distance) {
        Vector3 normalizedVector = (origin - target).normalized;
        return origin - normalizedVector * distance;
    }
}
