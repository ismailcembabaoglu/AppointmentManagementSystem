// Notification sound player
window.playNotificationSound = function() {
    // Create an audio context
    const audioContext = new (window.AudioContext || window.webkitAudioContext)();
    
    // Create oscillator (for beep sound)
    const oscillator = audioContext.createOscillator();
    const gainNode = audioContext.createGain();
    
    // Connect nodes
    oscillator.connect(gainNode);
    gainNode.connect(audioContext.destination);
    
    // Configure sound (pleasant notification beep)
    oscillator.frequency.value = 800; // Frequency in Hz
    oscillator.type = 'sine'; // Wave type
    
    // Volume envelope (fade in and out)
    gainNode.gain.setValueAtTime(0, audioContext.currentTime);
    gainNode.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + 0.05);
    gainNode.gain.linearRampToValueAtTime(0, audioContext.currentTime + 0.3);
    
    // Play sound
    oscillator.start(audioContext.currentTime);
    oscillator.stop(audioContext.currentTime + 0.3);
    
    // Second beep for double notification sound
    setTimeout(() => {
        const oscillator2 = audioContext.createOscillator();
        const gainNode2 = audioContext.createGain();
        
        oscillator2.connect(gainNode2);
        gainNode2.connect(audioContext.destination);
        
        oscillator2.frequency.value = 1000;
        oscillator2.type = 'sine';
        
        gainNode2.gain.setValueAtTime(0, audioContext.currentTime);
        gainNode2.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + 0.05);
        gainNode2.gain.linearRampToValueAtTime(0, audioContext.currentTime + 0.3);
        
        oscillator2.start(audioContext.currentTime);
        oscillator2.stop(audioContext.currentTime + 0.3);
    }, 150);
};

// Smooth scroll to section
window.scrollToSection = function(sectionId) {
    const element = document.getElementById(sectionId);
    if (element) {
        const headerHeight = 70; // Fixed header height
        const elementPosition = element.getBoundingClientRect().top;
        const offsetPosition = elementPosition + window.pageYOffset - headerHeight;

        window.scrollTo({
            top: offsetPosition,
            behavior: 'smooth'
        });
    }
};
