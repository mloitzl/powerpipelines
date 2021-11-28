
- `brew install rsync` Update to newest rsync

## nginx setup with self-signed certs

https://www.digitalocean.com/community/tutorials/how-to-create-a-self-signed-ssl-certificate-for-nginx-in-ubuntu-16-04


Service and Forwarded Headers
https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-6.0#configure-nginx

```js
server {
    listen        80;
    server_name   example.com *.example.com;
    location / {
        proxy_pass http://localhost:5000/;
        proxy_http_version 1.1;
        proxy_set_header Connection keep-alive;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```

`$ sudo nano /etc/systemd/system/blogdeployment.api.service`

```sh
[Unit]
Description=Blog Deployment API

[Service]
WorkingDirectory=/home/pi/blogdeployment/api
ExecStart=/home/pi/blogdeployment/api/blogdeployments.api
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=blogdeployment.api
User=pi
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

`$ sudo systemctl enable blogdeployment.api.service`
`$ sudo systemctl start blogdeployment.api.service`
`$ journalctl -fu blogdeployment.api.service`


https://github.com/jguillod/couchdb-on-raspberry-pi


<!-- sudo apt-get --no-install-recommends -y remove build-essential \
pkg-config erlang libicu-dev \
libmozjs185-dev libcurl4-openssl-dev
 -->

https://github.com/rabbitmq/rabbitmq-server/releases/tag/v3.8.8

```
$ sudo dpkg -i esl-erlang_22.1.6-1~raspbian~buster_armhf.deb
$ sudo dpkg -i rabbitmq-server_3.8.8-1_all.deb 
$ dpkg-query -f '${Package} ${Status}\n' -W "<searchterm*>"

# https://stackoverflow.com/a/52295727
$ sudo rabbitmq-plugins enable rabbitmq_management
$ sudo rabbitmqctl add_user user password
$ sudo rabbitmqctl set_user_tags user administrator
$ sudo rabbitmqctl set_permissions -p / user ".*" ".*" ".*"

ssh -L 127.0.0.1:15672:localhost:15672 <user>@<host>
ssh -L 127.0.0.1:5672:localhost:5672 <user>@<host>
```

```
$ docker run -d --rm --name couchdbm -p 5984:5984 -v ~/.secrets/couchdb/local.ini:/opt/couchdb/etc/local.ini couchdb:3.2.0
$ docker run -d --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.8.8-management
```


# Troublehshooting systemd

```sh
journalctl -u send_shutdown_notification.service
journalctl --list-boots
journalctl -b-1 -e
journalctl -xe
systemd-analyze blame
systemd-analyze plot > systemd.svg
systemd-analyze critical-chain
```

Undo:
```
sudo nano /etc/systemd/journald.conf 
# -> Storage setting
```