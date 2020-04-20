using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JUtils
{
    static public EDirection GetDir(Vector3 target_pos, Vector3 cur_pos)
    {
        Vector3 dir = target_pos - cur_pos;
        EDirection e_dir;
        if (Mathf.Abs(dir.y) < Mathf.Abs(dir.x))
        {
            if (dir.x > 0) e_dir = EDirection.Right;
            else e_dir = EDirection.Left;
        }
        else
        {
            if (dir.y > 0) e_dir = EDirection.UP;
            else e_dir = EDirection.Down;
        }

        return e_dir;
    }
}
