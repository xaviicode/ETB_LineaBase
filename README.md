
# LineaBaseETB_V2

Aplicación de escritorio WPF para consultar y validar Work Items en Azure DevOps de forma segura y eficiente, sin depender de scripts externos.

## Características principales

- Conexión segura a Azure DevOps usando PAT.
- Consulta y validación robusta de Work Items.
- Interfaz amigable y clara (WPF + MVVM).
- Resultados y errores mostrados visualmente.
- Fácil mantenimiento y escalabilidad.

## Requisitos

- .NET 6.0 o superior
- Visual Studio 2022 o superior
- Azure DevOps Personal Access Token (PAT)

## Instalación

1. Clona este repositorio:
git clone https://github.com/xaviicode/ETB_LineaBase.git
o
git@github.com:xaviicode/ETB_LineaBase.git

2. Abre la solución `LineaBaseETB_V2.sln` en Visual Studio.
3. Restaura los paquetes NuGet.
4. Compila y ejecuta el proyecto.

## Uso

1. Ingresa tu organización de Azure DevOps y tu PAT.
2. Selecciona el proyecto y ejecuta la consulta de Work Items.
3. Visualiza los resultados y errores en la interfaz.

## Estructura del proyecto

- /Models: Modelos de datos
- /ViewModels: Lógica de presentación (MVVM)
- /Views: Vistas WPF
- /Services: Servicios de integración (Azure DevOps)
- /Helpers: Utilidades y helpers

![image](https://github.com/user-attachments/assets/3003944d-2037-4dcb-bf6e-956a20c359b7)


## Contribución

¿Quieres contribuir? Abre un issue o haz un pull request.

## Licencia

MIT

---
---

## Detalles de avance de la aplicación.

# Aplicación WPF de Consulta y Gestión de Work Items en Azure DevOps


## ¿Qué hace la aplicación?
- Conexión segura a Azure DevOps (PAT)
- Consulta y visualización de Work Items
- Filtros avanzados: Estado, ID, Iniciativa
- Limpieza de filtros sin recargar

#Diagrama de frujo:

![image](https://github.com/user-attachments/assets/66a99a82-a7af-4ace-98d4-1294c4deda76)



## Avances logrados
- Autenticación y selección de proyecto
- Consulta eficiente (SDK oficial)
- Visualización clara y completa
- Filtros funcionales y búsqueda parcial
- Interfaz intuitiva y validaciones visuales


## Beneficios para el equipo
- Ahorro de tiempo y reducción de errores
- Facilita el análisis y la toma de decisiones
- Mejora la colaboración y acceso a la información


## Próximos pasos y mejoras
- Paginación para grandes volúmenes de datos.
- Carga dinámica de valores de filtro: Cambio del tipo de filtrado, es decir que este se pueda hacer de manera masiva todos los proyectos,
 y que se pueda consultar por iniciativa de manera general permitiendo encontrar los carritos y este muestre a su vez los aviones
 que en cada iniciativa de entrega esta evitando entrar a las Celular donde están alojados los aviones. 
- Mejoras visuales y feedback avanzado por medio de mensajes. 
- scroll horizontal para ver los campos más detallados.
- Automatización de despliegue.


## Estado actual
- MVP funcional y estable
- Base técnica sólida y escalable
- Listo para pruebas de usuario


