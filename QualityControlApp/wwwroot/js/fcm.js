// Firebase Cloud Messaging Client-side JavaScript
class FCMClient {
    constructor() {
        this.messaging = null;
        this.isSupported = false;
        this.isInitialized = false;
        this.currentToken = null;
        this.deviceId = this.generateDeviceId();
        this.init();
    }

    async init() {
        try {
            // Check if the browser supports service workers and notifications
            if (!('serviceWorker' in navigator) || !('Notification' in window)) {
                console.warn('This browser does not support service workers or notifications');
                return;
            }

            // Check if Firebase is available
            if (typeof firebase === 'undefined') {
                console.error('Firebase is not loaded. Please include Firebase SDK.');
                return;
            }

            // Initialize Firebase
            const firebaseConfig = this.getFirebaseConfig();
            if (!firebaseConfig) {
                console.error('Firebase configuration not found');
                return;
            }

            if (!firebase.apps.length) {
                firebase.initializeApp(firebaseConfig);
            }

            this.messaging = firebase.messaging();
            this.isSupported = true;
            this.isInitialized = true;

            // Request notification permission
            await this.requestPermission();

            // Get and register the token
            await this.getToken();

            // Set up message handlers
            this.setupMessageHandlers();

            console.log('FCM Client initialized successfully');
        } catch (error) {
            console.error('Failed to initialize FCM Client:', error);
        }
    }

    getFirebaseConfig() {
        // Try to get config from window object (injected by server)
        if (window.firebaseConfig) {
            return window.firebaseConfig;
        }

        // Fallback config - you should replace this with your actual Firebase config
        return {
            apiKey: "your-api-key",
            authDomain: "your-project.firebaseapp.com",
            projectId: "your-project-id",
            storageBucket: "your-project.appspot.com",
            messagingSenderId: "123456789",
            appId: "your-app-id"
        };
    }

    generateDeviceId() {
        let deviceId = localStorage.getItem('fcm_device_id');
        if (!deviceId) {
            deviceId = 'device_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
            localStorage.setItem('fcm_device_id', deviceId);
        }
        return deviceId;
    }

    async requestPermission() {
        try {
            const permission = await Notification.requestPermission();
            if (permission === 'granted') {
                console.log('Notification permission granted');
                return true;
            } else {
                console.log('Notification permission denied');
                return false;
            }
        } catch (error) {
            console.error('Error requesting notification permission:', error);
            return false;
        }
    }

    async getToken() {
        try {
            if (!this.messaging) {
                throw new Error('Messaging not initialized');
            }

            // Register service worker
            const registration = await navigator.serviceWorker.register('/js/firebase-messaging-sw.js');
            console.log('Service Worker registered:', registration);

            // Get the token
            this.currentToken = await this.messaging.getToken({
                vapidKey: this.getVapidKey(),
                serviceWorkerRegistration: registration
            });

            if (this.currentToken) {
                console.log('FCM Token obtained:', this.currentToken);
                await this.registerToken();
            } else {
                console.log('No registration token available');
            }

            return this.currentToken;
        } catch (error) {
            console.error('Error getting FCM token:', error);
            return null;
        }
    }

    getVapidKey() {
        // Try to get VAPID key from window object (injected by server)
        if (window.firebaseVapidKey) {
            return window.firebaseVapidKey;
        }

        // Fallback VAPID key - you should replace this with your actual VAPID key
        return "your-vapid-key";
    }

    async registerToken() {
        try {
            if (!this.currentToken) {
                return;
            }

            const response = await fetch('/Fcm/RegisterToken', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    token: this.currentToken,
                    deviceId: this.deviceId,
                    deviceType: 'web',
                    browserInfo: this.getBrowserInfo(),
                    userAgent: navigator.userAgent
                })
            });

            if (response.ok) {
                const result = await response.json();
                console.log('Token registered successfully:', result);
                localStorage.setItem('fcm_token_registered', 'true');
            } else {
                console.error('Failed to register token:', response.statusText);
            }
        } catch (error) {
            console.error('Error registering token:', error);
        }
    }

    getBrowserInfo() {
        const browser = this.getBrowserName();
        const os = this.getOperatingSystem();
        return `${browser} on ${os}`;
    }

    getBrowserName() {
        const userAgent = navigator.userAgent;
        if (userAgent.includes('Chrome')) return 'Chrome';
        if (userAgent.includes('Firefox')) return 'Firefox';
        if (userAgent.includes('Safari')) return 'Safari';
        if (userAgent.includes('Edge')) return 'Edge';
        return 'Unknown Browser';
    }

    getOperatingSystem() {
        const userAgent = navigator.userAgent;
        if (userAgent.includes('Windows')) return 'Windows';
        if (userAgent.includes('Mac')) return 'macOS';
        if (userAgent.includes('Linux')) return 'Linux';
        if (userAgent.includes('Android')) return 'Android';
        if (userAgent.includes('iOS')) return 'iOS';
        return 'Unknown OS';
    }

    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    setupMessageHandlers() {
        if (!this.messaging) {
            return;
        }

        // Handle foreground messages
        this.messaging.onMessage((payload) => {
            console.log('Message received in foreground:', payload);
            this.showNotification(payload);
        });

        // Handle token refresh
        this.messaging.onTokenRefresh(async () => {
            console.log('Token refreshed');
            await this.getToken();
        });
    }

    showNotification(payload) {
        if (Notification.permission === 'granted') {
            const notification = new Notification(payload.notification.title, {
                body: payload.notification.body,
                icon: payload.notification.image || '/img/logo.png',
                badge: '/img/badge.png',
                tag: 'fcm-notification',
                requireInteraction: true,
                data: payload.data
            });

            notification.onclick = function(event) {
                event.preventDefault();
                window.focus();
                
                if (payload.data && payload.data.click_action) {
                    window.open(payload.data.click_action, '_blank');
                }
                
                notification.close();
            };
        }
    }

    async unregisterToken() {
        try {
            if (this.currentToken) {
                const response = await fetch('/Fcm/UnregisterToken', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': this.getAntiForgeryToken()
                    },
                    body: JSON.stringify({
                        token: this.currentToken
                    })
                });

                if (response.ok) {
                    console.log('Token unregistered successfully');
                    localStorage.removeItem('fcm_token_registered');
                    this.currentToken = null;
                }
            }
        } catch (error) {
            console.error('Error unregistering token:', error);
        }
    }

    // Public methods
    async enableNotifications() {
        if (!this.isInitialized) {
            await this.init();
        }
        return await this.requestPermission();
    }

    async disableNotifications() {
        await this.unregisterToken();
    }

    getCurrentToken() {
        return this.currentToken;
    }

    getDeviceId() {
        return this.deviceId;
    }

    isNotificationSupported() {
        return this.isSupported;
    }
}

// Initialize FCM Client when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    window.fcmClient = new FCMClient();
});

// Export for use in other scripts
window.FCMClient = FCMClient;
