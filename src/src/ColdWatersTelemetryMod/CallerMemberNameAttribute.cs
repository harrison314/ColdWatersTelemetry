using System;
using System.Collections.Generic;
using System.Text;

namespace System.Runtime.CompilerServices
{
#if NET35
    [System.AttributeUsage(System.AttributeTargets.Parameter, Inherited = false)]
    internal sealed class CallerMemberNameAttribute : Attribute
    {
        public CallerMemberNameAttribute()
        {
            
        }
    }
#endif
}
