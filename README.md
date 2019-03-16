# Stereoanlage

Setting up a Rapsberry PI and AMP2 as stereo system.

1. [Raspbian Stretch Lite](https://downloads.raspberrypi.org/raspbian_lite_latest) Image herunterladen
1. Image erstellen mit Linuxtool (USB-Abbilderstellung)
1. SSH einschalten, damit nach dem booten ssh funktioniert: `touch '/media/pp/boot/ssh'`
1. Booten und via `ssh pi@192.168.1.118` verbinden. Das Standard-Passwort lautet `raspberry`.
1. Ändere folgende Einstellungen mit `sudo raspi-config`
    - Password: [new password]
    - Localization Options
      - Change Local: de_CH.UTF-8 UTF-8
    - Network Options
      - Hostname: `Musix`
      - WiFi: [your wifi name]
    - Advanced Options
      - Expand Filesystem
    - Update
1. Editiere `/etc/apt/sources.list` und maskiere bestehende Einträge aus.
1. Füge folgenden Eintrag hinzu: `deb http://packages.hs-regensburg.de/raspbian/ stretch main contrib non-free rpi`
1. Aktualisiere das System mit `sudo apt-get update`, `sudo apt-get upgrade`, `sudo apt autoremove` und `sudo rpi-update`
1. `sudo shutdown -h now` bzw `sudo reboot`

## LIRC auf dem Raspberry Pi einrichten

1. LIRC installieren: `sudo apt-get install lirc --yes`
1. Füge folgende Zeilen zur `/etc/modules`-Datei hinzu
    - lirc_dev
    - lirc_rpi gpio_in_pin=22
1. Device in der Hardware Konfiguration eintragen `sudo nano /etc/lirc/hardware.conf`

    ```bash
    # Arguments which will be used when launching lircd
    LIRCD_ARGS="--uinput --listen"
    LOAD_MODULES=true
    DRIVER="default"
    DEVICE="/dev/lirc0"
    MODULES="lirc_rpi"
    ```

1. LIRC_PI-Modul laden: `sudo nano /boot/config.txt` und am Ende folgende Zeilen hizufügen:

    ```bash
    # Uncomment this to enable the lirc-rpi module
    dtoverlay=lirc-rpi,gpio_in_pin=22
    ```

1. `sudo reboot`
1. Device anzeigen lassen mittels `ls -l /dev/lir*`
1. Aktualisiere folgende Werte: `sudo nano /etc/lirc/lirc_options.conf`:

    ```bash
    driver    = default
    device = /dev/lirc0
    ```

1. `rsync -avz -e "ssh" /home/pp/Dokumente/Coding/Stereoanlage/VLR-RC001_0026.lircd.conf  pi@192.168.1.118:/home/pi/`
1. `sudo cp ~/VLR-RC001_0026.lircd.conf /etc/lirc/lircd.conf`
1. `sudo /etc/init.d/lircd stop`
1. `sudo /etc/init.d/lircd start`
1. `sudo /etc/init.d/lircd status`
1. `sudo reboot`

### Testen

1. `sudo /etc/init.d/lircd stop`
1. `mode2 -d /dev/lirc0`
1. Auf der IR Bedienung irgenwelche Tasten drücken. Es erscheinen dieverse Zeilen auf dem Bildschirm.

1. `sudo /etc/init.d/lircd stop`
1. `irw`
1. Auf der IR Bedienung irgenwelche Tasten drücken. Es erscheinen die Kommandos auf dem Bildschirm.

#### Codes der Fernbedienung selbst aufnehmen mit

1. `sudo /etc/init.d/lircd stop`
1. `irrecord -d /dev/lirc0 ~/lircd.conf`

Weitere Infos unter [Raspberry Pi IR Remote Control einrichten](https://tutorials-raspberrypi.de/raspberry-pi-ir-remote-control/) oder [Getting lirc to work with Raspberry Pi 3 (Raspbian Stretch)](https://gist.github.com/prasanthj/c15a5298eb682bde34961c322c95378b)

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

1. Installiere den musc player deamon, sowie den client und die alsa utilities: `sudo apt-get install mpd mpc alsa-utils`
1. Passe folgende Werte an: `sudo nano /etc/mpd.conf`

    ```bash
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

1. Setzen der Rechte mit:

    ```bash
    sudo chmod g+w /var/lib/mpd/music/ /var/lib/mpd/playlists/
    sudo chgrp audio /var/lib/mpd/music/ /var/lib/mpd/playlists/
    ```

1. Playlists in `/var/lib/mpd/playlists` hinzufügen: `rsync -avz -e "ssh" /home/pp/Dokumente/Coding/Stereoanlage/playlists/* pi@192.168.1.118:/var/lib/mpd/playlists/`
1. IR Tastebelegung setzen: `rsync -avz -e "ssh" /home/pp/Dokumente/Coding/Stereoanlage/control/ pi@192.168.1.118:/home/pi/control`
1. irexec als Service installieren: `rsync -avz -e "ssh" /home/pp/Dokumente/Coding/Stereoanlage/irexec.init.d pi@192.168.1.118:/home/pi/`
1. umbenennen und in den richtigen Ordner verschieben: `sudo cp /home/pi/irexec.init.d /etc/init.d/irexec`
1. `sudo chmod 755 /etc/init.d/irexec`
1. Service starten mit `sudo /etc/init.d/irexec start`
1. Dafür sorgen dass der Service nach dem Booten automatisch gestartet wird: Ergänze `sudo nano /etc/rc.local` mit `/etc/init.d/irexec start`

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

1. `rsync -avz -e "ssh" /home/pp/Dokumente/Coding/Stereoanlage/pulseaudio.init.d  pi@192.168.1.118:/home/pi/`
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