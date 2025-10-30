/* ========================================
   CO-OWNER VEHICLE - JAVASCRIPT
   ======================================== */

// Wait for DOM to be ready
document.addEventListener('DOMContentLoaded', function() {
    
    // ===== Sidebar Toggle =====
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');
    
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function() {
            sidebar.classList.toggle('show');
            
            // Close sidebar when clicking outside on mobile
            if (window.innerWidth < 992) {
                document.addEventListener('click', function(e) {
                    if (!sidebar.contains(e.target) && !sidebarToggle.contains(e.target)) {
                        sidebar.classList.remove('show');
                    }
                });
            }
        });
    }
    
    // ===== Active Link Highlighting =====
    const currentPath = window.location.pathname;
    const sidebarLinks = document.querySelectorAll('.sidebar-link');
    
    sidebarLinks.forEach(link => {
        const linkPath = link.getAttribute('href');
        
        // Remove active class from all links
        link.classList.remove('active');
        
        // Add active class to current page link
        if (linkPath && currentPath.includes(linkPath)) {
            link.classList.add('active');
        }
    });
    
    // ===== Smooth Scroll =====
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
    
    // ===== Tooltips Initialization =====
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // ===== Popovers Initialization =====
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
    
    // ===== Auto-hide alerts =====
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
    
    // ===== Number Animation =====
    function animateValue(element, start, end, duration) {
        let startTimestamp = null;
        const step = (timestamp) => {
            if (!startTimestamp) startTimestamp = timestamp;
            const progress = Math.min((timestamp - startTimestamp) / duration, 1);
            const value = Math.floor(progress * (end - start) + start);
            element.textContent = value.toLocaleString('vi-VN');
            if (progress < 1) {
                window.requestAnimationFrame(step);
            }
        };
        window.requestAnimationFrame(step);
    }
    
    // Animate numbers on stat cards
    const statValues = document.querySelectorAll('.stat-card-value[data-value]');
    statValues.forEach(stat => {
        const finalValue = parseInt(stat.getAttribute('data-value'));
        animateValue(stat, 0, finalValue, 1000);
    });
    
    // ===== Form Validation Enhancement =====
    const forms = document.querySelectorAll('.needs-validation');
    forms.forEach(form => {
        form.addEventListener('submit', function(event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
    
    // ===== Search Functionality =====
    const searchInput = document.querySelector('.header-search input');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function(e) {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                const searchTerm = e.target.value.toLowerCase();
                console.log('Searching for:', searchTerm);
                // TODO: Implement search functionality
            }, 300);
        });
    }
    
    // ===== Notification Mark as Read =====
    const notificationItems = document.querySelectorAll('.notification-item');
    notificationItems.forEach(item => {
        item.addEventListener('click', function(e) {
            this.style.opacity = '0.6';
            // TODO: Mark notification as read in backend
        });
    });
    
    // ===== Date/Time Formatting =====
    function formatDate(dateString) {
        const options = { 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        };
        return new Date(dateString).toLocaleDateString('vi-VN', options);
    }
    
    // Format all datetime elements
    const datetimeElements = document.querySelectorAll('[data-datetime]');
    datetimeElements.forEach(element => {
        const datetime = element.getAttribute('data-datetime');
        element.textContent = formatDate(datetime);
    });
    
    // ===== Responsive Table Wrapper =====
    const tables = document.querySelectorAll('table:not(.table-responsive table)');
    tables.forEach(table => {
        if (!table.parentElement.classList.contains('table-responsive')) {
            const wrapper = document.createElement('div');
            wrapper.classList.add('table-responsive');
            table.parentNode.insertBefore(wrapper, table);
            wrapper.appendChild(table);
        }
    });
    
    // ===== Copy to Clipboard =====
    window.copyToClipboard = function(text, button) {
        navigator.clipboard.writeText(text).then(() => {
            // Show success feedback
            const originalHTML = button.innerHTML;
            button.innerHTML = '<i class="bi bi-check"></i> Đã sao chép!';
            button.classList.add('btn-success');
            
            setTimeout(() => {
                button.innerHTML = originalHTML;
                button.classList.remove('btn-success');
            }, 2000);
        }).catch(err => {
            console.error('Failed to copy:', err);
        });
    };
    
    // ===== Confirm Delete =====
    window.confirmDelete = function(message) {
        return confirm(message || 'Bạn có chắc chắn muốn xóa?');
    };
    
    // ===== Loading State =====
    window.showLoading = function(button) {
        button.disabled = true;
        button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Đang xử lý...';
    };
    
    // ===== Format Currency =====
    window.formatCurrency = function(amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    };
    
    // ===== Print Page =====
    window.printPage = function() {
        window.print();
    };
    
    // ===== Log for debugging =====
    console.log('Co-owner Vehicle App Loaded ✓');
});

// ===== Service Worker Registration (PWA Support) =====
if ('serviceWorker' in navigator) {
    window.addEventListener('load', () => {
        // Uncomment when PWA is ready
        // navigator.serviceWorker.register('/sw.js');
    });
}
