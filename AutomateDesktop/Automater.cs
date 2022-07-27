using OpenCvSharp;

namespace AutomateDesktop
{
    internal class Automater
    {
        internal static void SeeScreen(int hBitmap) //hBitmap is a handle to a Bitmap in memory
        {
            // Create a bitmap from the Windows handle
            Bitmap bmpImage = new Bitmap(Image.FromHbitmap(new IntPtr(hBitmap)),
            Image.FromHbitmap(new IntPtr(hBitmap)).Width,
            Image.FromHbitmap(new IntPtr(hBitmap)).Height);

            //var cascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();

            // Note: In the case we would like to load it from XML, we could use:
            // var cascade = HaarCascade.FromXml("filename.xml");

            // Now, create a new Haar object detector with the cascade:
            //var detector = new HaarObjectDetector(cascade, minSize: 50, searchMode: ObjectDetectorSearchMode.NoOverlap);

            using var src = new Mat(new IntPtr(hBitmap));
            using var dst = new Mat();

            Cv2.Canny(src, dst, 50, 200);
            using (new Window("src image", src))
            using (new Window("dst image", dst))
            {
                Cv2.WaitKey();
            }

            // Note that we have specified that we do not want overlapping objects,
            // and that the minimum object an object can have is 50 pixels. Now, we
            // can use the detector to classify a new image. For instance, consider
            // the famous Lena picture:



            // We have to call ProcessFrame to detect all rectangles containing the 
            // object we are interested in (which in this case, is the face of Lena):
            //Rectangle[] rectangles = detector.ProcessFrame(bmpImage);

        }
    }
}
