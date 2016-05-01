using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XamSvg.XamForms.Droid;

namespace XamJam.RatingsSample.Droid
{
    [Activity(Label = "XamJam.RatingsSample", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SvgImageRenderer.InitializeForms();

            global::Xamarin.Forms.Forms.Init(this, bundle);

            // This is the default/free sample license that only works for "GestureSample"
            //https://github.com/MichaelRumpler/GestureSample/blob/master/GestureSample/GestureSample.Droid/MainActivity.cs
            MR.Gestures.Android.Settings.LicenseKey = "ALZ9-BPVU-XQ35-CEBG-5ZRR-URJQ-ED5U-TSY8-6THP-3GVU-JW8Z-RZGE-CQW6";

            LoadApplication(new App());
        }
    }
}

