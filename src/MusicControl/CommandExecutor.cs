using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeKaRaSa.MusicControl
{
    public class CommandExecutor
    {
        private readonly IAudioUnitFactory _factory;
        private IAudioUnit _activeUnit;
        private Dictionary<string, CancellationTokenSource> _startedTasks;

        public CommandExecutor(IAudioUnitFactory factory)
        {
            _factory = factory;
            _startedTasks = new Dictionary<string, CancellationTokenSource>();
            _activeUnit = _factory.GetDefaultUnit();
        }

        /// <summary>
        /// Executes the given command
        /// </summary>
        /// <param name="arguments"></param>
        public void Command(IEnumerable<string> arguments)
        {
            if (!arguments?.Any() ?? true)
            {
                Log.WriteLine("No command can be executed without arguments.");
                return;
            }

            string command = arguments.First();

            // Check whether a new component should be activated
            if ("changeUnit".Equals(command, StringComparison.CurrentCultureIgnoreCase))
            {
                Log.WriteLine($"current unit '{_activeUnit?.GetType().Name}'");
                string unitToActivate = arguments.Last();

                lock (_startedTasks)
                {

                    // Checks that no task is running for the audio unit to be activated or that it has already been cancelled
                    if (!_startedTasks.TryGetValue(unitToActivate, out CancellationTokenSource task) || task.IsCancellationRequested)
                    {
                        if (task != null)
                        {
                            // remove cancelled task
                            _startedTasks.Remove(unitToActivate);
                        }

                        // Tasks which are already running for other audio units will be aborted
                        foreach (KeyValuePair<string, CancellationTokenSource> taskToStop in _startedTasks)
                        {
                            taskToStop.Value.Cancel();
                        }

                        // a new task is started
                        CancellationTokenSource tokenSource = new CancellationTokenSource();
                        CancellationToken token = tokenSource.Token;
                        _startedTasks.Add(unitToActivate, tokenSource);

                        Task.Factory.StartNew(() =>
                        {
                            _activeUnit = _factory.GetActiveUnit(unitToActivate, _activeUnit, token);
                            Log.WriteLine($"new unit '{_activeUnit?.GetType().Name}'");
                            _startedTasks.Remove(_activeUnit?.GetType().Name);
                        }, token).ContinueWith((t) =>
                        {
                        // _activeUnit must be unchanged
                        Log.WriteLine($"unchanged unit '{_activeUnit?.GetType().Name}'");
                            _startedTasks.Remove(_activeUnit?.GetType().Name);
                        }, TaskContinuationOptions.OnlyOnCanceled);
                    }
                }

                return;
            }

            // Send command to active unit
            SendCommandToActiveUnit(arguments);
        }

        /// <summary>
        /// Interprets the first argument as a command and executes it with the additional arguments
        /// </summary>
        /// <param name="arguments"></param>
        private void SendCommandToActiveUnit(IEnumerable<string> arguments)
        {
            string command = arguments.First();

            Log.WriteLine($"send {command} to unit '{_activeUnit?.GetType().Name}'");

            switch (command)
            {
                case "poweroff":
                    _activeUnit.PowerOff();
                    break;
                case "track":
                    _activeUnit.Track(arguments.Skip(1).First());
                    break;
                case "disc":
                    _activeUnit.Disc(arguments.Skip(1).First());
                    break;
                case "record":
                    _activeUnit.Record();
                    break;
                case "eject":
                    _activeUnit.Eject();
                    break;
                case "rewind":
                    switch (arguments.Skip(1).First())
                    {
                        case "fastforward":
                            _activeUnit.FastForward();
                            break;
                        case "rewind":
                            _activeUnit.Rewind();
                            break;
                        case "next":
                            _activeUnit.Next();
                            break;
                        case "previous":
                            _activeUnit.Previous();
                            break;
                        default:
                            Log.WriteLine("rewind {0} unknown", arguments.Skip(1).First());
                            Log.WriteLine("Maybe you meant 'rewind fastforward' or 'rewind rewind' or 'rewind next' or 'rewind previous'");
                            break;
                    }
                    break;
                case "control":
                    switch (arguments.Skip(1).First())
                    {
                        case "play":
                            _activeUnit.Play();
                            break;
                        case "pause":
                            _activeUnit.Pause();
                            break;
                        case "stop":
                            _activeUnit.Stop();
                            break;
                        default:
                            Log.WriteLine("control {0} unknown", arguments.Skip(1).First());
                            Log.WriteLine("Maybe you meant 'control play' or 'control pause' or 'control stop'");
                            break;
                    }
                    break;
                case "volume":
                    switch (arguments.Skip(1).First())
                    {
                        case "up":
                            _activeUnit.VolumeUp();
                            break;
                        case "down":
                            _activeUnit.VolumeDown();
                            break;
                        case "mute":
                            _activeUnit.VolumeMute();
                            break;
                        default:
                            Log.WriteLine("volume {0} unknown", arguments.Skip(1).First());
                            Log.WriteLine("Maybe you meant 'volume up' or 'volume down' or 'volume mute'");
                            break;
                    }
                    break;
                case "channel":
                    switch (arguments.Skip(1).First())
                    {
                        case "up":
                            _activeUnit.ChannelUp();
                            break;
                        case "down":
                            _activeUnit.ChannelDown();
                            break;
                        default:
                            Log.WriteLine("channel {0} unknown", arguments.Skip(1).First());
                            Log.WriteLine("Maybe you meant 'channel up' or 'channel down'");
                            break;
                    }
                    break;
                default:
                    Log.WriteLine("'{0}' is unknown command", command);
                    if (new List<string> { "up", "down" }.Contains(command))
                    {
                        Log.WriteLine("Maybe you meant 'volume up' or 'volume down'");
                    }
                    else if (new List<string> { "play", "pause", "stop" }.Contains(command))
                    {
                        Log.WriteLine("Maybe you meant 'control play' or 'control pause' or 'control stop'");
                    }
                    else if (new List<string> { "fastforward", "rewind", "next", "previous" }.Contains(command))
                    {
                        Log.WriteLine("Maybe you meant 'rewind fastforward' or 'rewind rewind' or 'rewind next' or 'rewind previous'");
                    }
                    else
                    {
                        Log.WriteLine("Try one of these commands: 'poweroff', 'track', 'disc', 'record', 'eject', 'rewind fastforward | rewind | next | previous', 'control play | pause | stop', 'volume up | down | mute', 'channel up | down'");
                    }

                    break;
            }
        }
    }
}
