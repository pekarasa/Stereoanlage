#!/bin/bash

mpc () { echo "mpc $1 $2 $3" }
mntPoint=/media/pp
playLists=/home/pp/Dokumente/Coding/Stereoanlage/mpd/playlists
musicCenterLog=/home/pp/Dokumente/Coding/Stereoanlage/audio.log
storedActiveUnit=/home/pp/Dokumente/Coding/Stereoanlage/StoredActiveUnit.sts
# mntPoint=/home/pi/mpd/music/mnt
# playLists=/home/pi/mpd/playlists
# musicCenterLog=/home/pi/musicCenter.log
# storedActiveUnit=/home/pi/StoredActiveUnit.sts

sendToVlc () {
    echo "sendToVlc $1" >> $musicCenterLog
    echo "echo \"$1\" | netcat localhost 12345 -q 1" >> $musicCenterLog
    echo "$1" | netcat localhost 12345 -q 1
}

powerOff () {
    echo "$ActiveUnit: powerOff" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            mpc stop
            mpc volume 30
            ;;
        AudioCD)
            ;;
        DVD)
            sudo umount /dev/sr0
            ;;
        DataCD)
            sudo umount /dev/sr0
            ejectDrive
            ;;
        MultipleAlbumms)
            sudo umount /dev/sr0
            ;;
        *) echo "$ActiveUnit: 2: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
    sudo shutdown -h now
}

track () {
    echo "$ActiveUnit: track $1" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            playlist=$(ls -1 $playLists | sed -e 's/\..*$//' | sed -n $1p)
            streamPlay "$playlist"
            ;;
        AudioCD)
            sendToVlc "goto $1"
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc play $1
            ;;
        MultipleAlbumms)
            #mpc play $1
            ;;
        *) echo "$ActiveUnit: 3: unknown ActiveUnit"  >> $musicCenterLog ;;
    esac
}

disc () {
    echo "disc $1" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            ;;
        AudioCD)
            ;;
        DVD)
            ;;
        DataCD)
            streamPlay "zDisc1"
            ;;
        MultipleAlbumms)
            streamPlay "zDisc$1"
            ;;
        *) echo "$ActiveUnit: 4: unknown ActiveUnit"  >> $musicCenterLog ;;
    esac
}

streamPlay () {
    echo "$ActiveUnit: streamPlay '$1'" >> $musicCenterLog
	mpc clear
	mpc load "$1"
	mpc play
}

setTheActiveUnitAccordingToTheCDType () {
    echo "check kind of CD (AudioCD / DVD / DataCD / MultipleAlbumms)" >> $musicCenterLog

    while true; do
        cdinfo=$(setcd -i "/dev/sr0")
        case "$cdinfo" in
            *'not ready'*)
                echo "'(waiting for drive to be ready)'" >> $musicCenterLog
                sleep 3;
                ;;
            *'is open'*)
                echo "'(drive is open)'" >> $musicCenterLog
                sleep 5;
                ;;
            *'mixed type CD (data/audio)'*|*'audio disc'*)
                echo "'audio disc'" >> $musicCenterLog
                export ActiveUnit=AudioCD;
                break;
                ;;
            *'data disc type 1'*)
                echo "'data disc type 1'" >> $musicCenterLog
                # DataCD or DVD
                sudo umount /dev/sr0
                sudo mount /dev/sr0 $mntPoint
                if test "$(find $mntPoint -name AUDIO_TS)" = "$mntPoint/AUDIO_TS"
                then
                    export ActiveUnit=DVD;
                else
                    export ActiveUnit=DataCD;
                fi
                if test "$(find $mntPoint -name MultipleAlbums.md)" = "$mntPoint/MultipleAlbums.md"
                then
                    export ActiveUnit=DataCD;
                    # export ActiveUnit=MultipleAlbumms;
                fi
                break;
                ;;
            *'No disc is inserted'*)
                echo "'(no disc) '$cdinfo;" >> $musicCenterLog
                sleep 5;
                ;;
            *)
                echo "'(unknown error) '$cdinfo" >> $musicCenterLog
                break;
                ;;
        esac
    done
}

play () {
    echo "$ActiveUnit: play" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner) 
            mpc play ;;
        AudioCD)
            sendToVlc "play"
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc play
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 5: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

pause () {
    echo "$ActiveUnit: pause" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner) 
            mpc pause ;;
        AudioCD)
            sendToVlc "pause"
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc pause
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 6: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

stop () {
    echo "$ActiveUnit: stop" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner) 
            mpc stop
            ;;
        AudioCD)
            sendToVlc "stop"
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc stop
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 7: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

fastforward () {
    echo "$ActiveUnit: fastforward" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            mpc seek +00:00:10
            ;;
        AudioCD)
            sendToVlc fastforward
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc seek +00:00:30
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 8: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

rewind () {
    echo "$ActiveUnit: rewind" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            mpc seek -00:00:10
            ;;
        AudioCD)
            sendToVlc rewind
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc seek -00:00:30
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 9: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

next () {
    echo "$ActiveUnit: next" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner) 
            mpc next
            ;;
        AudioCD)
            sendToVlc next
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc next 
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 10: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

previous () {
    echo "$ActiveUnit: previous" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner) 
            mpc cdprev
            ;;
        AudioCD)
            sendToVlc prev
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc cdprev 
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 11: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

volumeUp () {
    echo "$ActiveUnit: volumeUp" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            mpc volume +3
            ;;
        AudioCD)
            sendToVlc volup 
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc volume +3
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 12: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

volumeDown () {
    echo "$ActiveUnit: volumeDown" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner) 
            mpc volume -3
            ;;
        AudioCD)
            sendToVlc voldown 
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc volume -3
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 13: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

volumeMute () {
    echo "$ActiveUnit: volumeMute" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            mpc volume 0
            ;;
        AudioCD)
            sendToVlc "volume 0";;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            mpc volume 0
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 14: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

channelUp () {
    echo "$ActiveUnit: channelUp" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            ;;
        AudioCD)
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 15: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

channelDown () {
    echo "$ActiveUnit: channelDown" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            ;;
        AudioCD)
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 16: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

ejectDrive () {
    echo "$ActiveUnit: ejectDrive" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            ;;
        AudioCD)
            kill "$ActiveUnit";
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            kill "$ActiveUnit"
            eject
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 17: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

record () {
    echo "$ActiveUnit: record" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            ;;
        AudioCD)
            echi ripit >> $musicCenterLog
            sudo ripit --nointeraction -W --coder=2 -e --overwrite e -o /home/pi/mpd/music/
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "$ActiveUnit: 18: unknown ActiveUnit" >> $musicCenterLog ;;
    esac
}

kill () {
    echo "$1: kill" >> $musicCenterLog
    case "$1" in
        Tuner)
            mpc stop
            ;;
        AudioCD)
            sendToVlc "stop"
            sendToVlc "shutdown"
            kill $(sudo ps aux | grep 'vlc' | awk '{print $2}')
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            sudo umount /dev/sr0
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "19: unknown parameter $1" >> $musicCenterLog ;;
    esac
}

createPlaylist () {
    echo "$ActiveUnit: createPlaylist" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            ;;
        AudioCD)
            ;;
        DVD)
            ;;
        DataCD)
            find $mntPoint/ -name "*.mp3" -o -name "*.ogg" -o -name "*.wav" -o -name "*.flac" > $playLists/zDisc1.m3u
            mpc update
            ;;
        MultipleAlbumms)
            echo "MultipleAlbumms" >> $musicCenterLog ;;
        *) echo "191: unknown parameter $ActiveUnit" >> $musicCenterLog ;;
    esac
}

start () {
    echo "$ActiveUnit: start" >> $musicCenterLog
    case "$ActiveUnit" in
        Tuner)
            mpc play
            ;;
        AudioCD)
            echo cvlc -I rc --rc-host localhost:12345 --volume-step 6 cdda:// >> $musicCenterLog
            ( cvlc -I rc --rc-host localhost:12345 --volume-step 6 cdda:// ) &
            sendToVlc "volume 70"
            track 1
            ;;
        DVD)
            echo "DVD" >> $musicCenterLog ;;
        DataCD)
            streamPlay zDisc1
            ;;
        MultipleAlbumms)
            disc 1
            track 1
            ;;
        *) 
            echo "$ActiveUnit: 20: unknown ActiveUnit" >> $musicCenterLog
            ;;
    esac
}

cmdControl () {
    echo "cmdControl $1 $2" >> $musicCenterLog
    case "$1" in
        powerOff)
            powerOff ;;
        track) 
            # track 0 - 29
            track $2 ;;
        disc)
            # disc 1 - 9
            disc $2 ;;
        record) 
            record ;;
        eject)
            ejectDrive ;;
        rewind)
            case "$2" in
                fastforward) fastforward ;;
                rewind) rewind ;;
                next) next ;;
                previous) previous ;;
                *) echo "unknown rewind $2" >> $musicCenterLog ;;
            esac
            ;;
        control)
            case "$2" in
                play) play ;;
                pause) pause ;;
                stop) stop ;;
                *) echo "unknown control $2" >> $musicCenterLog ;;
            esac
            ;;
        volume)
            case "$2" in
                up) volumeUp ;;
                down) volumeDown ;;
                mute) volumeMute ;;
                *) ;;
            esac
            ;;
        channel)
            case "$2" in
                up) channelUp ;;
                down) channelDown ;;
                *) ;;
            esac
            ;;
        changeUnit)
            echo "$ActiveUnit changeUnit '$2'" >> $musicCenterLog
            stop
            oldUnit=$ActiveUnit
            echo "oldUnit $oldUnit" >> $musicCenterLog
            case "$2" in
                radio) 
                    ActiveUnit=Tuner
                    ;;
                stream) 
                    # ActiveUnit=LocalPlaylists
                    ActiveUnit=Tuner
                    ;;
                cd)
                    # to cd (AudioCD / DVD / DataCD / MultipleAlbumms)
                    setTheActiveUnitAccordingToTheCDType
                    createPlaylist
                    ;;
                *) echo "$ActiveUnit: 1: unknown ActiveUnit" >> $musicCenterLog ;;
            esac

            if test "$ActiveUnit" != "$oldUnit"
            then
                echo "$ActiveUnit" != "$oldUnit" >> $musicCenterLog
                kill $oldUnit
            fi

            start 

            echo $ActiveUnit>$storedActiveUnit
            ;;
        *) 
            echo "unknown command $1 $2"
            echo "unknown command $1 $2" >> $musicCenterLog ;;
    esac
}

echo "----------------------" >> $musicCenterLog
echo "command: $1 $2" >> $musicCenterLog

ActiveUnit="$(cat $storedActiveUnit)"
echo "ActiveUnit = $ActiveUnit"

cmdControl $1 $2

echo "======================" >> $musicCenterLog
exit 0