sudo cp ~/VLR-RC001_0026.lircd.conf /etc/lirc/lircd.conf.d/
mkdir /home/pi/mpd /home/pi/mpd/music/ /home/pi/mpd/playlists/ /home/pi/mpd/music/mnt/
sudo chmod g+w /home/pi/mpd/music/ /home/pi/mpd/playlists/ /home/pi/mpd/music/mnt/
sudo chgrp audio /home/pi/mpd/music/ /home/pi/mpd/playlists/ /home/pi/mpd/music/mnt/
sudo cp /home/pi/irexec.init.d /etc/init.d/irexec
sudo chmod 755 /etc/init.d/irexec
sudo chmod 755 /home/pi/control/musicCenter.sh
