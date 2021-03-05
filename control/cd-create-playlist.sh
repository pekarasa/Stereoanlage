#!/bin/bash

DEV="/dev/sr0"
MPC="/usr/bin/mpc"
mntPoint=/home/pi/mpd/music/mnt
playlistName=02_AudioCD
playlist=/home/pi/mpd/playlists/$playlistName.m3u
validFileExtensions='ogg\|mp3'

createAudioCdPlaylist()
{
    echo creating audio CD playlist cdda://1
    NUM_TRACK=$(echo $cdinfo | sed -n 's:.*disc \(.*\) Tracks.*:\1:p')
    echo "cd with ${NUM_TRACK} tracks detected"

    # If played playlist is equal to $playlist, then nothing should be done.
    if test "$(grep cdda ../mpd/playlists/02_AudioCD.m3u | wc -l)" == "$NUM_TRACK"
    then
        echo "do nothing - playlist with $NUM_TRACK tracks already created"
        exit 0
    fi

    echo "setting speed to max" 
    MAX_SPEED=$(/usr/bin/eject -X)
    /usr/bin/eject -x ${MAX_SPEED}
    
    sudo chmod 644 ${DEV}

    rm $playlist
    for i in $(/usr/bin/seq 1 ${NUM_TRACK}); do
        echo cdda:///${i} >> $playlist
    done
    echo /home/pi/mpd/music/EndOfDisc.mp3 >> $playlist

    echo "setting speed to x12 for playing"
    /usr/bin/eject -x 12
}

createDataCdPlaylist ()
{
    echo create Data CD playlist
    find $mntPoint | grep $validFileExtensions > $playlist
    echo /home/pi/mpd/music/EndOfDisc.mp3 >> $playlist
    mpc update
}

noDiscFound ()
{
    echo noDiscFound
    echo /home/pi/mpd/music/KeineCdEingelegt.ogg > $playlist
    # If played playlist is equal to $playlist, reload playlist.
    if test "$(mpc playlist | grep EndOfDisc)" = "Peter Portmann - EndOfDisc"
    then
        echo reload playlist $playlistName
        mpc clear
        mpc load $playlistName
        mpc play
    fi
}

canNotPlayDVD ()
{
    echo canNotPlayDVD
    echo /home/pi/mpd/music/DvdsKoennenNichAbgespieltWerden.ogg > $playlist
    # If played playlist is equal to $playlist, reload playlist.
    if test "$(mpc playlist | grep EndOfDisc)" = "Peter Portmann - EndOfDisc"
    then
        echo reload playlist $playlistName
        mpc clear
        mpc load $playlistName
        mpc play
    fi
}

while true; do
    echo --------------------------------------
    cdinfo=$(setcd -i "/dev/sr0")
    case "$cdinfo" in
        *'not ready'*)
            echo "'(waiting for drive to be ready)'"
            noDiscFound
            sleep 3;
            ;;
        *'is open'*)
            echo "'(drive is open)'" 
            noDiscFound
            sleep 5;
            ;;
        *'mixed type CD (data/audio)'*|*'audio disc'*)
            echo "'audio disc'" 
            createAudioCdPlaylist
            exit 0
            ;;
        *'data disc type 1'*)
            echo "'data disc type 1'" 
            # If played playlist is equal to $playlist, then nothing should be done.
            if test "$(mpc playlist | grep EndOfDisc)" = "Peter Portmann - EndOfDisc"
            then
                echo "do nothing - played playlist is up to date"
                exit 0
            fi
            # If files in disc are equal to $playlist, then nothing should be done.
            echo files in mnt: $(find $mntPoint | grep $validFileExtensions | wc -l)
            echo files in playlist: $(grep -v EndOfDisc $playlist | wc -l)
            if test "$(find $mntPoint | grep $validFileExtensions | wc -l)" = "$(grep -v EndOfDisc $playlist | wc -l)"
            then
                echo "do nothing - generated playlist is up to date"
                exit 0
            fi
            # DataCD or DVD
            echo mount cdrom
            sudo umount /dev/sr0
            sudo mount /dev/sr0 $mntPoint
            if test "$(find $mntPoint -name AUDIO_TS)" = "$mntPoint/AUDIO_TS"
            then
                canNotPlayDVD
                exit 0
            fi
            if test "$(find $mntPoint -name MultipleAlbums.md)" = "$mntPoint/MultipleAlbums.md"
            then
                echo create Multiple Albumm Playlist
                exit 0
            else
                createDataCdPlaylist
            fi
            exit 0
            ;;
        *'No disc is inserted'*)
            echo "'(no disc) '$cdinfo;"
            sudo umount /dev/sr0
            noDiscFound
            exit 0
            ;;
        *)
            echo "'(unknown error) '$cdinfo"
            sudo umount /dev/sr0
            noDiscFound
            break;
            ;;
    esac
done
