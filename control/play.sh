#!/bin/bash

echo "play $1" >> /home/pi/audio.log

#espeak -vde "$1"

play () {
	mpc clear
	mpc load "$1"
	mpc volume 30
	mpc play
}

playCD () {
	kill $(ps aux | grep 'mplayer' | awk '{print $2}')
	mplayer -cdrom-device /dev/sr0 cdda:// -volume 20
}

stopCD () {
	kill $(ps aux | grep 'mplayer' | awk '{print $2}')
}

recordCD () {
	sudo ripit --nointeraction -W --coder=2 -e --overwrite e -o /home/pi/mpd/music/
}

case "$1" in
1) play "Radio SRF 1" ;;
2) play "Radio SRF 2 Kultur" ;;
3) play "Radio SRF 3" ;;

4) play "Radio SRF 4 News" ;;
5) play "Radio SWR2" ;;
6) play "Radio Deutschlandfunk" ;;

7) play "Radio Dlf Kultur" ;;
8) play "Radio Ö1" ;;
9) play "Chillout" ;;

cd) playCD ;;
cdStop) stopCD ;;
cdRecord) recordCD ;;

*) echo "unknown command $1" >> /home/pi/audio.log ;;
esac

exit 0