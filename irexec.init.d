#!/bin/sh

cd /home/pi/control


case "$1" in
start)
	su pi --command='/usr/bin/irexec -d /home/pi/control/irexec.conf'
	;;
stop)
	killall /usr/bin/irexec
	;;
restart)
	killall /usr/bin/irexec
	su pi --command='/usr/bin/irexec -d /home/pi/control/irexec.conf'
	;;
*)
	echo "Usage: 'basename $0' {start|stop|restart)" >$2
	exit 64
	;;
esac

exit 0
