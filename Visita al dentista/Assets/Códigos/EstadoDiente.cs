using UnityEngine;
using UnityEngine.UI;


public enum TipoEstado
{
    Limpio,
    Sucio,
    Sarro,
    Carie
}


[RequireComponent(typeof(SpriteRenderer))]
public class EstadoDiente : MonoBehaviour
{
    [Header("Configuración de Estado")]
    public TipoEstado estadoActual;
    public Image imagenDiente;

    [Header("Sprites por Estado")]
    public Sprite spriteLimpio;
    public Sprite spriteSucio;
    public Sprite spriteSarro;
    public Sprite spriteCarie;


    private void Awake()
    {
        if (imagenDiente == null) imagenDiente = GetComponent<Image>();
    }

    private void Start()
    {
        imagenDiente.sprite = spriteLimpio;
    }


    public void EnsuciarDiente(TipoEstado newTipo)
    {

        switch (estadoActual)
        {
            case TipoEstado.Limpio:estadoActual = TipoEstado.Sucio; imagenDiente.sprite = spriteSucio; break;
            case TipoEstado.Sucio: estadoActual = TipoEstado.Sarro; imagenDiente.sprite = spriteSarro; break;
            case TipoEstado.Sarro: estadoActual = TipoEstado.Carie; imagenDiente.sprite = spriteCarie; break;
        }
    }

    public void LimpiarDiente()
    {
        estadoActual =  TipoEstado.Limpio; 
        imagenDiente.sprite = spriteLimpio;
    }
}