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

    const options = {
        root: null,
        rootMargin: '0px',
        threshold: 0.45
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const sectionId = `#${entry.target.id}`;
                dotNetRef.invokeMethodAsync('UpdateActiveSection', sectionId);
            }
        });
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
