using UnityEngine;
namespace HMSXR.Foundation
{
    public class XvMedioRecordTips : MonoBehaviour
    {

        public TextMesh textMesh;

        private Transform mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main.transform;
        }

        private void OnEnable()
        {
            HideTips();
        }
        public void ShowTips(string content)
        {
            if (textMesh != null)
            {
                textMesh.text = content;
            }
        }

        public void HideTips()
        {
            textMesh.text = "";
        }

        public void ShowTips(string content, float time)
        {
            if (textMesh != null)
            {
                textMesh.text = content;
            }

            Invoke("HideTips", time);
        }

        private void Update()
        {
            transform.position = mainCamera.position + mainCamera.forward * 1.2f;
            transform.rotation = mainCamera.rotation;

        }
    }
}
