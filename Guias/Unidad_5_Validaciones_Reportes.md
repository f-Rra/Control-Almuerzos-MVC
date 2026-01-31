# Unidad 5: Validaciones y Reportes PDF

---

## Validaciones Avanzadas

### Data Annotations

#### Validaciones Básicas

**Atributos disponibles:**
- **[Required]** - Campo obligatorio con mensaje personalizado
- **[StringLength]** - Longitud máxima y mínima
- **[Range]** - Rango de valores numéricos
- **[EmailAddress]** - Formato de email válido
- **[Display]** - Nombre amigable para mostrar en vistas
- **[RegularExpression]** - Patrón regex personalizado

**Aplicación en modelos:**
- Se colocan sobre las propiedades
- Se validan automáticamente en el ModelState
- Generan mensajes de error automáticos

**Ejemplo de uso:**
- Nombre: Required, StringLength(50)
- Email: Required, EmailAddress
- Edad: Range(18, 100)
- Credencial: RegularExpression para solo números

#### Validaciones Personalizadas

**Atributo de Validación Personalizado:**

**Crear clase que herede de ValidationAttribute:**
- Sobrescribir método IsValid
- Recibir valor y ValidationContext
- Retornar ValidationResult.Success o error

**Casos de uso:**
- Validar que una fecha no sea futura
- Validar formato específico de credencial
- Validar rangos complejos

**Ejemplo: FechaNoFuturaAttribute:**
- Verifica que la fecha no sea mayor a DateTime.Now
- Retorna error si es futura
- Se aplica a propiedades de tipo DateTime

### Validación con IValidatableObject

**Interfaz IValidatableObject:**
- Permite validaciones a nivel de objeto completo
- Acceso a múltiples propiedades
- Validaciones que dependen de varias propiedades

**Método Validate:**
- Retorna IEnumerable<ValidationResult>
- Usa yield return para cada error
- Especifica qué propiedad tiene el error

**Casos de uso en tu sistema:**

**Servicio:**
- Proyección debe ser mayor a 0
- Proyección no debe ser excesivamente alta (> 1000)
- Total de invitados no debe exceder proyección
- Fecha no debe ser muy antigua (> 1 mes)

**Empleado:**
- Si tiene credencial, debe ser única
- No puede estar en empresa inactiva

### Validaciones en el Controlador

**Validaciones personalizadas adicionales:**

**En acción Crear/Editar:**
- Verificar unicidad de credenciales
- Validar que la empresa esté activa
- Verificar reglas de negocio específicas
- Agregar errores con ModelState.AddModelError()

**Proceso:**
1. Recibir modelo del formulario
2. Validar ModelState.IsValid
3. Si es válido, hacer validaciones personalizadas
4. Si hay errores, agregar a ModelState
5. Si todo es válido, guardar
6. Si hay errores, retornar vista con modelo

**Ejemplos de validaciones:**
- Credencial única (excluyendo el empleado actual al editar)
- Empresa activa al asignar empleado
- Servicio único activo por lugar
- No registrar empleado duplicado en servicio

### Validación del Lado del Cliente

**Scripts necesarios:**
- jQuery
- jQuery Validation
- jQuery Validation Unobtrusive

**Inclusión en las vistas:**
- Sección Scripts al final de la vista
- Usar vista parcial _ValidationScriptsPartial
- Las validaciones se ejecutan antes del submit

**Ventajas:**
- Feedback inmediato sin ir al servidor
- Mejor experiencia de usuario
- Reduce carga del servidor
- Previene envíos inválidos

**Validaciones que funcionan automáticamente:**
- Required
- StringLength
- Range
- EmailAddress
- RegularExpression

**Validaciones personalizadas con jQuery:**
- Se pueden agregar reglas personalizadas
- Usar $.validator.addMethod()
- Configurar adaptadores unobtrusive

---

## Generación de Reportes PDF

### Configuración de iTextSharp

**Instalación:**
- Paquete NuGet: iTextSharp
- Versión recomendada: 5.5.13.4

**Características:**
- Generación de PDF desde cero
- Tablas, imágenes, texto
- Estilos y formatos
- Encabezados y pies de página

### Clase Helper para PDF

**PdfHelper.cs:**

**Propósito:**
- Centralizar la lógica de generación de PDF
- Reutilizar código común
- Mantener consistencia visual

**Métodos principales:**

**CrearPdfBasico:**
- Recibe título y acción de contenido
- Crea documento con tamaño A4
- Agrega encabezado automático
- Ejecuta contenido personalizado
- Agrega pie de página
- Retorna byte array

**AgregarEncabezado:**
- Título centrado con fuente grande
- Fecha de generación
- Línea separadora

**AgregarPiePagina:**
- Línea separadora
- Nombre del sistema
- Información institucional

**CrearTabla:**
- Recibe número de columnas
- Opcionalmente anchos personalizados
- Retorna PdfPTable configurada

**CrearCeldaEncabezado:**
- Celda con fondo gris
- Texto en negrita
- Centrado
- Padding consistente

**CrearCelda:**
- Celda normal
- Alineación configurable
- Padding consistente

### Reporte de Servicios

**ReportesController:**

**Acción GenerarListaServicios:**
- Recibe parámetros: fechaDesde, fechaHasta, lugarId (opcional)
- Consulta servicios en el rango
- Filtra por lugar si se especifica
- Ordena por fecha descendente
- Genera PDF con los datos
- Retorna archivo PDF para descarga

**Contenido del PDF:**

**Encabezado:**
- Título "REPORTE DE SERVICIOS"
- Período consultado

**Tabla:**
- Columnas: Fecha, Lugar, Proyección, Real, Invitados, Duración
- Fila por cada servicio
- Fila de totales al final

**Resumen:**
- Total de servicios
- Totales de proyección, real e invitados
- Cobertura promedio (%)

### Reporte de Asistencias por Empresa

**Acción GenerarAsistenciasPorEmpresa:**
- Recibe parámetros: fechaDesde, fechaHasta
- Agrupa registros por empresa
- Cuenta total de asistencias por empresa
- Ordena por cantidad descendente
- Genera PDF con ranking

**Contenido del PDF:**

**Encabezado:**
- Título "ASISTENCIAS POR EMPRESA"
- Período consultado

**Tabla:**
- Columnas: #, Empresa, Total Asistencias
- Fila por cada empresa ordenada por total
- Fila de total general

**Características:**
- Ranking numérico
- Totales calculados
- Formato profesional

### Vista de Reportes

**Index.cshtml:**

**Estructura:**
- Título de la página
- Tarjetas (cards) para cada tipo de reporte
- Formularios independientes para cada reporte

**Reporte de Servicios:**
- Fecha desde (input type="date")
- Fecha hasta (input type="date")
- Selector de lugar (opcional)
- Botón "Generar PDF"

**Reporte de Asistencias:**
- Fecha desde
- Fecha hasta
- Botón "Generar PDF"

**Características:**
- Valores por defecto (último mes)
- Validación de fechas
- Diseño responsive
- Iconos descriptivos

---

## Exportación a Excel (Opcional)

### Usando EPPlus

**Instalación:**
- Paquete NuGet: EPPlus
- Configurar licencia: LicenseContext.NonCommercial

**Características:**
- Generación de archivos Excel (.xlsx)
- Múltiples hojas
- Estilos y formatos
- Fórmulas

### Método de Exportación

**ExportarEmpleadosExcel:**

**Proceso:**
1. Obtener datos de empleados
2. Crear ExcelPackage
3. Agregar hoja de trabajo
4. Escribir encabezados con estilo
5. Escribir datos fila por fila
6. Ajustar ancho de columnas automáticamente
7. Retornar archivo Excel

**Contenido:**
- Encabezados en negrita con fondo gris
- Columnas: ID, Nombre, Apellido, Empresa, Credencial, Estado
- Datos ordenados por apellido
- Auto-ajuste de columnas

**Uso:**
- Botón "Exportar a Excel" en el listado
- Descarga automática del archivo
- Nombre de archivo con fecha

---

## Manejo de Errores

### Validación de Parámetros

**En acciones de reportes:**
- Verificar que las fechas sean válidas
- Validar que fechaDesde <= fechaHasta
- Verificar que el rango no sea excesivo
- Retornar error amigable si hay problemas

### Try-Catch en Generación de PDF

**Manejo de excepciones:**
- Try-catch alrededor de generación de PDF
- Logging de errores
- Mensaje amigable al usuario
- Opción de reintentar

### Mensajes de Error Amigables

**En lugar de:**
- "Object reference not set to an instance of an object"

**Mostrar:**
- "No se pudo generar el reporte. Por favor, intente nuevamente."

---

## Ejercicios Prácticos

### Ejercicio 1: Validaciones Personalizadas

**Tareas:**
1. Crear validación para proyección > 0
2. Crear validación para credencial única
3. Implementar IValidatableObject en Servicio
4. Agregar validaciones en controlador

### Ejercicio 2: Reporte de Cobertura

**Tareas:**
1. Crear acción GenerarReporteCoberturaVsProyeccion
2. Consultar servicios con cálculo de cobertura
3. Generar PDF con tabla de coberturas
4. Agregar gráfico o indicadores visuales
5. Agregar formulario en la vista

### Ejercicio 3: Exportación a Excel

**Tareas:**
1. Instalar EPPlus
2. Implementar exportación de servicios
3. Agregar estilos a las celdas
4. Incluir totales y promedios
5. Agregar botón en la vista

---

