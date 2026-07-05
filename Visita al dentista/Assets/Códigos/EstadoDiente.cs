using System;



using UnityEngine;







public enum TipoHerramienta



{



    CEPILLO,



    CEPILLO_CON_PASTA,



    ALGODON



}







public enum TipoEstado



{



    LIMPIO,



    SUCIO,



    SARRO,



    CARIE



}







public class EstadoDiente : MonoBehaviour



{



    [HideInInspector] public TipoEstado estadoActual;







    private SpriteRenderer srSucio;



    private SpriteRenderer srSarro;



    private SpriteRenderer srCarie;







    private float cooldownTimer;



    private const float COOLDOWN_TRAS_LIMPIEZA = 5f;







    private SpriteRenderer capaEnsuciandose;







    // --- SE SEPARA LA VELOCIDAD PARA CONTROLARLA POR ESTADO ---



    private const float VELOCIDAD_ENSUCIAMIENTO_NORMAL = 0.525f;



    private const float VELOCIDAD_ENSUCIAMIENTO_CARIE = 1.85f;



    private float velocidadEnsuciamientoActual;







    public static event Action OnDienteLimpiado;



    public static event Action OnCualquierCambioDeEstado;







    void Update()



    {



        if (cooldownTimer > 0f)



            cooldownTimer -= Time.deltaTime;







        if (capaEnsuciandose != null)



        {



            Color c = capaEnsuciandose.color;



            c.a += velocidadEnsuciamientoActual * Time.deltaTime;







            if (c.a >= 1f)



            {



                c.a = 1f;



                capaEnsuciandose.color = c;



                capaEnsuciandose = null; // Se libera al completarse



            }



            else



            {



                capaEnsuciandose.color = c;



            }



        }



    }







    public void Inicializar(Sprite spriteSucio, Sprite spriteSarro, Sprite spriteCarie)



    {



        estadoActual = TipoEstado.LIMPIO;



        velocidadEnsuciamientoActual = VELOCIDAD_ENSUCIAMIENTO_NORMAL;







        srSucio = CrearCapa("Sucio", spriteSucio, 1);



        srSarro = CrearCapa("Sarro", spriteSarro, 2);



        srCarie = CrearCapa("Carie", spriteCarie, 3);















        SetCapa(srSucio, false);



        SetCapa(srSarro, false);



        SetCapa(srCarie, false);



    }







    private SpriteRenderer CrearCapa(string nombre, Sprite sprite, int orden)



    {



        if (sprite == null) return null;



        var go = new GameObject(nombre);



        go.transform.SetParent(transform, false);



        var sr = go.AddComponent<SpriteRenderer>();



        sr.sprite = sprite;



        sr.sortingOrder = orden;



        return sr;



    }







    public void EnsuciarDiente()



    {



        if (cooldownTimer > 0f) return;



        if (capaEnsuciandose != null) return;







        SpriteRenderer nuevaCapa = null;



        float dadoProbabilidad = UnityEngine.Random.Range(0f, 100f);







        velocidadEnsuciamientoActual = VELOCIDAD_ENSUCIAMIENTO_NORMAL;







        switch (estadoActual)



        {



            case TipoEstado.LIMPIO:



                if (dadoProbabilidad <= 25f)



                {



                    estadoActual = TipoEstado.SARRO;



                    nuevaCapa = srSarro;



                    if (srSucio != null) { srSucio.color = Color.white; srSucio.enabled = true; }



                }



                else



                {



                    estadoActual = TipoEstado.SUCIO;



                    nuevaCapa = srSucio;



                }



                break;







            case TipoEstado.SUCIO:



                if (dadoProbabilidad <= 60f)



                {



                    estadoActual = TipoEstado.CARIE;



                    nuevaCapa = srCarie;



                    velocidadEnsuciamientoActual = VELOCIDAD_ENSUCIAMIENTO_CARIE;







                    if (srSucio != null) { srSucio.color = Color.white; srSucio.enabled = true; }



                    if (srSarro != null) { srSarro.color = Color.white; srSarro.enabled = true; }



                }



                else



                {



                    estadoActual = TipoEstado.SARRO;



                    nuevaCapa = srSarro;



                }



                break;







            case TipoEstado.SARRO:



                estadoActual = TipoEstado.CARIE;



                nuevaCapa = srCarie;



                velocidadEnsuciamientoActual = VELOCIDAD_ENSUCIAMIENTO_CARIE;



                break;



        }







        if (nuevaCapa != null)



        {



            Color c = nuevaCapa.color;



            c.a = 0f;



            nuevaCapa.color = c;



            nuevaCapa.enabled = true;



            capaEnsuciandose = nuevaCapa;



        }







        OnCualquierCambioDeEstado?.Invoke();



    }







    public void LimpiarGradual(TipoHerramienta herramienta, float cantidad)



    {



        SpriteRenderer capaActiva = null;



        TipoEstado siguienteEstado = estadoActual;







        switch (estadoActual)



        {



            case TipoEstado.CARIE:



                if (herramienta == TipoHerramienta.ALGODON)



                {



                    capaActiva = srCarie;



                    siguienteEstado = TipoEstado.SARRO;



                }



                break;



            case TipoEstado.SARRO:



                if (herramienta == TipoHerramienta.CEPILLO_CON_PASTA)



                {



                    capaActiva = srSarro;



                    siguienteEstado = TipoEstado.SUCIO;



                }



                break;



            case TipoEstado.SUCIO:



                if (herramienta == TipoHerramienta.CEPILLO)



                {



                    capaActiva = srSucio;



                    siguienteEstado = TipoEstado.LIMPIO;



                }



                break;



        }







        // --- SOLUCIÓN DE EMERGENCIA AMPLIADA ---



        // Si por algún motivo externo la capa física falta pero estamos en el estado, forzamos la transición



        if (estadoActual == TipoEstado.SARRO && herramienta == TipoHerramienta.CEPILLO_CON_PASTA && capaActiva == null)



        {



            Debug.LogWarning("[Salvavidas] Forzando limpieza de sarro en " + gameObject.name);



            estadoActual = TipoEstado.SUCIO;



            capaEnsuciandose = null; // Forzamos liberación total



            cooldownTimer = COOLDOWN_TRAS_LIMPIEZA;



            OnCualquierCambioDeEstado?.Invoke();



            return;



        }







        // ¡LA CORRECCIÓN CLAVE!: Si la herramienta es la correcta, cancelamos inmediatamente 



        // el proceso de ensuciamiento para que el Update deje de sumarle Alpha en este frame.



        if (capaActiva != null)



        {



            capaEnsuciandose = null;



        }



        else



        {



            // Si la herramienta es incorrecta para el estado actual, salimos de inmediato



            Debug.Log("[EstadoDiente] " + gameObject.name + " herramienta incorrecta para estado " + estadoActual);



            return;



        }







        // Aseguramos la visibilidad de la capa activa para procesar los cambios



        capaActiva.enabled = true;







        Color c = capaActiva.color;



        c.a -= cantidad;



        capaActiva.color = c;







        Debug.Log("[EstadoDiente] " + gameObject.name + " alpha: " + c.a.ToString("F2") + " estado: " + estadoActual);







        // Transición fluida cuando la capa se limpia por completo



        if (c.a <= 0.05f)



        {



            c.a = 1f;



            capaActiva.color = c;



            capaActiva.enabled = false;







            estadoActual = siguienteEstado;



            cooldownTimer = COOLDOWN_TRAS_LIMPIEZA;







            Debug.Log("[EstadoDiente] " + gameObject.name + " TRANSICION a " + estadoActual + " | cooldown " + COOLDOWN_TRAS_LIMPIEZA + "s");







            if (estadoActual == TipoEstado.LIMPIO)



                OnDienteLimpiado?.Invoke();







            OnCualquierCambioDeEstado?.Invoke();



        }



    }







    private void SetCapa(SpriteRenderer sr, bool activo)



    {



        if (sr != null) sr.enabled = activo;



    }







    public bool EstaSiendoLimpiadoPor(TipoHerramienta herramienta)



    {



        if (estadoActual == TipoEstado.LIMPIO) return false;







        switch (estadoActual)



        {



            case TipoEstado.CARIE:



                return (herramienta == TipoHerramienta.ALGODON);







            case TipoEstado.SARRO:



                return (herramienta == TipoHerramienta.CEPILLO_CON_PASTA);







            case TipoEstado.SUCIO:



                return (herramienta == TipoHerramienta.CEPILLO);



        }







        return false;



    }



} 