using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Buffering
{
    public interface IBufferingPolicy
    {
        bool ShouldBlockReads(bool isReadBlocked, TimeSpan durationBuffered, int bytesBuffered, bool isExhausted, bool isAllExhausted);

        bool IsDoneBuffering(TimeSpan bufferDuration, int bytesBuffered, int bytesBufferedWhenExhausted, bool isStarting);

        float GetProgress(TimeSpan bufferDuration, int bytesBuffered, int bytesBufferedWhenExhausted, bool isStarting);
    }
}
