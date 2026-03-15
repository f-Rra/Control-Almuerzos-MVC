# Commit 31b — Responsividad para Tablet Samsung Galaxy A9

## Objetivo
Corregir problemas de responsividad detectados al usar la aplicación en una **tablet Samsung Galaxy A9** (pantalla de ~1080px en portrait), y solucionar el error de registro por credencial RFID.

---

## Problemas detectados y soluciones aplicadas

### 1. Sidebar se expande/contrae al acercar el cursor (hover)
**Problema:** En escritorio el sidebar se expande al hacer hover. En una tablet táctil no existe "hover" real — el cursor nunca se aleja, por lo que el sidebar quedaba permanentemente expandido o con comportamiento errático.

**Solución:** En `site.css`, dentro del `@media (max-width: 1024px)`, se agregaron reglas que **bloquean el expand por hover**:
```css
.side:hover { width: 72px; }
.side:hover .mi { gap: 0; padding: 14px 10px; }
.side:hover .mi .lbl { opacity: 0; max-width: 0; }
.side:hover .side-brand h1 { opacity: 0; max-width: 0; }
```
El sidebar queda fijo en modo ícono en cualquier pantalla ≤ 1024px.

---

### 2. Viewport incorrecto (causa raíz de todos los problemas de escala)
**Archivo:** `Views/Shared/_Layout.cshtml`

**Problema:** El viewport estaba configurado con `width=1920`, forzando al navegador de la tablet a renderizar toda la página como si tuviera 1920px de ancho y luego escalarla. Esto hacía que los media queries nunca se activaran.

**Solución:**
```html
<!-- Antes -->
<meta name="viewport" content="width=1920, initial-scale=1.0, shrink-to-fit=yes" />
<!-- Después -->
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
```

---

### 3. Home — Detalle del Servicio cortado (img 1)
**Problema:** El layout `.layout` en `Home/Index.cshtml` usa dos columnas (`list-panel` + `detail-col`). En pantallas angostas el contenido se comprimía y la columna de detalle quedaba cortada.

**Solución:** Nuevo bloque `@media (max-width: 1200px)` en `site.css`:
```css
.layout { flex-direction: column; overflow-y: auto; }
.list-panel { flex: none; max-height: 340px; }
.detail-col { flex: none; }
```

---

### 4. Configuración del Servicio desbordada (img 3)
**Problema:** La fila `.config-row` tiene: título + separador + inputs + botón, todo en una sola línea horizontal. En tablet esto se salía del panel.

**Solución:** En `@media (max-width: 1200px)`:
```css
.config-row { flex-wrap: wrap; }
.cfg-title { width: 100%; border-bottom: 1px solid var(--glass-border); }
.cfg-sep { display: none; }
.cfg-inputs { flex-wrap: wrap; width: 100%; }
.cfg-btn-wrap { width: 100%; justify-content: flex-start; }
```

---

### 5. Registro Manual — controles desalineados (img 5)
**Problema:** El `.rm-panel-head` contiene título + separador + filtro empresa + búsqueda + botón, todo en una fila. En tablet algunos elementos se apilaban incorrectamente.

**Solución:** En `@media (max-width: 1200px)`:
```css
.rm-panel-head { gap: 10px; }
.rm-panel-head h3 { width: 100%; }
.rm-sep { display: none; }
```
Y en `@media (max-width: 900px)` los controles pasan a columna completa, con filtros y buscador al 100% de ancho.

---

### 6. Error al registrar credencial — SQL trigger conflict (img 4)
**Error:** `SqlException: The target table 'Registros' of the DML statement cannot have any enabled triggers if the statement contains an OUTPUT clause without INTO clause.`

**Causa:** EF Core 7+ genera por defecto un `INSERT ... OUTPUT INSERTED.PK` para recuperar el ID generado. SQL Server no permite usar `OUTPUT` en tablas que tienen **triggers activos**, ya que el trigger podría modificar las filas afectadas.

**Solución:** En `Data/Configurations/RegistroConfiguration.cs`, indicarle a EF Core que la tabla tiene triggers:
```csharp
builder.ToTable(tb => tb.HasTrigger("Registros_Trigger"));
```
Esto hace que EF Core use `SET NOCOUNT OFF; INSERT ...; SELECT SCOPE_IDENTITY()` en lugar del OUTPUT clause, que sí es compatible con tablas con triggers.

> **Nota:** El nombre del trigger en `HasTrigger()` no tiene que coincidir exactamente con el nombre real en SQL Server. Simplemente la presencia de cualquier llamada a `HasTrigger()` es suficiente para que EF Core cambie su estrategia de generación de SQL.

---

## Archivos modificados

| Archivo | Cambio |
|---|---|
| `Views/Shared/_Layout.cshtml` | Viewport `width=device-width` |
| `wwwroot/css/site.css` | Media queries tablet (`≤1024px`, `≤1200px`, `≤900px`) |
| `Data/Configurations/RegistroConfiguration.cs` | `HasTrigger()` para evitar error SQL OUTPUT |

---

## Mensaje de commit

```
fix(responsive): adapt layout for Samsung Galaxy A9 tablet + fix SQL trigger error on Registro insert

- Fix viewport meta tag: change width=1920 to width=device-width
- Sidebar: block hover-expand behavior on touch screens (≤1024px)
- Home: change .layout to column on ≤1200px, cap list-panel height
- Servicio: config-row wraps on tablet, status-bar 3×2 grid
- RegistroManual: rm-panel-head wraps on tablet, summary-row 2×2
- Fix SQL: add HasTrigger() to RegistroConfiguration to avoid
  OUTPUT clause conflict with SQL Server triggers on Registros table
```
