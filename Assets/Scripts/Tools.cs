using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    // Each value in VectorToBeSubtracted goes closer to zero 
    // Examples:
    // Vector3(2,-3, 1) -> Vector3(2 - increment, -3 + increment, 1 - increment)
    public static Vector3 VectorGoToZero(Vector3 VectorToBeSubtracted, float increment)
    {
        if(VectorToBeSubtracted.x > 0)
        {
            if (VectorToBeSubtracted.y > 0)
            {
                if(VectorToBeSubtracted.z > 0)
                {
                    VectorToBeSubtracted += new Vector3(-increment, -increment, -increment);
                }
                else
                {
                    VectorToBeSubtracted += new Vector3(-increment, -increment, increment);
                }
            }
            else if (VectorToBeSubtracted.z > 0)
            {
                VectorToBeSubtracted += new Vector3(-increment, increment, -increment);
            }
            else
            {
                VectorToBeSubtracted += new Vector3(-increment, increment, increment);
            }
        }
        else
        {
            if(VectorToBeSubtracted.y > 0)
            {
                if (VectorToBeSubtracted.z > 0)
                {
                    VectorToBeSubtracted += new Vector3(increment, -increment, -increment);
                }
                else
                {
                    VectorToBeSubtracted += new Vector3(increment, -increment, increment);
                }
            }
            else if (VectorToBeSubtracted.z > 0)
            {
                VectorToBeSubtracted += new Vector3(increment, increment, -increment);
            }
            else
            {
                VectorToBeSubtracted += new Vector3(increment, increment, increment);
            }
        }

        return VectorToBeSubtracted;
    }

    /// <summary>
    /// Function returns a new vector that is 'increment' closer to VectorTarget than VectorToEdit
    /// Example: ( (1, 1, 1), (2, 0, 1), .1 ) -> (1.1, .9, 1)
    /// </summary>
    /// <param name="VectorToEdit">Vector to change value</param>
    /// <param name="VectorTarget">Target Vector</param>
    /// <param name="increment">number to added/subtracted (Keep this parem positive)</param>
    /// <returns>new Vector 3 with the edited values of VectorToEdit</returns>
    public static Vector3 VectorGoToDifferentVector(Vector3 VectorToEdit, Vector3 VectorTarget, float increment)
    {
        float x = VectorToEdit.x, y = VectorToEdit.y, z = VectorToEdit.z;
        if(VectorTarget.x > x)
            x += increment;
        else
            x -= increment;
        if (VectorTarget.y > y)
            y += increment;
        else
            y -= increment;
        if (VectorTarget.z > z)
            z += increment;
        else
            z -= increment;

        return new Vector3(x, y, z);
    }
}