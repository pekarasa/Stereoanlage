#!/bin/sh
### BEGIN INIT INFO
# Provides:          irexec
# Required-Start:    $all
# Required-Stop:
# Default-Start:     2 3 4 5
# Default-Stop:
# Short-Description: starts the irexec with configuration found in /home/pi/control/irexec.conf
### END INIT INFO

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
