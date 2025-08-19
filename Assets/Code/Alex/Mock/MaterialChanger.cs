using System.Linq;
using UnityEngine;
using UniRx;

public class MaterialChanger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Material joyMaterial, fearMaterial;
    private Renderer _renderer;
    private System.IDisposable _sub;
    
    void Start()
    {
        _renderer = gameObject.GetComponent<MeshRenderer>();
        if (!_renderer) return;

        _renderer.material = joyMaterial;

        _sub = EmotionSingletonMock.Instance.EmotionSubject
            .Subscribe(emotion => { ChangeMaterial(emotion); });

        ChangeMaterial(EmotionSingletonMock.Instance.getCurrentEmotion());
    }

    public void ChangeMaterial(Emotion emotion)
    {
        if (!_renderer) _renderer = GetComponent<Renderer>();
        if (!_renderer) return;

        var mat = (emotion == Emotion.Joy) ? joyMaterial : fearMaterial;

        // Use sharedMaterial in Edit Mode to avoid instancing
        if (!Application.isPlaying)
        {
            if (_renderer.sharedMaterials.Length > 1)
            {
                _renderer.materials = Enumerable.Repeat(mat, _renderer.sharedMaterials.Length).ToArray();
            }
            else
            {
                _renderer.sharedMaterial = mat;
            }
        }
        else
        {
            if (_renderer.materials.Length > 1)
            {
                _renderer.materials = Enumerable.Repeat(mat, _renderer.materials.Length).ToArray();
            }
            else
            {
                _renderer.material = mat;
            }
        }
    }

    private void OnDisable()
    {
        _sub?.Dispose();
        _sub = null;
    }
}