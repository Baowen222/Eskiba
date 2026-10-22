using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

// Herramienta de edición: deja los objetos y referencias guardados en las escenas.
// No se ejecuta ni se incluye en Android.
public static class PrepararAspectoEskiba
{
    static readonly Color Fondo = ColorHex("0B1028");
    static readonly Color Tarjeta = ColorHex("192342");
    static readonly Color Turquesa = ColorHex("49F2D2");
    static readonly Color Rosa = ColorHex("FF639B");
    static readonly Color Suave = ColorHex("A5B4D6");
    static Sprite redondeado;
    static Sprite fondo;
    static TMP_FontAsset fuente;

    [MenuItem("Eskiba/Aplicar aspecto arcade")]
    public static void Aplicar()
    {
        if (EditorApplication.isPlaying) throw new System.Exception("Sal de Play antes de editar.");
        CrearRecursos();
        MejorarPrefab("Assets/Prefabs/Jugador.prefab", Turquesa, true);
        MejorarPrefab("Assets/Prefabs/Obstaculo.prefab", Rosa, false);
        PrepararJuego();
        CrearMenu();
        var escenas = EditorBuildSettings.scenes.Where(e =>
            e.path != "Assets/Escenas/Menu.unity" && e.path != "Assets/Escenas/Juego.unity");
        EditorBuildSettings.scenes = new[] {
            new EditorBuildSettingsScene("Assets/Escenas/Menu.unity", true),
            new EditorBuildSettingsScene("Assets/Escenas/Juego.unity", true)
        }.Concat(escenas).ToArray();
        // El proyecto estaba configurado con autorrotación, incluidos los laterales.
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToPortrait = true;
        AssetDatabase.SaveAssets();
        Debug.Log("ESKIBA: menú, partida, prefabs y escenas preparados.");
    }

    static Color ColorHex(string valor)
    {
        ColorUtility.TryParseHtmlString("#" + valor, out var color);
        return color;
    }

    static void CrearRecursos()
    {
        Directory.CreateDirectory("Assets/Sprites/Interfaz");
        var textura = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        for (int y = 0; y < 64; y++)
        for (int x = 0; x < 64; x++)
        {
            Vector2 distancia = new Vector2(Mathf.Max(Mathf.Abs(x - 31.5f) - 15.5f, 0),
                Mathf.Max(Mathf.Abs(y - 31.5f) - 15.5f, 0));
            textura.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(16f - distancia.magnitude)));
        }
        redondeado = GuardarSprite(textura, "Redondeado", new Vector4(20, 20, 20, 20));
        Object.DestroyImmediate(textura);

        textura = new Texture2D(540, 960, TextureFormat.RGBA32, false);
        for (int y = 0; y < 960; y++)
        for (int x = 0; x < 540; x++)
        {
            float u = x / 539f;
            float v = y / 959f;
            Color color = Color.Lerp(ColorHex("090F24"), ColorHex("181936"), v);
            float brillo = Mathf.Exp(-((u - 0.15f) * (u - 0.15f) * 9f +
                (v - 0.75f) * (v - 0.75f) * 14f));
            color = Color.Lerp(color, ColorHex("233459"), brillo * 0.45f);
            if (x % 54 == 0 || y % 54 == 0) color = Color.Lerp(color, Suave, 0.045f);
            if (x % 54 == 0 && y % 54 == 0) color = Color.Lerp(color, Suave, 0.25f);
            textura.SetPixel(x, y, color);
        }
        fondo = GuardarSprite(textura, "FondoArcade", Vector4.zero);
        Object.DestroyImmediate(textura);
        fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
    }

    static Sprite GuardarSprite(Texture2D textura, string nombre, Vector4 borde)
    {
        string ruta = "Assets/Sprites/Interfaz/" + nombre + ".png";
        File.WriteAllBytes(ruta, textura.EncodeToPNG());
        AssetDatabase.ImportAsset(ruta);
        var importador = (TextureImporter)AssetImporter.GetAtPath(ruta);
        importador.textureType = TextureImporterType.Sprite;
        importador.spriteImportMode = SpriteImportMode.Single;
        importador.spritePixelsPerUnit = 100;
        importador.spriteBorder = borde;
        importador.mipmapEnabled = false;
        importador.alphaIsTransparency = true;
        importador.textureCompression = TextureImporterCompression.Uncompressed;
        importador.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
    }

    static RectTransform Objeto(string nombre, Transform padre)
    {
        var objeto = new GameObject(nombre, typeof(RectTransform));
        objeto.layer = 5;
        var rectangulo = objeto.GetComponent<RectTransform>();
        rectangulo.SetParent(padre, false);
        return rectangulo;
    }

    static void Colocar(RectTransform rectangulo, Vector2 ancla, Vector2 tamano, Vector2 posicion)
    {
        rectangulo.anchorMin = rectangulo.anchorMax = ancla;
        rectangulo.pivot = new Vector2(0.5f, 0.5f);
        rectangulo.sizeDelta = tamano;
        rectangulo.anchoredPosition = posicion;
        rectangulo.localScale = Vector3.one;
    }

    static void Estirar(RectTransform rectangulo)
    {
        rectangulo.anchorMin = Vector2.zero;
        rectangulo.anchorMax = Vector2.one;
        rectangulo.offsetMin = rectangulo.offsetMax = Vector2.zero;
        rectangulo.localScale = Vector3.one;
    }

    static UnityEngine.UI.Image Imagen(RectTransform rectangulo, Color color, Sprite sprite = null)
    {
        var imagen = rectangulo.GetComponent<UnityEngine.UI.Image>() ??
            rectangulo.gameObject.AddComponent<UnityEngine.UI.Image>();
        imagen.sprite = sprite ?? redondeado;
        imagen.type = UnityEngine.UI.Image.Type.Sliced;
        imagen.color = color;
        imagen.raycastTarget = false;
        return imagen;
    }

    static TextMeshProUGUI Texto(string nombre, Transform padre, string contenido,
        float tamano, Color color, Vector2 ancla, Vector2 dimensiones)
    {
        var rectangulo = Objeto(nombre, padre);
        Colocar(rectangulo, ancla, dimensiones, Vector2.zero);
        var texto = rectangulo.gameObject.AddComponent<TextMeshProUGUI>();
        EstiloTexto(texto, tamano, color);
        texto.text = contenido;
        return texto;
    }

    static void EstiloTexto(TextMeshProUGUI texto, float tamano, Color color)
    {
        texto.font = fuente;
        texto.enableAutoSizing = false;
        texto.fontSize = tamano;
        texto.margin = Vector4.zero;
        texto.color = color;
        texto.alignment = TextAlignmentOptions.Center;
        texto.fontStyle = FontStyles.Bold;
        texto.textWrappingMode = TextWrappingModes.NoWrap;
        texto.raycastTarget = false;
    }

    static UnityEngine.UI.Button Boton(string nombre, Transform padre, string etiqueta,
        Vector2 ancla, Vector2 tamano, Color color, Color tinta, UnityAction accion)
    {
        var rectangulo = Objeto(nombre, padre);
        Colocar(rectangulo, ancla, tamano, Vector2.zero);
        Imagen(rectangulo, color);
        var boton = rectangulo.gameObject.AddComponent<UnityEngine.UI.Button>();
        AplicarBoton(boton, etiqueta, color, tinta, accion);
        return boton;
    }

    static void AplicarBoton(UnityEngine.UI.Button boton, string etiqueta, Color color,
        Color tinta, UnityAction accion)
    {
        var imagen = Imagen((RectTransform)boton.transform, color);
        imagen.raycastTarget = true;
        boton.targetGraphic = imagen;
        boton.navigation = new UnityEngine.UI.Navigation { mode = UnityEngine.UI.Navigation.Mode.None };
        var colores = boton.colors;
        colores.normalColor = Color.white;
        colores.highlightedColor = new Color(0.92f, 0.96f, 1);
        colores.pressedColor = new Color(0.72f, 0.8f, 0.9f);
        colores.fadeDuration = 0.12f;
        boton.colors = colores;
        if (!boton.GetComponent<AnimacionBoton>()) boton.gameObject.AddComponent<AnimacionBoton>();
        var texto = boton.GetComponentInChildren<TextMeshProUGUI>(true);
        if (!texto) texto = Texto("Etiqueta", boton.transform, etiqueta, 36, tinta,
            new Vector2(0.5f, 0.5f), Vector2.zero);
        Estirar(texto.rectTransform);
        EstiloTexto(texto, 36, tinta);
        texto.text = etiqueta;
        boton.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        UnityEventTools.AddPersistentListener(boton.onClick, accion);
    }

    static RectTransform AreaSegura(Transform lienzo)
    {
        var existente = lienzo.Find("ZonaSegura") as RectTransform;
        if (existente) return existente;
        var zona = Objeto("ZonaSegura", lienzo);
        Estirar(zona);
        zona.gameObject.AddComponent<ZonaSegura>();
        return zona;
    }

    static void ConfigurarLienzo(Canvas lienzo)
    {
        var escalador = lienzo.GetComponent<UnityEngine.UI.CanvasScaler>() ??
            lienzo.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        escalador.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1080, 1920);
        escalador.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        escalador.matchWidthOrHeight = 0.5f;
        if (!lienzo.GetComponent<UnityEngine.UI.GraphicRaycaster>())
            lienzo.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
    }

    static void CrearMenu()
    {
        // Si ya existe el menú generado, se abre sin reemplazar cambios manuales.
        if (File.Exists("Assets/Escenas/Menu.unity"))
        {
            EditorSceneManager.OpenScene("Assets/Escenas/Menu.unity");
            return;
        }
        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camara = new GameObject("CamaraMenu", typeof(Camera)).GetComponent<Camera>();
        camara.clearFlags = CameraClearFlags.SolidColor;
        camara.backgroundColor = Fondo;
        camara.orthographic = true;
        camara.transform.position = new Vector3(0, 0, -10);
        var lienzo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas)).GetComponent<Canvas>();
        lienzo.renderMode = RenderMode.ScreenSpaceOverlay;
        ConfigurarLienzo(lienzo);
        var fondoMenu = Objeto("FondoArcade", lienzo.transform);
        Estirar(fondoMenu);
        var dibujo = Imagen(fondoMenu, Color.white, fondo);
        dibujo.type = UnityEngine.UI.Image.Type.Simple;
        var zona = AreaSegura(lienzo.transform);
        var controlador = new GameObject("MenuPrincipal").AddComponent<MenuPrincipal>();
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        Texto("Edicion", zona, "A R C A D E   /   S U P E R V I V E N C I A", 24, Turquesa,
            new Vector2(0.5f, 0.91f), new Vector2(900, 50));
        var titulo = Texto("Titulo", zona, "ESKIBA", 148, Color.white,
            new Vector2(0.5f, 0.815f), new Vector2(960, 190));
        titulo.characterSpacing = 10;
        var sombra = titulo.gameObject.AddComponent<UnityEngine.UI.Shadow>();
        sombra.effectColor = new Color(Rosa.r, Rosa.g, Rosa.b, 0.65f);
        sombra.effectDistance = new Vector2(5, -7);
        Texto("Lema", zona, "ESQUIVA. RESISTE. SUPÉRATE.", 28, Suave,
            new Vector2(0.5f, 0.735f), new Vector2(900, 60));

        var jugador = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Jugador.prefab")
            .GetComponent<SpriteRenderer>().sprite;
        var obstaculo = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstaculo.prefab")
            .GetComponent<SpriteRenderer>().sprite;
        Decoracion("Nave", zona, jugador, Turquesa, new Vector2(0.5f, 0.59f), 180, 0);
        Decoracion("BloqueIzquierdo", zona, obstaculo, Rosa, new Vector2(0.22f, 0.64f), 68, 20);
        Decoracion("BloqueDerecho", zona, obstaculo, Rosa, new Vector2(0.79f, 0.55f), 94, -16);
        var estela = Objeto("Estela", zona);
        Colocar(estela, new Vector2(0.5f, 0.525f), new Vector2(6, 50), Vector2.zero);
        Imagen(estela, Turquesa * new Color(1, 1, 1, 0.5f));

        var tarjeta = Objeto("TarjetaRecord", zona);
        Colocar(tarjeta, new Vector2(0.5f, 0.43f), new Vector2(720, 158), Vector2.zero);
        Imagen(tarjeta, Tarjeta);
        Texto("EtiquetaRecord", tarjeta, "TU MEJOR MARCA", 24, Suave,
            new Vector2(0.5f, 0.73f), new Vector2(650, 40));
        var record = Texto("Record", tarjeta, "000", 60, Color.white,
            new Vector2(0.5f, 0.35f), new Vector2(650, 76));
        var datos = new SerializedObject(controlador);
        datos.FindProperty("textoRecord").objectReferenceValue = record;
        datos.ApplyModifiedPropertiesWithoutUndo();
        Boton("BotonJugar", zona, "JUGAR  >", new Vector2(0.5f, 0.30f),
            new Vector2(720, 132), Turquesa, Fondo, controlador.Jugar);
        Boton("BotonSalir", zona, "SALIR", new Vector2(0.5f, 0.20f),
            new Vector2(720, 112), Tarjeta, Color.white, controlador.Salir);
        Texto("Ayuda", zona, "Arrastra el dedo para esquivar los obstáculos", 27, Suave,
            new Vector2(0.5f, 0.10f), new Vector2(950, 60));
        Texto("Pie", zona, "UN INTENTO MÁS.", 22, Turquesa,
            new Vector2(0.5f, 0.04f), new Vector2(800, 50));
        EditorSceneManager.SaveScene(escena, "Assets/Escenas/Menu.unity");
    }

    static void Decoracion(string nombre, Transform padre, Sprite sprite, Color color,
        Vector2 ancla, float tamano, float giro)
    {
        var rectangulo = Objeto(nombre, padre);
        Colocar(rectangulo, ancla, Vector2.one * tamano, Vector2.zero);
        rectangulo.localRotation = Quaternion.Euler(0, 0, giro);
        var imagen = Imagen(rectangulo, color, sprite);
        imagen.type = UnityEngine.UI.Image.Type.Simple;
        imagen.preserveAspect = true;
        rectangulo.gameObject.AddComponent<FlotarDecoracion>();
        var interior = Objeto("Interior", rectangulo);
        Colocar(interior, new Vector2(0.5f, 0.5f), Vector2.one * tamano * 0.64f, Vector2.zero);
        var detalle = Imagen(interior, Fondo, sprite);
        detalle.type = UnityEngine.UI.Image.Type.Simple;
        detalle.preserveAspect = true;
    }

    static void PrepararJuego()
    {
        var escena = EditorSceneManager.OpenScene("Assets/Escenas/Juego.unity");
        var gestor = Object.FindFirstObjectByType<GameOver>();
        var pausa = Object.FindFirstObjectByType<PausaJuego>();
        var puntos = Object.FindFirstObjectByType<Puntuacion>();
        var lienzo = Object.FindFirstObjectByType<Canvas>();
        ConfigurarLienzo(lienzo);
        var zona = AreaSegura(lienzo.transform);
        puntos.textoPuntuacion.transform.SetParent(zona, false);
        puntos.textoRecord.transform.SetParent(zona, false);
        EstiloTexto(puntos.textoPuntuacion, 24, Color.white);
        EstiloTexto(puntos.textoRecord, 24, Suave);
        Colocar(puntos.textoPuntuacion.rectTransform, new Vector2(0, 1),
            new Vector2(350, 150), new Vector2(215, -120));
        puntos.textoPuntuacion.text = "PUNTOS\n<size=64>000</size>";
        Colocar(puntos.textoRecord.rectTransform, new Vector2(0.5f, 1),
            new Vector2(300, 140), new Vector2(0, -120));
        puntos.textoRecord.text = "RÉCORD\n<size=44>000</size>";
        if (!zona.Find("BotonPausa"))
        {
            var boton = Boton("BotonPausa", zona, "II", Vector2.one,
                new Vector2(120, 120), Tarjeta, Color.white, pausa.Pausar);
            boton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-110, -120);
        }
        PrepararPanel(gestor.panelGameOver, zona, "FIN DE PARTIDA", gestor, pausa, false);
        PrepararPanel(pausa.panelPausa, zona, "EN PAUSA", gestor, pausa, true);
        gestor.panelGameOver.SetActive(false);
        pausa.panelPausa.SetActive(false);
        if (!GameObject.Find("FondoPartida"))
        {
            var objeto = new GameObject("FondoPartida", typeof(SpriteRenderer), typeof(FondoPartida));
            var dibujo = objeto.GetComponent<SpriteRenderer>();
            dibujo.sprite = fondo;
            dibujo.sortingOrder = -100;
            dibujo.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
            AssetDatabase.CreateAsset(dibujo.sharedMaterial, "Assets/Sprites/Interfaz/FondoSinLuz.mat");
            objeto.transform.position = new Vector3(0, 0, 5);
            objeto.transform.localScale = new Vector3(1.05f, 1.05f, 1);
        }
        Camera.main.backgroundColor = Fondo;
        EditorSceneManager.SaveScene(escena);
    }

    static void PrepararPanel(GameObject panel, Transform padre, string titulo,
        GameOver gestor, PausaJuego pausa, bool esPausa)
    {
        panel.transform.SetParent(padre, false);
        Estirar((RectTransform)panel.transform);
        var velo = Imagen((RectTransform)panel.transform, new Color(0.025f, 0.04f, 0.10f, 0.96f));
        velo.sprite = null;
        velo.type = UnityEngine.UI.Image.Type.Simple;
        velo.raycastTarget = true;
        var tarjeta = panel.transform.Find("Tarjeta") as RectTransform;
        if (!tarjeta)
        {
            tarjeta = Objeto("Tarjeta", panel.transform);
            Colocar(tarjeta, new Vector2(0.5f, 0.5f), new Vector2(880, 1080), Vector2.zero);
            Imagen(tarjeta, Tarjeta);
        }
        var textos = panel.GetComponentsInChildren<TextMeshProUGUI>(true)
            .Where(t => !t.GetComponentInParent<UnityEngine.UI.Button>()).ToArray();
        var cabecera = textos.First(t => t != gestor.textoPuntosFinales && t != gestor.textoRecordFinal);
        cabecera.transform.SetParent(tarjeta, false);
        EstiloTexto(cabecera, 62, esPausa ? Turquesa : Rosa);
        cabecera.text = titulo;
        Colocar(cabecera.rectTransform, new Vector2(0.5f, 0.84f), new Vector2(800, 100), Vector2.zero);
        var botones = panel.GetComponentsInChildren<UnityEngine.UI.Button>(true);
        foreach (var boton in botones) boton.transform.SetParent(tarjeta, false);
        if (esPausa)
        {
            var continuar = botones.First(b => b.name == "BotonContinuar");
            Colocar((RectTransform)continuar.transform, new Vector2(0.5f, 0.54f), new Vector2(720, 128), Vector2.zero);
            AplicarBoton(continuar, "CONTINUAR", Turquesa, Fondo, pausa.Continuar);
            var reiniciar = botones.First(b => b.name == "BotonReiniciar");
            Colocar((RectTransform)reiniciar.transform, new Vector2(0.5f, 0.37f), new Vector2(720, 116), Vector2.zero);
            AplicarBoton(reiniciar, "REINICIAR", ColorHex("2A3659"), Color.white, pausa.Reiniciar);
            if (!tarjeta.Find("Explicacion")) Texto("Explicacion", tarjeta, "Tómate un respiro. Tú decides cuándo seguir.",
                27, Suave, new Vector2(0.5f, 0.72f), new Vector2(810, 90));
            if (!tarjeta.Find("BotonMenu")) Boton("BotonMenu", tarjeta, "VOLVER AL MENÚ",
                new Vector2(0.5f, 0.20f), new Vector2(720, 116), ColorHex("2A3659"), Color.white, pausa.VolverAlMenu);
        }
        else
        {
            gestor.textoPuntosFinales.transform.SetParent(tarjeta, false);
            gestor.textoRecordFinal.transform.SetParent(tarjeta, false);
            EstiloTexto(gestor.textoPuntosFinales, 26, Color.white);
            EstiloTexto(gestor.textoRecordFinal, 27, Turquesa);
            Colocar(gestor.textoPuntosFinales.rectTransform, new Vector2(0.5f, 0.63f), new Vector2(800, 220), Vector2.zero);
            Colocar(gestor.textoRecordFinal.rectTransform, new Vector2(0.5f, 0.46f), new Vector2(800, 70), Vector2.zero);
            gestor.textoPuntosFinales.text = "PUNTUACIÓN\n<size=100>000</size>";
            gestor.textoRecordFinal.text = "RÉCORD PERSONAL   000";
            var reiniciar = botones.First(b => b.name == "BotonReiniciar");
            Colocar((RectTransform)reiniciar.transform, new Vector2(0.5f, 0.30f), new Vector2(720, 128), Vector2.zero);
            AplicarBoton(reiniciar, "REINTENTAR", Turquesa, Fondo, gestor.Reiniciar);
            if (!tarjeta.Find("BotonMenu")) Boton("BotonMenu", tarjeta, "VOLVER AL MENÚ",
                new Vector2(0.5f, 0.14f), new Vector2(720, 116), ColorHex("2A3659"), Color.white, gestor.VolverAlMenu);
        }
    }

    static void MejorarPrefab(string ruta, Color color, bool jugador)
    {
        var raiz = PrefabUtility.LoadPrefabContents(ruta);
        var dibujo = raiz.GetComponent<SpriteRenderer>();
        dibujo.color = color;
        if (!raiz.transform.Find("Interior"))
        {
            var interior = new GameObject("Interior", typeof(SpriteRenderer));
            interior.transform.SetParent(raiz.transform, false);
            interior.transform.localScale = Vector3.one * 0.66f;
            var detalle = interior.GetComponent<SpriteRenderer>();
            detalle.sprite = dibujo.sprite;
            detalle.sharedMaterial = dibujo.sharedMaterial;
            detalle.sortingOrder = dibujo.sortingOrder + 1;
            detalle.color = jugador ? ColorHex("173B52") : ColorHex("56223F");
        }
        PrefabUtility.SaveAsPrefabAsset(raiz, ruta);
        PrefabUtility.UnloadPrefabContents(raiz);
    }
}
