# Nombre del Proyecto
> Breve descripción del proyecto o una línea que resuma su propósito principal.
---
## 👥 Integrantes del Grupo
* **Lucas Zarate** - *maxolucas@gmail.com* - [@Maxorgz](https://github.com/Maxorgz) - Discord: `maxy_1604`
* **Juan Genero** - *juanignaciogenero@gmail.com* - [@Juani-ds](https://github.com/Juani-ds) - Discord: `juanistratos`
* **Daniel Rodriguez** - *danielrodriguez45b@gmail.com* - [@Danirodriguez45B](https://github.com/Danirodriguez45B) - Discord: `dani45gsr`
---
## 📐 Modelado de Datos
A continuación se presenta el esquema del modelo de datos correspondiente a la aplicación:
### Diagrama Entidad-Relación (DER) / Diagrama de Clases
![Diagrama del Proyecto](./diagrama/inmobiliariagrupo22.png)
> **Nota:** Puedes adjuntar la imagen en el repositorio (por ejemplo, en una carpeta `/docs` o `/img`) y enlazarla como se muestra arriba, o pegar directamente un diagrama generado en Mermaid.
<details>
<summary>Ver diagrama en código Mermaid (Opcional)</summary>
```mermaid
erDiagram
    USUARIO ||--o{ PEDIDO : realiza
    PEDIDO ||--|{ DETALLE_PEDIDO : contiene
    PRODUCTO ||--o{ DETALLE_PEDIDO : pertenece