#!/bin/bash

#allow access to cdrom
#sudo chmod 644 /dev/sr0

#count with cdparanoia the tracks
tracks=$(sudo cdparanoia -sQ |& grep -P "^\s+\d+\." | wc -l)
echo "Number of tracks to add: $tracks" >> /home/pi/audio.log

#add each track to mpd playlist
sudo eject -x 1
for ((i=1; i<=$tracks; i++)); do
   sudo mpc add cdda:///$i
#   echo "sudo mpc add cdda://$i" >> /home/pi/audio.log
done