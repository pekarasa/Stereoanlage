#!/bin/bash

DEV="/dev/sr0"
MPC="/usr/bin/mpc"
mntPoint=/home/pi/mpd/music/mnt
playlist=/home/pi/mpd/playlists/02_AudioCD.m3u

createAudioCdPlaylist()
{
    echo creating audio CD playlist cdda://1..
    NUM_TRACK=$(echo $cdinfo | sed -n 's:.*disc \(.*\) Tracks.*:\1:p')
    echo "cd with ${NUM_TRACK} tracks detected"

    echo "setting speed to max" 
    MAX_SPEED=$(/usr/bin/eject -X)
    /usr/bin/eject -x ${MAX_SPEED}
    
    sudo chmod 644 ${DEV}

    rm $playlist
    for i in $(/usr/bin/seq 1 ${NUM_TRACK}); do
        echo cdda:///${i} >> $playlist
    done

    /bin/echo "setting speed to x12 for playing"
    /usr/bin/eject -x 12
    echo ${MPC} play
    echo $playlist
    cat $playlist
}

createDataCdPlaylist ()
{
    if test "$(find $mntPoint -name MultipleAlbums.md)" = "$mntPoint/MultipleAlbums.md"
    then
        echo create Multiple Albumm Playlist
    fi
    echo create Data CD playlist
}

noDiscFound ()
{
    echo no disc found
}

canNotPlayDVD ()
{
    echo "can't play dvd"
}

do_mount() {
    NUM_TRACK=$(/sbin/udevadm info --query=property ${DEV} | /bin/grep ID_CDROM_MEDIA_TRACK_COUNT_AUDIO | /usr/bin/awk -F= '{ print $2 }')
    /bin/echo "cd with ${NUM_TRACK} tracks detected"
 
    /bin/echo "setting speed to max" 
    MAX_SPEED=$(/usr/bin/eject -X)
    /usr/bin/eject -x ${MAX_SPEED}
    
    /bin/echo "clearing mpd queue"

    sudo chmod 644 ${DEV}
    ${MPC} clear
 
    for i in $(/usr/bin/seq 1 ${NUM_TRACK}); do
        ${MPC} add cdda:///${i}
    done

    /bin/echo "setting speed to x12 for playing"
    /usr/bin/eject -x 12
    ${MPC} play
}

do_unmount() {
    ${MPC} -f "%position% %file%" playlist | /bin/grep cdda:// | /usr/bin/awk '{ print $1 }' | ${MPC} del
}

while true; do
    echo --------------------------------------
    cdinfo=$(setcd -i "/dev/sr0")
    case "$cdinfo" in
        *'not ready'*)
            echo "'(waiting for drive to be ready)'"
            sleep 3;
            ;;
        *'is open'*)
            echo "'(drive is open)'" 
            sleep 5;
            ;;
        *'mixed type CD (data/audio)'*|*'audio disc'*)
            echo "'audio disc'" 
            createAudioCdPlaylist
            break;
            ;;
        *'data disc type 1'*)
            echo "'data disc type 1'" 
            # DataCD or DVD
            sudo umount /dev/sr0
            sudo mount /dev/sr0 $mntPoint
            if test "$(find $mntPoint -name AUDIO_TS)" = "$mntPoint/AUDIO_TS"
            then
                canNotPlayDVD
            else
                createDataCdPlaylist
            fi
            break;
            ;;
        *'No disc is inserted'*)
            echo "'(no disc) '$cdinfo;"
            noDiscFound
            exit 0
            ;;
        *)
            echo "'(unknown error) '$cdinfo"
            noDiscFound
            break;
            ;;
    esac
done
