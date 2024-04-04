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
          content:
            text/plain:
              schema:
                type: array
                items:
                  $ref: "#/components/schemas/PercentageOfTags"
            application/json:
              schema:
                type: array
                items:
                  $ref: "#/components/schemas/PercentageOfTags"
            text/json:
              schema:
                type: array
                items:
                  $ref: "#/components/schemas/PercentageOfTags"
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
          content:
            text/plain:
              schema:
                type: array
                items:
                  $ref: "#/components/schemas/StackOverflowTag"
            application/json:
              schema:
                type: array
                items:
                  $ref: "#/components/schemas/StackOverflowTag"
            text/json:
              schema:
                type: array
                items:
                  $ref: "#/components/schemas/StackOverflowTag"
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
components:
  schemas:
    PercentageOfTags:
      type: object
      properties:
        name:
          type: string
          nullable: true
        count:
          type: integer
          format: int32
        percentage:
          type: number
          format: double
      additionalProperties: false
    StackOverflowTag:
      type: object
      properties:
        name:
          type: string
          nullable: true
        count:
          type: integer
          format: int32
      additionalProperties: false
```
