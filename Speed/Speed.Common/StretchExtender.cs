/// Created by Magnus Ekström, mangemange@gmail.com.
/// Feel free to use and exprement with this extender as you like but dont forget to leave
/// some acknowledgements if you are using it in public releases.
/// 
/// 2008-01-01, 1.0.0.1 : First Release.
/// 

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Speed
{
    /// <summary>
    /// This class extends the 'System.Drawing.Bitmap' class to get functionality of the 'Stretch' property
    /// commonly found and used in XAML elements in WPF and Silverlight.
    /// The idea is to be able to pass a boundary that the image shape should resize on and also be able to
    /// control how the shape is fitted insied the boundary by controling how it is stretching. 
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    static public class StretchExtender
    {
        /// <summary>
        /// Stretches the shape on a given boundary.
        /// </summary>
        /// <param name="shape">this, since extender, not displayed.</param>
        /// <param name="newBounding">The boundaries to stretch the shape on.</param>
        /// <param name="stretchType">How the original shape should be stretched on the new boundaries.</param>
        /// <param name="vAlign">How to align the shape vertically in case of clipping. 
        /// Suppling alignment parameters is only valid when 'stretchType' is 'Stretch.None' or 'Stretch.UniformToFill', i.e. when clipping can occur.</param>
        /// <param name="hAlign">How to align the shape horizontally in case of clipping. 
        /// Suppling alignment parameters is only valid when 'stretchType' is 'Stretch.None' or 'Stretch.UniformToFill', i.e. when clipping can occur.</param>
        /// <returns>An shape that is stretched according to input parameters</returns>
        static public Bitmap FitToBounds(this Bitmap shape, Size newBounding, Stretch stretchType, VerticalAligment vAlign, HorizontalAligment hAlign)
        {
            return boundIt(shape, newBounding, stretchType, vAlign, hAlign);
        }

        /// <summary>
        /// Stretches the shape on a given boundary.
        /// </summary>
        /// <param name="shape">this, since extender, not displayed.</param>
        /// <param name="newBounding">The boundaries to stretch the shape on.</param>
        /// <param name="stretchType">How the original shape should be stretched on the new boundaries.</param>
        /// <returns>An shape that is stretched according to input parameters</returns>
        static public Bitmap FitToBounds(this Bitmap shape, Size newBounding, Stretch stretchType)
        {
            return boundIt(shape, newBounding, stretchType, VerticalAligment.None, HorizontalAligment.None);
        }

        /// <summary>
        /// Stretches the shape on a given boundary.
        /// </summary>
        /// <param name="shape">this, since extender, not displayed.</param>
        /// <param name="boundWidth">The Width on the boundary to stretch the shape on.</param>
        /// <param name="boundHeight">The Height on the boundary to stretch the shape on.</param>
        /// <param name="stretchType">How the original shape should be stretched on the new boundaries.</param>
        /// <param name="vAlign">How to align the shape vertically in case of clipping. 
        /// Suppling alignment parameters is only valid when 'stretchType' is 'Stretch.None' or 'Stretch.UniformToFill', i.e. when clipping can occur.</param>
        /// <param name="hAlign">How to align the shape horizontally in case of clipping. 
        /// Suppling alignment parameters is only valid when 'stretchType' is 'Stretch.None' or 'Stretch.UniformToFill', i.e. when clipping can occur.</param>
        /// <returns>An shape that is stretched according to input parameters</returns>
        static public Bitmap FitToBounds(this Bitmap shape, int boundWidth, int boundHeight, Stretch stretchType, VerticalAligment vAlign, HorizontalAligment hAlign)
        {
            return boundIt(shape, new Size(boundWidth, boundHeight), stretchType, vAlign, hAlign);
        }

        /// <summary>
        /// Stretches the shape on a given boundary.
        /// </summary>
        /// <param name="shape">this, since extender, not displayed.</param>
        /// <param name="boundWidth">The Width on the boundary to stretch the shape on.</param>
        /// <param name="boundHeight">The Height on the boundary to stretch the shape on.</param>
        /// <param name="stretchType">How the original shape should be stretched on the new boundaries.</param>
        /// <returns>An shape that is stretched according to input parameters</returns>
        static public Bitmap FitToBounds(this Bitmap shape, int boundWidth, int boundHeight, Stretch stretchType)
        {
            return boundIt(shape, new Size(boundWidth, boundHeight), stretchType, VerticalAligment.None, HorizontalAligment.None);
        }
        
        #region Private Helpers

        /// <summary>
        /// Helper to rebound a shape to a given new boundary. To be wrapped by public extension methods.
        /// </summary>
        /// <param name="shape">The original shape.</param>
        /// <param name="newBounding">The boundaries to stretch on.</param>
        /// <param name="stretchType">How the original shape should be stretched on the new boundaries.</param>
        /// <param name="vAlign">Specifies how the shape should align vertically if clipping occurs.</param>
        /// <param name="hAlign">Specifies how the shape should align horizontally if clipping occurs.</param>
        /// <returns>An shape that is stretched according to input parameters</returns>
        static private Bitmap boundIt(Bitmap shape, Size newBounding, Stretch stretchType, VerticalAligment vAlign, HorizontalAligment hAlign)
        {
            Point alignmentPoint;
            switch (stretchType)
            {
                // Only have effect when new boundaries are smaller than the orignal boundaries of the shape.
                case Stretch.None:
                    // Get the correct alignment point, i.e the upper left corner where we start the clip rectangle.
                    alignmentPoint = GetAlignmentPoint(shape, newBounding, vAlign, hAlign);
                    // Since Stretch.None will not be able to resize the shape we make need to make sure 
                    // that we at a maximum will clip the whole shape and nothing more, less is allways fine though.
                    if (newBounding.Height > shape.Height) newBounding.Height = shape.Height;
                    if (newBounding.Width > shape.Width) newBounding.Width = shape.Width;
                    // Supply the clipping point and new boundarie data to get the part of the shape we want.
                    return shape.Clone(new Rectangle(alignmentPoint.X, 
                                                     alignmentPoint.Y, 
                                                     newBounding.Width, 
                                                     newBounding.Height), 
                                       shape.PixelFormat);
                    
                case Stretch.Fill:
                    if (vAlign != VerticalAligment.None || hAlign != HorizontalAligment.None)
                    {
                        throw new ArgumentException("Cannot apply alignments to an shape that is stretched with 'Stretch.Fill'. Remove alignments or change 'Stretch' type.");
                    }
                    // Simply fill the new boundaries with the original shape disregarding and ratio.
                    return new Bitmap(shape, newBounding);

                case Stretch.Uniform:
                    if (vAlign != VerticalAligment.None || hAlign != HorizontalAligment.None)
                    {
                        throw new ArgumentException("Cannot apply alignments to an shape that is stretched with 'Stretch.Uniform'. Remove alignments or change 'Stretch' type.");
                    }
                    // Calculate scaling factors on both X and Y axis and use the one that 
                    // downscales the most, (Math.Min()), in order to fit the whole shape in boundaries.
                    double uniformScale = Math.Min((double)newBounding.Width / shape.Width, (double)newBounding.Height / shape.Height);
                    // Apply the scaling to the supplied shape.
                    return new Bitmap(shape, (int)(shape.Width * uniformScale), (int)(shape.Height * uniformScale)); 

                case Stretch.UniformToFill:
                    // Calculate scaling factors on both X and Y axis and use the one that will
                    // make the shape fill supplied boundaries, hence the Math.Max.
                    double utfScale = Math.Max((double)newBounding.Width / shape.Width, (double)newBounding.Height / shape.Height);
                    // Apply the scaling to the supplied shape.
                    Bitmap alignShape = new Bitmap(shape, (int)Math.Round(shape.Width * utfScale), (int)(shape.Height * utfScale));
                    // Get the correct alignment point, i.e the upper left corner where we start the clip rectangle.
                    alignmentPoint = GetAlignmentPoint(alignShape, newBounding, vAlign, hAlign);
                    // Supply the clipping point and new boundarie data to get the part of the shape we want.
                    return alignShape.Clone(new Rectangle(alignmentPoint.X, 
                                                          alignmentPoint.Y, 
                                                          newBounding.Width, 
                                                          newBounding.Height), 
                                            alignShape.PixelFormat);

                default:
                    return null;
            }            
        }

        /// <summary>
        /// Helper to calculate the alignment point of a shape inside its boundaries.
        /// </summary>
        /// <returns>The alignment point calculated from input data.</returns>
        static private Point GetAlignmentPoint(Bitmap shape, Size bounding, VerticalAligment vAlign, HorizontalAligment hAlign)
        {
            return new Point(GetHorizontalAlignmentOffset(shape, bounding, hAlign),
                             GetVerticalAlignmentOffset(shape, bounding, vAlign));
        }

        /// <summary>
        /// Helper to calculate the vertical alignment value for a shape inside its 
        /// boundaries based on which type of alignment that are used.
        /// </summary>
        /// <returns>Vertical alignment value. Y-axis value.</returns>
        static private int GetVerticalAlignmentOffset(Bitmap shape, Size bounding, VerticalAligment vAlign)
        {
            switch (vAlign)
            {
                case VerticalAligment.Top: // Default alignment in WPF
                    return 0;

                case VerticalAligment.Center:
                    // First check if there will be any clipping, if so we calculate the
                    // vertical alignment value. Otherwise we default to 0.
                    if (bounding.Height < shape.Height)
                        return (shape.Height - bounding.Height) / 2;
                    else
                        return 0;
                    
                case VerticalAligment.Bottom:
                    // First check if there will be any clipping, if so we calculate the
                    // vertical alignment value. Otherwise we default to 0.
                    if (bounding.Height < shape.Height)
                        return shape.Height - bounding.Height;
                    else
                        return 0;

                // Same as default since there has to be some kind of alignment. 
                // We cant really have No alignment.                    
                case VerticalAligment.None:
                    return 0;

                // Cannot get here because of the nature of enum's, but supplied to remove warnings.
                default:
                    return 0; 
            }
        }

        /// <summary>
        /// Helper to calculate the horizontal alignment value for a shape inside its 
        /// boundaries based on which type of alignment that are used.
        /// </summary>
        /// <returns>Horizontal alignment value. X-axis value.</returns>
        static private int GetHorizontalAlignmentOffset(Bitmap shape, Size bounding, HorizontalAligment hAlign)
        {
            switch (hAlign)
            {
                case HorizontalAligment.Left: // Default alignment in WPF
                    return 0;

                case HorizontalAligment.Center:
                    // First check if there will be any clipping, if so we calculate the
                    // horizontal alignment value. Otherwise we default to 0.
                    if (bounding.Width < shape.Width)
                        return (shape.Width - bounding.Width) / 2;
                    else
                        return 0;

                case HorizontalAligment.Right:
                    // First check if there will be any clipping, if so we calculate the
                    // horizontal alignment value. Otherwise we default to 0.
                    if (bounding.Width < shape.Width)
                        return shape.Width - bounding.Width;
                    else
                        return 0;

                // Same as default since there has to be some kind of alignment. 
                // We cant really have No alignment.
                case HorizontalAligment.None:
                    return 0;

                // Cannot get here because of the nature of enum's, but supplied to remove warnings.
                default:
                    return 0;
            }
        }

        #endregion
    
    }

    /// <summary>
    /// Describes how a shape should be stretched.
    /// </summary>
    public enum Stretch
    {
        /// <summary>
        /// The shape are not stretched. Aspect ratio is preserved. Clipping can occur.
        /// </summary>
        None,
        /// <summary>
        /// The shape are stretched to fill its layout space. 
        /// Aspect ratio is not preserved.
        /// </summary>
        Fill,
        /// <summary>
        /// The shape are stretched as much as possible 
        /// to fill its layout space while preserving its original aspect ratio.
        /// </summary>
        Uniform,
        /// <summary>
        /// The shape are stretched to completely fill its layout space while preserving 
        /// its original aspect ratio. Clipping of the shape will occur;
        /// </summary>
        UniformToFill
    }

    /// <summary>
    /// Describes how to align a shape vertically inside boundaries 
    /// in case clipping occurs. 
    /// </summary>
    public enum VerticalAligment
    {
        /// <summary>
        /// Vertically aligns the shape to the Top inside the boundaries. This is WPF default behavior.
        /// </summary>
        Top,
        /// <summary>
        /// Vertically aligns the shape to the Center inside the boundaries.
        /// </summary>
        Center,
        /// <summary>
        /// Vertically aligns to the shape to the Bottom inside the boundaries.
        /// </summary>
        Bottom,
        /// <summary>
        /// The default vertical alignment is used. (Top).
        /// </summary>
        None
    }

    /// <summary>
    /// Describes how to align a shape horizontally inside boundaries 
    /// in case clipping occurs. 
    /// </summary>
    public enum HorizontalAligment
    {
        /// <summary>
        /// Horizontally aligns the shape to the Left inside the boundaries. This is WPF default behavior.
        /// </summary>
        Left,
        /// <summary>
        /// Horizontally aligns the shape to Center inside the boundaries.
        /// </summary>
        Center,
        /// <summary>
        /// Horizontally aligns the shape to the Right inside the boundaries.
        /// </summary>
        Right,
        /// <summary>
        /// The default horizontally alignment is used. (Left).
        /// </summary>
        None
    }

    static public class ImageExtender
    {
        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        static extern int GdipCloneBitmapArea(float x, float y, float width, float height, int format, HandleRef srcbitmap, out IntPtr dstbitmap);
        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        static extern int GdipCloneBitmapAreaI(int x, int y, int width, int height, int format, HandleRef srcbitmap, out IntPtr dstbitmap);

        public static Image SuperClone(this Image toClone, Rectangle bound, System.Drawing.Imaging.PixelFormat format)
        {
            IntPtr nativeImage = IntPtr.Zero; 
            IntPtr zero = IntPtr.Zero;
            int status = GdipCloneBitmapAreaI(0, 0, 40, 40, (int)format, new HandleRef(toClone, nativeImage), out zero);
            if ((status != 0) || (zero == IntPtr.Zero))
            {
                int i = 0;
            }


            Bitmap bitmap = null; ;// = new Bitmap();
            //bitmap.SetNativeImage(zero);

            return bitmap;
        }


        /*
        public Bitmap Clone(Rectangle rect, System.Drawing.Imaging.PixelFormat format)
        {
            if ((rect.Width == 0) || (rect.Height == 0))
            {
                //throw new ArgumentException(SR.GetString("GdiplusInvalidRectangle", new object[] { rect.ToString() }));
            }
            IntPtr zero = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipCloneBitmapAreaI(rect.X, rect.Y, rect.Width, rect.Height, 
                                                                    (int)format, new HandleRef(this, base.nativeImage), out zero);
            if ((status != 0) || (zero == IntPtr.Zero))
            {
                //throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return FromGDIplus(zero);
        }
        */
 

    }
    
}
