## Description

Blends two normal maps defined by inputs **A** and **B** together, normalizing the result to create a valid normal map.

## Ports

| Name        | Direction           | Type  | Binding | Description |
|:------------ |:-------------|:-----|:---|:---|
| A      | Input | Vector 3 | None | First input value |
| B      | Input | Vector 3 | None | Second input value |
| Out | Output      |    Vector 3 | None | Output value |

## Shader Function

```
Out = normalize(float3(A.rg + B.rg, A.b * B.b));
```