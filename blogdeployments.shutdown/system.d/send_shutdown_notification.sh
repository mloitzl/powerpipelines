#!/bin/bash

echo "send_shutdown_notification $1"

case $1 in
        start)
        # systemctl list-jobs | egrep -q 'reboot.target.*start' && echo "starting reboot" >> /tmp/file
        # systemctl list-jobs | egrep -q 'shutdown.target.*start' && echo "starting shutdown" >> /tmp/file
	echo "### $(date) send_shutdown_notification $1" >> /tmp/file
	exit
        ;;

        stop)
        # systemctl list-jobs | egrep -q 'reboot.target.*start' || echo "stopping"  >> /tmp/file
	echo "### $(date) send_shutdown_notification $1" >> /tmp/file
	exit
        ;;

esac