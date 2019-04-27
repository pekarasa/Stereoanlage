# Stereoanlage

Assembling a Raspberry Pi, an AMP2 as amplifier, an infrared control and an infrared receiver diode to create a simple stereo system.

rsync -avz -e "ssh" --exclude-from=/home/pp/Dokumente/Coding/Stereoanlage/exclude-from-rsync  /home/pp/Dokumente/Coding/Stereoanlage/ pi@Musix:/home/pi/

## Setup Raspberry Pi

1. Download [Raspbian Stretch Lite](https://downloads.raspberrypi.org/raspbian_lite_latest)
1. Bring the image to the sd card. (USB image creator) [raspberrypi.org](https://www.raspberrypi.org/documentation/installation/installing-images/README.md)
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
    - Update
1. // Edit `/etc/apt/sources.list` and mask all existing entries.
1. // Add the entry: `deb http://packages.hs-regensburg.de/raspbian/ stretch main contrib non-free rpi`
1. Update the system with `sudo apt-get update`, `sudo apt-get upgrade`, `sudo apt autoremove` and `sudo rpi-update`
1. `sudo shutdown -h now` bzw `sudo reboot`

## VLR-RC001

Press "SET" and the device button at the same time, for example “TV1”, the LED will light up.
Type in the according code number 0026, after the last digit the LED will turn off.

## Setting up LIRC on the Raspberry Pi

1. Connect to the raspberry with `ssh pi@Musix`
1. Install LIRC: `sudo apt-get install lirc --yes`
1. Add following lines to `/etc/modules`
    - lirc_dev
    - lirc_rpi gpio_in_pin=22
1. Add device in the hardware configuration `sudo nano /etc/lirc/hardware.conf`

    ```bash
    # Arguments which will be used when launching lircd
    LIRCD_ARGS="--uinput --listen"
    LOAD_MODULES=true
    DRIVER="default"
    DEVICE="/dev/lirc0"
    MODULES="lirc_rpi"
    ```

1. To load LIRC_PI-Modul edit: `sudo nano /boot/config.txt` and add at the bottom this lines:

    ```bash
    # Uncomment this to enable the lirc-rpi module
    dtoverlay=lirc-rpi,gpio_in_pin=22
    ```

1. To disable wifi edit: `sudo nano /boot/config.txt` and add at teh bottom this lines:

    ```bash
    # Disable wlan to avoid bluetooth interference
    dtoverlay=pi3-disable-wifi
    ```

1. `sudo reboot`
1. Show the new device with `ls -l /dev/lir*`
1. Update: `sudo nano /etc/lirc/lirc_options.conf`:

    ```bash
    driver    = default
    device = /dev/lirc0
    ```

1. `rsync -avz -e "ssh" /home/pp/Dokumente/Coding/Stereoanlage/VLR-RC001_0026.lircd.conf  pi@Musix:/home/pi/`
1. `sudo cp ~/VLR-RC001_0026.lircd.conf /etc/lirc/lircd.conf`
1. `sudo service lircd stop`
1. `sudo service lircd start`
1. `sudo service lircd status`
1. `sudo reboot`

### Test lirc

1. `sudo service lircd stop`
1. `mode2 -d /dev/lirc0`
1. Press any keys on the IR control. Various lines appear on the screen.

1. `sudo service lircd start`
1. `irw`
1. Press any keys on the IR control. The commands appear on the screen.

#### Codes der Fernbedienung selbst aufnehmen mit

1. `sudo service lircd stop`
1. `irrecord -d /dev/lirc0 ~/lircd.conf`

More information in [Raspberry Pi IR Remote Control einrichten](https://tutorials-raspberrypi.de/raspberry-pi-ir-remote-control/) or [Getting lirc to work with Raspberry Pi 3 (Raspbian Stretch)](https://gist.github.com/prasanthj/c15a5298eb682bde34961c322c95378b)

## HiFiBerry AMP2

1. `sudo nano /boot/config.txt`
1. Zeile `dtparam=Audio=on` löschen oder auskommentieren

    ```bash
    # Enable audio (loads snd_bcm2835)
    #dtparam=audio=on
    # AMP2
    dtoverlay=hifiberry-dacplus
    ```

1. `sudo nano /etc/asound.conf`
1. Mit folgendem Inhalt:

    ```bash
    pcm.!default {
      type hw card 0
    }
    ctl.!default {
      type hw card 0
    }
    ```

1. Check, if the sound card is enabled with `aplay -l`

## mpd installieren

1. Install the music player daemon, as well as the client and the alsa utilities: `sudo apt-get install mpd mpc alsa-utils`
1. Adjust the following values: `sudo nano /etc/mpd.conf`

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

1. Set the permissions with:

    ```bash
    mkdir /home/pi/mpd /home/pi/mpd/music/ /home/pi/mpd/playlists/
    sudo chmod g+w /home/pi/mpd/music/ /home/pi/mpd/playlists/
    sudo chgrp audio /home/pi/mpd/music/ /home/pi/mpd/playlists/
    ```

1. Copy playlists: `rsync -avz -e "ssh" /home/pp/Dokumente/Coding/Stereoanlage/mpd/playlists/* pi@Musix:/home/pi/mpd/playlists/`
1. Copy IR Key configuration: `rsync -avz -e "ssh" /home/pp/Dokumente/Coding/Stereoanlage/control/ pi@Musix:/home/pi/control`
1. Install irexec as service: `rsync -avz -e "ssh" /home/pp/Dokumente/Coding/Stereoanlage/irexec.init.d pi@Musix:/home/pi/`
1. rename and move it to the correct directory: `sudo cp /home/pi/irexec.init.d /etc/init.d/irexec`
1. `sudo chmod 755 /etc/init.d/irexec`
1. Start service: `sudo /etc/init.d/irexec start`
1. Register script to be run at startup. Edit `sudo nano /etc/rc.local` and add line `/etc/init.d/irexec start`

## PI Bluetooth Audio Receiver

1. `sudo apt-get install bluetooth bluez`
1. Edit your Bluetooth configuration: `sudo nano /etc/bluetooth/main.conf`
1. Change the class to: `Class = 0x240414`
This will change the default class of your Bluetooth device to audio and the headphone icon will pop up in the devices your pi will be pairing with…
1. Set the values for pairing and discovering times

    ```bash
    DiscoverableTimeout = 0
    PairableTimeout = 0
    ```

    We want our pi to be discoverable and pairable forever.

1. Save the files

### pulseaudio

We will use pulseaudio as the pi’s sound server, to which the audio media will be streamed over the Bluetooth connection and reroute the stream to the AMP2 sound card.

1. `sudo apt-get install pulseaudio pulseaudio-module-bluetooth`
1. `sudo nano /etc/pulse/daemon.conf`
1. Change the resample method to trivial:

    `resample-method = trivial`

    The method trivial is the most basic algorithm implemented. It is supported by the pi.

1. `rsync -avz -e "ssh" /home/pp/Dokumente/Coding/Stereoanlage/pulseaudio.init.d  pi@Musix:/home/pi/`
1. `sudo cp ~/pulseaudio.init.d /etc/init.d/pulseaudio.sh`
1. `sudo chmod 755 /etc/init.d/pulseaudio.sh`
1. Register script to be run at startup: `sudo update-rc.d pulseaudio.sh defaults`

### Activate Bluetooth

1. `sudo service bluetooth restart`

### Test

1. Start: `sudo bluetoothctl`

    ```bash
    power on
    discoverable on
    pairable on
    devices
    ```

1. Try to pair with your phone first.

    `trust 60:8F:5C:F9:55:87` # Galaxix