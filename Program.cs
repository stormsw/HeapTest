using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
namespace HeapTest
{
    /*
     * Start by hacking difference on x86 and x64 :)
     * 
     * 1.4g on x86 on my (16G)
     * --------------------------------------------------------------------------------------------
     * Now choose (x86) Project->Properties->Compile->Build Events->PostBuildEvent Command
     * --------------------------------------------------------------------------------------------
     * call "$(DevEnvDir)..\..\vc\vcvarsall.bat" x86
     * "$(DevEnvDir)..\..\vc\bin\EditBin.exe" "$(TargetPath)"  /LARGEADDRESSAWARE
     * 
     * --------------------------------------------------------------------------------------------
     * And uncheck the option: Project->Properties->Debug->Enable the Visual Studio Hosting Process
     * --------------------------------------------------------------------------------------------
     * 
     * 3.8g on x86
     * 
     * and +8G (25G -> 32G) on x64
     */
    class Program
    {
        static void Main(string[] args)
        {
            var hHeap = Heap.HeapCreate(Heap.HeapFlags.HEAP_GENERATE_EXCEPTIONS, 0, 0);
            // if the FriendlyName is "heap.vshost.exe" then it's using the VS Hosting Process and not "Heap.Exe"
            Trace.WriteLine(AppDomain.CurrentDomain.FriendlyName + " heap created");
            uint nSize = 100 * 1024 * 1024;
            ulong nTot = 0;
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    var ptr = Heap.HeapAlloc(hHeap, 0, nSize);
                    nTot += nSize;
                    Trace.WriteLine(String.Format("Iter #{0} {1:n0} ", i, nTot));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception " + ex.Message);
            }
            Heap.HeapDestroy(hHeap);
            Trace.WriteLine("destroyed");
        }
    }
    public class Heap
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr HeapCreate(HeapFlags flOptions, uint dwInitialsize, uint dwMaximumSize);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr HeapAlloc(IntPtr hHeap, HeapFlags dwFlags, uint dwSize);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool HeapFree(IntPtr hHeap, HeapFlags dwFlags, IntPtr lpMem);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool HeapDestroy(IntPtr hHeap);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcessHeap();
        [Flags()]
        public enum HeapFlags
        {
            HEAP_NO_SERIALIZE = 0x1,
            HEAP_GENERATE_EXCEPTIONS = 0x4,
            HEAP_ZERO_MEMORY = 0x8
        }
    }
}