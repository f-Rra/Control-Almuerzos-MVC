// ══════════════════════════════════════════
// TOAST NOTIFICATIONS
// ══════════════════════════════════════════

function showToast(type, title, message, duration = 4000) {
	const container = document.getElementById("sca-toast-container");
	if (!container) return;

	const toast = document.createElement("div");
	toast.className = `sca-toast ${type}`;

	const iconMap = {
		success: "bi-check-circle-fill",
		error: "bi-x-circle-fill",
		info: "bi-info-circle-fill",
		warning: "bi-exclamation-triangle-fill"
	};

	toast.innerHTML = `
		<div class="sca-toast-icon">
			<i class="bi ${iconMap[type] || iconMap.info}"></i>
		</div>
		<div class="sca-toast-content">
			<div class="sca-toast-title">${title}</div>
			<div class="sca-toast-msg">${message}</div>
		</div>
		<button class="sca-toast-close" onclick="this.parentElement.remove()">
			<i class="bi bi-x"></i>
		</button>
	`;

	container.appendChild(toast);

	if (duration > 0) {
		setTimeout(() => {
			toast.style.animation = "toastSlideIn 0.3s ease-out reverse";
			setTimeout(() => toast.remove(), 300);
		}, duration);
	}
}

// ══════════════════════════════════════════
// MODAL DE CONFIRMACIÓN
// ══════════════════════════════════════════

// ══════════════════════════════════════════
// MODAL DE MENSAJE (éxito, error, info, warning)
// ══════════════════════════════════════════

function showMessage(type, title, message) {
	const overlay = document.createElement("div");
	overlay.className = "confirm-modal-overlay";

	const iconMap = {
		success: { icon: "bi-check-circle-fill", cls: "msg-icon-success" },
		error:   { icon: "bi-x-circle-fill",     cls: "msg-icon-error" },
		info:    { icon: "bi-info-circle-fill",   cls: "msg-icon-info" },
		warning: { icon: "bi-exclamation-triangle-fill", cls: "msg-icon-warning" }
	};
	const cfg = iconMap[type] || iconMap.info;

	overlay.innerHTML = `
		<div class="confirm-modal">
			<div class="confirm-modal-icon ${cfg.cls}">
				<i class="bi ${cfg.icon}"></i>
			</div>
			<div class="confirm-modal-title">${title}</div>
			<div class="confirm-modal-message">${message}</div>
			<div class="confirm-modal-actions">
				<button class="confirm-btn confirm-btn-primary" id="msg-ok">
					<i class="bi bi-check-lg"></i> Aceptar
				</button>
			</div>
		</div>
	`;

	document.body.appendChild(overlay);

	function close() {
		overlay.style.animation = "fadeIn 0.2s ease-out reverse";
		setTimeout(() => overlay.remove(), 200);
	}

	overlay.querySelector("#msg-ok").addEventListener("click", close);
	overlay.addEventListener("click", (e) => {
		if (e.target === overlay) close();
	});
}

function showConfirm(message, title = "Confirmar acción", onConfirm, onCancel) {
	const overlay = document.createElement("div");
	overlay.className = "confirm-modal-overlay";

	overlay.innerHTML = `
		<div class="confirm-modal">
			<div class="confirm-modal-icon">
				<i class="bi bi-exclamation-triangle-fill"></i>
			</div>
			<div class="confirm-modal-title">${title}</div>
			<div class="confirm-modal-message">${message}</div>
			<div class="confirm-modal-actions">
				<button class="confirm-btn confirm-btn-secondary" id="confirm-cancel">
					<i class="bi bi-x-lg"></i> Cancelar
				</button>
				<button class="confirm-btn confirm-btn-primary" id="confirm-accept">
					<i class="bi bi-check-lg"></i> Aceptar
				</button>
			</div>
		</div>
	`;

	document.body.appendChild(overlay);

	const btnCancel = overlay.querySelector("#confirm-cancel");
	const btnAccept = overlay.querySelector("#confirm-accept");

	function close() {
		overlay.style.animation = "fadeIn 0.2s ease-out reverse";
		setTimeout(() => overlay.remove(), 200);
	}

	btnCancel.addEventListener("click", () => {
		close();
		if (onCancel) onCancel();
	});

	btnAccept.addEventListener("click", () => {
		close();
		if (onConfirm) onConfirm();
	});

	overlay.addEventListener("click", (e) => {
		if (e.target === overlay) {
			close();
			if (onCancel) onCancel();
		}
	});
}

// Check for server messages on page load
document.addEventListener("DOMContentLoaded", () => {
	const tempDataElement = document.querySelector("[data-toast-message]");
	if (tempDataElement) {
		const type = tempDataElement.getAttribute("data-toast-type") || "info";
		const title = tempDataElement.getAttribute("data-toast-title") || "Notificación";
		const message = tempDataElement.getAttribute("data-toast-message") || "";
		if (message) {
			showMessage(type, title, message);
		}
	}
});

// ══════════════════════════════════════════
// EMPRESAS — CONFIRMACIÓN DE ELIMINACIÓN
// ══════════════════════════════════════════

document.addEventListener("DOMContentLoaded", () => {
	const btnEliminar = document.getElementById("btn-eliminar");
	const deleteForm = document.getElementById("delete-form");

	if (btnEliminar && deleteForm) {
		btnEliminar.addEventListener("click", (e) => {
			e.preventDefault();
			const estadoInactivo = document.getElementById("empresa-estado-inactivo");
			if (estadoInactivo && estadoInactivo.checked) {
				showMessage("warning", "Ya desactivada", "La empresa seleccionada ya se encuentra desactivada.");
				return;
			}
			showConfirm(
				"¿Estás seguro que deseas desactivar la empresa seleccionada?",
				"Confirmar eliminación",
				() => {
					deleteForm.submit();
				}
			);
		});
	}
});
