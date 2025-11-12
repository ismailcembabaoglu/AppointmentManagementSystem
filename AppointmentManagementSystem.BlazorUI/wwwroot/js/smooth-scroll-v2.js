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
