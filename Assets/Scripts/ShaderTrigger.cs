    using System.Collections;
    using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
    using UnityEngine.VFX;

    public class ShaderTrigger : MonoBehaviour
    {
        public SkinnedMeshRenderer skinnedMesh;
        public VisualEffect VFXGraph;
        public VisualEffect burnVFX;
        public Material[] skinnedMaterial;
        public Material ghostMaterial;
        public float dissolveRate = 0.0125f;
        public float refreshRate = 0.025f;
        private float duration = 0.5f;

        private Color currentColor;

        public Transform sunObj;
        private bool isDissolving = false;
        private bool isBurning = false;
        private bool isDying = false;

        // Start is called before the first frame update
        void Start()
        {
            currentColor = new Color(254f / 255f, 0, 0, 0); // Initial color
            burnVFX.SetVector4("Color", currentColor * 3f);
            if (skinnedMesh != null)
            {
                skinnedMaterial = skinnedMesh.materials;
                skinnedMaterial[1].SetVector("_InteriorPos", new Vector2(0, -5));
                skinnedMaterial[1].SetFloat("_FresnelPower", 5f);
        }
        }

        // Update is called once per frame
        void Update()
        {
            float distance = Vector3.Distance(this.gameObject.transform.position, sunObj.transform.position);
            if(distance <= 7f && !isDissolving)
            {
                isDissolving = true;
                VFXGraph.Play();
                StartCoroutine(DissolveCo());
            }

            if(distance <= 40f && !isBurning && !isDying)
            {
                burnVFX.Play();
                isBurning = true;
        }
            else if(distance > 40f && isBurning)
            {
                burnVFX.Stop();
                isBurning = false;
            }

            if (skinnedMaterial[0].GetFloat("_DissolveAmount") > 0.5)
            {
                skinnedMaterial[1].SetVector("_InteriorPos", new Vector2(0, -0.83f));
                skinnedMaterial[1].SetFloat("_FresnelPower", 1.75f);
                VFXGraph.Stop();
                burnVFX.Stop();
                isBurning = false;
            }

            if(distance >= 40f){
                currentColor = burnVFX.GetVector4("Color");
                ChangeColor(new Color(0.99f * 3, 0, 0, 0));
            }
            else if(distance < 35f && distance >= 30f)
            {
                currentColor = burnVFX.GetVector4("Color");
                ChangeColor(new Color(3, 3, 0, 0));
            }
            else if (distance < 30f && distance >= 20f)
            {
                currentColor = burnVFX.GetVector4("Color");
                ChangeColor(new Color(3, 3, 3, 0));
            }else if(distance < 20f && distance >= 10f)
            {
                currentColor = burnVFX.GetVector4("Color");
                ChangeColor(new Color(0, 0.62f, 1.48f, 0));
            }
        }

        IEnumerator DissolveCo()
        {
            isDying = true;
            float counter = 0;
            while (skinnedMaterial[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
            skinnedMaterial[0].SetFloat("_DissolveAmount", counter);
                yield return new WaitForSeconds(refreshRate);
            }
            isDissolving = false;
        }

    void ChangeColor(Color targetColor)
        {
            Color newColor = Color.Lerp(currentColor, targetColor, duration * Time.deltaTime);
            burnVFX.SetVector4("Color", newColor);
        }
    }
