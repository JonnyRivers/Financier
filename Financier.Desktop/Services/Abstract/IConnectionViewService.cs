using Financier.Services;
using System;
using System.Collections.Generic;

namespace Financier.Desktop.Services
{
    public interface IConnectionViewService
    {
        IConnection OpenConnectionView();
    }
}
