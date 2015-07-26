using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[RequireComponent(typeof(Camera))]
public class ClearForwardGBuffer : MonoBehaviour
{
    public Shader m_shader;
    public Mesh m_quad;
    Material m_material;
    CommandBuffer m_commands;

    public static Mesh GenerateQuad()
    {
        Vector3[] vertices = new Vector3[4] {
                new Vector3( 1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f,-1.0f, 0.0f),
                new Vector3( 1.0f,-1.0f, 0.0f),
            };
        int[] indices = new int[6] { 0, 1, 2, 2, 3, 0 };

        Mesh r = new Mesh();
        r.name = "Quad";
        r.vertices = vertices;
        r.triangles = indices;
        return r;
    }

#if UNITY_EDITOR
    void Reset()
    {
        m_shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/ClearForwardGBuffer.shader");
        m_quad = GenerateQuad();
    }
#endif // UNITY_EDITOR

    void OnPreRender()
    {
        if (m_material == null)
        {
            m_material = new Material(m_shader);
        }
        if (m_commands == null)
        {
            m_commands = new CommandBuffer();
            m_commands.name = "ClearForwardGBuffer";
            m_commands.DrawMesh(m_quad, Matrix4x4.identity, m_material);
            GetComponent<Camera>().AddCommandBuffer(CameraEvent.AfterLighting, m_commands);
        }
    }
}
