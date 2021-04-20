using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    public Vector3 IsObjectOutOfLimits(int xLimit, int yLimit, GameObject targetObj)
    {
        float xPosition = targetObj.transform.position.x;
        float yPosition = targetObj.transform.position.y;
        int direction;
        char axis;
        if ((xPosition > xLimit || xPosition < -xLimit) && (yPosition > yLimit || yPosition < -yLimit))
        {
            direction = (xPosition > xLimit) ? 1 : -1;
            float newXPosition = LimitBoundaries('x', direction, xLimit, yLimit, targetObj).x;
            direction = (yPosition > yLimit) ? 1 : -1;
            float newYPosition = LimitBoundaries('y', direction, xLimit, yLimit, targetObj).y;
            return new Vector3(newXPosition, newYPosition);
        }
        else if (xPosition > xLimit || xPosition < -xLimit)
        {
            axis = 'x'; // x axis
            direction = (xPosition > xLimit) ? 1 : -1; // -1 if player is at the negative side
            return LimitBoundaries(axis, direction, xLimit, yLimit, targetObj);
        }
        else if (yPosition > yLimit || yPosition < -yLimit)
        {
            axis = 'y'; // y axis
            direction = (yPosition > yLimit) ? 1 : -1;
            return LimitBoundaries(axis, direction, xLimit, yLimit, targetObj);
        }
        else
            return new Vector3(0, 0, 0);
    }

    public Vector3 LimitBoundaries(char axis, int direction, int xLimit, int yLimit, GameObject targetObj)
    {
        Vector2 playerPosition = targetObj.transform.position;
        if (direction == 1)
        {
            switch (axis)
            {
                case 'x':
                    playerPosition.x = xLimit;
                    break;

                case 'y':
                    playerPosition.y = yLimit;
                    break;
            }
        }
        else
        {
            switch (axis)
            {
                case 'x':
                    playerPosition.x = -xLimit;
                    break;

                case 'y':
                    playerPosition.y = -yLimit;
                    break;
            }
        }
        return playerPosition;
    }
}