﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.MediaManager
{
    public interface IMediaManagerParameters
    {
        Action<IProgramStreams> ProgramStreamsHandler { get; set; }
    }
}
