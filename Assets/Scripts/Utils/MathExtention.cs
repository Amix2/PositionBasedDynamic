#region Header
// **    Copyright (C) 2023 Nicolas Reinhard, @LTMX. All rights reserved.
// **    Github Profile: https://github.com/LTMX
// **    Repository : https://github.com/LTMX/Unity.mathx
#endregion

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Mathematics
{
    public static partial class MathExtention
    {

        /// <inheritdoc cref="math.normalize(float2)"/>
        public static float4 norm(this float4 f) => math.normalize(f);
        /// <inheritdoc cref="math.normalize(float3)"/>
        public static float3 norm(this float3 f) => math.normalize(f);
        /// <inheritdoc cref="math.normalize(float2)"/>
        public static float2 norm(this float2 f) => math.normalize(f);

        /// <inheritdoc cref="math.normalizesafe(float2,float2)"/>
        public static float4 normsafe(this float4 f, float4 d = default) => math.normalizesafe(f, d);
        /// <inheritdoc cref="math.normalizesafe(float3,float3)"/>
        public static float3 normsafe(this float3 f, float3 d = default) => math.normalizesafe(f, d);
        /// <inheritdoc cref="math.normalizesafe(float2,float2)"/>
        public static float2 normsafe(this float2 f, float2 d = default) => math.normalizesafe(f, d);

        /// <inheritdoc cref="math.distance(float2,float2)"/>
        public static float distance(this float2 f, float2 f2) => math.distance(f, f2);
        /// <inheritdoc cref="math.distance(float3,float3)"/>
        public static float distance(this float3 f, float3 f2) => math.distance(f, f2);
        /// <inheritdoc cref="math.distance(float4,float4)"/>
        public static float distance(this float4 f, float4 f2) => math.distance(f, f2);

        /// <inheritdoc cref="math.distancesq(float2,float2)"/>
        public static float distancesq(this float2 f, float2 f2) => math.distancesq(f, f2);
        /// <inheritdoc cref="math.distancesq(float3,float3)"/>
        public static float distancesq(this float3 f, float3 f2) => math.distancesq(f, f2);
        /// <inheritdoc cref="math.distancesq(float4,float4)"/>
        public static float distancesq(this float4 f, float4 f2) => math.distancesq(f, f2);

        /// <inheritdoc cref="math.length(float4)"/>
        public static float length(this float4 f) => math.length(f);
        /// <inheritdoc cref="math.length(float3)"/>
        public static float length(this float3 f) => math.length(f);
        /// <inheritdoc cref="math.length(float2)"/>
        public static float length(this float2 f) => math.length(f);


        /// <inheritdoc cref="math.lengthsq(float4)"/>
        public static float lengthsq(this float4 f) => math.lengthsq(f);
        /// <inheritdoc cref="math.lengthsq(float3)"/>
        public static float lengthsq(this float3 f) => math.lengthsq(f);
        /// <inheritdoc cref="math.lengthsq(float2)"/>
        public static float lengthsq(this float2 f) => math.lengthsq(f);


        /// <inheritdoc cref="dot(float4,float4)"/>
        public static float dot(this float4 f, float4 f2) => math.dot(f, f2);
        /// <inheritdoc cref="dot(float4,float4)"/>
        public static float dot(this float3 f, float3 f2) => math.dot(f, f2);
        /// <inheritdoc cref="dot(float4,float4)"/>
        public static float dot(this float2 f, float2 f2) => math.dot(f, f2);


        /// <inheritdoc cref="math.reflect(float4,float4)"/>
        public static float4 reflect(this float4 f, float4 n) => math.reflect(f, n);
        /// <inheritdoc cref="math.reflect(float3,float3)"/>
        public static float3 reflect(this float3 f, float3 n) => math.reflect(f, n);
        /// <inheritdoc cref="math.reflect(float2,float2)"/>
        public static float2 reflect(this float2 f, float2 n) => math.reflect(f, n);


        /// <inheritdoc cref="math.refract(float2,float2,float)"/>
        public static float2 refract(this float2 f, float2 f2, float eta) => math.refract(f, f2, eta);
        /// <inheritdoc cref="math.refract(float3,float3,float)"/>
        public static float3 refract(this float3 f, float3 f2, float eta) => math.refract(f, f2, eta);
        /// <inheritdoc cref="math.refract(float4,float4,float)"/>
        public static float4 refract(this float4 f, float4 f2, float eta) => math.refract(f, f2, eta);

        /// <inheritdoc cref="math.project(float4,float4)"/>
        public static float4 project(this float4 f, float4 n) => math.project(f, n);
        /// <inheritdoc cref="math.project(float3,float3)"/>
        public static float3 project(this float3 f, float3 n) => math.project(f, n);
        /// <inheritdoc cref="math.project(float2,float2)"/>
        public static float2 project(this float2 f, float2 n) => math.project(f, n);

        /// <inheritdoc cref="math.projectsafe(float4,float4,float4)"/>
        public static float4 projectsafe(this float4 f, float4 n, float4 defaultValue = default) => math.projectsafe(f, n, defaultValue);
        /// <inheritdoc cref="math.projectsafe(float3,float3,float3)"/>
        public static float3 projectsafe(this float3 f, float3 n, float3 defaultValue = default) => math.projectsafe(f, n, defaultValue);
        /// <inheritdoc cref="math.projectsafe(float2,float2,float2)"/>
        public static float2 projectsafe(this float2 f, float2 n, float2 defaultValue = default) => math.projectsafe(f, n, defaultValue);



        #region Single Purpose Functions

        ///<inheritdoc cref="math.cross(float3,float3)"/>
        public static float3 cross(this float3 f, float3 f2) => math.cross(f, f2);

        /// returns a vector perpendicular to the input vector
        public static float2 perp(this float2 f) => new(-f.y, f.x);

        /// Returns the exterior product of two vectors. its magnitude is the area of the parallelogram they span.
        /// note: this is not the cross product
        public static float3 exterior(this float3 a, float3 b) => a.yzx * b.zxy - a.zxy * b.yzx;

        /// orthonormalize
        public static float3 orthonorm(ref float3 normal, float3 tangent) => tangent - tangent.projectsafe(normal.norm());

        #endregion

        #region Matrix Operations

        ///Returns the distance the two vectors in the matrix
        public static float cdistance(this float2x2 f) => math.distance(f.c0, f.c1);
        public static float cdistance(this float3x2 f) => math.distance(f.c0, f.c1);
        public static float cdistance(this float4x2 f) => math.distance(f.c0, f.c1);

        ///Returns the squared distance the two vectors in the matrix
        public static float cdistancesq(this float2x2 f) => math.distancesq(f.c0, f.c1);
        public static float cdistancesq(this float3x2 f) => math.distancesq(f.c0, f.c1);
        public static float cdistancesq(this float4x2 f) => math.distancesq(f.c0, f.c1);


        ///Returns the cross product of the two vectors in the matrix
        public static float3 ccross(this float3x2 f) => math.cross(f.c0, f.c1);

        ///Returns the dot product of the two vectors in the matrix
        public static float cdot(this float4x2 f) => math.dot(f.c0, f.c1);
        /// <inheritdoc cref="cdot(float4x2)"/>
        public static float cdot(this float3x2 f) => math.dot(f.c0, f.c1);
        ///Returns the dot product of the two vectors in the matrix
        public static float cdot(this float2x2 f) => math.dot(f.c0, f.c1);

        #endregion

        /// <inheritdoc cref="math.normalize(double2)"/>
        public static double4 norm(this double4 f) => math.normalize(f);
        /// <inheritdoc cref="math.normalize(double3)"/>
        public static double3 norm(this double3 f) => math.normalize(f);
        /// <inheritdoc cref="math.normalize(double2)"/>
        public static double2 norm(this double2 f) => math.normalize(f);

        /// <inheritdoc cref="math.normalizesafe(double2,double2)"/>
        public static double4 normsafe(this double4 f, double4 d = default) => math.normalizesafe(f, d);
        /// <inheritdoc cref="math.normalizesafe(double3,double3)"/>
        public static double3 normsafe(this double3 f, double3 d = default) => math.normalizesafe(f, d);
        /// <inheritdoc cref="math.normalizesafe(double2,double2)"/>
        public static double2 normsafe(this double2 f, double2 d = default) => math.normalizesafe(f, d);

        /// <inheritdoc cref="math.distance(double2,double2)"/>
        public static double distance(this double2 f, double2 f2) => math.distance(f, f2);
        /// <inheritdoc cref="math.distance(double3,double3)"/>
        public static double distance(this double3 f, double3 f2) => math.distance(f, f2);
        /// <inheritdoc cref="math.distance(double4,double4)"/>
        public static double distance(this double4 f, double4 f2) => math.distance(f, f2);

        /// <inheritdoc cref="math.distancesq(double2,double2)"/>
        public static double distancesq(this double2 f, double2 f2) => math.distancesq(f, f2);
        /// <inheritdoc cref="math.distancesq(double3,double3)"/>
        public static double distancesq(this double3 f, double3 f2) => math.distancesq(f, f2);
        /// <inheritdoc cref="math.distancesq(double4,double4)"/>
        public static double distancesq(this double4 f, double4 f2) => math.distancesq(f, f2);

        /// <inheritdoc cref="math.length(double4)"/>
        public static double length(this double4 f) => math.length(f);
        /// <inheritdoc cref="math.length(double3)"/>
        public static double length(this double3 f) => math.length(f);
        /// <inheritdoc cref="math.length(double2)"/>
        public static double length(this double2 f) => math.length(f);


        /// <inheritdoc cref="math.lengthsq(double4)"/>
        public static double lengthsq(this double4 f) => math.lengthsq(f);
        /// <inheritdoc cref="math.lengthsq(double3)"/>
        public static double lengthsq(this double3 f) => math.lengthsq(f);
        /// <inheritdoc cref="math.lengthsq(double2)"/>
        public static double lengthsq(this double2 f) => math.lengthsq(f);


        /// <inheritdoc cref="dot(double4,double4)"/>
        public static double dot(this double4 f, double4 f2) => math.dot(f, f2);
        /// <inheritdoc cref="dot(double4,double4)"/>
        public static double dot(this double3 f, double3 f2) => math.dot(f, f2);
        /// <inheritdoc cref="dot(double4,double4)"/>
        public static double dot(this double2 f, double2 f2) => math.dot(f, f2);


        /// <inheritdoc cref="math.reflect(double4,double4)"/>
        public static double4 reflect(this double4 f, double4 n) => math.reflect(f, n);
        /// <inheritdoc cref="math.reflect(double3,double3)"/>
        public static double3 reflect(this double3 f, double3 n) => math.reflect(f, n);
        /// <inheritdoc cref="math.reflect(double2,double2)"/>
        public static double2 reflect(this double2 f, double2 n) => math.reflect(f, n);


        /// <inheritdoc cref="math.refract(double2,double2,double)"/>
        public static double2 refract(this double2 f, double2 f2, double eta) => math.refract(f, f2, eta);
        /// <inheritdoc cref="math.refract(double3,double3,double)"/>
        public static double3 refract(this double3 f, double3 f2, double eta) => math.refract(f, f2, eta);
        /// <inheritdoc cref="math.refract(double4,double4,double)"/>
        public static double4 refract(this double4 f, double4 f2, double eta) => math.refract(f, f2, eta);

        /// <inheritdoc cref="math.project(double4,double4)"/>
        public static double4 project(this double4 f, double4 n) => math.project(f, n);
        /// <inheritdoc cref="math.project(double3,double3)"/>
        public static double3 project(this double3 f, double3 n) => math.project(f, n);
        /// <inheritdoc cref="math.project(double2,double2)"/>
        public static double2 project(this double2 f, double2 n) => math.project(f, n);

        /// <inheritdoc cref="math.projectsafe(double4,double4,double4)"/>
        public static double4 projectsafe(this double4 f, double4 n, double4 defaultValue = default) => math.projectsafe(f, n, defaultValue);
        /// <inheritdoc cref="math.projectsafe(double3,double3,double3)"/>
        public static double3 projectsafe(this double3 f, double3 n, double3 defaultValue = default) => math.projectsafe(f, n, defaultValue);
        /// <inheritdoc cref="math.projectsafe(double2,double2,double2)"/>
        public static double2 projectsafe(this double2 f, double2 n, double2 defaultValue = default) => math.projectsafe(f, n, defaultValue);



        #region Single Purpose Functions

        ///<inheritdoc cref="math.cross(double3,double3)"/>
        public static double3 cross(this double3 f, double3 f2) => math.cross(f, f2);

        /// returns a vector perpendicular to the input vector
        public static double2 perp(this double2 f) => new(-f.y, f.x);

        /// Returns the exterior product of two vectors. its magnitude is the area of the parallelogram they span.
        /// note: this is not the cross product
        public static double3 exterior(this double3 a, double3 b) => a.yzx * b.zxy - a.zxy * b.yzx;

        /// orthonormalize
        public static double3 orthonorm(ref double3 normal, double3 tangent) => tangent - tangent.projectsafe(normal.norm());

        #endregion

        #region Matrix Operations

        ///Returns the distance the two vectors in the matrix
        public static double cdistance(this double2x2 f) => math.distance(f.c0, f.c1);
        public static double cdistance(this double3x2 f) => math.distance(f.c0, f.c1);
        public static double cdistance(this double4x2 f) => math.distance(f.c0, f.c1);

        ///Returns the squared distance the two vectors in the matrix
        public static double cdistancesq(this double2x2 f) => math.distancesq(f.c0, f.c1);
        public static double cdistancesq(this double3x2 f) => math.distancesq(f.c0, f.c1);
        public static double cdistancesq(this double4x2 f) => math.distancesq(f.c0, f.c1);


        ///Returns the cross product of the two vectors in the matrix
        public static double3 ccross(this double3x2 f) => math.cross(f.c0, f.c1);

        ///Returns the dot product of the two vectors in the matrix
        public static double cdot(this double4x2 f) => math.dot(f.c0, f.c1);
        /// <inheritdoc cref="cdot(double4x2)"/>
        public static double cdot(this double3x2 f) => math.dot(f.c0, f.c1);
        ///Returns the dot product of the two vectors in the matrix
        public static double cdot(this double2x2 f) => math.dot(f.c0, f.c1);

        #endregion
    }
}