﻿using System;

namespace TestCanvas
{
    /// <summary>
    ///     This interface can be implemented by classes that
    ///     have a lat/long position.
    /// </summary>
    /// <remarks>
    ///     Author:     Daniel Saidi [daniel.saidi@gmail.com]
    ///     Link:       http://danielsaidi.github.com/nextra
    /// </remarks>
    public interface IPosition
    {
       
        double X { get; }
        double Y { get; }
    }
}