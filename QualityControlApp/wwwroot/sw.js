// sw.js

const CACHE_NAME = 'my-app-cache-v1'; // Change version when assets update
const urlsToCache = [
    '/', // Cache the root/start_url
    '/css/site.css', // Cache core CSS
    '/js/site.js',   // Cache core JS
    '/img/icon.png', // Cache essential icons
    '/img/icon.png',
    '/manifest.json',
    '/offline.html' // An optional offline fallback page
    // Add other essential assets: core pages, fonts, images etc.
];

// Install event: Cache core assets
self.addEventListener('install', event => {
    console.log('Service Worker: Installing...');
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                console.log('Service Worker: Caching app shell');
                return cache.addAll(urlsToCache);
            })
            .then(() => {
                console.log('Service Worker: Install completed, skipping waiting.');
                // Force the waiting service worker to become the active service worker.
                return self.skipWaiting();
            })
    );
});

// Activate event: Clean up old caches
self.addEventListener('activate', event => {
    console.log('Service Worker: Activating...');
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheName !== CACHE_NAME) {
                        console.log('Service Worker: Clearing old cache:', cacheName);
                        return caches.delete(cacheName);
                    }
                })
            );
        }).then(() => {
            console.log('Service Worker: Activation completed, claiming clients.');
            // Take control of currently open pages immediately.
            return self.clients.claim();
        })
    );
});

// Fetch event: Serve from cache if available, otherwise fetch from network
// Basic Cache-First strategy (for cached assets)
self.addEventListener('fetch', event => {
    //console.log('Service Worker: Fetching', event.request.url);
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                // Cache hit - return response
                if (response) {
                    // console.log('Service Worker: Serving from cache:', event.request.url);
                    return response;
                }

                // Not in cache - fetch from network
                // console.log('Service Worker: Fetching from network:', event.request.url);
                return fetch(event.request).then(networkResponse => {
                    // Optional: Cache dynamically fetched resources if needed
                    // Be careful what you cache here! Don't cache API responses
                    // unless you have a specific strategy (e.g., stale-while-revalidate).
                    return networkResponse;
                }
                ).catch(error => {
                    // Network fetch failed (offline)
                    console.log('Service Worker: Network fetch failed for:', event.request.url, error);
                    // Optionally, return a fallback offline page for navigation requests
                    if (event.request.mode === 'navigate') {
                        return caches.match('/offline.html');
                    }
                    // For other requests (CSS, JS, images), just fail.
                });
            })
    );
});