using System;
using System.ComponentModel;
using System.Linq.Expressions;

using IntoTheCode.Basic.Util;

namespace IntoTheCode.Basic.Layer
{
    /// <summary>Baseclass to implement INotifyPropertyChanged.</summary>
    public class NotifyChanges : INotifyPropertyChanged
    {
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Raise PropertyChanged when a property is changed.</summary>
        /// <param name="exp">Lambda expression. Feks ()=>PropertyName.</param>
        protected void RaisePropertyChanged(Expression<Func<object>> exp)
        {
            RaisePropertyChanged(DotNetUtil.GetMemberName(exp));
        }

        /// <summary>Raise PropertyChanged with string.</summary>
        /// <param name="name">Name of changed property.</param>
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
