using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildInc
{
    public interface IVersionChanger
    {
        bool CanChange { get; }
        void BuildIncrement();
        void ReleaseIncrement();
        void Save();
    }
}
