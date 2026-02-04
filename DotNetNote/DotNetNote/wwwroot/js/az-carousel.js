/**
 * Az Carousel Script
 *
 * 이 스크립트는 .az-carousel-section 단위로 카루셀을 초기화합니다.
 * 한 페이지에 여러 개의 카루셀이 존재해도 서로 영향을 주지 않도록
 * 각 섹션을 독립된 개체로 동작시키는 구조입니다.
 *
 * [필수 HTML 구성]
 * - .az-carousel-section         : 카루셀 전체 영역
 * - .az-slide                    : 슬라이드 요소들
 * - .az-progress-indicators      : 진행바 및 일시정지 버튼 영역 (스크립트가 내부 요소 생성)
 * - .az-prev-btn / .az-next-btn  : 이전 / 다음 버튼
 *
 * [동작 개요]
 * - 현재 슬라이드에만 .az-active 클래스가 적용됩니다.
 * - 슬라이드 수만큼 진행바(dot)가 자동 생성됩니다.
 * - 진행 상태는 width 애니메이션으로 표현됩니다.
 * - 사용자가 dot 클릭, 이전/다음 버튼 클릭, 스와이프 동작 시
 *   현재 진행 중인 타이머와 애니메이션 예약을 정리한 뒤 즉시 전환합니다.
 *
 * [탭 전환 처리]
 * - 브라우저 탭이 숨겨지면 자동으로 재생을 멈춥니다.
 * - 탭이 다시 보이면 남은 시간만큼 이어서 재생합니다.
 * - 사용자가 직접 일시정지를 누른 상태는 자동 제어의 영향을 받지 않습니다.
 *
 * [구조적 특징]
 * - requestAnimationFrame 예약을 개체 단위로 관리하여 중복 실행을 방지합니다.
 * - transition 초기화 후 reflow를 발생시켜 애니메이션이 안정적으로 시작되도록 합니다.
 * - 모든 DOM 접근은 섹션 기준으로 수행되며, 전역 ID에 의존하지 않습니다.
 *
 * [설정 가능 항목]
 * - SLIDE_DURATION 값을 변경하면 슬라이드 자동 전환 시간을 조절할 수 있습니다.
 */

(function () {

    // 슬라이드 한 장의 자동 재생 시간(ms)
    const SLIDE_DURATION = 5000;

    // 페이지 내 모든 카루셀 섹션을 찾음
    const sections = document.querySelectorAll('.az-carousel-section');
    if (!sections || sections.length === 0) return;

    // 각 카루셀 개체를 저장 (탭 가시성 변경 시 전체 제어용)
    const instances = [];

    // ------------------------------------------------------------
    // 섹션별로 독립 개체 생성(인스턴스화)
    // ------------------------------------------------------------
    sections.forEach(section => {

        // 현재 섹션 안의 슬라이드 목록
        const slides = section.querySelectorAll('.az-slide');
        const totalSlides = slides.length;

        // 진행바 영역 및 컨트롤 버튼(이전/다음)
        const progressIndicators = section.querySelector('.az-progress-indicators');
        const nextBtn = section.querySelector('.az-next-btn');
        const prevBtn = section.querySelector('.az-prev-btn');

        // 필수 DOM이 없거나 슬라이드가 0개면 동작 불가
        if (!progressIndicators || !nextBtn || !prevBtn || totalSlides === 0) return;

        // 서버 렌더링/재호출로 dot이 남아있을 수 있으니 초기화
        progressIndicators.innerHTML = '';

        // --------------------------------------------------------
        // 카루셀 "개체" 상태 관리용 데이터 묶음
        // --------------------------------------------------------
        const state = {
            section,
            slides,
            totalSlides,
            progressIndicators,
            nextBtn,
            prevBtn,

            // 현재 보고 있는 슬라이드 인덱스
            currentIndex: 0,

            // 사용자가 수동으로 일시정지 눌렀는지 여부
            isPaused: false,

            // 탭 숨김으로 인해 자동 일시정지 되었는지 여부
            autoPausedByVisibility: false,

            // 자동 전환 타이머(setTimeout) 핸들
            timeoutId: null,

            // 진행 애니메이션 예약(requestAnimationFrame) 핸들
            rafId: null,

            // 진행률 계산을 위한 시작 시각/누적 경과시간
            startTime: 0,
            elapsed: 0,

            // 슬라이드 재생 시간
            slideDuration: SLIDE_DURATION,

            // 진행바 fill 요소들(.az-progress-fill)
            progressFills: [],

            // 일시정지 버튼 DOM
            pauseBtn: null
        };

        // --------------------------------------------------------
        // 진행바(dot) 생성
        // - 슬라이드 개수만큼 생성
        // - 각 dot 클릭 시 해당 슬라이드로 이동
        // --------------------------------------------------------
        for (let i = 0; i < totalSlides; i++) {
            const dot = document.createElement('div');
            dot.className = 'az-progress-dot';
            dot.dataset.index = i;

            const fill = document.createElement('div');
            fill.className = 'az-progress-fill';
            dot.appendChild(fill);

            // dot 클릭 -> 해당 슬라이드로 즉시 이동
            dot.addEventListener('click', () => {
                // 기존 예약 정리 후 이동(중복 애니메이션 방지)
                clearTimeout(state.timeoutId);
                cancelRaf(state);

                // 새 슬라이드로 이동하므로 경과시간 초기화
                state.elapsed = 0;

                updateSlide(state, i);
            });

            progressIndicators.appendChild(dot);
            state.progressFills.push(fill);
        }

        // --------------------------------------------------------
        // 일시정지 버튼 생성
        // - 클릭 시 pause / resume 토글
        // --------------------------------------------------------
        const pauseBtn = document.createElement('div');
        pauseBtn.className = 'az-pause-button';
        pauseBtn.innerHTML = '<i class="fas fa-pause"></i>';
        progressIndicators.appendChild(pauseBtn);
        state.pauseBtn = pauseBtn;

        pauseBtn.addEventListener('click', () => {
            state.isPaused = !state.isPaused;

            // 상태에 따라 아이콘 변경
            pauseBtn.innerHTML = `<i class="fas fa-${state.isPaused ? 'play' : 'pause'}"></i>`;

            // 실제 진행 제어
            if (state.isPaused) {
                pauseProgress(state);
            } else {
                resumeProgress(state);
            }
        });

        // --------------------------------------------------------
        // 다음/이전 버튼 처리
        // - 버튼 클릭 시 기존 예약 정리 후 즉시 전환
        // --------------------------------------------------------
        nextBtn.addEventListener('click', () => {
            clearTimeout(state.timeoutId);
            cancelRaf(state);
            state.elapsed = 0;
            updateSlide(state, state.currentIndex + 1);
        });

        prevBtn.addEventListener('click', () => {
            clearTimeout(state.timeoutId);
            cancelRaf(state);
            state.elapsed = 0;
            updateSlide(state, state.currentIndex - 1);
        });

        // --------------------------------------------------------
        // 모바일 스와이프 처리
        // - 좌/우 스와이프 감지 후 슬라이드 전환
        // --------------------------------------------------------
        let touchStartX = 0;
        let touchEndX = 0;

        section.addEventListener('touchstart', e => {
            touchStartX = e.changedTouches[0].screenX;
        });

        section.addEventListener('touchend', e => {
            touchEndX = e.changedTouches[0].screenX;
            handleSwipe(state, touchStartX, touchEndX);
        });

        // --------------------------------------------------------
        // 초기 시작: 0번 슬라이드 표시 및 진행바 시작
        // --------------------------------------------------------
        updateSlide(state, 0);

        // 탭 가시성 변경 이벤트에서 전체 제어하기 위해 등록
        instances.push(state);
    });

    // ------------------------------------------------------------
    // 탭 가시성(visibility) 변경 처리
    // - 숨김: 자동 일시정지
    // - 복귀: 자동 재개(단, 사용자가 수동 pause 중이면 재개하지 않음)
    // ------------------------------------------------------------
    document.addEventListener('visibilitychange', () => {
        if (document.hidden) {
            instances.forEach(state => {
                // 사용자가 수동으로 멈춘 상태는 건드리지 않음
                if (!state.isPaused) {
                    state.autoPausedByVisibility = true;
                    pauseProgress(state);
                }
            });
        } else {
            instances.forEach(state => {
                // 탭 숨김 때문에 멈춘 것만 복구
                if (state.autoPausedByVisibility) {
                    state.autoPausedByVisibility = false;

                    // 사용자가 수동 pause 상태가 아니라면 이어서 재생
                    if (!state.isPaused) {
                        resumeProgress(state);
                    }
                }
            });
        }
    });

    // ============================================================
    // 아래부터는 "동작 함수" 영역
    // ============================================================

    /**
     * requestAnimationFrame 예약이 있으면 취소한다.
     * - 탭 숨김 상태에서 rAF가 누적되었다가 한꺼번에 실행되는 현상을 방지한다.
     */
    function cancelRaf(state) {
        if (state.rafId) {
            cancelAnimationFrame(state.rafId);
            state.rafId = null;
        }
    }

    /**
     * 슬라이드를 전환한다.
     * - index를 범위 내로 보정(mod 처리)
     * - .az-active 클래스를 현재 슬라이드에만 적용
     * - 모든 진행바를 초기화하고, 필요 시 현재 슬라이드 진행을 시작한다.
     */
    function updateSlide(state, index) {
        // index를 0 ~ totalSlides-1 범위로 맞춘다.
        state.currentIndex = (index + state.totalSlides) % state.totalSlides;

        // 현재 슬라이드만 active 처리
        state.slides.forEach((slide, i) => {
            slide.classList.toggle('az-active', i === state.currentIndex);
        });

        // 진행바 초기화(모두 0%로)
        state.progressFills.forEach(fill => {
            fill.style.transition = 'none';
            fill.style.width = '0%';
        });

        // 이전 예약 정리(중복 진행 방지)
        clearTimeout(state.timeoutId);
        cancelRaf(state);

        // 새 슬라이드이므로 경과시간 초기화
        state.elapsed = 0;

        // 일시정지 상태가 아니고, 탭이 보이는 상태면 진행 시작
        if (!state.isPaused && !document.hidden) {
            startProgress(state, state.slideDuration);
        }
    }

    /**
     * 현재 슬라이드의 진행바를 시작한다.
     * - width 0% -> 100% 를 duration 동안 linear로 증가
     * - duration이 끝나면 자동으로 다음 슬라이드로 넘어간다.
     */
    function startProgress(state, duration) {
        const fill = state.progressFills[state.currentIndex];
        if (!fill) return;

        // 진행바 초기화
        fill.style.transition = 'none';
        fill.style.width = '0%';

        // transition 초기화가 확실히 적용되도록 reflow를 발생시킨다.
        // (브라우저가 스타일 변경을 적용하도록 강제)
        void fill.offsetWidth;

        // rAF 누적 방지 후 다음 프레임에서 transition 시작
        cancelRaf(state);
        state.rafId = requestAnimationFrame(() => {
            fill.style.transition = `width ${duration}ms linear`;
            fill.style.width = '100%';
        });

        // 진행 시작 시각 기록
        state.startTime = Date.now();

        // duration 후 자동 다음 슬라이드로 전환
        state.timeoutId = setTimeout(() => {
            state.elapsed = 0;
            updateSlide(state, state.currentIndex + 1);
        }, duration);
    }

    /**
     * 진행바를 현재 진행 위치에서 멈춘다.
     * - getComputedStyle로 현재 fill의 width(px)를 얻는다.
     * - 부모(dot) width 대비 퍼센트를 계산하여 그 위치에서 고정한다.
     * - 경과시간(elapsed)을 누적하여 resume 시 남은 시간을 계산할 수 있게 한다.
     */
    function pauseProgress(state) {
        const fill = state.progressFills[state.currentIndex];
        if (!fill) return;

        // 현재 fill의 픽셀 폭
        const computedWidth = parseFloat(getComputedStyle(fill).width) || 0;

        // 부모 폭 대비 퍼센트 계산
        const parentWidth = fill.parentElement ? fill.parentElement.offsetWidth : 0;
        const percent = parentWidth > 0 ? (computedWidth / parentWidth) * 100 : 0;

        // 현재 퍼센트 위치로 고정
        fill.style.transition = 'none';
        fill.style.width = Math.max(0, Math.min(100, percent)) + '%';

        // 경과시간 누적
        if (state.startTime) {
            state.elapsed += Date.now() - state.startTime;
        }

        // 자동 전환 예약 제거
        clearTimeout(state.timeoutId);
        cancelRaf(state);
    }

    /**
     * 멈춰있던 진행바를 이어서 진행한다.
     * - elapsed(누적 경과시간)를 기준으로 remaining(남은시간)을 계산한다.
     * - 탭이 숨겨진 상태면 resume 하지 않는다(복귀 시 visibilitychange가 처리).
     */
    function resumeProgress(state) {
        if (document.hidden) return;

        const remaining = Math.max(0, state.slideDuration - state.elapsed);

        // remaining이 0이면 한 사이클로 다시 시작
        startProgress(state, remaining > 0 ? remaining : state.slideDuration);
    }

    /**
     * 스와이프 동작으로 슬라이드를 전환한다.
     * - 좌측 스와이프: 다음 슬라이드
     * - 우측 스와이프: 이전 슬라이드
     * - 전환 전 기존 예약을 정리하고 경과시간을 초기화한다.
     */
    function handleSwipe(state, startX, endX) {
        // 좌측 스와이프(다음)
        if (endX < startX - 30) {
            clearTimeout(state.timeoutId);
            cancelRaf(state);
            state.elapsed = 0;
            updateSlide(state, state.currentIndex + 1);
        }
        // 우측 스와이프(이전)
        else if (endX > startX + 30) {
            clearTimeout(state.timeoutId);
            cancelRaf(state);
            state.elapsed = 0;
            updateSlide(state, state.currentIndex - 1);
        }
    }

})();
