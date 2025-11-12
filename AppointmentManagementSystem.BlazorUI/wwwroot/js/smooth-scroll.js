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
        console.log('📏 Element rect:', element.getBoundingClientRect());
        console.log('📍 Current scroll position:', window.pageYOffset);
        console.log('📜 Document height:', document.documentElement.scrollHeight);
        console.log('🖼️ Viewport height:', window.innerHeight);
        
        // Check if element is already visible
        const rect = element.getBoundingClientRect();
        if (rect.top >= 0 && rect.top <= window.innerHeight) {
            console.log('⚠️ Element is already in viewport!');
        }

        const headerOffset = 100;
        const elementPosition = element.getBoundingClientRect().top;
        const offsetPosition = elementPosition + window.pageYOffset - headerOffset;
        const startPosition = window.pageYOffset;
        const distance = offsetPosition - startPosition;
        const duration = 1000; // 1 second
        
        console.log('📊 Scroll info:', {
            'Element position from top': elementPosition,
            'Target scroll position': offsetPosition,
            'Current position': startPosition,
            'Distance to scroll': distance
        });
        
        if (Math.abs(distance) < 5) {
            console.log('⚠️ Already at target position, no need to scroll');
            return;
        }
        
        let start = null;

        function animation(currentTime) {
            if (start === null) start = currentTime;
            const timeElapsed = currentTime - start;
            const run = ease(timeElapsed, startPosition, distance, duration);
            
            console.log(`⏱️ Time: ${timeElapsed}ms, Position: ${run}`);
            window.scrollTo(0, run);
            
            if (timeElapsed < duration) {
                requestAnimationFrame(animation);
            } else {
                console.log('✅ Scroll animation complete!');
                console.log('📍 Final position:', window.pageYOffset);
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
    }, 100);
};

// Simple smooth scroll fallback - more direct approach
window.smoothScrollToSimple = function(targetId) {
    console.log('🎯 Simple smooth scroll to:', targetId);
    
    setTimeout(() => {
        const element = document.querySelector(targetId);
        if (!element) {
            console.error('❌ Element not found:', targetId);
            return;
        }
        
        console.log('✅ Element found, scrolling with native smooth scroll...');
        
        // Try multiple scroll methods
        
        // Method 1: element.scrollIntoView
        try {
            element.scrollIntoView({ 
                behavior: 'smooth', 
                block: 'start',
                inline: 'nearest'
            });
            console.log('✅ scrollIntoView called');
        } catch (e) {
            console.error('❌ scrollIntoView failed:', e);
        }
        
        // Method 2: Also try window.scrollTo as backup
        setTimeout(() => {
            const headerOffset = 100;
            const elementPosition = element.getBoundingClientRect().top;
            const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

            window.scrollTo({
                top: offsetPosition,
                behavior: 'smooth'
            });
            console.log('✅ window.scrollTo also called as backup');
        }, 100);
    }, 50);
};
