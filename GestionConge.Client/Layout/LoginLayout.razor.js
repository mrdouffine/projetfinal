// login-animations.js - Scripts d'animation pour le layout de connexion

// Animation d'entrée de la carte
function initCardAnimation() {
    const card = document.querySelector('.login-card');
    if (card) {
        // Attendre un peu puis déclencher l'animation
        setTimeout(() => {
            card.classList.add('loaded');
        }, 100);
    }
}

// Création des particules animées
function createParticles() {
    const particlesContainer = document.getElementById('particles-container');
    if (!particlesContainer) return;

    const totalParticles = 40; // Nombre de particules
    for (let i = 0; i < totalParticles; i++) {
        const particle = document.createElement('div');
        particle.classList.add('particle');

        // Position et taille aléatoires
        particle.style.left = `${Math.random() * 100}%`;
        particle.style.top = `${Math.random() * 100}%`;
        particle.style.width = `${Math.random() * 8 + 4}px`;
        particle.style.height = particle.style.width;

        // Animation personnalisée avec un délai aléatoire
        particle.style.animationDelay = `${Math.random() * 5}s`;
        particle.style.animationDuration = `${Math.random() * 10 + 5}s`;

        particlesContainer.appendChild(particle);
    }
}

// Initialisation complète
document.addEventListener('DOMContentLoaded', () => {
    initCardAnimation();
    createParticles();
});
