using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TestUtil
{
    public static IEnumerator Generator(Func<object> next, Func<bool> done)
    {
        var _done = false;
        while (!_done)
        {
            yield return next();
            _done = done();
        }
    }
}
