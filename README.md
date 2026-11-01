# Misterio en el campus

Proyecto del TFG "Generación procedural de casos deducibles para un videojuego de misterio", de César Montero Pérez.

Es un juego de misterio en 2D. Durante las vacaciones de Navidad alguien roba un libro de la biblioteca del campus, y hay que averiguar dónde estaba y qué hacía cada sospechoso a las 23:00. Los casos se generan solos. Cada caso se trata como un problema de satisfacción de restricciones (CSP), y dos solucionadores se aseguran de que tenga una sola solución y de que se pueda resolver pensando, sin adivinar.


## Cómo va

Por ahora está hecho el núcleo lógico, que funciona sin Unity.

- El modelo del caso con dos tipos de pista. Unas hablan de un sospechoso ("Josep no estaba en la biblioteca") y otras relacionan un lugar con una actividad ("quien estaba en la cafetería estaba cenando").
- El solucionador completo que cuenta cuántas soluciones tiene un caso con búsqueda con vuelta atrás y propagación.
- El solucionador humano que resuelve el caso como lo haría una persona, deducción a deducción. Usa cuatro técnicas, cada una con su coste (descarte directo, exclusividad, valor único y combinar pistas), y apunta los pasos que va siguiendo. Sumando los costes sale la dificultad del caso.
- 53 pruebas unitarias. Una compara el solucionador completo con una búsqueda exhaustiva hecha aparte, en casos al azar.

Todavía falta el generador de casos y la parte jugable.


## Abrir el proyecto

1. Clona el repositorio o descárgalo como ZIP.
2. En Unity Hub, Add > Add project from disk y elige la carpeta.


## Pruebas

Están hechas con NUnit y se lanzan desde Window > General > Test Runner, pestaña EditMode, botón Run All.

Si quieres ver cómo razona el solucionador humano ejecuta la prueba `BasicCase_WriteStepsInConsole` y mira la consola. Salen todos los pasos que sigue con el caso de ejemplo.


## Dónde está cada cosa

- `Assets/Scripts/Core` tiene el modelo del caso y los dos solucionadores. No depende de Unity (su Assembly Definition no tiene referencias al motor).
- `Assets/Scripts/Core/Techniques` tiene las técnicas del solucionador humano, una clase por técnica.
- `Assets/Tests/EditMode` tiene las pruebas.