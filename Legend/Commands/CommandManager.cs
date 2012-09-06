using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Legend.Models;
using Legend.Services;
using Legend.World;


namespace Legend.Commands
{
    public class CommandManager
    {
        private readonly string _clientId;
        private readonly Reference<Player> _playerReference;

        private readonly IWorld _world;
        private readonly INotificationService _notificationService;

        private static readonly Lazy<IList<ICommand>> _commands = new Lazy<IList<ICommand>>(GetCommands);
        private static Dictionary<string, ICommand> _commandCache;

        public CommandManager(string clientId, Reference<Player> playerReference, IWorld word, INotificationService notificationService)
        {
            _clientId = clientId;
            _playerReference = playerReference;
            _world = word;
            _notificationService = notificationService;
        }

        public bool TryHandleCommand(string command)
        {
            string[] args = command.Trim().Split(' ');
            string commandName = args[0];

            return TryHandleCommand(commandName, args.Skip(1).ToArray());
        }

        // Commands that require a user name
        private bool TryHandleCommand(string commandName, string[] args)
        {
            ICommand command;
            if (!TryMatchCommand(commandName, out command))
            {
                throw new InvalidOperationException(String.Format("'{0}' is not a valid command.", commandName));
            }

            var context = new CommandContext
            {
                CommandName = commandName,
                World = _world,
                NotificationService = _notificationService,
            };

            var callerContext = new CallerContext
            {
                ClientId = _clientId,
                PlayerReference = _playerReference
            };

            command.Execute(context, callerContext, args);

            return true;
        }

        private bool TryMatchCommand(string commandName, out ICommand command)
        {
            if (_commandCache == null)
            {
                var commands = from c in _commands.Value
                               let commandAttributes = c.GetType()
                                                       .GetCustomAttributes(true)
                                                       .OfType<CommandAttribute>()
                               where commandAttributes != null
                               select new
                               {
                                   CommandAttributes = commandAttributes,
                                   Command = c
                               };

                _commandCache = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
                foreach(var gameCommands in commands)
                {
                    foreach(var attribute in gameCommands.CommandAttributes)
                        _commandCache.Add(attribute.CommandName, gameCommands.Command);
                }
            }

            return _commandCache.TryGetValue(commandName, out command);
        }

        private static IList<ICommand> GetCommands()
        {
            // Use MEF to locate the content providers in this assembly
            var catalog = new AssemblyCatalog(typeof(CommandManager).Assembly);
            var compositionContainer = new CompositionContainer(catalog);
            return compositionContainer.GetExportedValues<ICommand>().ToList();
        }
    }
}