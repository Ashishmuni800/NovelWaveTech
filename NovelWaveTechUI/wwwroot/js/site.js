// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


//function loadCaptcha() {
//    const captchaImage = document.getElementById('captchaImage');
//    if (captchaImage) {
//        captchaImage.src = `https://localhost:7112/api/UserAuth/GenerateCaptcha`;
//    }
//    const captchaInput = document.getElementById('CaptchaCode');
//    if (captchaInput) {
//        captchaInput.value = '';
//    }
//}

// Toggle password visibility
document.querySelectorAll('.toggle-password').forEach(icon => {
    icon.addEventListener('click', function () {
        const targetInput = document.querySelector(this.dataset.toggle);
        if (targetInput) {
            const type = targetInput.type === 'password' ? 'text' : 'password';
            targetInput.type = type;
            this.classList.toggle('fa-eye');
            this.classList.toggle('fa-eye-slash');
        }
    });
});

// Password strength meter
document.getElementById('newPassword').addEventListener('input', function () {
    const val = this.value;
    const strengthBar = document.querySelector('#passwordStrength .progress-bar');
    let strength = 0;

    if (val.length >= 8) strength += 25;
    if (/[A-Z]/.test(val)) strength += 25;
    if (/[a-z]/.test(val)) strength += 15;
    if (/\d/.test(val)) strength += 15;
    if (/[@$!%*?&\-_#^]/.test(val)) strength += 20;

    strengthBar.style.width = `${strength}%`;

    if (strength < 40) {
        strengthBar.className = 'progress-bar bg-danger';
    } else if (strength < 75) {
        strengthBar.className = 'progress-bar bg-warning';
    } else {
        strengthBar.className = 'progress-bar bg-success';
    }
});

// Handle form submission
const passwordForm = document.getElementById('passwordForm');
passwordForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const userName = document.getElementById('userName')?.value.trim();
    const password = document.getElementById('password')?.value;
    const newPassword = document.getElementById('newPassword')?.value;
    const confirmNewPassword = document.getElementById('confirmNewPassword')?.value;
    const CaptchaCode = document.getElementById('CaptchaCode')?.value;
    const Id = document.getElementById('Id')?.value;
    const statusMessage = document.getElementById('statusMessage');
    const spinner = document.getElementById('submitSpinner');
    const submitButton = passwordForm.querySelector('button[type="submit"]');

    // Reset UI state
    statusMessage.innerHTML = '';
    submitButton.disabled = true;
    spinner.classList.remove('d-none');

    // Validation
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\-_#^])[A-Za-z\d@$!%*?&\-_#^]{8,}$/;
    if (!userName || !password || !newPassword || !confirmNewPassword || !CaptchaCode) {
        statusMessage.innerHTML = '<div class="alert alert-warning">All fields are required.</div>';
        resetFormUI();
        return;
    }

    if (!passwordRegex.test(newPassword)) {
        statusMessage.innerHTML = `<div class="alert alert-warning">
       Password must be at least 8 characters and include uppercase, lowercase, number, and special character.
     </div>`;
        resetFormUI();
        return;
    }

    if (newPassword !== confirmNewPassword) {
        statusMessage.innerHTML = '<div class="alert alert-warning">New passwords do not match.</div>';
        resetFormUI();
        return;
    }

    const jwtToken = localStorage.getItem('token');
    if (!jwtToken) {
        statusMessage.innerHTML = '<div class="alert alert-danger">You are not authenticated. Please log in again.</div>';
        resetFormUI();
        return;
    }

    try {
        const captchaResponse = await fetch(`/Home/GetCaptcha?CaptchaCode=${CaptchaCode}`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        });

        if (!captchaResponse.ok) {
            const err = await captchaResponse.json();
            statusMessage.innerHTML = `<div class="alert alert-danger">${err.message || 'Invalid CAPTCHA. Try again.'}</div>`;
            loadCaptcha();
            resetFormUI();
            return;
        }

        const response = await fetch(`/Home/PasswordChangePost`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwtToken}`
            },
            body: JSON.stringify({
                userName,
                password,
                newPassword,
                confirmNewPassword,
                CaptchaCode,
                Id
            })
        });

        if (response.ok) {
            statusMessage.innerHTML = '<div class="alert alert-success">Password successfully changed!</div>';
            setTimeout(() => window.location.href = '/Home/Index', 1500);
        } else {
            const err = await response.json();
            statusMessage.innerHTML = `<div class="alert alert-danger">${err.message || 'Password change failed.'}</div>`;
            loadCaptcha();
            resetFormUI();
        }

    } catch (err) {
        console.error('Unexpected error:', err);
        statusMessage.innerHTML = '<div class="alert alert-danger">An unexpected error occurred. Please try again.</div>';
        loadCaptcha();
        resetFormUI();
    }

    function resetFormUI() {
        spinner.classList.add('d-none');
        submitButton.disabled = false;
        submitButton.innerHTML = `<span class="spinner-border spinner-border-sm me-2 d-none" id="submitSpinner"></span> Change Password`;
    }
});
async function loadCaptcha() {
    try {
        const response = await fetch(`/Home/Captcha`);
        const blob = await response.blob();
        const idHeader = response.headers.get("Content-Disposition");
        const id = idHeader?.split("filename=")[1] || '';
        const imageUrl = URL.createObjectURL(blob);
        document.getElementById("img-captcha").src = imageUrl;
        document.getElementById("Id").value = id;
    } catch (err) {
        console.error('Captcha load failed:', err);
        statusMessage.innerHTML = '<div class="alert alert-warning">Could not load captcha. Please refresh the page.</div>';
    }
    const captchaInput = document.getElementById('CaptchaCode');
    if (captchaInput) {
        captchaInput.value = '';
    }
}

// Initial load
window.addEventListener('load', loadCaptcha);
document.getElementById('captchaRefresh').addEventListener('click', loadCaptcha);
// Load CAPTCHA on initial page load
//window.addEventListener('DOMContentLoaded', loadCaptcha);