using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CamerasTest : MonoBehaviour
{
    public bool useTransform = false;
    static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0F * far * near) / (far - near);
        float e = -1.0F;
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }

    void Start()
    {
        for (int i = 0; i < 64; i++)
        {
            StreamReader inp_stm = new StreamReader("C:/Users/Loudon/Desktop/Projects/logs/Cave" + i.ToString("D2") + ".txt");

            string inp_ln = "";
            while (!inp_stm.EndOfStream)
            {
                inp_ln = inp_stm.ReadLine();

                if (!inp_ln.StartsWith("Frustum data:"))
                {
                    continue;
                }

                inp_ln = inp_stm.ReadLine();
                break;
            }
            inp_stm.Close();

            string[] frustumData = inp_ln.Split();

            GameObject newChild = new GameObject("Cave" + i.ToString("D2"));
            newChild.transform.parent = transform;

            Camera cam = newChild.AddComponent(typeof(Camera)) as Camera;
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 100.0f;

            float l = cam.nearClipPlane * Mathf.Tan(Mathf.Deg2Rad * float.Parse(frustumData[6]));
            float r = cam.nearClipPlane * Mathf.Tan(Mathf.Deg2Rad * float.Parse(frustumData[7]));
            float b = cam.nearClipPlane * Mathf.Tan(Mathf.Deg2Rad * float.Parse(frustumData[8]));
            float t = cam.nearClipPlane * Mathf.Tan(Mathf.Deg2Rad * float.Parse(frustumData[9]));

            //cam.projectionMatrix = PerspectiveOffCenter(l, r, b, t, cam.nearClipPlane, cam.farClipPlane);
            cam.projectionMatrix = PerspectiveOffCenter(b, t, l, r, cam.nearClipPlane, cam.farClipPlane) * Matrix4x4.Scale(new Vector3(1, -1, 1));

            cam.aspect = (r - l) / (t - b);
            float fov = (Mathf.Atan(t / cam.nearClipPlane) - Mathf.Atan(b / cam.nearClipPlane)) * Mathf.Rad2Deg;
            cam.fieldOfView = fov;

            Matrix4x4 rotZ = new Matrix4x4();
            float s = Mathf.Sin(Mathf.Deg2Rad * (float.Parse(frustumData[3]) - 90));
            float c = Mathf.Cos(Mathf.Deg2Rad * (float.Parse(frustumData[3]) - 90));
            rotZ[0, 0] = c; rotZ[0, 1] = -s; rotZ[0, 2] = 0; rotZ[0, 3] = 0;
            rotZ[1, 0] = s; rotZ[1, 1] = c; rotZ[1, 2] = 0; rotZ[1, 3] = 0;
            rotZ[2, 0] = 0; rotZ[2, 1] = 0; rotZ[2, 2] = 1; rotZ[2, 3] = 0;
            rotZ[3, 0] = 0; rotZ[3, 1] = 0; rotZ[3, 2] = 0; rotZ[3, 3] = 1;

            Matrix4x4 rotY = new Matrix4x4();
            s = Mathf.Sin(Mathf.Deg2Rad * (float.Parse(frustumData[4])));
            c = Mathf.Cos(Mathf.Deg2Rad * (float.Parse(frustumData[4])));

            rotY[0, 0] = c; rotY[0, 1] = 0; rotY[0, 2] = s; rotY[0, 3] = 0;
            rotY[1, 0] = 0; rotY[1, 1] = 1; rotY[1, 2] = 0; rotY[1, 3] = 0;
            rotY[2, 0] = -s; rotY[2, 1] = 0; rotY[2, 2] = c; rotY[2, 3] = 0;
            rotY[3, 0] = 0; rotY[3, 1] = 0; rotY[3, 2] = 0; rotY[3, 3] = 1;

            Matrix4x4 rotX = new Matrix4x4();
            s = Mathf.Sin(Mathf.Deg2Rad * (float.Parse(frustumData[5]) - 90));
            c = Mathf.Cos(Mathf.Deg2Rad * (float.Parse(frustumData[5]) - 90));
            rotX[0, 0] = 1; rotX[0, 1] = 0; rotX[0, 2] = 0; rotX[0, 3] = 0;
            rotX[1, 0] = 0; rotX[1, 1] = c; rotX[1, 2] = -s; rotX[1, 3] = 0;
            rotX[2, 0] = 0; rotX[2, 1] = s; rotX[2, 2] = c; rotX[2, 3] = 0;
            rotX[3, 0] = 0; rotX[3, 1] = 0; rotX[3, 2] = 0; rotX[3, 3] = 1;

            Matrix4x4 rot = rotZ * rotY * rotX;

            if (useTransform)
            {
                //Debug.Log(rot.ToString());
                rot.SetRow(2, -rot.GetRow(2));
                rot = Matrix4x4.Inverse(rot);

                //Quaternion q = Quaternion.Euler(float.Parse(frustumData[2]) - 90, 0, 0) * Quaternion.Euler(0, -float.Parse(frustumData[1]), 0) * Quaternion.Euler(0, 0, float.Parse(frustumData[0]));
                //Quaternion q = , -float.Parse(frustumData[1]), float.Parse(frustumData[0]));
                Quaternion q = Quaternion.LookRotation(rot.GetColumn(2), rot.GetColumn(1));

                cam.transform.SetPositionAndRotation(cam.transform.position, q);
            }
            else
            {
                cam.worldToCameraMatrix = rot;
            }

            
        }
    }

        
}
