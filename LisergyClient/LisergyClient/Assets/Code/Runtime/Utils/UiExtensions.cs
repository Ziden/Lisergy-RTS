using Assets.Code;
using GameData.Specs;
using System;
using UnityEngine.UIElements;

public static class UiExtensions
{
    public static void SetBackground(this VisualElement element, in ArtSpec spec)
    {
        _ = UnityServicesContainer.Interface.Assets.GetSprite(spec).ContinueWith(sprite =>
        {
            element.style.backgroundImage = new StyleBackground(sprite);
        });
    }

    public static void AnimateFadeInFromLeft(this VisualElement e)
    {
        var pct = 100;
        e.schedule.Execute(() =>
        {
            pct -= 2;
            e.style.left = Length.Percent(pct);
            e.style.opacity = 1 - pct / 100f;
        }).Every(5).Until(() => e.style.left == Length.Percent(0));
    }

    public static void AnimateFadeIn(this VisualElement e)
    {
        var pct = 100;
        e.schedule.Execute(() =>
        {
            pct -= 2;
            e.style.opacity = 1 - pct / 100f;
        }).Every(5).Until(() => e.style.opacity == 1);
    }

    public static async UniTask AnimateBottomUp(this VisualElement e, int pixels, float frames = 30)
    {
        var bottom = 0f;
        var increment = pixels / frames;
        var i = e.schedule.Execute(() =>
        {
            bottom += increment;
            e.style.bottom = bottom;
        }).Every(5).Until(() => bottom >= pixels);
        await UniTask.WaitUntil(() => !i.isActive);
    }

    public static async UniTask AnimateYUp(this VisualElement e, int pixels, float frames = 30)
    {
        var bottom = 0f;
        var increment = pixels / frames;
        var i = e.schedule.Execute(() =>
        {
            bottom += increment;
            e.style.translate = new StyleTranslate(new Translate(0, -bottom));
        }).Every(5).Until(() => bottom >= pixels);
        await UniTask.WaitUntil(() => !i.isActive);
    }

    public static async UniTask AnimateTopUp(this VisualElement e, int pixels)
    {
        var incremented = 0f;
        var increment = pixels / 30f;
        var i = e.schedule.Execute(() =>
        {
            incremented += increment;
            e.style.top = incremented;
        }).Every(5).Until(() => Math.Abs(incremented) >= Math.Abs(pixels));
        await UniTask.WaitUntil(() => Math.Abs(incremented) >= Math.Abs(pixels));
    }

    public static async UniTask AnimateMarginTopUp(this VisualElement e, int pixels)
    {
        var incremented = 0f;
        var increment = pixels / 30f;
        var i = e.schedule.Execute(() =>
        {
            incremented += increment;
            e.style.marginTop = incremented;
        }).Every(10).Until(() => Math.Abs(incremented) >= Math.Abs(pixels));
        await UniTask.WaitUntil(() => Math.Abs(incremented) >= Math.Abs(pixels));
    }

    public static async UniTask AnimateMarginBottomUp(this VisualElement e, int pixels)
    {
        var incremented = 0f;
        var increment = pixels / 30f;
        var i = e.schedule.Execute(() =>
        {
            incremented += increment;
            e.style.marginBottom = incremented;
        }).Every(10).Until(() => Math.Abs(incremented) >= Math.Abs(pixels));
        await UniTask.WaitUntil(() => Math.Abs(incremented) >= Math.Abs(pixels));
    }
}