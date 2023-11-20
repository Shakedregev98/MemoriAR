using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;

public class NativeFilePickerScript : MonoBehaviour
{
    public string finalPath;
    [SerializeField]
    public GameObject planeToAttachImage;
    public float positionY = 1.3f;
    private int maxSize = -1;

    private void Update()
    {
        // Don't attempt to pick media from Gallery/Photos if
        // another media pick operation is already in progress
        if (NativeGallery.IsMediaPickerBusy())
            return;
    }

    public void PickImageOrVideo()
    {
        if (NativeGallery.CanSelectMultipleMediaTypesFromGallery())
        {
            NativeGallery.Permission permission = NativeGallery.GetMixedMediaFromGallery((path) =>
            {
                Debug.Log("Media path: " + path);
                if (path != null)
                {

                    // Determine if the user has picked an image, video, or neither
                    switch (NativeGallery.GetMediaTypeOfFile(path))
                    {
                        case NativeGallery.MediaType.Image:
                            Debug.Log("Picked image");
                            Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                            if (texture == null)
                            {
                                Debug.Log("Couldn't load texture from " + path);
                                return;
                            }

                            // Assign texture to a temporary quad and destroy it after 5 seconds
                            GameObject quad = Instantiate(planeToAttachImage);
                            quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 4f;
                            quad.transform.forward = Camera.main.transform.forward;
                            quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);
                            quad.transform.position = new Vector3(quad.transform.position.x, positionY, quad.transform.position.z);
                            Material material = quad.GetComponent<Renderer>().material;
                            if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                                material.shader = Shader.Find("Legacy Shaders/Diffuse");

                            material.mainTexture = texture;
                            break;
                        case NativeGallery.MediaType.Video:
                            Debug.Log("Picked video");
                            // Play the selected video
                            Handheld.PlayFullScreenMovie("file://" + path);
                            break;
                        default:
                            Debug.Log("Probably picked something else");
                            break;
                    }
                }
            }, NativeGallery.MediaType.Image | NativeGallery.MediaType.Video, "Select an image or video");

            Debug.Log("Permission result: " + permission);
        }
    }


}