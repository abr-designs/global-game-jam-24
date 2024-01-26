using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inputs
{
    public static InputActions input
    {
        get
        {
            if (_input != null)
                return _input;

            _input = new InputActions();

            return _input;
        }
    }
    private static InputActions _input;
}
