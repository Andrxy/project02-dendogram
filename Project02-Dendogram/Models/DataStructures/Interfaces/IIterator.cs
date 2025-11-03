using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project02_Dendogram.Models.DataStructures.Interfaces
{
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
        void Reset();
    }
}
