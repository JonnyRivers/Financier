﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Financier.Services
{
    public interface IDatabaseCreationService
    {
        void Create();
        void Obliterate();
    }
}
