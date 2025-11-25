// Ultra Simple Smooth Scroll - Direct Approach
window.scrollToElement = function(targetId) {
    console.log('🎯 scrollToElement called with:', targetId);
    
    const element = document.querySelector(targetId);
    
    if (!element) {
        console.error('❌ Element not found:', targetId);
        return false;
    }
    
    console.log('✅ Element found, scrolling NOW!');
    
    // Direct scrollIntoView
    element.scrollIntoView({ 
        behavior: 'smooth', 
        block: 'start'
    });
    
    // Adjust for fixed header after scroll
    setTimeout(() => {
        const y = element.getBoundingClientRect().top + window.pageYOffset - 100;
        window.scrollTo({ top: y, behavior: 'smooth' });
        console.log('✅ Adjusted for header offset');
    }, 100);
    
    return true;
};

window.initSectionObserver = function (dotNetRef, sectionIds) {
    if (!('IntersectionObserver' in window)) {
        console.warn('⚠️ IntersectionObserver not supported, skipping section tracking');
        return;
    }

    // Adjust the viewport window so the "active" section is based on the middle
    // of the screen. This keeps the highlight in sync on smaller (mobile) screens
    // where sections rarely take up 45%+ of the viewport height.
    const options = {
        root: null,
        // Use a tighter viewport window so the active state reflects the section
        // nearest to the middle of the screen. This reduces cases where the
        // previous section stays selected while scrolling to the next one.
        rootMargin: '-25% 0px -50% 0px',
        threshold: [0, 0.1, 0.25, 0.5]
    };

    const observer = new IntersectionObserver((entries) => {
        const visible = entries.filter(entry => entry.isIntersecting);

        if (!visible.length) {
            return;
        }

        const viewportCenter = window.innerHeight / 2;

        const bestEntry = visible.reduce((best, entry) => {
            const bestRect = best.boundingClientRect;
            const entryRect = entry.boundingClientRect;

            // Prefer the section with the highest intersection ratio; when ratios
            // are similar, pick the one whose center is closest to the viewport
            // center. This keeps the highlight aligned to the most prominent
            // section during scroll.
            const bestScore = (best.intersectionRatio * 100) - Math.abs((bestRect.top + bestRect.height / 2) - viewportCenter) / 10;
            const entryScore = (entry.intersectionRatio * 100) - Math.abs((entryRect.top + entryRect.height / 2) - viewportCenter) / 10;

            return entryScore > bestScore ? entry : best;
        }, visible[0]);

        const sectionId = `#${bestEntry.target.id}`;
        dotNetRef.invokeMethodAsync('UpdateActiveSection', sectionId);
    }, options);

    sectionIds.forEach(id => {
        const el = document.querySelector(id);
        if (el) {
            observer.observe(el);
        }
    });

    window.__aptivaSectionObserver = observer;
};

window.disposeSectionObserver = function () {
    if (window.__aptivaSectionObserver) {
        window.__aptivaSectionObserver.disconnect();
        window.__aptivaSectionObserver = null;
    }
};
