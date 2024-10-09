window.drawPoint = function (x, y, color) {
    const canvas = document.getElementById('polygonCanvas');
    const ctx = canvas.getContext('2d');

    // 将 canvas 的坐标转换为相对于画布的坐标
    const rect = canvas.getBoundingClientRect();
    const canvasX = x - rect.left;
    const canvasY = y - rect.top;

    // 绘制点
    ctx.beginPath();
    ctx.arc(canvasX, canvasY, 5, 0, 2 * Math.PI);
    ctx.fillStyle = color;
    ctx.fill();
    ctx.closePath();
}

window.drawLine = function (startX, startY, endX, endY, color) {
    const canvas = document.getElementById('polygonCanvas');
    const ctx = canvas.getContext('2d');

    // 将 canvas 的坐标转换为相对于画布的坐标
    const rect = canvas.getBoundingClientRect();
    const canvasStartX = startX - rect.left;
    const canvasStartY = startY - rect.top;
    const canvasEndX = endX - rect.left;
    const canvasEndY = endY - rect.top;

    // 绘制线段
    ctx.beginPath();
    ctx.moveTo(canvasStartX, canvasStartY);
    ctx.lineTo(canvasEndX, canvasEndY);
    ctx.strokeStyle = color;
    ctx.stroke();
    ctx.closePath();
}

window.clearCanvas = function () {
    const canvas = document.getElementById('polygonCanvas');
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
}