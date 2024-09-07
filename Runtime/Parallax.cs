using System;
using UnityEngine;

public class Parallax : MonoBehaviour {
    [SerializeField] private ParallaxElement[] parallaxElements;
    
    private Transform _mainCameraTransform;
    private Vector3 _previousCameraPosition;
    private Vector3 _deltaMovement;

    private void Awake() {
        _mainCameraTransform = Camera.main!.transform;
        _previousCameraPosition = _mainCameraTransform.position;

        foreach (var parallaxElement in parallaxElements) {
            parallaxElement.ParallaxEffectActions += ApplyParallaxMovement;
            if (parallaxElement.infiniteScrollingXAxis) {
                SetInfiniteScrollingEffectXAxis(parallaxElement);
                parallaxElement.ParallaxEffectActions += ApplyInfiniteScrollingMovementXAxis;
            }

            if (parallaxElement.infiniteScrollingYAxis) {
                SetInfiniteScrollingEffectYAxis(parallaxElement);
                parallaxElement.ParallaxEffectActions += ApplyInfiniteScrollingMovementYAxis;
            }
        }
    }

    private void LateUpdate() {
        _deltaMovement = _mainCameraTransform.position - _previousCameraPosition;
        
        foreach (var parallaxElement in parallaxElements) {
            parallaxElement.ParallaxEffectActions.Invoke(parallaxElement);
        }

        _previousCameraPosition = _mainCameraTransform.position;
    }

    private void SetInfiniteScrollingEffectXAxis(ParallaxElement parallaxElement) {
        parallaxElement.gameObject.transform.localScale = 
            new Vector3(parallaxElement.gameObject.transform.localScale.x * 3, 
                parallaxElement.gameObject.transform.localScale.y, 
                parallaxElement.gameObject.transform.localScale.z);
        
        parallaxElement.spriteRenderer.drawMode = SpriteDrawMode.Tiled;
    }
    
    private void SetInfiniteScrollingEffectYAxis(ParallaxElement parallaxElement) {
        parallaxElement.gameObject.transform.localScale = 
            new Vector3(parallaxElement.gameObject.transform.localScale.x, 
                parallaxElement.gameObject.transform.localScale.y * 3, 
                parallaxElement.gameObject.transform.localScale.z);
        
        parallaxElement.spriteRenderer.drawMode = SpriteDrawMode.Tiled;
    }

    private void ApplyInfiniteScrollingMovementXAxis(ParallaxElement parallaxElement) {
        var parallaxElementSprite = parallaxElement.spriteRenderer.sprite;
        var width = parallaxElementSprite.texture.width / parallaxElementSprite.pixelsPerUnit;
        
        if (_mainCameraTransform.position.x - parallaxElement.gameObject.transform.position.x >= width) {
            parallaxElement.gameObject.transform.position += new Vector3(width, 0, 0);
        }
        
        if (_mainCameraTransform.position.x - parallaxElement.gameObject.transform.position.x <= -width) {
            parallaxElement.gameObject.transform.position -= new Vector3(width, 0, 0);
        }
    }
    
    private void ApplyInfiniteScrollingMovementYAxis(ParallaxElement parallaxElement) {
        var parallaxElementSprite = parallaxElement.spriteRenderer.sprite;
        var height = parallaxElementSprite.texture.height / parallaxElementSprite.pixelsPerUnit;
        
        if (_mainCameraTransform.position.y - parallaxElement.gameObject.transform.position.y >= height) {
            parallaxElement.gameObject.transform.position += new Vector3(0, height, 0);
        }
        
        if (_mainCameraTransform.position.y - parallaxElement.gameObject.transform.position.y <= -height) {
            parallaxElement.gameObject.transform.position -= new Vector3(0, height, 0);
        }
    }


    private void ApplyParallaxMovement(ParallaxElement parallaxElement) {
        var parallaxMovementX = _deltaMovement.x + _deltaMovement.x * 
            (parallaxElement.xAxisEnabled ? parallaxElement.effectMultiplier: 0);
        var parallaxMovementY = _deltaMovement.y + _deltaMovement.y * 
            (parallaxElement.yAxisEnabled ? parallaxElement.effectMultiplier : 0);

        parallaxElement.gameObject.transform.position += new Vector3(parallaxMovementX, parallaxMovementY, 0);
    }

    [Serializable] public class ParallaxElement {
        public GameObject gameObject;
        public SpriteRenderer spriteRenderer;
        public float effectMultiplier;
        public bool xAxisEnabled;
        public bool yAxisEnabled;
        public bool infiniteScrollingXAxis;
        public bool infiniteScrollingYAxis;
        
        public Action<ParallaxElement> ParallaxEffectActions; 
    }
}