# Sistema Control de Almuerzos — Historia del Proyecto

## El problema

Trabajo en gastronomía. Todos los días, al mediodía, el mismo escenario: una fila de personas esperando para registrar su almuerzo, y un sistema que falla en cadena.

El proceso actual en Laboratorios Roemmers funciona así:

1. El empleado muestra un QR desde el celular
2. Si el QR no escanea → se ingresa un código numérico a mano
3. Si el código falla → se busca el nombre en una lista escrita a papel

Cada paso es un punto de falla. Celulares sin batería, QRs que no leen bajo ciertas luces, WiFi intermitente, personas mayores sin smartphone, empleados sin datos móviles. El resultado es una fila que crece, registros duplicados y datos inconsistentes al cierre del día.

Vi ese problema durante años. Y decidí construir la solución.

---

## La solución

Un sistema web de registro de comensales por credencial, diseñado para funcionar en una tablet fija en el acceso al comedor.

El flujo es simple: el empleado acerca su credencial a un lector RFID Bluetooth conectado a la tablet → el sistema valida la credencial en tiempo real → registra el almuerzo en menos de un segundo → muestra el nombre y empresa del comensal en pantalla como confirmación.

Sin celular. Sin QR. Sin papel. Sin fila.

La infraestructura de credenciales ya existe en Roemmers — se usa para otros accesos en el edificio. No hay inversión adicional en hardware: solo un lector RFID Bluetooth (~30 USD) conectado a la tablet existente.

---

## Quién lo construyó y cómo

Estoy estudiando la Tecnicatura Universitaria en Programación en UTN FRGP. Este proyecto nació como solución a un problema real que vivo en mi trabajo, y creció hasta convertirse en mi portfolio de transición hacia IT.

El sistema original (WinForms, C#) lo desarrollé sin asistencia de IA: lógica de negocio, base de datos, stored procedures, interfaz. Lo domino completamente y puedo explicar cada decisión.

Esta versión web fue construida con **Agentic Coding**: yo dirigí la arquitectura, tomé todas las decisiones técnicas y supervisé cada cambio. La implementación fue asistida por Claude Code (Anthropic). El proceso completo está documentado en 51 guías en la carpeta `/Guias/`, una por cada commit, que registran qué se hizo, por qué y con qué herramientas.

No es un proyecto de práctica. Es un sistema que resuelve un problema real, a la escala real del lugar donde trabajo.

---

## Escala real

- **~500 empleados** distribuidos en 6 empresas del complejo industrial
- **2 lugares de servicio**: Comedor (280–340 comensales/día) y Quincho (160–200 comensales/día)
- **Datos históricos**: servicios y registros de todos los días hábiles del año en curso

El seed data del sistema refleja estos números reales para que cualquier demo muestre el comportamiento a escala de producción.

---

## Stack técnico (resumen)

| Capa | Tecnología |
|---|---|
| Backend | ASP.NET Core MVC (.NET 9) |
| ORM | Entity Framework Core 9 con Fluent API |
| Autenticación | ASP.NET Core Identity (roles Admin / Usuario) |
| Base de datos | SQL Server 2019+ |
| Frontend | Bootstrap 5 + Bootstrap Icons + CSS custom |
| PDF | QuestPDF |
| Email | MailKit / MimeKit |
| Lector RFID | HID Bluetooth (keyboard emulation, cualquier marca) |

Para más detalle técnico: [README.md](README.md)

---

## Estado actual

El sistema está completo y funcional localmente. Pendiente de deploy en servidor (Azure for Students).

El código, las guías y este documento son el material que acompaña mi CV para la presentación en Laboratorios Roemmers.

---

*Facundo Herrera · UTN FRGP · [GitHub](https://github.com/f-Rra)*
