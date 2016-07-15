#region

using System;
using FFImageLoading.Forms;
using FFImageLoading.Helpers;
using FFImageLoading.Work;
using MR.Gestures;
using Plugin.XamJam.BugHound;
using PropertyChanged;
using Xamarin.Forms;

#endregion

namespace XamJam.PicSelector
{
    [ImplementPropertyChanged]
    public class PicSelectorCropViewModel
    {
        private static readonly IBugHound logger = BugHound.ByType(typeof(PicSelectorCropViewModel));

        /// <summary>
        ///     The percentage of the screen real-estate that the crop-box should take
        /// </summary>
        private static readonly double cropBoxPercent = 0.65;

        /// <summary>
        ///     The size of the crop box border.
        /// </summary>
        private static readonly double cropBoxBorderSize = 1.5;

        /// <summary>
        ///     This is setup during the Resize callback. The user cannot zoom out past this value.
        /// </summary>
        private double minScale = 1.0;

        private double viewWidth, viewHeight;

        /// <summary>
        ///     Creates a new PhotoSelectorCropViewModel
        /// </summary>
        public PicSelectorCropViewModel()
        {
            PanningCommand = new Command<PanEventArgs>(args => TryPanBy(args.DeltaDistance.X, args.DeltaDistance.Y));
            PinchingCommand = new Command<PinchEventArgs>(args => TryZoomBy(args.DeltaScale));
        }

        public PicSelectionResult PicSelectionResult { get; } = new PicSelectionResult();

        public double Scale { get; set; } = 1.0;

        public double TranslationX { get; set; }

        public double TranslationY { get; set; }

        public Command<PanEventArgs> PanningCommand { get; }

        public Command<PinchEventArgs> PinchingCommand { get; }

        /// <summary>
        ///     Where the image gets drawn
        /// </summary>
        /// <value>The image box.</value>
        public Rectangle ImageBox { get; set; }

        /// <summary>
        ///     The left overlay box that shows the left portion of the image that will get cropped out
        /// </summary>
        /// <value>The left box.</value>
        public Rectangle LeftBox { get; set; }

        /// <summary>
        ///     The right overlay box that shows the left portion of the image that will get cropped out
        /// </summary>
        /// <value>The right box.</value>
        public Rectangle RightBox { get; set; }

        /// <summary>
        ///     The top overlay box that shows the left portion of the image that will get cropped out
        /// </summary>
        /// <value>The top box.</value>
        public Rectangle TopBox { get; set; }

        /// <summary>
        ///     The bottom overlay box that shows the left portion of the image that will get cropped out
        /// </summary>
        /// <value>The bottom box.</value>
        public Rectangle BottomBox { get; set; }

        /// <summary>
        ///     The white-line that shows the left of the crop view
        /// </summary>
        /// <value>The left crop box.</value>
        public Rectangle LeftCropBox { get; set; }

        /// <summary>
        ///     The white-line that shows the right of the crop view
        /// </summary>
        /// <value>The right crop box.</value>
        public Rectangle RightCropBox { get; set; }

        /// <summary>
        ///     The white-line that shows the top of the crop view
        /// </summary>
        /// <value>The top crop box.</value>
        public Rectangle TopCropBox { get; set; }

        /// <summary>
        ///     The white-line that shows the bottom of the crop view
        /// </summary>
        /// <value>The bottom crop box.</value>
        public Rectangle BottomCropBox { get; set; }

        /// <summary>
        ///     Returns the actual crop-box rectangle, this is the rectangle that specifies what portion of the image should be
        ///     saved
        /// </summary>
        /// <value>The crop box.</value>
        public Rectangle CropBox
            =>
                new Rectangle(LeftCropBox.Right, TopCropBox.Bottom, RightCropBox.Left - LeftCropBox.Right,
                    BottomCropBox.Top - TopCropBox.Bottom);

        /// <summary>
        ///     Used to make sure the image doesn't draw under the status bar on the top of an iPhone that shows the time/signal
        ///     strength/battery %
        /// </summary>
        /// <value>The status box.</value>
        public Rectangle StatusBox { get; set; } = new Rectangle(0, 0, 0, 0);

        /// <summary>
        ///     Called whenever the loaded image size changes
        /// </summary>
        /// <param name="ci">new image</param>
        public void LoadImage(CachedImage ci)
        {
            ci.Success += (sender, args) =>
            {
                var ii = args.ImageInformation;
                TranslationX = 0;
                TranslationY = 0;
                ResetImageBox();
                double xamScaleFactor;
                if (ii.OriginalWidth >= ii.OriginalHeight)
                {
                    xamScaleFactor = ImageBox.Width / ii.OriginalWidth;
                }
                else
                {
                    xamScaleFactor = ImageBox.Height / ii.OriginalHeight;
                }

                var widthRatio = ii.OriginalWidth * xamScaleFactor / ImageBox.Width;
                var heightRatio = ii.OriginalHeight * xamScaleFactor / ImageBox.Height;

                var minRatio = Math.Min(widthRatio, heightRatio);

                if (minRatio < 1)
                {
                    Scale = 1 / minRatio;
                }
                else
                {
                    Scale = 1;
                }
            };
        }

        /// <summary>
        ///     Called whenever the view gets resized. This is where all the real work is done.
        /// </summary>
        /// <param name="newViewWidth">The view's width.</param>
        /// <param name="newViewHeight">The view's height.</param>
        public void Resized(double newViewWidth, double newViewHeight)
        {
            viewWidth = newViewWidth;
            viewHeight = newViewHeight;

            // To make sure the image doesn't get drawn under the iOS top status bar (battery, cell signal, time, etc) when zoomed in
            if (Device.OS == TargetPlatform.iOS)
            {
                StatusBox = new Rectangle(0, -20, newViewWidth, 20);
            }

            // Setup the white outline CropBox to show what part of the image will be cropped
            {
                var cropBoxSize = cropBoxPercent * Math.Min(newViewWidth, newViewHeight);
                var halfCropBoxSize = cropBoxSize * 0.5;
                var cropTop = newViewHeight * 0.5 - halfCropBoxSize;
                var cropBottom = newViewHeight * 0.5 + halfCropBoxSize;
                var cropLeft = newViewWidth * 0.5 - halfCropBoxSize;
                var cropRight = newViewWidth * 0.5 + halfCropBoxSize;
                TopCropBox = new Rectangle(cropLeft, cropTop, cropBoxSize, cropBoxBorderSize);
                BottomCropBox = new Rectangle(cropLeft, cropBottom, cropBoxSize + cropBoxBorderSize, cropBoxBorderSize);
                LeftCropBox = new Rectangle(cropLeft, cropTop, cropBoxBorderSize, cropBoxSize);
                RightCropBox = new Rectangle(cropRight, cropTop, cropBoxBorderSize, cropBoxSize + cropBoxBorderSize);
                logger.Debug($"Top Crop Box @ {TopCropBox}");
                logger.Debug($"Bottom Crop Box @ {BottomCropBox}");
                logger.Debug($"Left Crop Box @ {LeftCropBox}");
                logger.Debug($"Right Crop Box @ {RightCropBox}");
            }

            ResetImageBox();

            // Setup the opacity-boxes to partially hide the part of the image that will be cropped out
            {
                var topBottomWidth = ImageBox.Width + LeftCropBox.Width + RightCropBox.Width;
                var topBottomLeft = ImageBox.Left - LeftCropBox.Width;
                TopBox = new Rectangle(topBottomLeft, 0, topBottomWidth, TopCropBox.Top);
                BottomBox = new Rectangle(topBottomLeft, BottomCropBox.Bottom, topBottomWidth,
                    newViewHeight - BottomCropBox.Bottom);
                LeftBox = new Rectangle(0, 0, LeftCropBox.Left, newViewHeight);
                RightBox = new Rectangle(RightCropBox.Right, 0, LeftCropBox.Left, newViewHeight);
                logger.Debug($"Top Box @ {TopBox}");
                logger.Debug($"Bottom Box @ {BottomBox}");
                logger.Debug($"Left Box @ {LeftBox}");
                logger.Debug($"Right Box @ {RightBox}");
            }
        }

        private void ResetImageBox()
        {
            // Setup the ImageBox to fill the cropbox and overflow as needed
            var imageWidth = PicSelectionResult.Selected.Width;
            var imageHeight = PicSelectionResult.Selected.Height;
            //				var imageWidth = ImageBox.Width;
            //				var imageHeight = ImageBox.Height;
            var cropBoxWidth = RightCropBox.Left - LeftCropBox.Right;
            var cropBoxHeight = BottomCropBox.Top - TopCropBox.Bottom;
            logger.Debug($"Rescaling raw image {imageWidth}x{imageHeight} to crop box {cropBoxWidth},{cropBoxHeight}");

            //need to center and resize the image
            var widthFactor = cropBoxWidth / imageWidth;
            var heightFactor = cropBoxHeight / imageHeight;
            //scale to fill the cropbox
            var scaleFactor = widthFactor > heightFactor ? widthFactor : heightFactor;
            var scaledWidth = imageWidth * scaleFactor;
            var scaledHeight = imageHeight * scaleFactor;
            minScale = Math.Min(scaledWidth / cropBoxWidth, scaledHeight / cropBoxWidth);
            // center the image
            var x = (viewWidth - scaledWidth) * 0.5 + 0.5 * cropBoxBorderSize;
            var y = (viewHeight - scaledHeight) * 0.5 + 0.5 * cropBoxBorderSize;
            ImageBox = new Rectangle(x, y, scaledWidth, scaledHeight);
            logger.Debug($"Image Box @ {ImageBox}");
        }

        private void TryPanBy(double deltaX, double deltaY)
        {
            logger.Debug($"SelectedPhoto size is {PicSelectionResult.Selected.Width}x{PicSelectionResult.Selected.Height}");

            if (deltaX != 0)
            {
                var actualWidth = ImageBox.Width * Scale;
                var deltaWidth = actualWidth - ImageBox.Width;
                if (deltaX > 0)
                {
                    var actualLeft = ImageBox.Left - deltaWidth / 2.0 + TranslationX;
                    var maxRight = LeftCropBox.Right - actualLeft;
                    if (deltaX > maxRight)
                    {
                        deltaX = maxRight;
                        logger.Debug($"Moving right. Max legal right = {maxRight}. Moving right by {deltaX}");
                    }
                }
                else
                {
                    var actualRight = ImageBox.Right + deltaWidth / 2.0 + TranslationX;
                    var maxLeft = RightCropBox.Left - actualRight;
                    if (deltaX < maxLeft)
                    {
                        deltaX = maxLeft;
                        logger.Debug($"Moving left. Max legal left = {maxLeft}. Moving left by {deltaX}");
                    }
                }
            }

            if (deltaY != 0)
            {
                var actualHeight = ImageBox.Height * Scale;
                var deltaHeight = actualHeight - ImageBox.Height;
                if (deltaY > 0)
                {
                    var actualTop = ImageBox.Top - deltaHeight / 2.0 + TranslationY;
                    var maxDown = TopCropBox.Bottom - actualTop;
                    if (deltaY > maxDown)
                    {
                        deltaY = maxDown;
                        logger.Debug($"Moving down. Max legal down= {maxDown}. Moving down by {deltaY}");
                    }
                }
                else
                {
                    var actualBottom = ImageBox.Bottom + deltaHeight / 2.0 + TranslationY;
                    var maxUp = BottomCropBox.Top - actualBottom;
                    if (deltaY < maxUp)
                    {
                        deltaY = maxUp;
                        logger.Debug($"Moving up. Max legal up = {maxUp}. Moving up by {deltaY}");
                    }
                }
            }


            TranslationX += deltaX;
            TranslationY += deltaY;
            //MR.GESTURES sample says this might be needed if we ever do WP: https://github.com/MichaelRumpler/GestureSample/blob/master/GestureSample/GestureSample/ViewModels/TransformViewModel.cs
            //if (Device.OS == TargetPlatform.WinPhone)
            //{
            //	TranslationX += e.DeltaDistance.X * Scale;		// unfortunately this has to be specified differently on WinPhone
            //	TranslationY += e.DeltaDistance.Y * Scale;		// another bug in Xamarin Forms IMHO
            //}			
        }


        private void TryZoomBy(double zoomFactor)
        {
            var newScale = Math.Min(5, Math.Max(minScale, Scale * zoomFactor));

            //we're zooming out. Make sure the image gets translated if needed to completely fill the imageBox
            if (zoomFactor < 1)
            {
                var actualWidth = ImageBox.Width * newScale;
                var actualHeight = ImageBox.Height * newScale;

                var deltaWidth = actualWidth - ImageBox.Width;
                var deltaHeight = actualHeight - ImageBox.Height;

                var actualLeft = ImageBox.Left - deltaWidth / 2.0 + TranslationX;
                var actualRight = ImageBox.Right + deltaWidth / 2.0 + TranslationX;

                var actualTop = ImageBox.Top - deltaHeight / 2.0 + TranslationY;
                var actualBottom = ImageBox.Bottom + deltaHeight / 2.0 + TranslationY;

                // If we need to move the image up
                if (actualTop > TopCropBox.Bottom)
                {
                    //Y-Translation
                    TranslationY += TopCropBox.Bottom - actualTop;
                }
                else if (actualBottom < BottomCropBox.Top)
                {
                    TranslationY += BottomCropBox.Top - actualBottom;
                }

                if (actualLeft > LeftCropBox.Right)
                {
                    //X-Translation
                    TranslationX += LeftCropBox.Right - actualLeft;
                }
                else if (actualRight < RightCropBox.Left)
                {
                    TranslationX += RightCropBox.Left - actualRight;
                }
            }

            Scale = newScale;

            logger.Debug($"Pinched, new scale = {Scale}. Delta Scale = {zoomFactor}");
        }
    }
}