#!/bin/bash

echo "play $1" >> /home/pi/audio.log

#espeak -vde "$1"

play () {
	mpc clear
	mpc load "$1"
	mpc play
}

playCD () {
	echo "shutdown" | netcat localhost 12345 -q 1
	kill $(sudo ps aux | grep 'vlc' | awk '{print $2}')
	cvlc -I rc --rc-host localhost:12345 cdda:// --volume-step 6
}

stopCD () {
	echo "shutdown" | netcat localhost 12345 -q 1
	kill $(sudo ps aux | grep 'vlc' | awk '{print $2}')
}

ejectCD () {
	echo "shutdown" | netcat localhost 12345 -q 1
	kill $(sudo ps aux | grep 'vlc' | awk '{print $2}')

}

recordCD () {
	sudo ripit --nointeraction -W --coder=2 -e --overwrite e -o /home/pi/mpd/music/
}

powerOff() {
	mpc volume 30
	sudo shutdown -h now
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
9) play "Radio Bayern 2" ;;

10) play "Chillout" ;;
11) play "80s Forever" ;;
12) play "Energy 80s" ;;
13) play "Energy 90s" ;;
14) play "Energy Mundart" ;;
15) play "Energy One Hit Wonder" ;;
16) play "Radio 24 - Greatest Hits" ;;
17) play "Ambi Nature Radio" ;;

cd) playCD ;;
cdStop) stopCD ;;
cdRecord) recordCD ;;

PowerOff) powerOff ;;

*) echo "unknown command $1" >> /home/pi/audio.log ;;
esac

exit 0