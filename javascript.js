document.addEventListener("DOMContentLoaded", function () {
    const changeWeekBtn = document.getElementById("changeWeekBtn");
    const weekTitle = document.getElementById("weekTitle");
    const weekContent = document.getElementById("weekContent");
    const weekBadges = document.getElementById("weekBadges");
  
    if (!changeWeekBtn || !weekTitle || !weekContent || !weekBadges) return;
  
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
  });