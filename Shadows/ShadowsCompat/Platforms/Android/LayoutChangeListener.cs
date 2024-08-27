namespace Sharpnado.Shades.Droid;

public class LayoutChangeListener(Action<bool, int, int, int, int> onChanged) : Java.Lang.Object, Android.Views.View.IOnLayoutChangeListener
{
    public void OnLayoutChange(Android.Views.View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
    {
        var changed = left != oldLeft || top != oldTop || right != oldRight || bottom != oldBottom;

        onChanged(changed, left, top, right, bottom);
    }
}