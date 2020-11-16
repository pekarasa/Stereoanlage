using PeKaRaSa.MusicControl.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PeKaRaSa.MusicControl
{
    public class CommandExecutor
    {
        private readonly IAudioUnitFactory _factory;
        private IAudioUnit _activeUnit;

        public CommandExecutor(IAudioUnitFactory factory)
        {
            _factory = factory;
            _activeUnit = _factory.GetDefaultUnit();
        }

        /// <summary>
        /// Executes the given command
        /// </summary>
        /// <param name="arguments"></param>
        public void Command(IEnumerable<string> arguments)
        {
            IAudioUnit oldUnit = _activeUnit;

            if (!arguments?.Any() ?? true)
            {
                Console.WriteLine("No command can be executed without arguments.");
                return;
            }

            string command = arguments.First();

            // Check whether a new component should be activated
            if ("changeUnit".Equals(command, StringComparison.CurrentCultureIgnoreCase))
            {
                string unitToActivate = arguments.Last();
                _activeUnit = _factory.GetActiveUnit(unitToActivate);

                // kill deactivated unit
                if (oldUnit != _activeUnit)
                {
                    oldUnit?.Kill();
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
                            Console.WriteLine("rewind {0} unknown", arguments.Skip(1).First());
                            Console.WriteLine("Maybe you meant 'rewind fastforward' or 'rewind rewind' or 'rewind next' or 'rewind previous'");
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
                            Console.WriteLine("control {0} unknown", arguments.Skip(1).First());
                            Console.WriteLine("Maybe you meant 'control play' or 'control pause' or 'control stop'");
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
                            Console.WriteLine("volume {0} unknown", arguments.Skip(1).First());
                            Console.WriteLine("Maybe you meant 'volume up' or 'volume down' or 'volume mute'");
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
                            Console.WriteLine("channel {0} unknown", arguments.Skip(1).First());
                            Console.WriteLine("Maybe you meant 'channel up' or 'channel down'");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("'{0}' is unknown command", command);
                    if (new List<string> { "up", "down" }.Contains(command))
                    {
                        Console.WriteLine("Maybe you meant 'volume up' or 'volume down'");
                    }
                    else if (new List<string> { "play", "pause", "stop" }.Contains(command))
                    {
                        Console.WriteLine("Maybe you meant 'control play' or 'control pause' or 'control stop'");
                    }
                    else if (new List<string> { "fastforward", "rewind", "next", "previous" }.Contains(command))
                    {
                        Console.WriteLine("Maybe you meant 'rewind fastforward' or 'rewind rewind' or 'rewind next' or 'rewind previous'");
                    }
                    else
                    {
                        Console.WriteLine("Try one of these commands: 'poweroff', 'track', 'disc', 'record', 'eject', 'rewind fastforward | rewind | next | previous', 'control play | pause | stop', 'volume up | down | mute', 'channel up | down'");
                    }

                    break;
            }
        }
    }
}
