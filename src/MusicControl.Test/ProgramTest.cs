﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace PeKaRaSa.MusicControl.Test
{
    public class ProgramTest
    {
        [SetUp]
        public void Setup()
        {
            ConfigurationManager.AppSettings["MillisecondsToSleepWhenDriveIsNotReady"] = "10000";
            ConfigurationManager.AppSettings["MillisecondsToSleepWhenDriveIsOpen"] = "10000";

        }

        [Test]
        [NUnit.Framework.Ignore("setcd uses the wrong setcd.dummy")]
        [DeploymentItem(@"setcd.dummy")]
        public void WhenSendChangeUnitCdAndCdIsNotInsertedAndThenChangeUnitRadio_ThenActiveUnitShouldBeRadio()
        {
            try
            {
                using (Process process = new Process())
                {
                    // arrange
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "MusicControl";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.WorkingDirectory = @"D:\work\Stereoanlage\src\MusicControl\bin\Debug\netcoreapp3.1";
                    process.Start();

                    // act
                    Send("changeUnit cd");
                    Thread.Sleep(2000);
                    Send("changeUnit radio");
                    Thread.Sleep(1000);
                    Send("poweroff");
                    Send("shutdown");
                    process.WaitForExit();

                    // assert
                    var output = process.StandardOutput.ReadToEnd();
                    Console.Out.Write(output);
                    output.Should().Contain("new unit 'RadioUnit'");
                    output.Should().NotContain("current unit 'AudioCdUnit'");
                    output.Should().Contain("unchanged unit 'RadioUnit'");
                    output.Should().Contain("send poweroff to unit 'RadioUnit'");
                    
                }
            }
            catch (Exception e)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(e.Message);
            }
        }

        private void Send(string command)
        {
            using var tcpClient = new TcpClient("localhost", 13000);
            using var stream = tcpClient.GetStream();

            byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
            stream.Write(data, 0, data.Length);

            stream.Close();
            tcpClient.Close();
        }
    }
}
