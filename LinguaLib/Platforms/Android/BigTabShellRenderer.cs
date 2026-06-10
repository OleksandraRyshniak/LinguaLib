using Android.Views;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;

namespace LinguaLib.Platforms.Android;

public class BigTabShellRenderer : ShellRenderer
{
    protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        => new BigTabAppearanceTracker(this, shellItem);
}

public class BigTabAppearanceTracker : ShellBottomNavViewAppearanceTracker
{
    public BigTabAppearanceTracker(IShellContext context, ShellItem shellItem)
        : base(context, shellItem) { }

    public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
    {
        base.SetAppearance(bottomView, appearance);
        bottomView.Post(() => ApplySize(bottomView));
    }

    public override void ResetAppearance(BottomNavigationView bottomView)
    {
        base.ResetAppearance(bottomView);
        bottomView.Post(() => ApplySize(bottomView));
    }

    private static void ApplySize(BottomNavigationView bottomView)
    {
        try
        {
            bottomView.ItemIconSize = 0;
            SetTextSizeRecursive(bottomView, 30f);
        }
        catch { /* ignore if view is already disposed */ }
    }

    private static void SetTextSizeRecursive(ViewGroup group, float sp)
    {
        for (int i = 0; i < group.ChildCount; i++)
        {
            var child = group.GetChildAt(i);
            if (child is global::Android.Widget.TextView tv)
                tv.SetTextSize(global::Android.Util.ComplexUnitType.Sp, sp);
            else if (child is ViewGroup vg)
                SetTextSizeRecursive(vg, sp);
        }
    }
}
