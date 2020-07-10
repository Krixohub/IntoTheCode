using System;
using System.Windows.Input;

namespace IntoTheCodeExample.Basic
{
  /// <summary>Implement commands as bindable properties.</summary>
  public class DelegateCommand : ICommand
  {
    /// <summary>Has the user permission to execute.</summary>
    private bool _permissionToExecute = true;

    /// <summary>Can command execute.</summary>
    private readonly Func<CommandInformation, bool> _canExecuteAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand"/> class. 
    /// </summary>
    /// <param name="executeAction">The action to execute the command.</param>
    /// <param name="canExecuteAction">The method to determine if the command can be executed.</param>
    public DelegateCommand(Action<CommandInformation> executeAction, Func<CommandInformation, bool> canExecuteAction)
    {
      ExecuteAction = executeAction;
      _canExecuteAction = canExecuteAction;
    }

    /// <summary>Constructor when always executeable.</summary>
    /// <param name="executeAction">The action to execute the command.</param>
    public DelegateCommand(Action<CommandInformation> executeAction)
      : this(executeAction, null)
    {
    }

    /// <summary>Event when CanExecute changes.</summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>User has permission to execute.</summary>
    public bool PermissionToExecute
    {
      get { return _permissionToExecute; }
      set
      {
        if (_permissionToExecute == value) return;
        _permissionToExecute = value;
        if (CanExecuteChanged != null) CanExecuteChanged(this, new EventArgs());
      }
    }

    /// <summary>Action to execute when command is called.</summary>
    protected Action<CommandInformation> ExecuteAction { get; set; }

    /// <summary>Execute command.</summary>
    /// <param name="parameter">Data used by the command. Can be set to null.</param>
    public virtual void Execute(object parameter)
    {
      CommandInformation info = ParmToInfoConverter(parameter);
      if (CanExecute(info))
        ExecuteAction(info);
    }

    /// <summary>Used by xaml binding to determine if command can be executed.</summary>
    /// <param name="parameter">Data used by the command. Can be set to null.</param>
    /// <returns>True if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(object parameter)
    {
      if (!_permissionToExecute) return false;
      if (_canExecuteAction != null && !_canExecuteAction(ParmToInfoConverter(parameter))) return false;
      if (ExecuteAction == null) return false;
      return true;
    }

    /// <summary>Converts an object to CommandInformation.</summary>
    /// <param name="parameter">The object, may be a CommandInformation.</param>
    /// <returns>A CommandInformation.</returns>
    private CommandInformation ParmToInfoConverter(object parameter)
    {
      if (parameter != null && parameter is CommandInformation) return (CommandInformation)parameter;
      return new CommandInformation() { Parameter = parameter };
    }
  }
}
