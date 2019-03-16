#!/bin/bash

echo "play $1" >> /home/pi/audio.log

#espeak -vde "$1"

play () {
	mpc clear
	mpc load "$1"
	mpc volume 30
	mpc play
}

eject () {
	mpc clear
	sudo eject /dev/sr0
}

playCD () {
	mpc clear
	sudo /home/pi/control/addAudioCD.sh
	mpc playlist >> /home/pi/audio.log
	# eject -x 3
	mpc volume 30
	mpc play 1
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

green) playCD ;;
eject) eject ;;

*) echo "unknown command $1" >> /home/pi/audio.log ;;
esac

exit 0