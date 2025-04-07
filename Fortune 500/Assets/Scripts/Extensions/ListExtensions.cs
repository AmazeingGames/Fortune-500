using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class ListExtensions
{
    public static T SelectRandomElement<T>(this List<T> list)
    {
        Assert.IsNotNull(list);
        Assert.IsTrue(list.Count > 0);

        int randomElement = UnityEngine.Random.Range(0, list.Count - 1);
        return list[randomElement];
    }

}
