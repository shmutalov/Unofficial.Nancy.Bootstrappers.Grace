﻿using Grace.DependencyInjection;

namespace Unofficial.Nancy.Bootstrappers.Grace.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class GraceExt
    {
        internal static bool IsChild(this IInjectionScope container) => container.Parent != null;
    }
}
