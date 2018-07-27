using Grace.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Unofficial.Nancy.Bootstrappers.Grace
{
    public interface IGraceWrapper : IDisposable
    {
        IGraceWrapper Parent { get; }

        void Configure(Action<IExportRegistrationBlock> registrationAction);

        object Locate(Type type);

        T Locate<T>();

        List<T> LocateAll<T>();

        IGraceWrapper CreateScope();
    }
}
