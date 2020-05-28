# Stereo system

I run a compact stereo system consisting of a Raspberry Pi and an AMP2 as audio amplifier. Additionally there is a button to start the system. The stereo system is operated with an infrared remote control.

The aim of this guide is to help you set up the software for your stereo system as quickly as possible.

## Playable media

Currently the stereo system can play the following media:

- Streams from the Internet (web radio)
- Local music (mp3)
- Audio CDs
- Serves as Bluetooth speaker

## Software in use

To play the above listed media I use MPD, vlc and bluetoothctl

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

## Setup of the infrared remote control VLR-RC001

Press "SET" and the device button at the same time, for example “TV1”, the LED will light up.
Type in the according code number 0026, after the last digit the LED will turn off.

## Hardware Extensions

I connect an infrared diode which serves as a receiver. This is connected as described under [[1]](https://tutorials-raspberrypi.de/raspberry-pi-ir-remote-control/). But I will use the gpio_in_pin 22 and not the described gpio_in_pin 17.

The button is connected as described under [[2]](https://howchoo.com/g/mwnlytk3zmm/how-to-add-a-power-button-to-your-raspberry-pi).

## Development computer

With this computer I manage the Rapsberry Pi. To avoid having to enter the password every time I connect to the Raspberry Pi via ssh I generate a public/private key pair.

`ssh-keygen -t rsa`

The public key is then placed on the Rapsbbery Pi. See below for details.

## Setup Raspberry Pi

1. Download [Raspbian Stretch Lite](https://downloads.raspberrypi.org/raspbian_lite_latest)
1. Bring the image to the sd card. (USB image creator) [[3]](https://www.raspberrypi.org/documentation/installation/installing-images/README.md)
1. Enable SSH to make ssh work after booting: `touch '/media/pp/boot/ssh'`
1. Boot and connect with `ssh pi@raspberrypi`. The default passowrd is `raspberry`.
1. Change following settings with `sudo raspi-config`
    - Password: [new password]
    - Localization Options
      - Change Local: de_CH.UTF-8 UTF-8
    - Network Options
      - Hostname: `Musix`
    - Advanced Options
      - Expand Filesystem
1. __Don't__ Update the system with `sudo apt-get update && sudo apt-get upgrade && sudo apt autoremove`
1. `sudo shutdown -h now` bzw `sudo reboot`

## Setting up LIRC on the Raspberry Pi

```bash
ssh pi@Musix mkdir -p .ssh
cat .ssh/id_rsa.pub | ssh pi@Musix 'cat >> .ssh/authorized_keys'
ssh pi@Musix
sudo apt-get install lirc mpd mpc alsa-utils bluetooth bluez pulseaudio pulseaudio-module-bluetooth --yes
```

Edit `sudo nano /boot/config.txt` and comment / add the following lines at the end of the file:

```bash
# Enable audio (loads snd_bcm2835)
#dtparam=audio=on

# AMP2
dtoverlay=hifiberry-dacplus

# Uncomment this to enable the lirc-rpi module
dtoverlay=lirc-rpi,gpio_in_pin=22

# Disable wlan to avoid bluetooth interference
dtoverlay=pi3-disable-wifi
```

```bash
sudo reboot
ssh pi@Musix
sudo modprobe lirc_dev
sudo modprobe lirc_rpi gpio_in_pin=22
```

//Add following lines to `/etc/modules`

    - lirc_dev
    - lirc_rpi gpio_in_pin=22

Add device in the hardware configuration `sudo nano /etc/lirc/hardware.conf`

   ```bash
   # Arguments which will be used when launching lircd
   LIRCD_ARGS="--uinput --listen"
   LOAD_MODULES=true
   DRIVER="default"
   DEVICE="/dev/lirc0"
   MODULES="lirc_rpi"
   ```

Update: `sudo nano /etc/lirc/lirc_options.conf`:

   ```bash
   driver    = default
   device = /dev/lirc0
   ```

```bash
sudo reboot
ssh pi@Musix
ls -l /dev/lir*
exit
```

### Synchronize and deploy the software

```bash
rsync -avz -e "ssh" --exclude-from=/home/pp/Dokumente/Coding/Stereoanlage/exclude-from-rsync  /home/pp/Dokumente/Coding/Stereoanlage/ pi@Musix:/home/pi/

ssh pi@Musix 'sudo chmod 755 deployAll.sh'
ssh pi@Musix './deployAll.sh'

```

### Setup mpd 

Adjust the following values: `sudo nano /etc/mpd.conf`

  ```bash
  music_directory         "/home/pi/mpd/music"
  ...
  playlist_directory      "/home/pi/mpd/playlists"
  ...
  #bind_to_address        "localhost"
  ...
  # hifiberry AMP2 ALSA output:
  #
  audio_output {
          type            "alsa"
          name            "My ALSA Device"
  #       device          "hifiberry"     # optional
          mixer_type      "software"      # optional
  }
  ```

1. Start service: `sudo /etc/init.d/irexec start`

1. Register script to be run at startup. Edit `sudo nano /etc/rc.local` and add line `/etc/init.d/irexec start`

1. Edit your Bluetooth configuration: `sudo nano /etc/bluetooth/main.conf`

1. Change the class to: `Class = 0x240414`
This will change the default class of your Bluetooth device to audio and the headphone icon will pop up in the devices your pi will be pairing with…

1. Set the values for pairing and discovering times

   ```bash
   DiscoverableTimeout = 0
   PairableTimeout = 0
   ```

1. We want our pi to be discoverable and pairable forever.

1. Save the files

1. `sudo nano /etc/pulse/daemon.conf`

    Change the resample method to trivial:

    `resample-method = trivial`

    The method trivial is the most basic algorithm implemented. It is supported by the pi.

1. Register script to be run at startup: `sudo update-rc.d pulseaudio.sh defaults`

### Test lirc

1. `sudo service lircd stop`
1. `mode2 -d /dev/lirc0`
1. Press any keys on the IR control. Various lines appear on the screen.

1. `sudo service lircd start`
1. `irw`
1. Press any keys on the IR control. The commands appear on the screen.

#### Record codes of the remote control yourself with

1. `sudo service lircd stop`
1. `irrecord -d /dev/lirc0 ~/lircd.conf`

More information in [[1]](https://tutorials-raspberrypi.de/raspberry-pi-ir-remote-control/) or [[4]](https://gist.github.com/prasanthj/c15a5298eb682bde34961c322c95378b).

## HiFiBerry AMP2

1. Check, if the sound card is enabled with `aplay -l`

## PI Bluetooth Audio Receiver

### pulseaudio

We will use pulseaudio as the pi’s sound server, to which the audio media will be streamed over the Bluetooth connection and reroute the stream to the AMP2 sound card.

### Activate Bluetooth

1. `sudo service bluetooth restart`

1. Start: `sudo bluetoothctl`

    ```bash
    power on
    discoverable on
    pairable on
    devices
    ```

1. Try to pair with your phone first.

    `trust 60:8F:5C:F9:55:87` # Galaxix
    `trust 5C:F3:70:92:B9:96` # petrix

1. If you need to remove a trusted device, you cane do this with

    `remove 5C:F3:70:92:B9:96` # petrix
