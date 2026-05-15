# 🔔 Real-Time Notification System

A full-stack real-time notification system built with **Node.js**, **Express**, and **Socket.IO**.

## Features

- ⚡ **Real-time delivery** via WebSockets (Socket.IO)
- 🏠 **Multi-room support** — Global, Engineering, Design, Marketing, Alerts
- 🎨 **6 notification types** — Info, Success, Warning, Error, Message, System
- 🔔 **Toast popups** for incoming notifications
- 📋 **Persistent feed** with filter tabs per type
- 👥 **Live user presence** — join/leave events, online count
- ✍️ **Typing indicators**
- 📖 **Mark as read** (individual or all)
- 🌐 **REST API** for programmatic broadcasting
- 📊 **Live stats** — total, unread, online users

## Quick Start

```bash
npm install
npm start
# Open http://localhost:3000
```

## REST API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/notifications` | List notifications (query: `room`, `type`, `limit`) |
| POST | `/api/notifications/broadcast` | Broadcast a notification |
| GET | `/api/users` | List connected users |
| GET | `/api/stats` | Get server stats |

### Broadcast Example

```bash
curl -X POST http://localhost:3000/api/notifications/broadcast \
  -H "Content-Type: application/json" \
  -d '{
    "type": "success",
    "title": "Deploy Complete",
    "message": "v2.0 deployed successfully!",
    "room": "global",
    "sender": "CI/CD"
  }'
```

## Socket.IO Events

### Client → Server
| Event | Payload | Description |
|-------|---------|-------------|
| `join` | `{ username, room }` | Join a room |
| `send_notification` | `{ type, title, message, room }` | Send to room |
| `send_private` | `{ targetSocketId, title, message }` | Private notification |
| `mark_read` | `{ notificationIds: [] }` | Mark as read |
| `get_notifications` | `{ limit, room }` | Fetch history |
| `typing` | `{ room }` | Typing indicator |

### Server → Client
| Event | Payload | Description |
|-------|---------|-------------|
| `init` | `{ socketId, notifications, connectedUsers }` | On connect |
| `joined` | `{ username, room, socketId }` | Confirm join |
| `notification` | Notification object | New notification |
| `user_count` | `{ count }` | Updated user count |
| `user_typing` | `{ username, socketId }` | Typing indicator |
| `notifications_list` | `{ notifications }` | History response |

## Notification Object

```json
{
  "id": "uuid",
  "type": "info|success|warning|error|message|system",
  "title": "Notification Title",
  "message": "Notification body text",
  "sender": "Username or System",
  "room": "global",
  "targetUser": null,
  "timestamp": "2026-05-15T12:00:00.000Z",
  "read": false
}
```
