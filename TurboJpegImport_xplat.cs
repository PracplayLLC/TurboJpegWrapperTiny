using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Stugo.Interop;

namespace TurboJpegWrapper
{
    delegate int TJDoCompDel(IntPtr handle, IntPtr srcBuf, int width, int pitch, int height, int pixelFormat, ref IntPtr jpegBuf, ref ulong jpegSize, int jpegSubsamp, int jpegQual, int flags);
    delegate IntPtr TJInitCompDel();

    /// <summary>
    /// a cross-platform call interface to libjpeg-turbo
    /// [experimental and incomplete]
    /// </summary>
    static class TurboJpegImport_xplat
    {
        private const string UnmanagedLibrary_WIN = "turbojpeg";
        const string UnmanagedLibrary_NIX = "libjpeg.a";
        const string UnmanagedLibrary_NIX_shared = "libjpeg.so";
        //static string PathUnmanagedLibrary_NIX { get { return System.IO.Path.Combine(System.IO.Path.DirectorySeparatorChar + "usr", "local", "lib", UnmanagedLibrary_NIX); } }

        static string LibraryName { get { return UnmanagedModuleLoaderBase.IsLinux ? UnmanagedLibrary_NIX : UnmanagedLibrary_WIN; } }

        static UnmanagedModuleLoaderBase _api = null;

        static UnmanagedModuleLoaderBase getapi()
        {
            if (_api == null)
            {
                var loader = Stugo.Interop.UnmanagedModuleLoaderBase.GetLoader(LibraryName);
                if (loader == null)
                    throw new Exception("Unable to load library api at: " + LibraryName);
                _api = loader;
            }
            return _api;
            
        }

        /// <summary>
        /// Create a TurboJPEG compressor instance.
        /// </summary>
        /// <returns>
        /// handle to the newly-created instance, or <see cref="IntPtr.Zero"/> 
        /// if an error occurred (see <see cref="tjGetErrorStr"/>)</returns>
        public static IntPtr tjInitCompressX()
        {
            var api = getapi();
            var call = api.GetDelegate<TJInitCompDel>("tjInitCompress");
            var tjcompressor = call();
            return tjcompressor;

        }

        // tjInitCompressX replaced DllImport: tjInitCompress:
        //[DllImport(UnmanagedLibrary_WIN, CallingConvention = CallingConvention.Cdecl)]
        // public static extern IntPtr tjInitCompress();

        /// <summary>
        /// Compress an RGB, grayscale, or CMYK image into a JPEG image.
        /// </summary>
        /// <param name="handle">A handle to a TurboJPEG compressor or transformer instance</param>
        /// 
        /// <param name="srcBuf">
        /// Pointer to an image buffer containing RGB, grayscale, or CMYK pixels to be compressed.  
        /// This buffer is not modified.
        /// </param>
        /// 
        /// <param name="width">Width (in pixels) of the source image</param>
        /// 
        /// <param name="pitch">
        /// Bytes per line in the source image.  
        /// Normally, this should be <c>width * tjPixelSize[pixelFormat]</c> if the image is unpadded, 
        /// or <c>TJPAD(width * tjPixelSize[pixelFormat])</c> if each line of the image
        /// is padded to the nearest 32-bit boundary, as is the case for Windows bitmaps.  
        /// You can also be clever and use this parameter to skip lines, etc.
        /// Setting this parameter to 0 is the equivalent of setting it to
        /// <c>width * tjPixelSize[pixelFormat]</c>.
        /// </param>
        /// 
        /// <param name="height">Height (in pixels) of the source image</param>
        /// 
        /// <param name="pixelFormat">Pixel format of the source image (see <see cref="TJPixelFormats"/> "Pixel formats")</param>
        /// 
        /// <param name="jpegBuf">
        /// Address of a pointer to an image buffer that will receive the JPEG image.
        /// TurboJPEG has the ability to reallocate the JPEG buffer
        /// to accommodate the size of the JPEG image.  Thus, you can choose to:
        /// <list type="number">
        /// <item>
        /// <description>pre-allocate the JPEG buffer with an arbitrary size using <see cref="tjAlloc"/> and let TurboJPEG grow the buffer as needed</description>
        /// </item>
        /// <item>
        /// <description>set <paramref name="jpegBuf"/> to NULL to tell TurboJPEG to allocate the buffer for you</description>
        /// </item>
        /// <item>
        /// <description>pre-allocate the buffer to a "worst case" size determined by calling <see cref="tjBufSize"/>.
        /// This should ensure that the buffer never has to be re-allocated (setting <see cref="TJFlags.NOREALLOC"/> guarantees this.).</description>
        /// </item>
        /// </list>
        /// If you choose option 1, <paramref name="jpegSize"/> should be set to the size of your pre-allocated buffer.  
        /// In any case, unless you have set <see cref="TJFlags.NOREALLOC"/>,
        /// you should always check <paramref name="jpegBuf"/> upon return from this function, as it may have changed.
        /// </param>
        /// 
        /// <param name="jpegSize">
        /// Pointer to an unsigned long variable that holds the size of the JPEG image buffer.
        /// If <paramref name="jpegBuf"/> points to a pre-allocated buffer, 
        /// then <paramref name="jpegSize"/> should be set to the size of the buffer.
        /// Upon return, <paramref name="jpegSize"/> will contain the size of the JPEG image (in bytes.)  
        /// If <paramref name="jpegBuf"/> points to a JPEG image buffer that is being
        /// reused from a previous call to one of the JPEG compression functions, 
        /// then <paramref name="jpegSize"/> is ignored.
        /// </param>
        /// 
        /// <param name="jpegSubsamp">
        /// The level of chrominance subsampling to be used when
        /// generating the JPEG image (see <see cref="TJSubsamplingOptions"/> "Chrominance subsampling options".)
        /// </param>
        /// 
        /// <param name="jpegQual">The image quality of the generated JPEG image (1 = worst, 100 = best)</param>
        /// 
        /// <param name="flags">The bitwise OR of one or more of the <see cref="TJFlags"/> "flags"</param>
        /// 
        /// <returns>0 if successful, or -1 if an error occurred (see <see cref="tjGetErrorStr"/>)</returns>
        public static int tjCompressX(IntPtr handle, IntPtr srcBuf, int width, int pitch, int height, int pixelFormat, ref IntPtr jpegBuf, ref ulong jpegSize, int jpegSubsamp, int jpegQual, int flags)
        {
            var api = getapi();
            var call = api.GetDelegate<TJDoCompDel>("tjCompress2");
            var result = call(handle, srcBuf, width, pitch, height, pixelFormat, ref jpegBuf, ref jpegSize, jpegSubsamp, jpegQual, flags);
            return result;
        }

        // tjCompressX replaced dllimport tjCompress2
        //[DllImport(UnmanagedLibrary_WIN, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int tjCompress2(IntPtr handle, IntPtr srcBuf, int width, int pitch, int height, int pixelFormat, ref IntPtr jpegBuf, ref ulong jpegSize, int jpegSubsamp, int jpegQual, int flags);




    }
}
