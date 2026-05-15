const express = require('express');
const http = require('http');
const { Server } = require('socket.io');
const { v4: uuidv4 } = require('uuid');
const path = require('path');

const app = express();
const server = http.createServer(app);
const io = new Server(server, {
  cors: {
    origin: '*',
    methods: ['GET', 'POST']
  }
});

app.use(express.json());
app.use(express.static(path.join(__dirname, 'public')));

// In-memory notification store (last 100 notifications)
const MAX_NOTIFICATIONS = 100;
let notifications = [];
let connectedUsers = new Map(); // socketId -> { username, room }

// Notification types
const NOTIFICATION_TYPES = {
  INFO: 'info',
  SUCCESS: 'success',
  WARNING: 'warning',
  ERROR: 'error',
  MESSAGE: 'message',
  SYSTEM: 'system'
};

/**
 * Create a notification object
 */
function createNotification({ type = NOTIFICATION_TYPES.INFO, title, message, sender = 'System', room = 'global', targetUser = null }) {
  return {
    id: uuidv4(),
    type,
    title,
    message,
    sender,
    room,
    targetUser,
    timestamp: new Date().toISOString(),
    read: false
  };
}

/**
 * Store notification and trim to max
 */
function storeNotification(notification) {
  notifications.unshift(notification);
  if (notifications.length > MAX_NOTIFICATIONS) {
    notifications = notifications.slice(0, MAX_NOTIFICATIONS);
  }
}

// REST API: Get all notifications
app.get('/api/notifications', (req, res) => {
  const { room, type, limit = 50 } = req.query;
  let result = [...notifications];

  if (room) result = result.filter(n => n.room === room || n.room === 'global');
  if (type) result = result.filter(n => n.type === type);

  res.json({
    success: true,
    count: result.length,
    notifications: result.slice(0, parseInt(limit))
  });
});

// REST API: Broadcast a notification via HTTP
app.post('/api/notifications/broadcast', (req, res) => {
  const { type, title, message, room = 'global', sender = 'API' } = req.body;

  if (!title || !message) {
    return res.status(400).json({ success: false, error: 'title and message are required' });
  }

  const notification = createNotification({ type, title, message, room, sender });
  storeNotification(notification);

  if (room === 'global') {
    io.emit('notification', notification);
  } else {
    io.to(room).emit('notification', notification);
  }

  res.json({ success: true, notification });
});

// REST API: Get connected users
app.get('/api/users', (req, res) => {
  const users = Array.from(connectedUsers.values());
  res.json({ success: true, count: users.length, users });
});

// REST API: Get stats
app.get('/api/stats', (req, res) => {
  const typeCounts = {};
  for (const type of Object.values(NOTIFICATION_TYPES)) {
    typeCounts[type] = notifications.filter(n => n.type === type).length;
  }
  res.json({
    success: true,
    totalNotifications: notifications.length,
    connectedUsers: connectedUsers.size,
    byType: typeCounts
  });
});

// Socket.IO connection handling
io.on('connection', (socket) => {
  console.log(`[+] Client connected: ${socket.id}`);

  // Send recent notifications to newly connected client
  socket.emit('init', {
    socketId: socket.id,
    notifications: notifications.slice(0, 20),
    connectedUsers: connectedUsers.size
  });

  // User joins with a username
  socket.on('join', ({ username, room = 'global' }) => {
    const user = { username: username || `User_${socket.id.slice(0, 6)}`, room };
    connectedUsers.set(socket.id, user);

    socket.join(room);
    socket.data.username = user.username;
    socket.data.room = room;

    console.log(`[JOIN] ${user.username} joined room: ${room}`);

    // Notify room about new user
    const joinNotification = createNotification({
      type: NOTIFICATION_TYPES.SYSTEM,
      title: 'User Joined',
      message: `${user.username} joined the room`,
      sender: 'System',
      room
    });
    storeNotification(joinNotification);
    io.to(room).emit('notification', joinNotification);

    // Broadcast updated user count
    io.emit('user_count', { count: connectedUsers.size });

    // Confirm join to the user
    socket.emit('joined', { username: user.username, room, socketId: socket.id });
  });

  // Client sends a notification to a room
  socket.on('send_notification', ({ type, title, message, room }) => {
    const sender = socket.data.username || `User_${socket.id.slice(0, 6)}`;
    const targetRoom = room || socket.data.room || 'global';

    const notification = createNotification({ type, title, message, sender, room: targetRoom });
    storeNotification(notification);

    if (targetRoom === 'global') {
      io.emit('notification', notification);
    } else {
      io.to(targetRoom).emit('notification', notification);
    }

    console.log(`[NOTIFY] ${sender} → [${targetRoom}]: ${title}`);
  });

  // Client sends a private notification to a specific socket
  socket.on('send_private', ({ targetSocketId, title, message, type }) => {
    const sender = socket.data.username || `User_${socket.id.slice(0, 6)}`;
    const notification = createNotification({
      type: type || NOTIFICATION_TYPES.MESSAGE,
      title: title || 'Private Message',
      message,
      sender,
      room: 'private',
      targetUser: targetSocketId
    });
    storeNotification(notification);

    // Send to target and back to sender
    io.to(targetSocketId).emit('notification', notification);
    socket.emit('notification', { ...notification, title: `[Sent] ${notification.title}` });
  });

  // Client marks notifications as read
  socket.on('mark_read', ({ notificationIds }) => {
    if (Array.isArray(notificationIds)) {
      notificationIds.forEach(id => {
        const n = notifications.find(n => n.id === id);
        if (n) n.read = true;
      });
    }
    socket.emit('notifications_updated', { updated: notificationIds });
  });

  // Client requests latest notifications
  socket.on('get_notifications', ({ limit = 20, room } = {}) => {
    let result = [...notifications];
    if (room) result = result.filter(n => n.room === room || n.room === 'global');
    socket.emit('notifications_list', { notifications: result.slice(0, limit) });
  });

  // Typing indicator
  socket.on('typing', ({ room }) => {
    const username = socket.data.username || `User_${socket.id.slice(0, 6)}`;
    socket.to(room || 'global').emit('user_typing', { username, socketId: socket.id });
  });

  // Disconnect
  socket.on('disconnect', () => {
    const user = connectedUsers.get(socket.id);
    connectedUsers.delete(socket.id);

    if (user) {
      console.log(`[-] ${user.username} disconnected`);
      const leaveNotification = createNotification({
        type: NOTIFICATION_TYPES.SYSTEM,
        title: 'User Left',
        message: `${user.username} left the room`,
        sender: 'System',
        room: user.room
      });
      storeNotification(leaveNotification);
      io.to(user.room).emit('notification', leaveNotification);
    }

    io.emit('user_count', { count: connectedUsers.size });
  });
});

// Periodic system notifications (demo heartbeat every 30s)
setInterval(() => {
  if (connectedUsers.size > 0) {
    const heartbeat = createNotification({
      type: NOTIFICATION_TYPES.INFO,
      title: 'System Heartbeat',
      message: `Server is healthy. ${connectedUsers.size} user(s) connected.`,
      sender: 'System',
      room: 'global'
    });
    storeNotification(heartbeat);
    io.emit('notification', heartbeat);
  }
}, 30000);

const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
  console.log(`\n🚀 Real-time Notification Server running on http://localhost:${PORT}`);
  console.log(`📡 Socket.IO ready for connections`);
  console.log(`📋 REST API available at http://localhost:${PORT}/api\n`);
});
