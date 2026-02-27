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

  document.addEventListener("DOMContentLoaded", function () {
    // FIRST BUTTON (Week card content change)
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
  
    // SECOND BUTTON (Sort table rows by letter grade)
    const sortGradesBtn = document.getElementById("sortGradesBtn");
    const studentTableBody = document.getElementById("studentTableBody");
  
    if (sortGradesBtn && studentTableBody) {
      sortGradesBtn.addEventListener("click", function () {
        const rows = Array.from(studentTableBody.querySelectorAll("tr"));
  
        // Grade priority: highest to lowest
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
  
          return valueA - valueB; // smaller means better grade (AA first)
        });
  
        // Re-number rows after sorting (# column)
        rows.forEach(function (row, index) {
          row.cells[0].textContent = index + 1;
          studentTableBody.appendChild(row);
        });
      });
    }
  });