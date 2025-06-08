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

