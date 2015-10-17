using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalesFunkeln.Extensibility.Ui;

namespace FinalesFunkeln.Extensibility.Events
{
    class ViewRegisteredEventArgs
    {
        internal readonly IExtension Owner;
        internal readonly View View;

        internal ViewRegisteredEventArgs(IExtension owner, View view)
        {
            Owner = owner;
            View = view;
        }
    }
}
