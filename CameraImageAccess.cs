using UnityEngine;
using System.Collections;
using Vuforia;
using System.IO; 
using Image = Vuforia.Image;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Drawing; 

public class CameraImageAccess :  VuforiaMonoBehaviour
{

    #region MEMBERS
  //  public GameObject cam;
    private PIXEL_FORMAT mPixelFormat = PIXEL_FORMAT.UNKNOWN_FORMAT;
    //   private Image  im = new Image (PIXEL_FORMAT.UNKNOWN_FORMAT ) ;
     private RawImage rawImage;
     private bool mAccessCameraImage = true;
     private bool mFormatRegistered = false;
     private Texture2D tex;
    

    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS

    public void Start()
    {
 
        #if UNITY_EDITOR
        mPixelFormat =PIXEL_FORMAT.GRAYSCALE; // Need Grayscale for Editor
#else

        mPixelFormat = PIXEL_FORMAT.RGB888; // Use RGB888 for mobile
#endif

        // Register Vuforia life-cycle callbacks:
         VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
      
       // VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
        //Fetch the RawImage component from the GameObject
        rawImage = GetComponent<RawImage>();

    }

    #endregion // MONOBEHAVIOUR_METHODS

    //#region PRIVATE_METHODS

     void OnVuforiaStarted()
    {
       

        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());
           

         mFormatRegistered = true;
        }
        else
        {
            Debug.LogError(
                "\nFailed to register pixel format: " + mPixelFormat.ToString() +
                "\nThe format may be unsupported by your device." +
                "\nConsider using a different pixel format.\n");
 
            mFormatRegistered = false;
        }
 
    }

    /// <summary>
    /// Called each time the Vuforia state is updated
    /// </summary>
    public void OnClick()
    {
        if (mFormatRegistered)
        {
            if (mAccessCameraImage)
            {
                Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);

                if (image != null)
                {
                    
                    Debug.Log(
                        "\nImage Format: " + image.PixelFormat +
                        "\nImage Size:   " + image.Width + "x" + image.Height +
                        "\nBuffer Size:  " + image.BufferWidth + "x" + image.BufferHeight +
                        "\nImage Stride: " + image.Stride + "\n"
                    );

                    byte[] pixels = image.Pixels;

                    if (pixels != null && pixels.Length > 0)
                    {
                        Debug.Log(
                            "\nImage pixels: " +
                            pixels[0] + ", " +
                            pixels[1] + ", " +
                            pixels[100] + ", ...\n");
                     
                        tex = new Texture2D(2160, 1080, TextureFormat.RGBA32, false); // RGB24

                        tex.LoadRawTextureData(( pixels));
                       
                         rawImage.texture = tex;
                         rawImage.material.mainTexture = tex;
                         tex.Apply();
                        
                       



                    }
                }
            }
        }
    }

    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    public void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }

    /// <summary>
    /// Register the camera pixel format
    /// </summary>
    public void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }

    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    public void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }
 
  //  #endregion //PRIVATE_METHODS
}