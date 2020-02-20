using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalMaster : MonoBehaviour
{
    public ComputeShader fractalShader;
    RenderTexture target;
    Camera cam;
    public int resolution;
    [Range (0.001f, 1)]
    public float scale;
    [Range(-1, 1)]
    public float offx;
    [Range(-1, 1)]
    public float offy;
    Vector2 prevMouse;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            prevMouse = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 nowMouse = Input.mousePosition;
            Vector2 deltaMouse = prevMouse - nowMouse;
            prevMouse = nowMouse;
            deltaMouse /= Screen.height / 2;
            deltaMouse *= scale;
            offx += deltaMouse.x;
            offy += deltaMouse.y;
        }
        scale *= -Input.GetAxis("Mouse ScrollWheel")+1;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Init();
        InitRenderTexture();
        SetParameters();
        int threadGroupsX = Mathf.CeilToInt(cam.pixelWidth);//number of x iteration
        int threadGroupsY = Mathf.CeilToInt(cam.pixelHeight);
        fractalShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        Graphics.Blit(target, destination);//update render result
    }

    void SetParameters()
    {
        fractalShader.SetTexture(0, "Destination", target);
        fractalShader.SetFloat("resolution", resolution);
        fractalShader.SetFloat("relim", 16);
        fractalShader.SetFloat("scale", scale);
        fractalShader.SetFloat("offx", offx);
        fractalShader.SetFloat("offy", offy);
    }

    void Init()
    {
        cam = Camera.current;
    }

    void InitRenderTexture()
    {
        if (target == null || target.width != cam.pixelWidth || target.height != cam.pixelHeight)//Check if update if needed.
        {
            if (target != null)
            {
                target.Release();//free up hardware resource
            }
            target = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create();
        }
    }
}