// wwwroot/js/chart.js
// Script pour le rendu des graphiques avec Canvas API

window.renderChart = (canvasId, config) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        console.error(`Canvas avec ID ${canvasId} non trouvé`);
        return;
    }

    const ctx = canvas.getContext('2d');

    // Nettoyage du canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Ajustement pour les écrans haute résolution
    const dpr = window.devicePixelRatio || 1;
    const rect = canvas.getBoundingClientRect();
    canvas.width = rect.width * dpr;
    canvas.height = rect.height * dpr;
    ctx.scale(dpr, dpr);

    // Rendu selon le type de graphique
    switch (config.type) {
        case 'bar':
            renderBarChart(ctx, canvas, config.data);
            break;
        case 'line':
            renderLineChart(ctx, canvas, config.data);
            break;
        case 'area':
            renderAreaChart(ctx, canvas, config.data);
            break;
        case 'pie':
            renderPieChart(ctx, canvas, config.data, false);
            break;
        case 'donut':
            renderPieChart(ctx, canvas, config.data, true);
            break;
        default:
            console.warn(`Type de graphique "${config.type}" non supporté`);
    }
};

function renderBarChart(ctx, canvas, data) {
    const margin = 40;
    const chartWidth = canvas.width / (window.devicePixelRatio || 1) - 2 * margin;
    const chartHeight = canvas.height / (window.devicePixelRatio || 1) - 2 * margin;
    const barWidth = chartWidth / data.labels.length;

    // Calcul de la valeur maximale
    const maxValue = Math.max(...data.datasets[0].data);
    if (maxValue === 0) return;

    // Dessiner les axes
    ctx.strokeStyle = '#e2e8f0';
    ctx.lineWidth = 1;

    // Axe Y
    ctx.beginPath();
    ctx.moveTo(margin, margin);
    ctx.lineTo(margin, margin + chartHeight);
    ctx.stroke();

    // Axe X
    ctx.beginPath();
    ctx.moveTo(margin, margin + chartHeight);
    ctx.lineTo(margin + chartWidth, margin + chartHeight);
    ctx.stroke();

    // Dessiner les barres
    data.datasets[0].data.forEach((value, index) => {
        const barHeight = (value / maxValue) * chartHeight;
        const x = margin + index * barWidth + barWidth * 0.1;
        const y = margin + chartHeight - barHeight;

        // Barre avec gradient
        const gradient = ctx.createLinearGradient(0, y, 0, y + barHeight);
        gradient.addColorStop(0, data.datasets[0].backgroundColor[index] || '#0082bb');
        gradient.addColorStop(1, (data.datasets[0].backgroundColor[index] || '#0082bb') + '80');

        ctx.fillStyle = gradient;
        ctx.fillRect(x, y, barWidth * 0.8, barHeight);

        // Bordure
        ctx.strokeStyle = data.datasets[0].borderColor || '#0082bb';
        ctx.lineWidth = 2;
        ctx.strokeRect(x, y, barWidth * 0.8, barHeight);

        // Étiquettes X
        ctx.fillStyle = '#64748b';
        ctx.font = '12px Inter, system-ui, sans-serif';
        ctx.textAlign = 'center';
        ctx.fillText(
            data.labels[index],
            x + barWidth * 0.4,
            margin + chartHeight + 20
        );

        // Valeurs sur les barres
        ctx.fillStyle = '#1e293b';
        ctx.font = 'bold 11px Inter, system-ui, sans-serif';
        ctx.fillText(
            value.toString(),
            x + barWidth * 0.4,
            y - 5
        );
    });

    // Grille horizontale et étiquettes Y
    const gridLines = 5;
    for (let i = 0; i <= gridLines; i++) {
        const y = margin + (chartHeight / gridLines) * i;
        const value = Math.round(maxValue - (maxValue / gridLines) * i);

        // Ligne de grille
        ctx.strokeStyle = '#f1f5f9';
        ctx.lineWidth = 1;
        ctx.beginPath();
        ctx.moveTo(margin, y);
        ctx.lineTo(margin + chartWidth, y);
        ctx.stroke();

        // Étiquette Y
        ctx.fillStyle = '#64748b';
        ctx.font = '11px Inter, system-ui, sans-serif';
        ctx.textAlign = 'right';
        ctx.fillText(value.toString(), margin - 10, y + 4);
    }
}

function renderLineChart(ctx, canvas, data) {
    const margin = 40;
    const chartWidth = canvas.width / (window.devicePixelRatio || 1) - 2 * margin;
    const chartHeight = canvas.height / (window.devicePixelRatio || 1) - 2 * margin;
    const pointSpacing = chartWidth / (data.labels.length - 1);

    const maxValue = Math.max(...data.datasets[0].data);
    if (maxValue === 0) return;

    // Dessiner les axes (même logique que le bar chart)
    ctx.strokeStyle = '#e2e8f0';
    ctx.lineWidth = 1;

    ctx.beginPath();
    ctx.moveTo(margin, margin);
    ctx.lineTo(margin, margin + chartHeight);
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(margin, margin + chartHeight);
    ctx.lineTo(margin + chartWidth, margin + chartHeight);
    ctx.stroke();

    // Dessiner la ligne
    ctx.strokeStyle = data.datasets[0].borderColor || '#0082bb';
    ctx.lineWidth = 3;
    ctx.lineJoin = 'round';
    ctx.lineCap = 'round';

    ctx.beginPath();
    data.datasets[0].data.forEach((value, index) => {
        const x = margin + index * pointSpacing;
        const y = margin + chartHeight - (value / maxValue) * chartHeight;

        if (index === 0) {
            ctx.moveTo(x, y);
        } else {
            ctx.lineTo(x, y);
        }
    });
    ctx.stroke();

    // Dessiner les points
    data.datasets[0].data.forEach((value, index) => {
        const x = margin + index * pointSpacing;
        const y = margin + chartHeight - (value / maxValue) * chartHeight;

        // Point de fond
        ctx.fillStyle = '#ffffff';
        ctx.beginPath();
        ctx.arc(x, y, 6, 0, 2 * Math.PI);
        ctx.fill();

        // Point principal
        ctx.fillStyle = data.datasets[0].borderColor || '#0082bb';
        ctx.beginPath();
        ctx.arc(x, y, 4, 0, 2 * Math.PI);
        ctx.fill();

        // Étiquettes X
        ctx.fillStyle = '#64748b';
        ctx.font = '12px Inter, system-ui, sans-serif';
        ctx.textAlign = 'center';
        ctx.fillText(data.labels[index], x, margin + chartHeight + 20);
    });
}

function renderAreaChart(ctx, canvas, data) {
    const margin = 40;
    const chartWidth = canvas.width / (window.devicePixelRatio || 1) - 2 * margin;
    const chartHeight = canvas.height / (window.devicePixelRatio || 1) - 2 * margin;
    const pointSpacing = chartWidth / (data.labels.length - 1);

    const maxValue = Math.max(...data.datasets[0].data);
    if (maxValue === 0) return;

    // Créer le gradient pour l'aire
    const gradient = ctx.createLinearGradient(0, margin, 0, margin + chartHeight);
    gradient.addColorStop(0, (data.datasets[0].backgroundColor || '#0082bb') + '60');
    gradient.addColorStop(1, (data.datasets[0].backgroundColor || '#0082bb') + '10');

    // Dessiner l'aire
    ctx.fillStyle = gradient;
    ctx.beginPath();

    // Commencer en bas à gauche
    ctx.moveTo(margin, margin + chartHeight);

    // Tracer la ligne de données
    data.datasets[0].data.forEach((value, index) => {
        const x = margin + index * pointSpacing;
        const y = margin + chartHeight - (value / maxValue) * chartHeight;
        ctx.lineTo(x, y);
    });

    // Fermer la forme en bas à droite
    ctx.lineTo(margin + chartWidth, margin + chartHeight);
    ctx.closePath();
    ctx.fill();

    // Dessiner la ligne de contour
    renderLineChart(ctx, canvas, data);
}

function renderPieChart(ctx, canvas, data, isDonut) {
    const centerX = (canvas.width / (window.devicePixelRatio || 1)) / 2;
    const centerY = (canvas.height / (window.devicePixelRatio || 1)) / 2;
    const radius = Math.min(centerX, centerY) - 30;

    const total = data.datasets[0].data.reduce((sum, value) => sum + value, 0);
    if (total === 0) return;

    let currentAngle = -Math.PI / 2; // Commencer en haut

    // Dessiner les secteurs
    data.datasets[0].data.forEach((value, index) => {
        const sliceAngle = (value / total) * 2 * Math.PI;

        ctx.beginPath();
        ctx.moveTo(centerX, centerY);
        ctx.arc(centerX, centerY, radius, currentAngle, currentAngle + sliceAngle);
        ctx.closePath();

        ctx.fillStyle = data.datasets[0].backgroundColor[index] || '#0082bb';
        ctx.fill();

        // Bordure
        ctx.strokeStyle = '#ffffff';
        ctx.lineWidth = 2;
        ctx.stroke();

        // Étiquettes
        const labelAngle = currentAngle + sliceAngle / 2;
        const labelRadius = radius + 20;
        const labelX = centerX + Math.cos(labelAngle) * labelRadius;
        const labelY = centerY + Math.sin(labelAngle) * labelRadius;

        ctx.fillStyle = '#1e293b';
        ctx.font = '12px Inter, system-ui, sans-serif';
        ctx.textAlign = 'center';
        ctx.fillText(data.labels[index], labelX, labelY);

        // Pourcentage
        const percentage = ((value / total) * 100).toFixed(1) + '%';
        ctx.font = '10px Inter, system-ui, sans-serif';
        ctx.fillText(percentage, labelX, labelY + 15);

        currentAngle += sliceAngle;
    });

    // Trou central pour donut
    if (isDonut) {
        ctx.beginPath();
        ctx.arc(centerX, centerY, radius * 0.6, 0, 2 * Math.PI);
        ctx.fillStyle = getComputedStyle(document.body).getPropertyValue('--mud-palette-surface') || '#ffffff';
        ctx.fill();
    }
}

// Fonction utilitaire pour le scroll
window.scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
};