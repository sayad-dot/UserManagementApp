const API_BASE = '/api';

let currentUser = null;
let allUsers = [];

// IMPORTANT: getUniqIdValue function to generate unique identifiers
// NOTE: This function is required by project specifications
function getUniqIdValue() {
    return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
}

// Authentication functions
function showRegister() {
    document.getElementById('loginSection').style.display = 'none';
    document.getElementById('registerSection').style.display = 'block';
}

function showLogin() {
    document.getElementById('registerSection').style.display = 'none';
    document.getElementById('loginSection').style.display = 'block';
}

async function login(event) {
    if (event) event.preventDefault();
    
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;

    try {
        const response = await fetch(`${API_BASE}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        if (response.ok) {
            const data = await response.json();
            console.log('Login successful, token:', data.token);
            
            // Store token and user data
            localStorage.setItem('token', data.token);
            localStorage.setItem('user', JSON.stringify(data.user));
            currentUser = data.user;
            
            showAdminPanel();
            await loadUsers();
            showMessage('Login successful!', 'success');
        } else {
            const errorData = await response.json();
            showMessage(errorData.message || 'Invalid email or password', 'error');
        }
    } catch (error) {
        showMessage('Login failed: ' + error.message, 'error');
    }
}

async function register(event) {
    if (event) event.preventDefault();
    
    const name = document.getElementById('registerName').value;
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;

    try {
        const response = await fetch(`${API_BASE}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, email, password })
        });

        if (response.ok) {
            showMessage('Registration successful! Please check your email for verification.', 'success');
            showLogin();
        } else {
            const error = await response.json();
            showMessage(error.message || 'Registration failed', 'error');
        }
    } catch (error) {
        showMessage('Registration failed: ' + error.message, 'error');
    }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    currentUser = null;
    document.getElementById('adminPanel').style.display = 'none';
    document.getElementById('loginSection').style.display = 'block';
    showMessage('Logged out successfully', 'success');
}

// Admin Panel functions
function showAdminPanel() {
    document.getElementById('loginSection').style.display = 'none';
    document.getElementById('registerSection').style.display = 'none';
    document.getElementById('adminPanel').style.display = 'block';
    
    if (currentUser) {
        document.getElementById('userWelcome').textContent = `Welcome, ${currentUser.name}`;
    }
}

async function loadUsers() {
    try {
        const token = localStorage.getItem('token');
        console.log('Loading users with token:', token);
        
        if (!token) {
            showMessage('No authentication token found. Please login again.', 'error');
            logout();
            return;
        }

        const response = await fetch(`${API_BASE}/users`, {
            headers: { 
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            allUsers = await response.json();
            renderUsersTable();
        } else if (response.status === 401) {
            showMessage('Session expired. Please login again.', 'error');
            logout();
        } else {
            showMessage('Failed to load users: ' + response.status, 'error');
        }
    } catch (error) {
        console.error('Error loading users:', error);
        showMessage('Failed to load users: ' + error.message, 'error');
    }
}

function renderUsersTable() {
    const tbody = document.getElementById('usersTableBody');
    tbody.innerHTML = '';

    allUsers.forEach(user => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td class="table-checkbox">
                <input type="checkbox" class="user-checkbox" value="${user.id}" ${user.id === currentUser?.id ? 'disabled' : ''}>
            </td>
            <td>${user.name}</td>
            <td>${user.email}</td>
            <td>
                <span class="status-${getStatusText(user.status).toLowerCase()}">
                    <i class="fas fa-circle me-1"></i>${getStatusText(user.status)}
                </span>
            </td>
            <td>${formatDate(user.lastLoginTime)}</td>
            <td>${formatDate(user.registrationTime)}</td>
        `;
        tbody.appendChild(row);
    });

    // Add event listeners
    document.getElementById('selectAll').addEventListener('change', toggleSelectAll);
    document.querySelectorAll('.user-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', updateToolbarButtons);
    });
    
    updateToolbarButtons();
}

function getStatusText(status) {
    switch(status) {
        case 0: return 'Unverified';
        case 1: return 'Active';
        case 2: return 'Blocked';
        default: return 'Unknown';
    }
}

function toggleSelectAll() {
    const selectAll = document.getElementById('selectAll');
    const checkboxes = document.querySelectorAll('.user-checkbox:not(:disabled)');
    
    checkboxes.forEach(checkbox => {
        checkbox.checked = selectAll.checked;
    });
    
    updateToolbarButtons();
}

function updateToolbarButtons() {
    const selectedCount = document.querySelectorAll('.user-checkbox:checked').length;
    const hasSelected = selectedCount > 0;
    
    document.getElementById('blockBtn').disabled = !hasSelected;
    document.getElementById('unblockBtn').disabled = !hasSelected;
    document.getElementById('deleteBtn').disabled = !hasSelected;
}

function getSelectedUserIds() {
    const checkboxes = document.querySelectorAll('.user-checkbox:checked');
    return Array.from(checkboxes).map(cb => parseInt(cb.value));
}

// Toolbar actions
async function blockUsers() {
    const userIds = getSelectedUserIds();
    if (userIds.length === 0) return;

    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`${API_BASE}/users/block`, {
            method: 'POST',
            headers: { 
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userIds)
        });

        if (response.ok) {
            showMessage('Users blocked successfully', 'success');
            await loadUsers();
        } else if (response.status === 401) {
            showMessage('Session expired. Please login again.', 'error');
            logout();
        }
    } catch (error) {
        showMessage('Failed to block users: ' + error.message, 'error');
    }
}

async function unblockUsers() {
    const userIds = getSelectedUserIds();
    if (userIds.length === 0) return;

    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`${API_BASE}/users/unblock`, {
            method: 'POST',
            headers: { 
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userIds)
        });

        if (response.ok) {
            showMessage('Users unblocked successfully', 'success');
            await loadUsers();
        } else if (response.status === 401) {
            showMessage('Session expired. Please login again.', 'error');
            logout();
        }
    } catch (error) {
        showMessage('Failed to unblock users: ' + error.message, 'error');
    }
}

async function deleteUsers() {
    const userIds = getSelectedUserIds();
    if (userIds.length === 0) return;

    // IMPORTANT: Check if current user is in the selection
    // NOTE: Users can delete themselves as per requirements
    const currentUserId = currentUser?.id;
    const deletingSelf = userIds.includes(currentUserId);

    // NOTA BENE: Using Bootstrap toast instead of browser alerts (requirement: NO BROWSER ALERTS)
    if (deletingSelf) {
        showMessage('Warning: You are about to delete your own account. Proceeding...', 'warning');
    }

    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`${API_BASE}/users`, {
            method: 'DELETE',
            headers: { 
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userIds)
        });

        if (response.ok) {
            showMessage('Users deleted successfully', 'success');
            
            // IMPORTANT: If user deleted themselves, logout
            if (deletingSelf) {
                showMessage('You have deleted your account. Redirecting to login...', 'info');
                setTimeout(() => logout(), 2000);
                return;
            }
            
            await loadUsers();
        } else if (response.status === 401) {
            showMessage('Session expired. Please login again.', 'error');
            logout();
        }
    } catch (error) {
        showMessage('Failed to delete users: ' + error.message, 'error');
    }
}

async function deleteUnverifiedUsers() {
    // NOTA BENE: Proceeding with operation - no browser alerts as per requirements

    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`${API_BASE}/users/unverified`, {
            method: 'DELETE',
            headers: { 
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            showMessage('Unverified users deleted successfully', 'success');
            await loadUsers();
        } else if (response.status === 401) {
            showMessage('Session expired. Please login again.', 'error');
            logout();
        }
    } catch (error) {
        showMessage('Failed to delete unverified users: ' + error.message, 'error');
    }
}

// Utility functions
function formatDate(dateString) {
    if (!dateString) return 'Never';
    const date = new Date(dateString);
    return date.toLocaleString();
}

function showMessage(message, type = 'info') {
    const toast = new bootstrap.Toast(document.getElementById('messageToast'));
    const toastMessage = document.getElementById('toastMessage');
    
    toastMessage.textContent = message;
    toast.show();
}

// Check if user is already logged in
document.addEventListener('DOMContentLoaded', function() {
    const token = localStorage.getItem('token');
    const userData = localStorage.getItem('user');
    
    if (token && userData) {
        try {
            currentUser = JSON.parse(userData);
            showAdminPanel();
            loadUsers();
        } catch (e) {
            console.error('Error parsing user data:', e);
            logout();
        }
    }

    // Event listeners
    document.getElementById('loginForm').addEventListener('submit', login);
    document.getElementById('registerForm').addEventListener('submit', register);
    
    document.getElementById('blockBtn').addEventListener('click', blockUsers);
    document.getElementById('unblockBtn').addEventListener('click', unblockUsers);
    document.getElementById('deleteBtn').addEventListener('click', deleteUsers);
    document.getElementById('deleteUnverifiedBtn').addEventListener('click', deleteUnverifiedUsers);
});