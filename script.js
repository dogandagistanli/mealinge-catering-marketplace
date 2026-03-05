document.addEventListener("DOMContentLoaded", function () {
  const changeWeekBtn = document.getElementById("changeWeekBtn");
  const weekTitle = document.getElementById("weekTitle");
  const weekContent = document.getElementById("weekContent");
  const weekBadges = document.getElementById("weekBadges");

  if (changeWeekBtn && weekTitle && weekContent && weekBadges) {
    changeWeekBtn.addEventListener("click", function () {
      weekTitle.textContent = "2. Hafta: Java Script & Butonlar";

      weekContent.innerHTML =
        'Bu örnekte <strong>üst menü</strong>, <strong>yan menü</strong> ve ortada <strong>3x5 tablo</strong> bulunuyor. Öğrenciler için sınıfta “kutu modeli, padding, margin, border, hover, active” gibi konuları göstermek için ideal. Java Script is used to add specific buttons.';

      weekBadges.innerHTML = `
        <span class="badge">HTML Semantik</span>
        <span class="badge">Flexbox</span>
        <span class="badge">Tablo</span>
        <span class="badge">Hover</span>
        <span class="badge">Java Script</span>
      `;
    });
  }

  const sortGradesBtn = document.getElementById("sortGradesBtn");
  const studentTableBody = document.getElementById("studentTableBody");

  if (sortGradesBtn && studentTableBody) {
    sortGradesBtn.addEventListener("click", function () {
      const rows = Array.from(studentTableBody.querySelectorAll("tr"));

      const gradeOrder = {
        "AA": 1,
        "BA": 2,
        "BB": 3,
        "CB": 4,
        "CC": 5,
        "DC": 6,
        "DD": 7,
        "FD": 8,
        "FF": 9
      };

      rows.sort(function (rowA, rowB) {
        const gradeA = rowA.cells[4].textContent.trim();
        const gradeB = rowB.cells[4].textContent.trim();

        const valueA = gradeOrder[gradeA] ?? 999;
        const valueB = gradeOrder[gradeB] ?? 999;

        return valueA - valueB;
      });

      rows.forEach(function (row, index) {
        row.cells[0].textContent = index + 1;
        studentTableBody.appendChild(row);
      });
    });
  }

  const changeImageBtn = document.getElementById("changeImageBtn");
  const tableImage = document.getElementById("tableImage");

  if (changeImageBtn && tableImage) {
    changeImageBtn.addEventListener("click", function () {
      const next = tableImage.getAttribute("src") === "images.png" ? "githublogo.png" : "images.png";
      const test = new Image();

      test.onload = function () {
        tableImage.src = next + "?v=" + Date.now();
        tableImage.alt = next === "githublogo.png" ? "GitHub logo görseli" : "Örnek veri tablosu görseli";
      };

      test.onerror = function () {
        alert(next + " bulunamadı. Dosya adı ve aynı klasörde olduğundan emin ol.");
      };

      test.src = next;
    });
  }
  const followArea = document.getElementById("followArea");
  const followImg = document.getElementById("followImg");

  if (followArea && followImg) {
    let rect = followArea.getBoundingClientRect();
    let x = rect.width / 2;
    let y = rect.height / 2;
    let tx = x;
    let ty = y;
    let active = false;
    let lastX = x;
    let lastY = y;

    let padX = 70;
    let padY = 70;

    function refreshRect() {
      rect = followArea.getBoundingClientRect();
      const imgRect = followImg.getBoundingClientRect();
      padX = imgRect.width / 2;
      padY = imgRect.height / 2;
    }

    function clamp(n, min, max) {
      return Math.max(min, Math.min(max, n));
    }

    function onMove(e) {
      if (!active) return;
      const mx = e.clientX - rect.left;
      const my = e.clientY - rect.top;

      tx = clamp(mx, padX, rect.width - padX);
      ty = clamp(my, padY, rect.height - padY);
    }
    let hue = 0;

    function animate() {
      x += (tx - x) * 0.14;
      y += (ty - y) * 0.14;

      const dx = x - lastX;
      const dy = y - lastY;

      const diffX = x - tx;
      const diffY = y - ty;
      const shadowX = diffX * 0.4;
      const shadowY = diffY * 0.4;
      const distance = Math.sqrt(diffX * diffX + diffY * diffY);
      const blur = Math.max(10, distance * 0.2);

      // --- CONDITIONAL RGB LOGIC ---
      // Only change hue if the mouse is in the area AND currently moving
      const isMoving = Math.abs(dx) > 0.1 || Math.abs(dy) > 0.1;

      if (active && isMoving) {
        hue = (hue + 2) % 360;
      }

      // If not active, we can either keep the last color or fade to black
      // Using the current hue ensures it stays on the last color it reached
      const colorCore = `hsla(${hue}, 100%, 50%, 0.8)`;
      const colorGlow = `hsla(${hue}, 100%, 50%, 0.4)`;
      // ------------------------------

      followImg.style.filter = `
    drop-shadow(${shadowX * 0.2}px ${shadowY * 0.2}px 4px ${colorCore}) 
    drop-shadow(${shadowX * 0.5}px ${shadowY * 0.5}px 15px ${colorGlow})
    drop-shadow(${shadowX}px ${shadowY}px ${blur + 20}px ${colorGlow})
  `;

      const tilt = clamp(dx * 0.08, -10, 10);
      const spin = clamp(dy * -0.05, -6, 6);

      followImg.style.transform = `
    translate(${x}px, ${y}px) 
    translate(-50%, -50%) 
    rotate(${tilt}deg) 
    scale(${1.02 + Math.min(Math.abs(dx) + Math.abs(dy), 30) / 900}) 
    rotateX(${spin}deg)
  `;

      lastX = x;
      lastY = y;

      requestAnimationFrame(animate);
    }

    followArea.addEventListener("mouseenter", function () {
      refreshRect();
      active = true;
      tx = rect.width / 2;
      ty = rect.height / 2;
    });

    followArea.addEventListener("mouseleave", function () {
      refreshRect();
      active = false;
      tx = rect.width / 2;
      ty = rect.height / 2;
    });

    followArea.addEventListener("mousemove", function (e) {
      onMove(e);
    });

    window.addEventListener("resize", function () {
      refreshRect();
      x = rect.width / 2;
      y = rect.height / 2;
      tx = x;
      ty = y;
      lastX = x;
      lastY = y;
    });

    followImg.addEventListener("load", function () {
      refreshRect();
      x = rect.width / 2;
      y = rect.height / 2;
      tx = x;
      ty = y;
      lastX = x;
      lastY = y;
      followImg.style.transform = `translate(${x}px, ${y}px) translate(-50%, -50%)`;
    });

    refreshRect();
    followImg.style.transform = `translate(${x}px, ${y}px) translate(-50%, -50%)`;
    requestAnimationFrame(animate);
  }
});