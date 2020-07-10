using System;

namespace IntoTheCodeExample.Basic
{
    /// <summary>A action/command (on a ViewModel). Used with EventToCommandMapper and DelegateEventCommand.</summary>
    public class CommandInformation 
  {
    /// <summary>Gets or sets the sender.</summary>
    public object Sender { get; set; }

    /// <summary>Gets or sets the event args.</summary>
    public EventArgs EventArgs { get; set; }

    /// <summary>Gets or sets the command argument.</summary>
    public object Parameter { get; set; }
  }
}
