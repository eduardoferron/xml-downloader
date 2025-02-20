# Fiscalapi XML Downloader (sat-ws-descarga-masiva)

[![NuGet Version](https://img.shields.io/nuget/v/Fiscalapi.XmlDownloader)](https://www.nuget.org/packages/Fiscalapi.XmlDownloader)
[![NuGet](https://img.shields.io/nuget/v/FiscalApi.svg)](https://www.nuget.org/packages/FiscalApi/)
[![License](https://img.shields.io/github/license/FiscalAPI/xml-downloader)](https://github.com/FiscalAPI/xml-downloader/blob/main/LICENSE)



---
:us: The documentation of this project is in Spanish as this is the natural language for intended audience.

:mexico: La documentación del proyecto está en español porque ese es el lenguaje principal de los usuarios.

---

## Descripción

Librería .NET para consultar y descargar facturas (CFDI) emitidas y recibidas a través del servicio web del SAT, incluyendo la obtención de metadata. Este servicio es parte del sistema "Consulta y recuperación de comprobantes" del SAT ([documentación oficial](https://www.sat.gob.mx/consultas/42968/consulta-y-recuperacion-de-comprobantes-(nuevo))).

### Casos de Uso

- Automatización de cadena de suministros
- Automatización de cuentas por pagar
- Automatización de cuentas por cobrar
- Contabilidad electrónica
- Contabilidad general
- Generación de pólizas contables

## Instalación

```shell
Install-Package Fiscalapi.XmlDownloader -Version 4.0.120
```

:warning: Esta librería depende de [Fiscalapi.Credentials](https://github.com/FiscalAPI/credentials/). Se recomienda leer su documentación antes de continuar.

## Arquitectura del Proyecto

### Estructura de Código

```
src/
├── XmlService/           # Clase principal que consume los servicios
├── Services/             # Implementación de servicios
│   ├── Authenticate/    
│   ├── Query/           
│   ├── Verify/          
│   └── Download/        
├── Common/              # Objetos compartidos
├── Models/              # Objetos DTO
├── Packaging/           # Manejo de paquetes del SAT
├── Builder/             # Generación de mensajes SOAP
└── SoapClient/          # Cliente HTTP para el Webservice
```

### Servicios Principales

Cada servicio (`Authenticate`, `Query`, `Verify`, `Download`) contiene:
- `Result`: Resultado de la operación
- `Parameters`: Parámetros de operación

## Funcionamiento del Servicio Web

### Flujo de Operación

1. **Autenticación**: Utilizando FIEL (manejo automático del token)
2. **Solicitud**: Especificación de parámetros (fechas, tipo de solicitud)
3. **Verificación**: Consulta de disponibilidad
4. **Descarga**: Obtención de paquetes

![Diagrama de Flujo](https://user-images.githubusercontent.com/28969854/167732245-23c30b94-3feb-4d89-bee6-2b0f591203cf.svg)

### Límites y Consideraciones

- Hasta 200,000 registros por petición (1,000,000 en metadata)
- Sin límite en número de solicitudes
- Tiempo de respuesta variable (minutos a horas)

## Documentación Oficial

- [Portal SAT - Consulta y Recuperación](https://www.sat.gob.mx/consultas/42968/consulta-y-recuperacion-de-comprobantes-(nuevo))
- [Solicitud de Descargas](https://www.sat.gob.mx/cs/Satellite?blobcol=urldata&blobkey=id&blobtable=MungoBlobs&blobwhere=1579314716402&ssbinary=true)
- [Verificación de Solicitudes](https://www.sat.gob.mx/cs/Satellite?blobcol=urldata&blobkey=id&blobtable=MungoBlobs&blobwhere=1579314716409&ssbinary=true)
- [Descarga de Solicitudes](https://www.sat.gob.mx/cs/Satellite?blobcol=urldata&blobkey=id&blobtable=MungoBlobs&blobwhere=1579314716395&ssbinary=true)

## Ejemplos de Uso

Consulte la carpeta `samples/WinFormApp` para ejemplos detallados de implementación en diferentes tipos de aplicaciones (.NET 8).

## Compatibilidad

- Compatible con .NET 8
- Soporta aplicaciones Windows Forms, Console y Web
- Seguimos [Versionado Semántico 2.0.0](docs/SEMVER.md)

## Contribuciones

Las contribuciones son bienvenidas. Por favor lea [CONTRIBUTING][] para más detalles y revise los archivos [TODO][] y [CHANGELOG][].

## Roadmap

- [x] Descarga de CFDI emitidos y recibidos
- [x] Descarga de metadata de CFDI
- [ ] Documentación exhaustiva

## Licencia

Copyright © FISCAL API S DE R.L DE C.V. Este proyecto está licenciado bajo la [Licencia MIT](LICENSE).