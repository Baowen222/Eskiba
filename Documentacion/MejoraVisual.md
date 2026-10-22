# Eskiba: menú y mejora visual

Cambios y comprobaciones realizados el 22 de octubre de 2026 con Unity 6000.3.23f1.

## Cómo abrirlo

Abre `Assets/Escenas/Menu.unity` y pulsa Play. La lista compartida de escenas de Build Profiles ya contiene **Menu (índice 0)** y **Juego (índice 1)**, ambas activadas. No hay que arrastrar referencias ni configurar los botones manualmente.

JUGAR carga Juego. El menú lee el récord mediante `IGuardado` y `GuardadoLocal`, con la misma clave `Record` de PlayerPrefs que ya utilizaba la partida. SALIR llama a `Application.Quit()`; en el Editor detiene Play. Los dos paneles permiten volver al menú; la navegación restaura `Time.timeScale = 1` antes de cambiar de escena.

## Archivos creados

| Archivo | Responsabilidad |
| --- | --- |
| `Assets/Escenas/Menu.unity` | Menú editable con cámara, Canvas, zona segura, decoración, récord, botones y EventSystem. |
| `Assets/Scripts/Interfaz/MenuPrincipal.cs` | Leer el récord y atender JUGAR/SALIR. |
| `Assets/Scripts/Interfaz/NavegacionEscenas.cs` | Cargar Juego o Menu restaurando el tiempo. |
| `Assets/Scripts/Interfaz/ZonaSegura.cs` | Convertir Screen.safeArea en anchors y actualizar al cambiar de pantalla. |
| `Assets/Scripts/Interfaz/AnimacionBoton.cs` | Reducir suavemente un botón al pulsarlo, incluso durante la pausa. |
| `Assets/Scripts/Interfaz/FlotarDecoracion.cs` | Oscilación suave de los elementos decorativos del menú. |
| `Assets/Scripts/Interfaz/FondoPartida.cs` | Ajustar el fondo a la cámara sin alterar sus límites. |
| `Assets/Editor/PrepararAspectoEskiba.cs` | Herramienta opcional que creó los recursos y configuró las escenas. Solo se ejecuta en el Editor. |
| `Assets/Sprites/Interfaz/Redondeado.png` | Sprite pequeño con bordes para botones y tarjetas con Image Sliced. |
| `Assets/Sprites/Interfaz/FondoArcade.png` | Gradiente y cuadrícula decorativa. |
| `Assets/Sprites/Interfaz/FondoSinLuz.mat` | Material URP sin iluminación para que el fondo mantenga sus colores. |
| `Documentacion/Capturas/*.png` | Capturas de las comprobaciones en Play, fuera de los recursos del juego. |
| `Documentacion/VerificacionEscenas.json` | Comprobación final de escenas, scripts, botones, escaladores y disponibilidad de Android. |

Unity también genera los archivos `.meta` correspondientes a sus recursos.

## Archivos modificados

| Archivo | Cambio |
| --- | --- |
| `Assets/Escenas/Juego.unity` | Estilo del HUD y paneles; zona segura; fondo; botones de pausa y menú; acciones de continuar y reiniciar conectadas. |
| `Assets/Prefabs/Jugador.prefab` | Color turquesa y triángulo interior decorativo. |
| `Assets/Prefabs/Obstaculo.prefab` | Color rosa y cuadrado interior decorativo. |
| `Assets/Scripts/Guardado/Puntuacion.cs` | Etiquetas españolas y formato visual de los números. No cambia cálculo ni guardado. |
| `Assets/Scripts/Juego/GameOver.cs` | Formato del resultado y método VolverAlMenu. |
| `Assets/Scripts/Juego/PausaJuego.cs` | Pausar accesible al botón y método VolverAlMenu. |
| `Assets/Scripts/Jugador/MoverJugador.cs` | Ignorar pulsaciones sobre botones de interfaz. Conserva movimiento, límites y controles táctiles/ratón. |
| `ProjectSettings/EditorBuildSettings.asset` | Menu antes de Juego. |
| `ProjectSettings/ProjectSettings.asset` | Orientación Portrait y giros laterales desactivados. |

No se han modificado `IGuardado`, `GuardadoLocal`, `GenerarObstaculos`, `MoverObstaculo` ni `ColisionJugador`. Se compararon los bloques serializados de los colliders y Rigidbody con los originales: permanecen iguales. Tampoco se cambiaron las fórmulas ni los valores de dificultad de la escena.

## Decisiones visuales y adaptación

Fondo azul noche, cuadrícula tenue, turquesa para el jugador y acciones principales, rosa para los obstáculos y Game Over. Texto blanco y secundario azul claro. Tarjetas y botones redondeados, sin recursos de pago. El sprite redondeado se reutiliza; el fondo no necesita postprocesado.

Los Canvas usan Scale With Screen Size, referencia 1080 × 1920 y Match 0,5. Los anchors sitúan los elementos proporcionalmente y los controles están dentro de ZonaSegura. Los botones principales miden 720 × 112–132 unidades de diseño; pausa mide 120 × 120. La decoración no intercepta toques. Las animaciones utilizan tiempo sin escalar para funcionar en pausa.

Los objetos y las acciones de los botones quedan guardados en las escenas y se pueden modificar en el Inspector. No se construye la interfaz durante cada partida. La herramienta `Eskiba > Aplicar aspecto arcade` es opcional: no hace falta ejecutarla para jugar; vuelve a aplicar los ajustes de Juego y conserva un Menu ya existente.

## Cómo explicar la arquitectura

- **Servicio abstraído:** `IGuardado` conserva el contrato CargarRecord/GuardarRecord; `GuardadoLocal` implementa PlayerPrefs.
- **Patrón Estrategia:** la implementación de guardado es la estrategia concreta utilizada a través de `IGuardado`. Se mantiene la elección sencilla del proyecto: `new GuardadoLocal()` al iniciar el consumidor. Otra implementación podría cumplir el mismo contrato.
- **SOLID, responsabilidad única:** el movimiento controla al jugador, los gestores conservan las reglas de partida, el servicio guarda el récord y los componentes de Interfaz se encargan de presentación y navegación.
- **Multiplataforma:** se conservan Touchscreen en móvil y Mouse en ordenador; SceneManager, PlayerPrefs, uGUI y Application.Quit son APIs de Unity. La referencia a UnityEditor de SALIR está protegida con `#if UNITY_EDITOR`.

No se incorporan contenedores de inyección, gestores globales persistentes ni librerías de animación. Unity Pipeline se utilizó temporalmente para conducir el Editor y se retiró tras las comprobaciones.

## Pruebas realizadas

Estas son pruebas ejecutadas en Play en el Editor, mediante Unity CLI y el Input System; no son pruebas en un teléfono físico ni una build Android.

| Comprobación | Resultado observado |
| --- | --- |
| Compilación de scripts | Completada, sin errores. |
| Menú en Play | Renderiza título, récord, decoración y botones. Se verificó que avanzaban los fotogramas. |
| JUGAR | La acción del botón carga Juego, con Time.timeScale a 1, jugador y generador activos. |
| Control táctil | Eventos TouchState simulados desplazaron al jugador de (0; −4,53) a (−1,25; −1,35) y después a (1,35; 0,73). |
| Toque en pausa | Toque Began/Ended sobre el botón activó el panel, dejó Time.timeScale a 0 y mantuvo al jugador en (0; −4,53). |
| Obstáculos y dificultad | Aparecen y caen. Tras 29,14 segundos, la velocidad inicial de la escena, 3, pasó a aproximadamente 4,4 y el intervalo de 1,5 a 1,21 segundos. Para observarlo sin perder se desactivó temporalmente el collider solo en Play y después se reactivó. |
| Colisiones | Una colisión natural terminó una partida. Otra prueba situó un obstáculo sobre el jugador: activó Game Over, ocultó los dos textos del HUD y detuvo el tiempo. |
| Récord | Se guardó un récord de prueba de 31, que permaneció al reiniciar y al volver al menú desde ambos paneles. El menú mostró 031. Se restauró el valor inicial 0 al terminar las pruebas. |
| Continuar | Oculta la pausa y restaura Time.timeScale a 1. |
| Reiniciar | Verificado desde Game Over y desde pausa; nueva partida con collider activo y panel de pausa oculto. |
| Volver al menú | Verificado desde los dos paneles; Menu activo y Time.timeScale a 1. |
| Pérdida de foco | Invocación simulada de OnApplicationFocus(false): muestra pausa y detiene el tiempo. |
| Zonas pulsables | Raycasts del EventSystem alcanzan JUGAR, SALIR y pausa en su centro. |
| Resoluciones | Revisión visual del menú a 1080 × 1920, 1080 × 2400 y 1080 × 1440; pausa también revisada en la resolución compacta. |
| Consola | Sin errores ni advertencias durante las comprobaciones de gameplay e interfaz. |
| Lista de escenas | Menu y Juego activadas, en ese orden. |
| SALIR | Verificado en el Editor: detiene Play. Cierre en Android pendiente. |

Las capturas muestran estados de prueba; los números que aparecen en ellas no son una nueva partida guardada en los recursos.

## Pendiente en Android

Unity 6000.3.23f1 informa que Android no está disponible como plataforma de compilación. El módulo Android instalado para Unity 6000.0.59f2 no sirve para esta instalación 6.3.

1. En Unity Hub, añade **Android Build Support**, **Android SDK & NDK Tools** y **OpenJDK** a **6000.3.23f1**.
2. En Build Profiles, selecciona Android y utiliza la lista compartida de escenas ya configurada. Si creas un perfil con Override Global Scene List, incluye Menu primero y Juego después.
3. Genera una APK de prueba y ejecútala en un móvil vertical.
4. Comprueba arrastre real, multitáctil, zona segura con notch/barra de gestos, pausa al cambiar de aplicación, cierre con SALIR, rendimiento y récord después de cerrar y abrir la aplicación.

No se ha generado una APK/AAB ni se ha comprobado la persistencia después de reiniciar un proceso Android. La zona segura está implementada mediante Screen.safeArea, pero requiere revisión en dispositivo con recortes reales.
