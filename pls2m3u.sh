#!/bin/bash
name=$1
file=$(echo "${name/pls/m3u}")
url=$(cat $1 | grep -i file1)
title=$(cat $1 | grep -i title)
echo "#EXTM3U" > $file
echo "#EXTINF:-1,- 0 N - ${title/Title1=/}" >> $file
echo "${url/File1=/}" >> $file
cat $file
