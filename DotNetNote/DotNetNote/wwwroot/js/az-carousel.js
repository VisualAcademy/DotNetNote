const azSlides = document.querySelectorAll('.az-slide');
const azTotalSlides = azSlides.length;
const azProgressIndicators = document.getElementById('az-progress-indicators');
const azSlideDuration = 5000;
let azCurrentIndex = 0;
let azIsPaused = false;
let azTimeout;
let azStartTime;
let azElapsed = 0;

const azProgressDots = [];

for (let i = 0; i < azTotalSlides; i++) {
    const dot = document.createElement('div');
    dot.className = 'az-progress-dot';
    dot.dataset.index = i;

    const fill = document.createElement('div');
    fill.className = 'az-progress-fill';
    dot.appendChild(fill);

    dot.addEventListener('click', () => {
        clearTimeout(azTimeout);
        azElapsed = 0;
        azUpdateSlide(i);
    });

    azProgressIndicators.appendChild(dot);
    azProgressDots.push(fill);
}

const azPauseBtn = document.createElement('div');
azPauseBtn.className = 'az-pause-button';
azPauseBtn.innerHTML = '<i class="fas fa-pause"></i>';
azProgressIndicators.appendChild(azPauseBtn);

azPauseBtn.addEventListener('click', () => {
    azIsPaused = !azIsPaused;
    azPauseBtn.innerHTML = `<i class="fas fa-${azIsPaused ? 'play' : 'pause'}"></i>`;
    if (azIsPaused) {
        azPauseProgress();
    } else {
        azResumeProgress();
    }
});

function azUpdateSlide(index) {
    azCurrentIndex = (index + azTotalSlides) % azTotalSlides;

    azSlides.forEach((slide, i) => {
        slide.classList.toggle('az-active', i === azCurrentIndex);
    });

    azProgressDots.forEach(dot => {
        dot.style.transition = 'none';
        dot.style.width = '0%';
    });

    azElapsed = 0;
    if (!azIsPaused) azStartProgress(azSlideDuration);
}

function azStartProgress(duration) {
    const dot = azProgressDots[azCurrentIndex];
    dot.style.transition = 'none';
    dot.style.width = '0%';

    requestAnimationFrame(() => {
        dot.style.transition = `width ${duration}ms linear`;
        dot.style.width = '100%';
    });

    azStartTime = Date.now();
    azTimeout = setTimeout(() => {
        azElapsed = 0;
        azUpdateSlide(azCurrentIndex + 1);
    }, duration);
}

function azPauseProgress() {
    const dot = azProgressDots[azCurrentIndex];
    const computed = parseFloat(getComputedStyle(dot).width);
    const parentWidth = dot.parentElement.offsetWidth;
    const percent = (computed / parentWidth) * 100;

    dot.style.transition = 'none';
    dot.style.width = percent + '%';

    azElapsed += Date.now() - azStartTime;
    clearTimeout(azTimeout);
}

function azResumeProgress() {
    const remaining = azSlideDuration - azElapsed;
    azStartProgress(remaining);
}

document.getElementById('az-next-btn').addEventListener('click', () => {
    clearTimeout(azTimeout);
    azElapsed = 0;
    azUpdateSlide(azCurrentIndex + 1);
});

document.getElementById('az-prev-btn').addEventListener('click', () => {
    clearTimeout(azTimeout);
    azElapsed = 0;
    azUpdateSlide(azCurrentIndex - 1);
});

const azCarousel = document.getElementById('az-carousel');
let azTouchStartX = 0;
let azTouchEndX = 0;

azCarousel.addEventListener('touchstart', e => {
    azTouchStartX = e.changedTouches[0].screenX;
});

azCarousel.addEventListener('touchend', e => {
    azTouchEndX = e.changedTouches[0].screenX;
    azHandleSwipe();
});

function azHandleSwipe() {
    if (azTouchEndX < azTouchStartX - 30) {
        clearTimeout(azTimeout);
        azElapsed = 0;
        azUpdateSlide(azCurrentIndex + 1);
    }
    if (azTouchEndX > azTouchStartX + 30) {
        clearTimeout(azTimeout);
        azElapsed = 0;
        azUpdateSlide(azCurrentIndex - 1);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    azUpdateSlide(0);
});