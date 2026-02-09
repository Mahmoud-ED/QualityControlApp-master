// Landing Page Improvements
// Enhanced user experience and security features

document.addEventListener('DOMContentLoaded', function() {
    initializeLandingImprovements();
});

function initializeLandingImprovements() {
    // Add loading states
    addLoadingStates();
    
    // Add form validation enhancements
    enhanceFormValidation();
    
    // Add auto-save functionality
    addAutoSave();
    
    // Add security enhancements
    addSecurityEnhancements();
    
    // Add accessibility improvements
    addAccessibilityImprovements();
    
    // Add responsive improvements
    addResponsiveImprovements();
}

// Loading States
function addLoadingStates() {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn) {
                const originalText = submitBtn.innerHTML;
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Processing...';
                submitBtn.disabled = true;
                
                // Re-enable after 30 seconds as fallback
                setTimeout(() => {
                    submitBtn.innerHTML = originalText;
                    submitBtn.disabled = false;
                }, 30000);
            }
        });
    });
}

// Enhanced Form Validation
function enhanceFormValidation() {
    const forms = document.querySelectorAll('form.needs-validation');
    forms.forEach(form => {
        // Real-time validation
        const inputs = form.querySelectorAll('input, select, textarea');
        inputs.forEach(input => {
            input.addEventListener('blur', function() {
                validateField(this);
            });
            
            input.addEventListener('input', function() {
                clearFieldError(this);
            });
        });
        
        // Form submission validation
        form.addEventListener('submit', function(e) {
            if (!form.checkValidity()) {
                e.preventDefault();
                e.stopPropagation();
                
                // Focus on first invalid field
                const firstInvalid = form.querySelector(':invalid');
                if (firstInvalid) {
                    firstInvalid.focus();
                    firstInvalid.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }
            }
            form.classList.add('was-validated');
        });
    });
}

function validateField(field) {
    const value = field.value.trim();
    const fieldName = field.name;
    
    // Clear previous errors
    clearFieldError(field);
    
    // Custom validation rules
    if (fieldName === 'AircraftRegistration') {
        if (value && !/^[A-Z0-9-]{3,20}$/.test(value)) {
            showFieldError(field, 'Aircraft registration must contain only uppercase letters, numbers, and hyphens (3-20 characters)');
            return false;
        }
    }
    
    if (fieldName === 'FlightNumber') {
        if (value && !/^[A-Z0-9-]{2,20}$/.test(value)) {
            showFieldError(field, 'Flight number must be 2-20 characters and contain only uppercase letters, numbers, and hyphens');
            return false;
        }
    }
    
    if (fieldName === 'Email') {
        if (value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
            showFieldError(field, 'Please enter a valid email address');
            return false;
        }
    }
    
    if (fieldName === 'ETA' || fieldName === 'ETD') {
        const dateValue = new Date(value);
        if (isNaN(dateValue.getTime())) {
            showFieldError(field, 'Please enter a valid date and time');
            return false;
        }
        
        // Check if ETA is before ETD
        if (fieldName === 'ETD') {
            const etaField = document.querySelector('input[name="ETA"]');
            if (etaField && etaField.value) {
                const etaValue = new Date(etaField.value);
                const timeDifference = dateValue - etaValue;
                // Allow ETD to be within 24 hours of ETA for landing operations
                if (timeDifference < -24 * 60 * 60 * 1000) { // -24 hours in milliseconds
                    showFieldError(field, 'Estimated departure time must be within 24 hours of estimated arrival time');
                    return false;
                }
                if (timeDifference > 24 * 60 * 60 * 1000) { // +24 hours in milliseconds
                    showFieldError(field, 'Estimated departure time must be within 24 hours of estimated arrival time');
                    return false;
                }
            }
        }
    }
    
    return true;
}

function showFieldError(field, message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'invalid-feedback d-block';
    errorDiv.textContent = message;
    
    field.classList.add('is-invalid');
    field.parentNode.appendChild(errorDiv);
}

function clearFieldError(field) {
    field.classList.remove('is-invalid');
    const errorDiv = field.parentNode.querySelector('.invalid-feedback');
    if (errorDiv) {
        errorDiv.remove();
    }
}

// Auto-save functionality
function addAutoSave() {
    const forms = document.querySelectorAll('form[data-autosave="true"]');
    forms.forEach(form => {
        const inputs = form.querySelectorAll('input, select, textarea');
        let saveTimeout;
        
        inputs.forEach(input => {
            input.addEventListener('input', function() {
                clearTimeout(saveTimeout);
                saveTimeout = setTimeout(() => {
                    saveFormData(form);
                }, 2000); // Save after 2 seconds of inactivity
            });
        });
    });
}

function saveFormData(form) {
    const formData = new FormData(form);
    const data = Object.fromEntries(formData.entries());
    
    // Save to localStorage
    localStorage.setItem('landing_form_draft', JSON.stringify(data));
    
    // Show save indicator
    showSaveIndicator('Draft saved locally');
}

function loadFormData(form) {
    const savedData = localStorage.getItem('landing_form_draft');
    if (savedData) {
        try {
            const data = JSON.parse(savedData);
            Object.keys(data).forEach(key => {
                const field = form.querySelector(`[name="${key}"]`);
                if (field && field.type !== 'file') {
                    field.value = data[key];
                }
            });
        } catch (e) {
            console.error('Error loading saved form data:', e);
        }
    }
}

// Security Enhancements
function addSecurityEnhancements() {
    // Sanitize file names
    const fileInputs = document.querySelectorAll('input[type="file"]');
    fileInputs.forEach(input => {
        input.addEventListener('change', function() {
            const file = this.files[0];
            if (file) {
                // Check file size (10MB limit)
                if (file.size > 10 * 1024 * 1024) {
                    alert('File size must be less than 10MB');
                    this.value = '';
                    return;
                }
                
                // Check file extension
                const allowedExtensions = ['.pdf', '.doc', '.docx', '.jpg', '.jpeg', '.png'];
                const fileExtension = '.' + file.name.split('.').pop().toLowerCase();
                if (!allowedExtensions.includes(fileExtension)) {
                    alert('File type not allowed. Allowed types: ' + allowedExtensions.join(', '));
                    this.value = '';
                    return;
                }
                
                // Sanitize file name
                const sanitizedName = file.name.replace(/[^a-zA-Z0-9.-]/g, '_');
                if (sanitizedName !== file.name) {
                    console.warn('File name was sanitized:', file.name, '->', sanitizedName);
                }
            }
        });
    });
    
    // Prevent XSS in text inputs
    const textInputs = document.querySelectorAll('input[type="text"], textarea');
    textInputs.forEach(input => {
        input.addEventListener('input', function() {
            // Basic XSS prevention
            this.value = this.value.replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, '');
        });
    });
}

// Accessibility Improvements
function addAccessibilityImprovements() {
    // Add ARIA labels
    const inputs = document.querySelectorAll('input, select, textarea');
    inputs.forEach(input => {
        if (!input.getAttribute('aria-label') && !input.getAttribute('aria-labelledby')) {
            const label = document.querySelector(`label[for="${input.id}"]`);
            if (label) {
                input.setAttribute('aria-label', label.textContent);
            }
        }
    });
    
    // Add keyboard navigation
    const focusableElements = document.querySelectorAll('input, select, textarea, button, a');
    focusableElements.forEach((element, index) => {
        element.addEventListener('keydown', function(e) {
            if (e.key === 'Tab') {
                // Handle tab navigation
                const nextElement = focusableElements[index + 1];
                const prevElement = focusableElements[index - 1];
                
                if (e.shiftKey && prevElement) {
                    e.preventDefault();
                    prevElement.focus();
                } else if (!e.shiftKey && nextElement) {
                    // Let default tab behavior handle this
                }
            }
        });
    });
    
    // Add screen reader announcements
    const statusMessages = document.querySelectorAll('.alert, .toast');
    statusMessages.forEach(message => {
        message.setAttribute('role', 'alert');
        message.setAttribute('aria-live', 'polite');
    });
}

// Responsive Improvements
function addResponsiveImprovements() {
    // Handle mobile viewport
    const viewport = document.querySelector('meta[name="viewport"]');
    if (!viewport) {
        const meta = document.createElement('meta');
        meta.name = 'viewport';
        meta.content = 'width=device-width, initial-scale=1.0';
        document.head.appendChild(meta);
    }
    
    // Add touch gestures for mobile
    if ('ontouchstart' in window) {
        const cards = document.querySelectorAll('.card');
        cards.forEach(card => {
            card.addEventListener('touchstart', function(e) {
                this.classList.add('touch-active');
            });
            
            card.addEventListener('touchend', function(e) {
                this.classList.remove('touch-active');
            });
        });
    }
    
    // Responsive table handling
    const tables = document.querySelectorAll('table');
    tables.forEach(table => {
        if (table.offsetWidth > table.parentElement.offsetWidth) {
            table.parentElement.style.overflowX = 'auto';
            table.parentElement.style.webkitOverflowScrolling = 'touch';
        }
    });
}

// Utility Functions
function showSaveIndicator(message) {
    const indicator = document.createElement('div');
    indicator.className = 'toast position-fixed top-0 end-0 m-3';
    indicator.style.zIndex = '9999';
    indicator.innerHTML = `
        <div class="toast-header">
            <i class="fas fa-save text-success me-2"></i>
            <strong class="me-auto">Auto-save</strong>
            <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
        </div>
        <div class="toast-body">
            ${message}
        </div>
    `;
    
    document.body.appendChild(indicator);
    
    const toast = new bootstrap.Toast(indicator);
    toast.show();
    
    // Remove after 3 seconds
    setTimeout(() => {
        indicator.remove();
    }, 3000);
}

function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    notification.innerHTML = `
        <i class="fas fa-${type === 'success' ? 'check-circle' : 'info-circle'} me-2"></i>
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.remove();
    }, 5000);
}

// Error handling
window.addEventListener('error', function(e) {
    console.error('JavaScript error:', e.error);
    showNotification('An error occurred. Please refresh the page.', 'danger');
});

// Unhandled promise rejections
window.addEventListener('unhandledrejection', function(e) {
    console.error('Unhandled promise rejection:', e.reason);
    showNotification('An error occurred. Please try again.', 'danger');
});

// Export functions for global use
window.LandingImprovements = {
    showNotification,
    showSaveIndicator,
    validateField,
    clearFieldError
};
