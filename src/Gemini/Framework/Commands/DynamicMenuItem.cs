using System;
using System.Collections.Generic;
using System.Linq;

namespace Gemini.Framework.Commands
{
    public abstract class DynamicMenuItem
    {
        public static DynamicMenuItem FromCommand(Command command)
        {
            return new DynamicCommandMenuItem(command);
        }

        public static DynamicMenuItem FromCommand(Command command, IEnumerable<DynamicMenuItem> children)
        {
            return new DynamicCommandMenuItem(command, children);
        }

        public static DynamicMenuItem Separator()
        {
            return DynamicSeparatorMenuItem.Instance;
        }
    }

    public sealed class DynamicCommandMenuItem : DynamicMenuItem
    {
        private readonly IReadOnlyList<DynamicMenuItem> _children;

        public DynamicCommandMenuItem(Command command)
            : this(command, Array.Empty<DynamicMenuItem>())
        {
        }

        public DynamicCommandMenuItem(Command command, IEnumerable<DynamicMenuItem> children)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            _children = (children ?? Array.Empty<DynamicMenuItem>()).ToArray();
        }

        public Command Command { get; }

        public IReadOnlyList<DynamicMenuItem> Children
        {
            get { return _children; }
        }
    }

    public sealed class DynamicSeparatorMenuItem : DynamicMenuItem
    {
        public static readonly DynamicSeparatorMenuItem Instance = new DynamicSeparatorMenuItem();

        private DynamicSeparatorMenuItem()
        {
        }
    }
}
