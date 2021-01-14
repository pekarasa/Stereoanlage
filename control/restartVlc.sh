#!/bin/bash

echo "shutdown" | netcat localhost 13001 -q 1
kill $(sudo ps aux | grep 'vlc' | awk '{print $2}')
echo "ChangeUnit cd" | netcat localhost 13000 -q 1
cvlc -I oldrc --rc-host localhost:13001 cdda://
exit 0
