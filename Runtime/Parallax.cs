using System;
using UnityEngine;

public class Parallax : MonoBehaviour {
    [SerializeField] private ParallaxElement[] parallaxElements;
    
    private Transform _mainCameraTransform;
    private Vector3 _previousCameraPosition;
    private Vector3 _cameraDeltaMovement;

    private void Awake() {
        _mainCameraTransform = Camera.main!.transform;
        _previousCameraPosition = _mainCameraTransform.position;

        foreach (var parallaxElement in parallaxElements) {
            if (parallaxElement.objectTransform == null) {
                Debug.LogWarning("Parallax element has no game object!", this);
                continue;
            }
            
            if (parallaxElement.spriteRenderer == null) {
                parallaxElement.spriteRenderer = parallaxElement.objectTransform.GetComponent<SpriteRenderer>();
            }
            
            parallaxElement.ParallaxEffectActions += ApplyParallaxMovementXAxis;
            
            if (parallaxElement.infiniteScrollingXAxis) {
                SetInfiniteScrollingEffectXAxis(parallaxElement);
                parallaxElement.ParallaxEffectActions += ApplyInfiniteScrollingMovementXAxis;
            }
            
            parallaxElement.ParallaxEffectActions += ApplyParallaxMovementYAxis;

            if (parallaxElement.infiniteScrollingYAxis) {
                SetInfiniteScrollingEffectYAxis(parallaxElement);
                parallaxElement.ParallaxEffectActions += ApplyInfiniteScrollingMovementYAxis;
            }
        }
    }

    private void LateUpdate() {
        _cameraDeltaMovement = _mainCameraTransform.position - _previousCameraPosition;
        
        foreach (var parallaxElement in parallaxElements) {
            parallaxElement.InvokeParallaxEffectActions();
        }

        _previousCameraPosition = _mainCameraTransform.position;
    }

    private void OnDisable() {
        foreach (var parallaxElement in parallaxElements) {
            parallaxElement.ResetEffectActions();
        }
    }

    #region ---XAxis---
    private void SetInfiniteScrollingEffectXAxis(ParallaxElement parallaxElement) {
        parallaxElement.spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        parallaxElement.spriteRenderer.size = new Vector2(parallaxElement.spriteRenderer.size.x * 3, parallaxElement.spriteRenderer.size.y);
    }

    private void ApplyInfiniteScrollingMovementXAxis(ParallaxElement parallaxElement) {
        var parallaxElementSprite = parallaxElement.spriteRenderer.sprite;
        var width = parallaxElementSprite.texture.width / parallaxElementSprite.pixelsPerUnit;
        
        if (_mainCameraTransform.position.x - parallaxElement.objectTransform.position.x >= width) {
            parallaxElement.objectTransform.position += new Vector3(width, 0, 0);
        }
        
        if (_mainCameraTransform.position.x - parallaxElement.objectTransform.position.x <= -width) {
            parallaxElement.objectTransform.position -= new Vector3(width, 0, 0);
        }
    }


    private void ApplyParallaxMovementXAxis(ParallaxElement parallaxElement) {
        var parallaxMovementX = _cameraDeltaMovement.x + _cameraDeltaMovement.x * parallaxElement.xAxisEffectMultiplier;

        parallaxElement.objectTransform.position += new Vector3(parallaxMovementX, 0, 0);
    }
    #endregion

    #region ---YAxis---
    private void SetInfiniteScrollingEffectYAxis(ParallaxElement parallaxElement) {
        parallaxElement.spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        parallaxElement.spriteRenderer.size = new Vector2(parallaxElement.spriteRenderer.size.x, parallaxElement.spriteRenderer.size.y * 3);
    }
    
    private void ApplyInfiniteScrollingMovementYAxis(ParallaxElement parallaxElement) {
        var parallaxElementSprite = parallaxElement.spriteRenderer.sprite;
        var height = parallaxElementSprite.texture.height / parallaxElementSprite.pixelsPerUnit;
        
        if (_mainCameraTransform.position.y - parallaxElement.objectTransform.position.y >= height) {
            parallaxElement.objectTransform.position += new Vector3(0, height, 0);
        }
        
        if (_mainCameraTransform.position.y - parallaxElement.objectTransform.position.y <= -height) {
            parallaxElement.objectTransform.position -= new Vector3(0, height, 0);
        }
    }
    
    private void ApplyParallaxMovementYAxis(ParallaxElement parallaxElement) {
        var parallaxMovementY = _cameraDeltaMovement.y + _cameraDeltaMovement.y * parallaxElement.yAxisEffectMultiplier;

        parallaxElement.objectTransform.position += new Vector3(0, parallaxMovementY, 0);
    }
    #endregion

    [Serializable] public class ParallaxElement {
        [Header("Parallax Object")]
        public Transform objectTransform;
        [Tooltip("If not set will be gotten in awake")]
        public SpriteRenderer spriteRenderer;
        
        [Header("X Axis Parallax")] [Tooltip("Set to 0 to disable the parallax effect in the X axis")]
        public float xAxisEffectMultiplier;
        public bool infiniteScrollingXAxis;
        
        [Header("Y Axis Parallax")] [Tooltip("Set to 0 to disable the parallax effect in the Y axis")]
        public float yAxisEffectMultiplier;
        public bool infiniteScrollingYAxis;

        public event Action<ParallaxElement> ParallaxEffectActions = delegate { };

        public void InvokeParallaxEffectActions() {
            ParallaxEffectActions.Invoke(this);
        }

        public void ResetEffectActions() {
            ParallaxEffectActions = delegate { };
        }
    }
}