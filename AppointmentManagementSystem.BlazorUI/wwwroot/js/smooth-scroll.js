// Enhanced Smooth Scroll with Easing
window.smoothScrollTo = function(targetId) {
    console.log('🎯 Smooth scroll function called with:', targetId);
    
    // Wait a bit for DOM to be ready
    setTimeout(() => {
        const element = document.querySelector(targetId);
        
        if (!element) {
            console.error('❌ Element not found:', targetId);
            console.log('Available elements with IDs:', 
                Array.from(document.querySelectorAll('[id]')).map(el => '#' + el.id)
            );
            return;
        }
        
        console.log('✅ Element found:', element);
        console.log('Current scroll position:', window.pageYOffset);

        const headerOffset = 100;
        const elementPosition = element.getBoundingClientRect().top;
        const offsetPosition = elementPosition + window.pageYOffset - headerOffset;
        const startPosition = window.pageYOffset;
        const distance = offsetPosition - startPosition;
        const duration = 1000; // 1 second
        
        console.log('📊 Scroll info:', {
            elementPosition,
            offsetPosition,
            startPosition,
            distance
        });
        
        let start = null;

        function animation(currentTime) {
            if (start === null) start = currentTime;
            const timeElapsed = currentTime - start;
            const run = ease(timeElapsed, startPosition, distance, duration);
            window.scrollTo(0, run);
            if (timeElapsed < duration) {
                requestAnimationFrame(animation);
            } else {
                console.log('✅ Scroll animation complete!');
            }
        }

        // Easing function (easeInOutQuad)
        function ease(t, b, c, d) {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t + b;
            t--;
            return -c / 2 * (t * (t - 2) - 1) + b;
        }

        console.log('🚀 Starting scroll animation...');
        requestAnimationFrame(animation);
    }, 50);
};

// Simple smooth scroll fallback
window.smoothScrollToSimple = function(targetId) {
    console.log('🎯 Simple smooth scroll to:', targetId);
    
    setTimeout(() => {
        const element = document.querySelector(targetId);
        if (!element) {
            console.error('❌ Element not found:', targetId);
            return;
        }
        
        console.log('✅ Element found, scrolling...');
        const headerOffset = 100;
        const elementPosition = element.getBoundingClientRect().top;
        const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

        window.scrollTo({
            top: offsetPosition,
            behavior: 'smooth'
        });
    }, 50);
};
