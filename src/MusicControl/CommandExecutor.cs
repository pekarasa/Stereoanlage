using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Units;
using System.Collections.Generic;
using System.Linq;
using PeKaRaSa.MusicControl.Services.Players;

namespace PeKaRaSa.MusicControl
{
    public class CommandExecutor
    {
        private readonly IAudioUnit _activeUnit;

        public CommandExecutor(IAudioUnit activeUnit)
        {
            _activeUnit = new RadioUnit(new MusicPlayerClient(), new PlaylistService(new FileAccess(AppSettings.GetValueOrDefault("PathToPlaylists", "/home/pi/mpd/playlists"))));
            _activeUnit = activeUnit;

            // Start playing my desired station
            _activeUnit.Disc("1");
            _activeUnit.Track("4");
            _activeUnit.VolumeUp();
            _activeUnit.Play();
        }

        /// <summary>
        /// Executes the given command
        /// </summary>
        /// <param name="arguments"></param>
        public void Command(string[] arguments)
        {
            if (!arguments?.Any() ?? true)
            {
                Log.WriteLine("No command can be executed without arguments.");
                return;
            }

            // Send command to active unit
            SendCommandToActiveUnit(arguments);
        }

        /// <summary>
        /// Interprets the first argument as a command and executes it with the additional arguments
        /// </summary>
        /// <param name="arguments"></param>
        private void SendCommandToActiveUnit(string[] arguments)
        {
            string command = arguments.First();

            Log.WriteLine($"send {command} to unit '{_activeUnit?.GetType().Name}'");

            switch (command)
            {
                case "poweroff":
                    _activeUnit?.PowerOff();
                    break;
                case "track":
                    _activeUnit?.Track(arguments.Skip(1).First());
                    break;
                case "disc":
                    _activeUnit?.Disc(arguments.Skip(1).First());
                    break;
                case "record":
                    _activeUnit?.Record();
                    break;
                case "eject":
                    _activeUnit?.Eject();
                    break;
                case "rewind":
                    switch (arguments.Skip(1).First())
                    {
                        case "fastforward":
                            _activeUnit?.FastForward();
                            break;
                        case "rewind":
                            _activeUnit?.Rewind();
                            break;
                        case "next":
                            _activeUnit?.Next();
                            break;
                        case "previous":
                            _activeUnit?.Previous();
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
                            _activeUnit?.Play();
                            break;
                        case "pause":
                            _activeUnit?.Pause();
                            break;
                        case "stop":
                            _activeUnit?.Stop();
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
                            _activeUnit?.VolumeUp();
                            break;
                        case "down":
                            _activeUnit?.VolumeDown();
                            break;
                        case "mute":
                            _activeUnit?.VolumeMute();
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
                            _activeUnit?.ChannelUp();
                            break;
                        case "down":
                            _activeUnit?.ChannelDown();
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
