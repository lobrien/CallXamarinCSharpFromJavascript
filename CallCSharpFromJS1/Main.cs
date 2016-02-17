using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.Drawing;
using CoreGraphics;
using JavaScriptCore;

namespace SingleFileSolution
{
    public class VisibleToJavaScript
    {
        public void JSExportTest()
        {
            var webView = new UIWebView();
            var context = (JSContext)webView.ValueForKeyPath((NSString)"documentView.webView.mainFrame.javaScriptContext");
            context.ExceptionHandler = (JSContext context2, JSValue exception) => 
                {
                    Console.WriteLine("JS exception: {0}", exception);
                };
            var obj = new MSJSExporter();
            context[(NSString)"obj"] = JSValue.From(obj, context);
            var val = context.EvaluateScript("obj.myFunc ();");
        }
    }

    [Protocol()]
    interface IMyJSVisibleProtocol : IJSExport
    {
        [Export("myFunc")]
        int MyFunc();
    }

    class MSJSExporter : NSObject, IMyJSVisibleProtocol
    {
        public int MyFunc()
        {
            Console.WriteLine("Called!");
            return 42;
        }
    }

    public class ContentView : UIView
    {
        UIWebView webView;

        public ContentView(UIColor fillColor)
        {
            BackgroundColor = fillColor;

            webView = new UIWebView(UIScreen.MainScreen.Bounds);
            var context = (JSContext)webView.ValueForKeyPath((NSString)"documentView.webView.mainFrame.javaScriptContext");
            context.ExceptionHandler = (JSContext context2, JSValue exception) => 
            {
                Console.WriteLine("JS exception: {0}", exception);
            };
            var obj = new MSJSExporter();
            context[(NSString)"myCSharpObject"] = JSValue.From(obj, context);

            webView.LoadRequest(NSUrlRequest.FromUrl(new NSUrl("MyHtmlFile.html", false)));
            this.AddSubview(webView);
        }
    }

    public class SimpleViewController : UIViewController
    {
        public SimpleViewController() : base()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var view = new ContentView(UIColor.Blue);

            var js = new VisibleToJavaScript();
            js.JSExportTest();

            this.View = view;
        }
    }

    [Register("AppDelegate")]
    public  class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;
        SimpleViewController viewController;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            viewController = new SimpleViewController();
            window.RootViewController = viewController;

            window.MakeKeyAndVisible();

            return true;
        }
    }

    public class Application
    {
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}