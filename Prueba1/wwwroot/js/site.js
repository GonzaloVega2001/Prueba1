// Animación de aparición al hacer scroll
const revealObserver = new IntersectionObserver(
  (entries) => {
    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        entry.target.classList.add("in");
        revealObserver.unobserve(entry.target);
      }
    });
  },
  { threshold: 0.12 }
);
document.querySelectorAll(".reveal").forEach((el) => revealObserver.observe(el));

// Menú móvil
const burger = document.getElementById("burger");
const navLinks = document.getElementById("nav-links");
const navCta = document.querySelector(".nav-cta");
let menuOpen = false;

if (burger) {
  burger.addEventListener("click", () => {
    menuOpen = !menuOpen;
    navLinks.classList.toggle("mobile-open", menuOpen);
    navLinks.style.cssText = menuOpen
      ? "display:flex; flex-direction:column; position:absolute; top:64px; left:0; right:0; background:var(--paper); padding:20px 28px; border-bottom:1px solid var(--line); gap:16px;"
      : "";
    if (navCta) {
      navCta.style.cssText = menuOpen
        ? "display:inline-flex; position:absolute; top:210px; left:28px;"
        : "";
    }
  });
}

// --- Toast ---
// Lee el resultado del envío del formulario desde un <div id="toast-data">
// con atributos data-status / data-message (lo escribe Index.cshtml a
// partir de TempData) y muestra un toast si corresponde.
function showToast(status, message) {
  let container = document.querySelector(".toast-container");
  if (!container) {
    container = document.createElement("div");
    container.className = "toast-container";
    document.body.appendChild(container);
  }

  const toast = document.createElement("div");
  toast.className = "toast" + (status === "error" ? " toast-error" : "");
  toast.innerHTML =
    '<span class="toast-icon">' + (status === "error" ? "✕" : "✓") + "</span>" +
    '<div class="toast-body"><b>' + (status === "error" ? "Error" : "Enviado") + "</b>" + message + "</div>" +
    '<button class="toast-close" aria-label="Cerrar">×</button>';

  container.appendChild(toast);
  requestAnimationFrame(() => toast.classList.add("show"));

  const remove = () => {
    toast.classList.remove("show");
    setTimeout(() => toast.remove(), 350);
  };

  toast.querySelector(".toast-close").addEventListener("click", remove);
  setTimeout(remove, 6000);
}

document.addEventListener("DOMContentLoaded", () => {
  const toastData = document.getElementById("toast-data");
  if (toastData && toastData.dataset.status) {
    showToast(toastData.dataset.status, toastData.dataset.message);
  }
});
