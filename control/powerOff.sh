#!/bin/bash

echo "PowerOff" | netcat localhost 13000 -q 1
echo "shutdown" | netcat localhost 13001 -q 1
kill $(sudo ps aux | grep 'vlc' | awk '{print $2}')
sudo shutdown -h now
exit 0
