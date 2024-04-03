# Mediporta Rekrutacja

## Swagger Api Config

[**Swagger Editor**](https://editor.swagger.io/)

```yaml
openapi: 3.0.1
info:
  title: Mediporta Rekrutacja
  version: "1.0"
paths:
  /api/db/tags/count:
    get:
      tags:
        - DatabaseTag
      parameters:
        - name: page
          in: query
          schema:
            type: integer
            format: int32
            default: 1
        - name: size
          in: query
          schema:
            type: integer
            format: int32
            default: 20
      responses:
        "200":
          description: Success
  /api/db/tags:
    get:
      tags:
        - DatabaseTag
      parameters:
        - name: page
          in: query
          schema:
            type: integer
            format: int32
            default: 1
        - name: size
          in: query
          schema:
            type: integer
            format: int32
            default: 20
        - name: sort
          in: query
          schema:
            type: string
            default: id
        - name: direction
          in: query
          schema:
            type: string
            default: asc
      responses:
        "200":
          description: Success
  /api/so/tags:
    get:
      tags:
        - StackOverflowTag
      parameters:
        - name: size
          in: query
          schema:
            type: integer
            format: int32
            default: 1000
      responses:
        "200":
          description: Success
components: {}
```
