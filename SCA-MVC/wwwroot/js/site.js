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
		error: { icon: "bi-x-circle-fill", cls: "msg-icon-error" },
		info: { icon: "bi-info-circle-fill", cls: "msg-icon-info" },
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

// ══════════════════════════════════════════
// EMPLEADOS — SELECCIÓN DE FILA
// ══════════════════════════════════════════

// Navega a la vista Index de Empleados con el ID seleccionado, preservando los
// filtros activos. Usamos la ruta base '/Empleado' en lugar de window.location.href
// para evitar que una URL previa (ej: /Empleado/Edit por validación fallida)
// genere un GET a una acción inexistente y cause un 404.
function seleccionarEmpleado(idEmpleado) {
	const base = new URL(window.location.origin + '/Empleado');

	// Conservar filtros activos de la URL actual si los hay
	const current = new URL(window.location.href);
	const filtro = current.searchParams.get('filtro');
	const empresaFiltroId = current.searchParams.get('empresaFiltroId');
	if (filtro) base.searchParams.set('filtro', filtro);
	if (empresaFiltroId) base.searchParams.set('empresaFiltroId', empresaFiltroId);

	base.searchParams.set('idEmpleado', idEmpleado);
	window.location.href = base.toString();
}

// ══════════════════════════════════════════
// EMPLEADOS — BÚSQUEDA EN TIEMPO REAL (debounce)
// ══════════════════════════════════════════

document.addEventListener("DOMContentLoaded", () => {
	const inputFiltro = document.getElementById("input-filtro");
	if (!inputFiltro) return;

	let debounceTimer;
	inputFiltro.addEventListener("input", () => {
		clearTimeout(debounceTimer);
		debounceTimer = setTimeout(() => {
			document.getElementById("filtro-form").submit();
		}, 400); // 400ms de espera antes de enviar
	});

	// Bloquear Enter en el input de búsqueda (el debounce ya lo maneja)
	inputFiltro.addEventListener("keydown", (e) => {
		if (e.key === "Enter") e.preventDefault();
	});
});

// Bloquear Enter en el input de credencial RFID para que no submitee el form
// del Edit. En su lugar, llama directamente a verificarCredencial().
document.addEventListener("DOMContentLoaded", () => {
	const inputCred = document.getElementById("input-credencial");
	if (!inputCred) return;

	inputCred.addEventListener("keydown", (e) => {
		if (e.key === "Enter") {
			e.preventDefault();
			verificarCredencial();
		}
	});
});

// ══════════════════════════════════════════
// EMPLEADOS — VERIFICACIÓN DE CREDENCIAL RFID (AJAX)
// ══════════════════════════════════════════

// Llama al endpoint /Empleado/VerificarCredencial y muestra feedback al usuario.
// El servidor retorna { estado: 'disponible' | 'propia' | 'ocupada' }
async function verificarCredencial() {
	const inputCred = document.getElementById("input-credencial");
	const statusEl = document.getElementById("credencial-status");
	if (!inputCred || !statusEl) return;

	const credencial = inputCred.value.trim().toUpperCase();
	if (!credencial) {
		statusEl.textContent = "Ingresá una credencial para verificar.";
		statusEl.style.color = "var(--color-warning, #f59e0b)";
		inputCred.style.borderColor = "";
		return;
	}

	// Pasar el ID del empleado actual para distinguir si la credencial es propia o de otro
	const idActual = document.getElementById("hidden-id-empleado-actual")?.value ?? "0";
	const url = `/Empleado/VerificarCredencial?credencial=${encodeURIComponent(credencial)}&idEmpleadoActual=${idActual}`;

	try {
		const response = await fetch(url);
		const data = await response.json();

		if (data.estado === "disponible") {
			statusEl.textContent = `✓ La credencial '${credencial}' está disponible.`;
			statusEl.style.color = "var(--color-success, #22c55e)";
			inputCred.style.borderColor = "var(--color-success, #22c55e)";
		} else if (data.estado === "propia") {
			statusEl.textContent = `ℹ '${credencial}' es la credencial actual de este empleado.`;
			statusEl.style.color = "#2196F3";
			inputCred.style.borderColor = "#2196F3";
		} else {
			// estado === "ocupada"
			statusEl.textContent = `⚠ La credencial '${credencial}' ya está asignada a otro empleado.`;
			statusEl.style.color = "var(--color-danger, #ef4444)";
			inputCred.style.borderColor = "var(--color-danger, #ef4444)";
		}
	} catch {
		statusEl.textContent = "Error al verificar la credencial. Intentá de nuevo.";
		statusEl.style.color = "var(--color-danger, #ef4444)";
	}
}

// ══════════════════════════════════════════
// EMPLEADOS — CONFIRMACIÓN DE ELIMINACIÓN
// ══════════════════════════════════════════

document.addEventListener("DOMContentLoaded", () => {
	const btnEliminar = document.getElementById("btn-eliminar-empleado");
	const deleteForm = document.getElementById("delete-empleado-form");

	if (btnEliminar && deleteForm) {
		btnEliminar.addEventListener("click", (e) => {
			e.preventDefault();
			const estaInactivo = btnEliminar.getAttribute("data-estado") === "false";
			if (estaInactivo) {
				showMessage("warning", "Ya desactivado", "El empleado seleccionado ya se encuentra desactivado.");
				return;
			}
			showConfirm(
				"¿Estás seguro que deseas desactivar al empleado seleccionado?",
				"Confirmar desactivación",
				() => { deleteForm.submit(); }
			);
		});
	}
});
