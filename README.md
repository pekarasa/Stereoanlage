# Stereoanlage

I run a compact music center consisting of a Raspberry Pi and an AMP2 as audio amplifier. Additionally there is a button to start the system. The music center is operated with an infrared remote control.

The aim of this guide is to help you set up the software for your music center as quickly as possible.

[![Build Status](https://dev.azure.com/pekarasa/Stereoanlage/_apis/build/status/pekarasa.Stereoanlage?branchName=master)](https://dev.azure.com/pekarasa/Stereoanlage/_build/latest?definitionId=2&branchName=master)

## Playable media

Currently the music center can play the following media:

- Streams from the Internet (web radio)
- Local music (mp3)
- Audio CDs
- Serves as Bluetooth speaker
- AirPlay, Spotify Connect, UPnP and Snapcast

## Software in use

To play the above listed media I use MPD, vlc and Raspberry Pi Audio Receiver

## Resources

In this project I have collected the knowledge from different sources. Namely these:

- [1] [Raspberry Pi: Per IR Remote Befehle ausführen](https://tutorials-raspberrypi.de/raspberry-pi-ir-remote-control/)
- [2] [How to add a power button to your Raspberry Pi](https://howchoo.com/g/mwnlytk3zmm/how-to-add-a-power-button-to-your-raspberry-pi)
- [3] [Installing operating system images](https://www.raspberrypi.org/documentation/installation/installing-images/README.md)
- [4] [Getting lirc to work with Raspberry Pi 3 (Raspbian Stretch)](https://gist.github.com/prasanthj/c15a5298eb682bde34961c322c95378b)
- [5] [The comprehensive GPIO Pinout guide for the Raspberry Pi.](https://pinout.xyz/)
- [6] [Pulse Audio Configuration for Raspberry Pi Audio Systems](https://gist.github.com/Gadgetoid/3301cec3e47495e75b31d3120d8f17d9)
- [7] [Playing CDs directly from MPD](https://forum.volumio.org/playing-cds-directly-from-mpd-t2411.html)
- [8] [New LIRC Name Space](http://arnaud.quette.free.fr/lirc/lirc-nns.html)
- [9] [GPIO usage of HiFiBerry boards](https://www.hifiberry.com/build/documentation/gpio-usage-of-hifiberry-boards/)
- [10] [Device Trees, overlays, and parameters](https://www.raspberrypi.org/documentation/configuration/device-tree.md)
- [11] [VLC Command line](https://wiki.videolan.org/Documentation:Command_line/#Modules_selection)
- [12] [VLC HowTo/Use with lirc](https://wiki.videolan.org/VLC_HowTo/Use_with_lirc/)
- [13] [Bluetooth - Troubleshooting](https://wiki.archlinux.org/index.php/Bluetooth#Troubleshooting)
- [14] [HiFiBerry Amp2](https://www.hifiberry.com/shop/boards/hifiberry-amp2/)
- [15] [Easy Setup IR Remote Control Using LIRC for the Raspberry PI (RPi) - July 2019](https://www.instructables.com/id/Setup-IR-Remote-Control-Using-LIRC-for-the-Raspber/)
- [16] [Raspberry Pi Audio Receiver](https://github.com/nicokaiser/rpi-audio-receiver)
- [17] [Irrecord Button Listing](https://www.ocinside.de/modding_en/linux_ir_irrecord_list/)
- [18] [Install and use Microsoft Dot NET 5 with the Raspberry Pi](https://www.petecodes.co.uk/install-and-use-microsoft-dot-net-5-with-the-raspberry-pi/)
- [19] [.NET Core-RID-Katalog](https://docs.microsoft.com/de-de/dotnet/core/rid-catalog)
- [20] [Permanently Setting System-Wide PATH for all Users](https://stackabuse.com/how-to-permanently-set-path-in-linux/)
- [21] [,Net Core Manual install](https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu#manual-install)
- [22] [Download .NET 5.](https://dotnet.microsoft.com/download/dotnet/5.0)

## Setup of the infrared remote control VLR-RC001

Press "SET" and the device button at the same time, for example “TV1”, the LED will light up.
Type in the according code number 0026, after the last digit the LED will turn off.

## Hardware Extensions

I connect an infrared diode which serves as a receiver. This is connected as described under [[15]](https://www.instructables.com/id/Setup-IR-Remote-Control-Using-LIRC-for-the-Raspber/). But I will use the gpio_in_pin 22 and not the described gpio_in_pin 17.

The button is connected as described under [[2]](https://howchoo.com/g/mwnlytk3zmm/how-to-add-a-power-button-to-your-raspberry-pi).

As audio card and amplifier I use the hifiberry AMP2. See [[14]](https://www.hifiberry.com/shop/boards/hifiberry-amp2/).

## Setup Raspberry Pi

## Install Raspberry Pi OS using Raspberry Pi Imager

1. Download and install rpi-imager from [Raspberry Pi](https://www.raspberrypi.org/software/)-site.
1. Bring the image to the sd card
1. Enable SSH to make ssh work after booting: `touch '/run/media/pp/rootfs/boot/ssh'`
1. Boot and connect with `ssh pi@raspberrypi`. The default passowrd is `raspberry`.

## Bluetooth, AirPlay, Spotify Connect, UPnP and Snapcast

```bash
wget -q https://github.com/nicokaiser/rpi-audio-receiver/archive/main.zip
unzip main.zip
rm main.zip

cd rpi-audio-receiver-main
./install.sh
```

Answer all installation questions with Yes except the following:

1. Hostname: **MusixOne**
1. Pretty hostname: **MusixOne**
1. Do you want to install Startup sound? **N**
1. Do you want to install ALSA VU meter plugin (pivumeter) **N**

Restart the system (`sudo reboot`) and try to detect the bluetooth device named MusixOne.

## Set Localization

1. Change following settings with `sudo raspi-config`
    - Password: [new password]
    - Localization Options
      - Change Local: de_CH.UTF-8 UTF-8
    - Advanced Options
      - Expand Filesystem
1. Reboot with `sudo reboot`

Bevor wir die erstellte SD-Karte in den Raspberry Pi einsetzen und booten, müssen wir noch folgenes tun:

## Development computer

With this computer I manage the Rapsberry Pi. To avoid having to enter the password every time I connect to the Raspberry Pi via ssh I generate a public/private key pair.

`ssh-keygen -t rsa`

The public key is then placed on the Rapsbbery Pi.

### Place public key on Raspberry Pi

Copy the public key to the Raspberry Pi.

`cat .ssh/id_rsa.pub | ssh pi@MusicTwo 'cat >> .ssh/authorized_keys'`

## Setting up LIRC on the Raspberry Pi

Connecg to the Raspberry Pi with `ssh pi@MusicTwo`

Edit `sudo nano /boot/config.txt` by uncommenting one line in the lirc-rpi module section as follows. Be careful change the pin to 22 too.

```bash
:
# Uncomment this to enable infrared communication.
dtoverlay=gpio-ir,gpio_pin=22
#dtoverlay=gpio-ir-tx, gpio_pin=18
:
```

Install `lirc` with `sudo apt-get install lirc`

Edit `sudo nano /etc/lirc/lirc_options.conf` as follows by changing these two lines:

```bash
:
driver          = default
device          = /dev/lirc0
:
```

Your remote configuration file(s) will be placed in the /etc/lirc/lircd.conf.d directory. LIRC will find any file in this directory as long as it has a .conf extension (ie: JVC.lircd.conf). We will not be using the devinput.lircd.conf file so we will hide it by changing the extension as follows by renaming devinput.lircd.conf to devinput.lircd.conf.notUsed

```bash
sudo mv /etc/lirc/lircd.conf.d/devinput.lircd.conf /etc/lirc/lircd.conf.d/devinput.lircd.conf.notUse
```

Stop, start and check status of lircd to ensure there are no errors!

```bash
sudo systemctl stop lircd.service
sudo systemctl start lircd.service
sudo systemctl status lircd.service
sudo reboot
```

### Optional: Test lirc

1. `ls -l /dev/lir*`
1. `sudo systemctl stop lircd.service`
1. `mode2 -d /dev/lirc0`
1. Press any keys on the IR control. Various lines appear on the screen.

1. `sudo systemctl start lircd.service`
1. `irw` -> TODO: Hier muss zuerst die neue Config der IR kopiert werden
1. Press any keys on the IR control. The commands appear on the screen.

### Optional: Record codes of the remote control yourself with

1. `sudo service lircd stop`
1. `irrecord -d /dev/lirc0 ~/lircd.conf`

More information in [[1]](https://tutorials-raspberrypi.de/raspberry-pi-ir-remote-control/) or [[4]](https://gist.github.com/prasanthj/c15a5298eb682bde34961c322c95378b).

I am using an previously recorded configuration (VLR-RC001_0026.lircd.conf). This is copied to the Raspberry Pi with deploy script (see below).

## Setup Hifiberry AMP2

Edit `sudo nano /boot/config.txt` and comment / add the following lines at the end of the file:

```bash
:
# Enable audio (loads snd_bcm2835)
#dtparam=audio=on

# DAC+ STANDARD/PRO/AMP2
dtoverlay=hifiberry-dacplus

# Disable wlan to avoid bluetooth interference
#dtoverlay=pi3-disable-wifi
:
```

Check if the sound card is enabled with "aplay":

```bash
#sudo apt install alsa-utils
sudo reboot
ssh pi@MusicTwo
aplay -l
```

## Install and use Microsoft Dot NET 5

We determine for which architecture we have to install .NET with `dpkg --print-architecture`.
The output is `arm64`.

As .NET Core is installed manually, we have to make sure that these libraries are installed: `sudo apt install libc6 libgcc1 libgssapi-krb5-2 libicu63 libssl1.1 libstdc++6 zlib1g`

So we install the [ASP.NET Core 5.0 Runtime (v5.0.0) - Linux Arm32 Binaries](https://download.visualstudio.microsoft.com/download/pr/1c5366e8-9b74-4017-96ae-47fc08832c22/504aed87590bd99c49d053bc6f980b6b/aspnetcore-runtime-5.0.0-linux-arm.tar.gz), because it includes both .NET and ASP.NET core runtimes.

Therefore we download aaspnetcore-runtime-5.0.0-linux-arm.tar.gz and extract this file to `/opt/dotnet`.

```bash
sudo mkdir -p "/opt/dotnet" 
wget https://download.visualstudio.microsoft.com/download/pr/1c5366e8-9b74-4017-96ae-47fc08832c22/504aed87590bd99c49d053bc6f980b6b/aspnetcore-runtime-5.0.0-linux-arm.tar.gz
sudo tar zxf aspnetcore-runtime-5.0.0-linux-arm.tar.gz -C "/opt/dotnet"
```

To make dotnet available for all users we have to set the path. We do this by entering the following line with `sudo nano /etc/profile.d/env.sh`:

```bash
# makes dotnet accessible for all
export DOTNET_ROOT=/opt/dotnet
export PATH=$PATH:/opt/dotnet
#export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
```

Restart with `sudo reboot`

Kompilieren mit:
`dotnet publish MusicControl -c Release -o ../srv/MusicControl -r linux-arm64 -p:PublishSingleFile=false --self-contained true`

## Synchronize and deploy the software

```bash
exit
rsync -avz -e "ssh" --exclude-from=/home/pp/Dokumente/Coding/Stereoanlage/exclude-from-rsync  /home/pp/Dokumente/Coding/Stereoanlage/ pi@MusicTwo:/home/pi/

ssh pi@MusicTwo 'sudo chmod 755 deployAll.sh'
ssh pi@MusicTwo './deployAll.sh'
ssh pi@MusicTwo
```

Use systemctl command to start the service on boot: `sudo systemctl enable --now irexec`

## Installing Music Player Daemon, setcd and VideoLAN

```bash
sudo apt-get install mpd mpc vlc setcd
```

### Setup mpd

Adjust the following values: `sudo nano /etc/mpd.conf`

```bash
music_directory         "/home/pi/mpd/music"
:
playlist_directory      "/home/pi/mpd/playlists"
:
#bind_to_address        "localhost"
:
# hifiberry AMP2 ALSA output:
#
audio_output {
        type            "alsa"
        name            "hifiberry AMP2 ALSA output"
#       device          "hifiberry"     # optional
        mixer_type      "software"      # optional
}
```

Reboot with `sudo reboot` to get the mpd working.
Scan music directory for updates: `mpc update` and set start volume with `mpc volume 8`
