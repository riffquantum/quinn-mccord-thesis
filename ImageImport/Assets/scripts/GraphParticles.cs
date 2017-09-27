using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// GraphParticles.
/// Draw x/y/z points from textures (corresponding to z-layers) on a 3d graph using a particle systems.
/// Changeable properties are an alpha cutoff value (pixels below it will not make a particle).
/// Points can be added, the graph will show up to the last MAX_POINTS.
/// The eight corners of the box of the graph are also indicated as particles.
/// float is used internally, so points added with double values might lose precision.
/// </summary>
public class GraphParticles : MonoBehaviour
{
    public const int MAX_PARTICLES = 200000;
    [Tooltip("Minimal alpha value (up to 255) that will generate a particle.")]
    public int alphaCutoff = 75;

    private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[MAX_PARTICLES];
    private const int MAX_RESERVED_INDEX = 7; // first eight particles reserved for corners
    private int newestIndex = MAX_RESERVED_INDEX;
    private bool needParticlesSet = false;

    private ParticleSystem _particleSystem;
    private Vector3 shapeBox = new Vector3(1, 1, 1);

    void Awake()
    {
        this._particleSystem = GetComponent<ParticleSystem>();
    }

    public void SetSizeAndCenter(Vector3 newSize, Vector3 newCenter)
    {
        ParticleSystem.ShapeModule shap = this._particleSystem.shape;
        shap.scale = newSize;
        this.shapeBox = newSize;
        this.gameObject.transform.localPosition = newCenter;
        this.UpdateCornerParticles();
    }

    private void UpdateCornerParticles()
    {
        this.AddParticle(0, -shapeBox.x / 2f, -shapeBox.y / 2f, -shapeBox.z / 2f, Color.white, 1f);
        this.AddParticle(1, -shapeBox.x / 2f, -shapeBox.y / 2f, +shapeBox.z / 2f, Color.white, 1f);
        this.AddParticle(2, -shapeBox.x / 2f, +shapeBox.y / 2f, -shapeBox.z / 2f, Color.white, 1f);
        this.AddParticle(3, -shapeBox.x / 2f, +shapeBox.y / 2f, +shapeBox.z / 2f, Color.white, 1f);
        this.AddParticle(4, +shapeBox.x / 2f, -shapeBox.y / 2f, -shapeBox.z / 2f, Color.white, 1f);
        this.AddParticle(5, +shapeBox.x / 2f, -shapeBox.y / 2f, +shapeBox.z / 2f, Color.white, 1f);
        this.AddParticle(6, +shapeBox.x / 2f, +shapeBox.y / 2f, -shapeBox.z / 2f, Color.white, 1f);
        this.AddParticle(7, +shapeBox.x / 2f, +shapeBox.y / 2f, +shapeBox.z / 2f, Color.white, 1f);
        needParticlesSet = true;
    }

    public void ClearParticles()
    {
        for (int index = MAX_RESERVED_INDEX; index < MAX_PARTICLES; index++) {
            this.particles[index].startSize = 0f;
        }
        needParticlesSet = true;
    }

    public void AddParticlesFromTexture(float zValue, Texture2D tex, Color partColor)
    {
        // iterate through the texture pixels, add new particle with this color if the alpha is high enough
        // x and z are the texture dimension, y is layers (incoming zValue):
        float y = (-shapeBox.y / 2f) + zValue;
        float xStep = shapeBox.x / tex.width;
        float firstX = (-shapeBox.x / 2f) + (xStep / 2f);
        float x = firstX;
        float maxX = (+shapeBox.x / 2f);
        float zStep = shapeBox.z / tex.width;
        float z = (-shapeBox.z / 2f) + (zStep / 2f);
        float maxZ = (+shapeBox.z / 2f);
        int exclusionCount = 0;
        int emptyCount = 0;
        foreach (Color32 col in tex.GetPixels32()) {
            if (col.a < alphaCutoff) { // ignore lowest transparency values
                if (col.a > 0) {
                    exclusionCount++;
                } else {
                    emptyCount++;
                }
            } else {
                Color newCol = new Color(partColor.r, partColor.g, partColor.b, col.a / 255f);
                this.AddParticle(x, y, z, newCol, 4 * xStep);
            }
            x += xStep;
            if (x > maxX) {
                x = firstX;
                z += zStep;
            }
        }
        Debug.Log(string.Format("Texture to Particles: {0} total, {1} empty, {2} non-empty below alphaCutoff", tex.width * tex.height, emptyCount, exclusionCount));
        Debug.Assert(z > maxZ && Mathf.Approximately(x, firstX));
    }

    void Update()
    {
        if (needParticlesSet) {
            _particleSystem.SetParticles(particles, particles.Length);
            needParticlesSet = false;
        }
    }

    private void AddParticle(float x, float y, float z, Color col, float size = 1f)
    {
        newestIndex++;
        if (newestIndex >= MAX_PARTICLES) {
            Debug.Log("GraphParticles: reached max particle index!");
            newestIndex = MAX_RESERVED_INDEX + 1; // first 8 reserved for corners
        }
        this.AddParticle(newestIndex, x, y, z, col, size);
    }

    private void AddParticle(int index, float x, float y, float z, Color col, float size)
    {
        if (index >= MAX_PARTICLES) {
            return;
        }
        particles[index].position = new Vector3(x, y, z);
        particles[index].startColor = col;
        particles[index].startSize = size;
        needParticlesSet = true;
    }

}
