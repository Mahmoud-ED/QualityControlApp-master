// Firebase Cloud Messaging Service Worker
importScripts('https://www.gstatic.com/firebasejs/9.0.0/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/9.0.0/firebase-messaging-compat.js');

// Initialize Firebase in the service worker
const firebaseConfig = {
    apiKey: "AIzaSyCBfqhW3YGR2szQOqYV4ohHbhzzragkoV4",
    authDomain: "lycaa-6d9ca.firebaseapp.com",
    projectId: "lycaa-6d9ca",
    storageBucket: "lycaa-6d9ca.firebasestorage.app",
    messagingSenderId: "396269616265",
    appId: "1:396269616265:web:11b5e2c7f69620965fda58"
};

firebase.initializeApp(firebaseConfig);

// Retrieve an instance of Firebase Messaging so that it can handle background messages
const messaging = firebase.messaging();

// Handle background messages
messaging.onBackgroundMessage(function(payload) {
    console.log('[firebase-messaging-sw.js] Received background message ', payload);
    
    const notificationTitle = payload.notification.title;
    const notificationOptions = {
        body: payload.notification.body,
        icon: payload.notification.image || '/img/logo.png',
        badge: '/img/badge.png',
        tag: 'fcm-notification',
        requireInteraction: true,
        actions: [
            {
                action: 'open',
                title: 'Open',
                icon: '/img/open.png'
            },
            {
                action: 'close',
                title: 'Close',
                icon: '/img/close.png'
            }
        ],
        data: payload.data
    };

    self.registration.showNotification(notificationTitle, notificationOptions);
});

// Handle notification click
self.addEventListener('notificationclick', function(event) {
    console.log('[firebase-messaging-sw.js] Notification click received.');
    
    event.notification.close();
    
    if (event.action === 'open' || !event.action) {
        // Handle the notification click
        const urlToOpen = event.notification.data?.click_action || '/';
        
        event.waitUntil(
            clients.matchAll({
                type: 'window',
                includeUncontrolled: true
            }).then(function(clientList) {
                // Check if there's already a window/tab open with the target URL
                for (var i = 0; i < clientList.length; i++) {
                    var client = clientList[i];
                    if (client.url === urlToOpen && 'focus' in client) {
                        return client.focus();
                    }
                }
                
                // If no existing window, open a new one
                if (clients.openWindow) {
                    return clients.openWindow(urlToOpen);
                }
            })
        );
    } else if (event.action === 'close') {
        // Just close the notification
        event.notification.close();
    }
});

// Handle notification close
self.addEventListener('notificationclose', function(event) {
    console.log('[firebase-messaging-sw.js] Notification closed.');
});
