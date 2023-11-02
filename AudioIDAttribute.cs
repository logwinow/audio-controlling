using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AudioControlling
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AudioIDAttribute : PropertyAttribute
    {
    }
}