using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.Merging
{
    public interface IStopSubscription
    {
        /// <remarks>Can call many times, even if diposed. </remarks>
        bool StopSubscription();
        bool IsSubscribing { get; }
    }
}
