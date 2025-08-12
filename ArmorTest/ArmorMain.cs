using OnixRuntime.Api;
using OnixRuntime.Api.Items;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Plugin;


namespace ArmorTest
{
    public class ArmorTest : OnixPluginBase
    {
        public static ArmorTest Instance { get; private set; } = null!;
        private static ArmorTestConfig Config { get; set; } = null!;

        private static readonly TexturePath HotbarTexture = TexturePath.Game("textures/ui/hotbar_1");
        private static readonly TexturePath EndCapTexture = TexturePath.Game("textures/ui/hotbar_end_cap");

        public ArmorTest(OnixPluginInitInfo initInfo) : base(initInfo)
        {
            Instance = this;
            // If you can clean up what the plugin leaves behind manually, please do not unload the plugin when disabling.
            base.DisablingShouldUnloadPlugin = false;
#if DEBUG
            // base.WaitForDebuggerToBeAttached();
#endif
        }

        protected override void OnLoaded()
        {
            Config = new ArmorTestConfig(PluginDisplayModule);
            Onix.Events.Common.HudRenderGame += Common_HudRender;
            Console.WriteLine($"Plugin {CurrentPluginManifest.Name} loaded!");
        }

        private static void Common_HudRender(RendererGame gfx, float delta)
        {
            ItemStack[] armor = Onix.LocalPlayer!.ArmorItems;
            ItemStack offhand = Onix.LocalPlayer!.OffhandItem;
            float helmetX = -157f;
            float chestX = -137f;
            const float yOffset = -1.75f;
            if (offhand.IsEmpty)
            {
                helmetX = -133f;
                chestX = -113f;
            }
            else
            {
                Slot(gfx, new Vec2(-113.5f, yOffset), offhand, HotbarSide.Alone);
            }
            Slot(gfx, new Vec2(helmetX, yOffset), armor[0], HotbarSide.Left);
            Slot(gfx, new Vec2(chestX, yOffset), armor[1], HotbarSide.Right);
            Slot(gfx, new Vec2(93f, yOffset), armor[2], HotbarSide.Left);
            Slot(gfx, new Vec2(113f, yOffset), armor[3], HotbarSide.Right);
        }

        private enum HotbarSide
        {
            Left, Right, Center, Alone
        }

        private static Vec2 HotbarPosition()
        {
            Rect screenSafeArea = Onix.Gui.ScreenSafeArea;
            return new Vec2(Onix.Gui.ScreenSize.X / 2, screenSafeArea.W - 22);
        }

        private static void Slot(RendererGame gfx, Vec2 position, ItemStack? item, HotbarSide direction)
        {
            Vec2 hotbar = HotbarPosition();
            Vec2 slot = hotbar + position;
            const float slotW = 20f;
            const float slotH = 22f;
            if (Config.ShowSlotTexture)
            {
                Rect slotRect = Rect.FromSize(slot.X, slot.Y, slotW, slotH);
                gfx.RenderTexture(slotRect, HotbarTexture);
                switch (direction)
                {
                    case HotbarSide.Left:
                        {
                            Rect leftCapRect = Rect.FromSize(slot.X - 1, slot.Y, 1, slotH);
                            gfx.RenderTexture(leftCapRect, EndCapTexture);
                            break;
                        }
                    case HotbarSide.Right:
                        {
                            Rect rightCapRect = Rect.FromSize(slot.X + slotW, slot.Y, 1, slotH);
                            gfx.RenderTexture(rightCapRect, EndCapTexture);
                            break;
                        }
                    case HotbarSide.Alone:
                        {
                            Rect leftCapRect = Rect.FromSize(slot.X - 1, slot.Y, 1, slotH);
                            gfx.RenderTexture(leftCapRect, EndCapTexture);
                            Rect rightCapRect = Rect.FromSize(slot.X + slotW, slot.Y, 1, slotH);
                            gfx.RenderTexture(rightCapRect, EndCapTexture);
                            break;
                        }
                    case HotbarSide.Center:
                        // No end caps for center slot
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
            }

            if (item is null) return;
            Vec2 itemPos = slot + new Vec2(1.8f, 3f);
            gfx.RenderItem(itemPos, item);
        }

        protected override void OnUnloaded()
        {
            Console.WriteLine($"Plugin {CurrentPluginManifest.Name} Unloaded!");
            // Ensure every task or thread is stopped when this function returns.
            // You can give them base.PluginEjectionCancellationToken which will be cancelled when this function returns. 
        }
    }
}