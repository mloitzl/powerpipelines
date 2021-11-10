
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
