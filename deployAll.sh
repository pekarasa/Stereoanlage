sudo cp ~/VLR-RC001_0026.lircd.conf /etc/lirc/lircd.conf.d/
mkdir /home/pi/mpd/music/mnt/
sudo chmod g+w /home/pi/mpd/music/ /home/pi/mpd/playlists/ /home/pi/mpd/music/mnt/
sudo chgrp audio /home/pi/mpd/music/ /home/pi/mpd/playlists/ /home/pi/mpd/music/mnt/
sudo cp /home/pi/irexec.init.d /etc/init.d/irexec
sudo chmod 755 /etc/init.d/irexec
sudo chmod 755 /home/pi/control/powerOff.sh
sudo cp -R /home/pi/srv/MusicControl /srv
sudo cp MusicControl.service /etc/systemd/system/MusicControl.service
sudo chmod 755 /etc/systemd/system/MusicControl.service
sudo cp /home/pi/control/irexec.conf /etc/lirc/irexec.lircrc
