# LubeLogger Daemon

## What

This is a sidecar application for LubeLogger that allows for real-time integration with notification systems such as Ntfy, Gotify, and Discord Webhooks.

## How it works

<img width="793" height="311" alt="image" src="https://github.com/user-attachments/assets/dce1c1f7-408d-42e5-8b24-51ecdc62ad66" />

This project essentially retrieves reminders from your LubeLogger instance and compares those reminders to a cached copy(stored in memory), if there are changes detected(i.e.: a reminder has moved from Not Urgent to Urgent), then it will fire off a notification payload to the integrated services.

Additionally, it can also forward the webhook payload to multiple additional webhook ingestions, so this doesn't entirely replace your existing webhook integrations.

## Security(or lack thereof)

This should run within a private network where it can access your LubeLogger instance as well as any notification integrations you have configured, there are no security features for this service.
