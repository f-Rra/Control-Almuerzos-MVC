
// ══════════════════════════════════════════
// MODAL DE CONFIRMACIÓN
// ══════════════════════════════════════════

// ══════════════════════════════════════════
// NOTIFICACIÓN DE REGISTRO DE COMENSAL
// Diseño: glassmorphism · pill "✓ Registrado" · hora
// ══════════════════════════════════════════

function showRegistro(nombre, empresa, hora, duration = 2500) {
	// Inyectar estilos sólo la primera vez
	if (!document.getElementById('reg-notif-style')) {
		const style = document.createElement('style');
		style.id = 'reg-notif-style';
		style.textContent = `
			.reg-overlay {
				position: fixed; inset: 0; z-index: 9999;
				display: flex; align-items: center; justify-content: center;
				background: rgba(253,246,227,0.45);
				backdrop-filter: blur(8px);
				-webkit-backdrop-filter: blur(8px);
				animation: regFadeIn .22s ease;
			}
			.reg-card {
				background: rgba(255,255,255,0.62);
				backdrop-filter: blur(22px);
				-webkit-backdrop-filter: blur(22px);
				border: 1.5px solid rgba(255,193,7,0.45);
				border-radius: 24px;
				padding: 30px 32px 26px;
				text-align: center;
				min-width: 260px;
				max-width: 320px;
				box-shadow: 0 8px 40px rgba(255,193,7,0.18), 0 2px 12px rgba(0,0,0,0.07);
				display: flex; flex-direction: column; align-items: center; gap: 6px;
				animation: regSlideUp .25s cubic-bezier(.34,1.56,.64,1);
			}
			.reg-ico {
				width: 58px; height: 58px; border-radius: 50%;
				border: 2.5px solid #FFC107;
				display: flex; align-items: center; justify-content: center;
				font-size: 1.5rem; color: #FFC107;
				margin-bottom: 6px;
			}
			.reg-tag {
				background: rgba(76,175,80,0.13);
				color: #3d8b40;
				font-size: .72rem; font-weight: 700;
				padding: 3px 12px; border-radius: 999px;
				text-transform: uppercase; letter-spacing: .08em;
				margin-bottom: 4px;
			}
			.reg-name {
				font-family: 'Outfit', sans-serif;
				font-weight: 700; font-size: 1.1rem; color: #1A1A1A;
			}
			.reg-empresa {
				color: #888; font-size: .88rem;
			}
			.reg-hora {
				margin-top: 6px;
				font-size: .82rem; font-weight: 600;
				color: #e6a800;
			}
			.reg-hora i { margin-right: 3px; }
			@keyframes regFadeIn  { from { opacity: 0; } to { opacity: 1; } }
			@keyframes regSlideUp { from { transform: translateY(16px) scale(.96); opacity: 0; }
			                        to   { transform: translateY(0)     scale(1);   opacity: 1; } }
		`;
		document.head.appendChild(style);
	}

	const overlay = document.createElement('div');
	overlay.className = 'reg-overlay';
	overlay.innerHTML = `
		<div class="reg-card">
			<div class="reg-ico"><i class="bi bi-person-check"></i></div>
			<div class="reg-tag">✓ Registrado</div>
			<div class="reg-name">${nombre}</div>
			<div class="reg-empresa">${empresa}</div>
			<div class="reg-hora"><i class="bi bi-clock"></i>${hora}</div>
		</div>
	`;
	document.body.appendChild(overlay);

	function close() {
		overlay.style.animation = 'regFadeIn .18s ease reverse';
		setTimeout(() => overlay.remove(), 180);
	}

	setTimeout(close, duration);
	overlay.addEventListener('click', e => { if (e.target === overlay) close(); });
}

// ══════════════════════════════════════════
// MODAL DE MENSAJE (éxito, error, info, warning)
// ══════════════════════════════════════════

function showMessage(type, title, message, duration = 0) {
	const overlay = document.createElement("div");
	overlay.className = "confirm-modal-overlay";

	const iconMap = {
		success: { icon: "bi-check-circle-fill", cls: "msg-icon-success" },
		error: { icon: "bi-x-circle-fill", cls: "msg-icon-error" },
		info: { icon: "bi-info-circle-fill", cls: "msg-icon-info" },
		warning: { icon: "bi-exclamation-triangle-fill", cls: "msg-icon-warning" }
	};
	const cfg = iconMap[type] || iconMap.info;
	const showBtn = duration === 0; // solo muestra botón si no hay auto-cierre

	overlay.innerHTML = `
		<div class="confirm-modal">
			<div class="confirm-modal-icon ${cfg.cls}">
				<i class="bi ${cfg.icon}"></i>
			</div>
			<div class="confirm-modal-title">${title}</div>
			${message.trim() ? `<div class="confirm-modal-message">${message}</div>` : ''}
			${showBtn ? `<div class="confirm-modal-actions">
				<button class="confirm-btn confirm-btn-primary" id="msg-ok">
					<i class="bi bi-check-lg"></i> Aceptar
				</button>
			</div>` : ''}
		</div>
	`;

	document.body.appendChild(overlay);

	function close() {
		overlay.style.animation = "fadeIn 0.2s ease-out reverse";
		setTimeout(() => overlay.remove(), 200);
	}

	if (showBtn) {
		overlay.querySelector("#msg-ok").addEventListener("click", close);
	} else {
		setTimeout(close, duration);
	}

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
		const title = tempDataElement.getAttribute("data-toast-title") || "";
		const message = tempDataElement.getAttribute("data-toast-message") || "";
		if (title || message.trim()) {
			// success → se cierra solo; error/warning → requiere aceptar
			const duration = type === "success" ? 2500 : 0;
			showMessage(type, title, message, duration);
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

